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
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;

using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using SharpPcap.WinPcap;

namespace EightSixBoxVPN.Comm
{
    /// <summary>
    /// This implements the base communications socket listener and session manager.
    /// </summary>
    public class CommManager
    {
        /**
         * Fields
         */
        public const int port = 10234;
        public const int readTimeoutMilliseconds = 250;

        private bool runThread = false;
        private Thread commMgrThread;

        private TcpListener socketServer;

        private List<Thread> sessionThreads;
        private List<Session> sessions;

        private static CommManager instance;

        private ICaptureDevice capDevice;

        /**
         * Properties
         */
        /// <summary>
        /// Gets a singleton instance.
        /// </summary>
        /// <value>The instance.</value>
        public static CommManager Instance
        {
            get
            {
                if (instance == null)
                {
                    var newInstance = new CommManager();
                    Interlocked.CompareExchange(ref instance, newInstance, null);
                }

                return instance;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ICaptureDevice CaptureDevice
        {
            get { return capDevice; }
        }

        /**
         * Events
         */
        public delegate void EthCapture(EthernetPacket packet);
        public event EthCapture PacketCapture;

        /**
         * Methods
         */
        /// <summary>
        /// Initializes a new instance of the <see cref="CommManager"/> class.
        /// </summary>
        public CommManager()
        {
            instance = this;
            sessionThreads = new List<Thread>(16);
            sessions = new List<Session>(16);

            commMgrThread = new Thread(new ThreadStart(CommManagerThread));
            commMgrThread.Name = "CommManagerThread";

            Messages.Trace("created comm manager thread [" + commMgrThread.Name + "]");

            IPAddress allAddr = IPAddress.Parse("0.0.0.0");
            socketServer = new TcpListener(allAddr, port);
        }

        /// <summary>
        /// Starts the target manager thread.
        /// </summary>
        public void Start()
        {
            Messages.Trace("starting communications manager");

            // get the packet capture device
            CaptureDeviceList devices = CaptureDeviceList.Instance;

            capDevice = devices[Program.interfaceToUse];
            Messages.Trace("using [" + capDevice.Description + "] for network capture");

            capDevice.OnPacketArrival += capDevice_OnPacketArrival;

            if (capDevice is WinPcapDevice)
            {
                WinPcapDevice winPcap = capDevice as WinPcapDevice;
                winPcap.Open(OpenFlags.Promiscuous | OpenFlags.NoCaptureLocal | OpenFlags.MaxResponsiveness, readTimeoutMilliseconds);
            }
            else if (capDevice is LibPcapLiveDevice)
            {
                LibPcapLiveDevice livePcapDevice = capDevice as LibPcapLiveDevice;
                livePcapDevice.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
            }
            else
                throw new InvalidOperationException("unknown device type of " + capDevice.GetType().ToString());

            capDevice.StartCapture();

            // start loop
            runThread = true;
            commMgrThread.Start();
        }

        /// <summary>
        /// Event that occurs when we capture a packet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void capDevice_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            try
            {
                Packet packet = Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
                if (packet is EthernetPacket)
                {
                    EthernetPacket eth = (EthernetPacket)packet;

                    if (PacketCapture != null)
                        PacketCapture(eth);
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                /* stub */
            }
            catch (InvalidOperationException)
            {
                /* stub */
            }
        }

        /// <summary>
        /// Stops the comm manager thread.
        /// </summary>
        public void Stop()
        {
            DestroySessionThreads();

            try
            {
                Messages.Trace("stopping communications manager");

                // kill loop
                runThread = false;
                Thread.Sleep(50);

                if (capDevice != null)
                {
                    if (capDevice.Started)
                        capDevice.StopCapture();
                    capDevice.Close();
                    capDevice = null;
                }

                // abort and join
                commMgrThread.Abort();
                commMgrThread.Join();
            }
            catch (Exception e)
            {
                Messages.StackTrace(e, false);
            }
        }

        /// <summary>
        /// Helper function to shutdown and destroy any running session threads.
        /// </summary>
        private void DestroySessionThreads()
        {
            try
            {
                Messages.Trace("stopping communications sessions");

                // iterate through session threads and kill em'
                foreach (Thread sessionThread in sessionThreads)
                {
                    sessionThread.Abort();
                    sessionThread.Join();
                }
            }
            catch (Exception e)
            {
                Messages.StackTrace(e, false);
            }

        }

        /// <summary>
        /// Event that occurs when the TCP listener receives a connection.
        /// </summary>
        /// <param name="ar"></param>
        private void AcceptClientCallback(IAsyncResult ar)
        {
            // get the listener that handles the client request
            TcpListener listener = (TcpListener)ar.AsyncState;

            // end the operation and display the received data on  
            // the console
            TcpClient client = listener.EndAcceptTcpClient(ar);
            client.NoDelay = true;

            Messages.Trace("accepted new client connection (" + client.Client.RemoteEndPoint.ToString() + ")");

            // generate the thread name
            string threadName = "CommSession (" + client.Client.RemoteEndPoint.ToString() + ")";

            // create and execute a new session thread
            Thread sessionThread = new Thread(() => SessionThread(client, threadName));
            sessionThread.Name = threadName;
            sessionThread.Start();

            Messages.Trace("created new session thread [" + sessionThread.Name + "]");

            sessionThreads.Add(sessionThread);
        }

        /// <summary>
        /// Implements the comm manager thread.
        /// </summary>
        private void CommManagerThread()
        {
            socketServer.Start();

            try
            {
                // implement main work loop
                while (runThread)
                {
                    socketServer.BeginAcceptTcpClient(AcceptClientCallback, socketServer);
                    Thread.Sleep(1);
                } // while (runThread)
            }
            catch (ThreadAbortException)
            {
                Messages.WriteWarning("comm manager thread commanded to abort!");
                socketServer.Stop();
                runThread = false;
            }
            catch (Exception e)
            {
                Messages.StackTrace(e, false);
                socketServer.Stop();
                runThread = false; // terminate thread
            }
        }

        /// <summary>
        /// Implements the target session thread.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="threadName"></param>
        private void SessionThread(TcpClient client, string threadName)
        {
            bool internalRunThread = runThread;
            NetworkStream clientStream = client.GetStream();
            Session clientSession = new Session(client);

            sessions.Add(clientSession);

            try
            {
                // implement main work loop
                while (internalRunThread)
                {
                    internalRunThread = runThread;

                    if (clientSession.isShutdown)
                        internalRunThread = false;

                    if (client.Connected)
                        clientSession.HandleNetwork();
                    else
                    {
                        Messages.Trace("client connection lost, thread [" + threadName + "]");
                        clientSession.Close();
                        internalRunThread = false;
                    }
                } // while (runThread)

                Messages.Trace("net loop terminated, thread [" + threadName + "]");

                clientStream.Dispose();
                client.Close();
                clientSession.Close();
            }
            catch (SocketException se)
            {
                Messages.StackTrace(se, false);
                clientSession.Close();
                internalRunThread = false; // terminate thread
            }
            catch (ThreadAbortException)
            {
                Messages.WriteWarning("comm session thread commanded to abort!");
                clientSession.Close();
                internalRunThread = false;
            }
            catch (Exception e)
            {
                Messages.StackTrace(e, false);
                clientSession.Close();
                internalRunThread = false; // terminate thread
            }

            sessions.Remove(clientSession);
        }
    } // public class CommManager
} // namespace EightSixBoxVPN.Comm
