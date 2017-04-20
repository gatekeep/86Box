/**
 * 86Box VPN Server
 * Copyright (c) 2017 Bryan Biedenkapp., All Rights Reserved.
 * INTERNAL/PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 * DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
 *
 * @package 86Box VPN Server
 * @version 1.00
 *
 */
//#define DUMP_ALL
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

using EightSixBoxVPN.zlib;

namespace EightSixBoxVPN.Comm
{
    /// <summary>
    /// This class implements the base code for the individual connection session handling.
    /// </summary>
    public class Session
    {
        /**
         * Fields
         */
        internal const uint MAX_STREAM_EXECPTIONS = 256;
        internal const int TEMP_BUFFER_SIZE = 262144;

        private ManualResetEventSlim waitMRE;

        private CommManager commMgr;

        protected TcpClient client;

        private byte[] macAddress;

        private byte[] tempBuffer;

        /// <summary>
        /// Flag indicating whether this session is shutdown.
        /// </summary>
        public bool isShutdown;

        internal uint streamExceptionCount;

        /**
         * Properties
         */
        /// <summary>
        /// Gets the TCP client associated with this session.
        /// </summary>
        public TcpClient Client
        {
            get { return client; }
        }

        /// <summary>
        /// Gets the network stream associated with this session.
        /// </summary>
        public NetworkStream Stream
        {
            get { return client.GetStream(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] MAC
        {
            get { return macAddress; }
        }

        /// <summary>
        /// 
        /// </summary>
        public PhysicalAddress PhysicalAddress
        {
            get { return new PhysicalAddress(macAddress); }
        }

        /**
         * Methods
         */
        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="commMgr"></param>
        /// <param name="client"></param>
        public Session(CommManager commMgr, TcpClient client)
        {
            this.commMgr = commMgr;
            this.client = client;
            this.client.ReceiveBufferSize = TEMP_BUFFER_SIZE;
            this.tempBuffer = new byte[TEMP_BUFFER_SIZE];

            this.macAddress = new byte[6];

            this.isShutdown = false;

            this.streamExceptionCount = 0;

            this.waitMRE = new ManualResetEventSlim(false);
        }

        /// <summary>
        /// Close and cleanup this session and the associated TCP client.
        /// </summary>
        public void Close()
        {
            if (client != null && client.Connected)
            {
                client.Close();
                client = null;
            }

            isShutdown = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private byte PacketCRC(byte[] buffer, int length)
        {
            byte tmpCRC = 0;
            for (int i = 0; i < length; i++)
                tmpCRC ^= buffer[i];
            return tmpCRC;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pdu"></param>
        public void HandleIncoming(ProtocolDataUnit pdu)
        {
            if (isShutdown)
                return;
            if (client == null)
                return;
            if (!client.Connected)
                return;

            // is this our mac?
            if (PhysicalAddress.ToString() != pdu.Header.MacAddr.ToString())
                return;

#if TRACE
            Messages.Trace("session [" + PhysicalAddress.ToString() + "] destHW [" + pdu.Header.MacAddr.ToString() + "]");
#if DUMP_ALL
            if (pdu != null)
            {
                Messages.Trace("[SNIP .. Packet Rx from Client]");
                Messages.TraceHex("hdr", pdu.HeaderData, pdu.HeaderData.Length);
                if (pdu.ContentData != null)
                {
                    Messages.TraceHex("packet", pdu.ContentData, pdu.ContentData.Length);
                    Messages.Trace("packet length " + pdu.ContentData.Length);
                }
                Messages.Trace("hdr->checksum = " + pdu.Header.CRC.ToString("X"));
                Messages.Trace("hdr->length = " + pdu.Header.Length.ToString());
                Messages.Trace("hdr->macAddr = " +
                    pdu.Header.MacAddr[0].ToString("X") + ":" +
                    pdu.Header.MacAddr[1].ToString("X") + ":" +
                    pdu.Header.MacAddr[2].ToString("X") + ":" +
                    pdu.Header.MacAddr[3].ToString("X") + ":" +
                    pdu.Header.MacAddr[4].ToString("X") + ":" +
                    pdu.Header.MacAddr[5].ToString("X"));
                Messages.Trace("hdr->dataLength = " + pdu.Header.DataLength);
                Messages.Trace("hdr->compressLength = " + pdu.Header.CompressedLength);
                Messages.Trace("[SNIP .. Packet Rx from Client]");
            }
#endif
#endif
            // repackage data
            HandshakeHeader header = new HandshakeHeader();
            header.CRC = PacketCRC(pdu.ContentData, pdu.ContentData.Length);
            header.DataLength = (ushort)pdu.ContentData.Length;
            header.MacAddr = this.macAddress;

            // compress data
            byte[] compressedData = ZStream.CompressBuffer(pdu.ContentData);
            header.CompressedLength = (ushort)compressedData.Length;
            header.Length = (ushort)(header.Size + header.CompressedLength);

            // build final payload
            byte[] buffer = new byte[Util.RoundUp(header.Size + header.CompressedLength, 4)];
            header.WriteTo(buffer, 0);

            Array.Copy(compressedData, 0, buffer, header.Size, compressedData.Length);

            // transmit
            if (Stream.CanWrite)
            {
                Stream.Flush();
                Stream.Write(buffer, 0, buffer.Length);
                Stream.Flush();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private void NetworkReadBuffer(IAsyncResult ar)
        {
            int bytesRead = Stream.EndRead(ar);

            try
            {
                // read PDU data from stream
                ProtocolDataUnit pdu = ProtocolDataUnit.ReadFrom(tempBuffer);
#if TRACE && DUMP_ALL
                if (pdu != null)
                {
                    Messages.Trace("[SNIP .. Packet Rx from Client]");
                    Messages.TraceHex("hdr", pdu.HeaderData, pdu.HeaderData.Length);
                    if (pdu.ContentData != null)
                    {
                        Messages.TraceHex("packet", pdu.ContentData, pdu.ContentData.Length);
                        Messages.Trace("packet length " + pdu.ContentData.Length);
                    }
                    Messages.Trace("hdr->checksum = " + pdu.Header.CRC.ToString("X"));
                    Messages.Trace("hdr->length = " + pdu.Header.Length.ToString());
                    Messages.Trace("hdr->macAddr = " +
                        pdu.Header.MacAddr[0].ToString("X") + ":" +
                        pdu.Header.MacAddr[1].ToString("X") + ":" +
                        pdu.Header.MacAddr[2].ToString("X") + ":" +
                        pdu.Header.MacAddr[3].ToString("X") + ":" +
                        pdu.Header.MacAddr[4].ToString("X") + ":" +
                        pdu.Header.MacAddr[5].ToString("X"));
                    Messages.Trace("hdr->dataLength = " + pdu.Header.DataLength);
                    Messages.Trace("hdr->compressLength = " + pdu.Header.CompressedLength);
                    Messages.Trace("[SNIP .. Packet Rx from Client]");
                }
#endif

                // reset exception counter
                if (streamExceptionCount > 0)
                    streamExceptionCount = 0;

                // did we receive a pdu?
                if (pdu != null)
                {
                    // are we trying to shut down the session?
                    if ((pdu.Header.CRC == 250) && (pdu.Header.DataLength == 65535))
                    {
                        Close();
                        return;
                    }

                    // are we trying to register a client?
                    if ((pdu.Header.CRC == 255) && (pdu.Header.DataLength == 0))
                    {
                        HandshakeHeader header = new HandshakeHeader();
                        header.CRC = 255;
                        header.DataLength = 0;
                        header.CompressedLength = 0;
                        header.Length = (ushort)header.Size;

                        byte[] node = new byte[6];

                        // generate a new MAC address
                        // set first 3 octets 
                        node[0] = 0xAC;
                        node[1] = 0xDE;
                        node[2] = 0x48;

                        Random rand = new Random();
                        node[3] = (byte)rand.Next(255);
                        node[4] = (byte)rand.Next(255);
                        node[5] = (byte)rand.Next(255);

                        header.MacAddr = node;
                        this.macAddress = node;

                        Messages.Trace("new session macAddr = " +
                            macAddress[0].ToString("X") + ":" +
                            macAddress[1].ToString("X") + ":" +
                            macAddress[2].ToString("X") + ":" +
                            macAddress[3].ToString("X") + ":" +
                            macAddress[4].ToString("X") + ":" +
                            macAddress[5].ToString("X"));

                        // build final payload
                        byte[] buffer = new byte[Util.RoundUp(header.Size, 4)];
                        header.WriteTo(buffer, 0);

                        // transmit
                        if (Stream.CanWrite)
                        {
                            Stream.Flush();
                            Stream.Write(buffer, 0, buffer.Length);
                            Stream.Flush();
                        }
                        return;
                    }

                    commMgr.BroadcastPacket(pdu);
                }
            }
            catch (IOException)
            {
                waitMRE.Wait(1); // wait a bit before reading again
                waitMRE.Set();

                // increment exception counter
                streamExceptionCount++;
                if (streamExceptionCount >= MAX_STREAM_EXECPTIONS)
                    this.Close();
            }
            finally
            {
                waitMRE.Set();
            }
        }

        /// <summary>
        /// Reads some data from the opened TCP client socket.
        /// </summary>
        public void HandleNetwork()
        {
            if (isShutdown)
                return;

            if (tempBuffer == null)
                tempBuffer = new byte[client.ReceiveBufferSize + 1];

            this.waitMRE.Reset();

            try
            {
                if (Stream.DataAvailable)
                    Stream.BeginRead(tempBuffer, 0, client.ReceiveBufferSize, NetworkReadBuffer, null);

                this.waitMRE.Wait(1);
                this.waitMRE.Reset();
            }
            catch (IOException)
            {
                this.waitMRE.Wait(1); // wait a bit before reading again
                this.waitMRE.Reset();

                // increment exception counter
                streamExceptionCount++;
                if (streamExceptionCount >= MAX_STREAM_EXECPTIONS)
                    this.Close();
            }
            catch (ObjectDisposedException ode)
            {
                Messages.StackTrace(ode, false);
                this.Close();
            }
            catch (SocketException se)
            {
                Messages.StackTrace(se, false);
            }
        }
    } // public class Session
} // namespace EightSixBoxVPN.Comm
