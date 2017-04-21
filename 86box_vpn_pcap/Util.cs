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
//
// Based on code from the DiscUtils project. (http://discutils.codeplex.com/)
// Copyright (c) 2008-2011, Kenneth Bell
// Licensed under the MIT License (http://www.opensource.org/licenses/MIT)
//
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace EightSixBoxVPN
{
    /// <summary>
    /// Common global utility methods.
    /// </summary>
    internal static class Util
    {
        /**
         * Fields
         */
        internal static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);

        /// <summary>
        /// Multipler for bytes to kilobytes [Bytes * 1024]
        /// </summary>
        public const long MultiplerOneKB = 1024L;
        /// <summary>
        /// Multipler for bytes to megabytes [Bytes * 1024 * 1024 (or KByte * 1024)]
        /// </summary>
        public const long MultiplerOneMB = 1024L * MultiplerOneKB;
        /// <summary>
        /// Multipler for bytes to gigabytes [Bytes * 1024 * 1024 * 1024 (or MByte * 1024, KByte * 1024 * 1024)]
        /// </summary>
        public const long MultiplerOneGB = 1024L * MultiplerOneMB;

        /// <summary>
        /// Stored sector size (in bytes)
        /// </summary>
        public const int Sector = 512;

        /**
         * Methods
         */
        /// <summary>
        /// Conversion helper to a value to kilobytes. [value * 1024L]
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static long ToKilo(long value)
        {
            return value * MultiplerOneKB;
        }

        /// <summary>
        /// Conversion helper to a value to megabytes. [value * 1024L * 1024L]
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static long ToMega(long value)
        {
            return value * MultiplerOneMB;
        }

        /// <summary>
        /// Conversion helper to a value to gigabytes. [value * 1024L * 1024L * 1024L]
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static long ToGiga(long value)
        {
            return value * MultiplerOneGB;
        }

        /// <summary>
        /// Conversion helper to get number of kilobytes in the given number of bytes. [value / 1024L]
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static double KiloFromBytes(double bytes)
        {
            return bytes / 1024L;
        }

        /// <summary>
        /// Conversion helper to get number of megabytes in the given number of bytes. [value / 1024L / 1024L]
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static double MegaFromBytes(double bytes)
        {
            return bytes / 1024L / 1024L;
        }

        /// <summary>
        /// Conversion helper to get number of gigabytes in the given number of bytes. [value / 1024L / 1024L / 1024L]
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static double GigaFromBytes(double bytes)
        {
            return bytes / 1024L / 1024L / 1024L;
        }

        /// <summary>
        /// Perform static casting from one type to another.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T CastTo<T>(object value)
        {
            return value != null ? (T)value : default(T);
        }

        /// <summary>
        /// Round up a value to a multiple of a unit size.
        /// </summary>
        /// <param name="value">The value to round up.</param>
        /// <param name="unit">The unit (the returned value will be a multiple of this number).</param>
        /// <returns>The rounded-up value.</returns>
        public static long RoundUp(long value, long unit)
        {
            return ((value + (unit - 1)) / unit) * unit;
        }

        /// <summary>
        /// Round up a value to a multiple of a unit size.
        /// </summary>
        /// <param name="value">The value to round up.</param>
        /// <param name="unit">The unit (the returned value will be a multiple of this number).</param>
        /// <returns>The rounded-up value.</returns>
        public static int RoundUp(int value, int unit)
        {
            return ((value + (unit - 1)) / unit) * unit;
        }

        /// <summary>
        /// Round down a value to a multiple of a unit size.
        /// </summary>
        /// <param name="value">The value to round down.</param>
        /// <param name="unit">The unit (the returned value will be a multiple of this number).</param>
        /// <returns>The rounded-down value.</returns>
        public static long RoundDown(long value, long unit)
        {
            return (value / unit) * unit;
        }

        /// <summary>
        /// Calculates the CEIL function.
        /// </summary>
        /// <param name="numerator">The value to divide.</param>
        /// <param name="denominator">The value to divide by.</param>
        /// <returns>The value of CEIL(numerator/denominator).</returns>
        public static int Ceil(int numerator, int denominator)
        {
            return (numerator + (denominator - 1)) / denominator;
        }

        /// <summary>
        /// Calculates the CEIL function.
        /// </summary>
        /// <param name="numerator">The value to divide.</param>
        /// <param name="denominator">The value to divide by.</param>
        /// <returns>The value of CEIL(numerator/denominator).</returns>
        public static uint Ceil(uint numerator, uint denominator)
        {
            return (numerator + (denominator - 1)) / denominator;
        }

        /// <summary>
        /// Calculates the CEIL function.
        /// </summary>
        /// <param name="numerator">The value to divide.</param>
        /// <param name="denominator">The value to divide by.</param>
        /// <returns>The value of CEIL(numerator/denominator).</returns>
        public static long Ceil(long numerator, long denominator)
        {
            return (numerator + (denominator - 1)) / denominator;
        }

        /// <summary>
        /// Converts between two arrays.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the source array.</typeparam>
        /// <typeparam name="U">The type of the elements of the destination array.</typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="func">The function to map from source type to destination type.</param>
        /// <returns>The resultant array.</returns>
        public static U[] Map<T, U>(ICollection<T> source, Func<T, U> func)
        {
            U[] result = new U[source.Count];
            int i = 0;

            foreach (T sVal in source)
                result[i++] = func(sVal);

            return result;
        }

        /// <summary>
        /// Converts between two arrays.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the source array.</typeparam>
        /// <typeparam name="U">The type of the elements of the destination array.</typeparam>
        /// <param name="source">The source array.</param>
        /// <param name="func">The function to map from source type to destination type.</param>
        /// <returns>The resultant array.</returns>
        public static U[] Map<T, U>(IEnumerable<T> source, Func<T, U> func)
        {
            List<U> result = new List<U>();

            foreach (T sVal in source)
                result.Add(func(sVal));

            return result.ToArray();
        }

        /// <summary>
        /// Filters a collection into a new collection.
        /// </summary>
        /// <typeparam name="C">The type of the new collection.</typeparam>
        /// <typeparam name="T">The type of the collection entries.</typeparam>
        /// <param name="source">The collection to filter.</param>
        /// <param name="predicate">The predicate to select which entries are carried over.</param>
        /// <returns>The new collection, containing all entries where the predicate returns <c>true</c>.</returns>
        public static C Filter<C, T>(ICollection<T> source, Func<T, bool> predicate) where C : ICollection<T>, new()
        {
            C result = new C();
            foreach (T val in source)
                if (predicate(val))
                    result.Add(val);

            return result;
        }

        /// <summary>
        /// Indicates if two ranges overlap.
        /// </summary>
        /// <typeparam name="T">The type of the ordinals.</typeparam>
        /// <param name="xFirst">The lowest ordinal of the first range (inclusive).</param>
        /// <param name="xLast">The highest ordinal of the first range (exclusive).</param>
        /// <param name="yFirst">The lowest ordinal of the second range (inclusive).</param>
        /// <param name="yLast">The highest ordinal of the second range (exclusive).</param>
        /// <returns><c>true</c> if the ranges overlap, else <c>false</c>.</returns>
        public static bool RangesOverlap<T>(T xFirst, T xLast, T yFirst, T yLast) where T : IComparable<T>
        {
            return !((xLast.CompareTo(yFirst) <= 0) || (xFirst.CompareTo(yLast) >= 0));
        }

        /// <summary>
        /// Primitive conversion from Unicode to ASCII that preserves special characters.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="dest">The buffer to fill.</param>
        /// <param name="offset">The start of the string in the buffer.</param>
        /// <param name="count">The number of characters to convert.</param>
        /// <remarks>The built-in ASCIIEncoding converts characters of codepoint > 127 to ?,
        /// this preserves those code points by removing the top 16 bits of each character.</remarks>
        public static void StringToBytes(string value, byte[] dest, int offset, int count)
        {
            char[] chars = value.ToCharArray();

            int i = 0;
            while (i < chars.Length)
            {
                dest[i + offset] = (byte)chars[i];
                ++i;
            }

            while (i < count)
            {
                dest[i + offset] = 0;
                ++i;
            }
        }

        /// <summary>
        /// Primitive conversion from ASCII to Unicode that preserves special characters.
        /// </summary>
        /// <param name="data">The data to convert.</param>
        /// <param name="offset">The first byte to convert.</param>
        /// <param name="count">The number of bytes to convert.</param>
        /// <returns>The string.</returns>
        /// <remarks>The built-in ASCIIEncoding converts characters of codepoint > 127 to ?,
        /// this preserves those code points.</remarks>
        public static string BytesToString(byte[] data, int offset, int count)
        {
            char[] result = new char[count];

            // iterate through the individual bytes, and convert them to a character
            for (int i = 0; i < count; ++i)
                result[i] = (char)data[i + offset];

            return new string(result);
        }

        /// <summary>
        /// Primitive conversion from ASCII to Unicode that stops at a null-terminator.
        /// </summary>
        /// <param name="data">The data to convert.</param>
        /// <param name="offset">The first byte to convert.</param>
        /// <param name="count">The number of bytes to convert.</param>
        /// <returns>The string.</returns>
        /// <remarks>The built-in ASCIIEncoding converts characters of codepoint > 127 to ?,
        /// this preserves those code points.</remarks>
        public static string BytesToZString(byte[] data, int offset, int count)
        {
            char[] result = new char[count];

            // iterate through the individiual bytes
            for (int i = 0; i < count; ++i)
            {
                byte ch = data[i + offset];
                if (ch == 0)
                    return new string(result, 0, i);

                result[i] = (char)ch;
            }

            return new string(result);
        }

        /**
         * Bit Twiddling
         */
        /// <summary>
        /// Check if the given buffer has all zeros at the given offset for the given length.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static bool IsAllZeros(byte[] buffer, int offset, int count)
        {
            int end = offset + count;
            for (int i = offset; i < end; ++i)
                if (buffer[i] != 0)
                    return false;

            return true;
        }

        /// <summary>
        /// Checks if the given unsigned integer value is a power of 2.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(uint val)
        {
            if (val == 0)
                return false;

            while ((val & 1) != 1)
                val >>= 1;

            return val == 1;
        }

        /// <summary>
        /// Checks if the given long value is a power of 2.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(long val)
        {
            if (val == 0)
                return false;

            while ((val & 1) != 1)
                val >>= 1;

            return val == 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int Log2(uint val)
        {
            if (val == 0)
                throw new ArgumentException("Cannot calculate log of Zero", "val");

            int result = 0;
            while ((val & 1) != 1)
            {
                val >>= 1;
                ++result;
            }

            if (val == 1)
                return result;
            else
                throw new ArgumentException("Input is not a power of Two", "val");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int Log2(int val)
        {
            if (val == 0)
                throw new ArgumentException("Cannot calculate log of Zero", "val");

            int result = 0;
            while ((val & 1) != 1)
            {
                val >>= 1;
                ++result;
            }

            if (val == 1)
                return result;
            else
                throw new ArgumentException("Input is not a power of Two", "val");
        }

        /// <summary>
        /// Checks if the given byte arrays are equal.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool AreEqual(byte[] a, byte[] b)
        {
            // if the lengths are different ... well we know don't we?
            if (a.Length != b.Length)
                return false;

            // iterate through array and verify bytes are the same
            for (int i = 0; i < a.Length; ++i)
                if (a[i] != b[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Swap the bits of the given unsigned short.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ushort BitSwap(ushort value)
        {
            return (ushort)(((value & 0x00FF) << 8) | ((value & 0xFF00) >> 8));
        }

        /// <summary>
        /// Swap the bits of the given unsigned int.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static uint BitSwap(uint value)
        {
            return ((value & 0xFF) << 24) | ((value & 0xFF00) << 8) | ((value & 0x00FF0000) >> 8) | ((value & 0xFF000000) >> 24);
        }

        /// <summary>
        /// Swap the bits of the given unsigned long.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ulong BitSwap(ulong value)
        {
            return (((ulong)BitSwap((uint)(value & 0xFFFFFFFF))) << 32) | BitSwap((uint)(value >> 32));
        }

        /// <summary>
        /// Swap the bits of the given signed short.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static short BitSwap(short value)
        {
            return (short)BitSwap((ushort)value);
        }

        /// <summary>
        /// Swap the bits of the given signed integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int BitSwap(int value)
        {
            return (int)BitSwap((uint)value);
        }

        /// <summary>
        /// Swap the bits of the given long.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long BitSwap(long value)
        {
            return (long)BitSwap((ulong)value);
        }

        /// <summary>
        /// Write the given bytes in the unsigned short into the given buffer (by least significant byte)
        /// </summary>
        /// <param name="val"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public static void WriteBytesLittleEndian(ushort val, byte[] buffer, int offset)
        {
            buffer[offset] = (byte)(val & 0xFF);
            buffer[offset + 1] = (byte)((val >> 8) & 0xFF);
        }

        /// <summary>
        /// Write the given bytes in the unsigned integer into the given buffer (by least significant byte)
        /// </summary>
        /// <param name="val"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public static void WriteBytesLittleEndian(uint val, byte[] buffer, int offset)
        {
            buffer[offset] = (byte)(val & 0xFF);
            buffer[offset + 1] = (byte)((val >> 8) & 0xFF);
            buffer[offset + 2] = (byte)((val >> 16) & 0xFF);
            buffer[offset + 3] = (byte)((val >> 24) & 0xFF);
        }

        /// <summary>
        /// Write the given bytes in the unsigned long into the given buffer (by least significant byte)
        /// </summary>
        /// <param name="val"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public static void WriteBytesLittleEndian(ulong val, byte[] buffer, int offset)
        {
            buffer[offset] = (byte)(val & 0xFF);
            buffer[offset + 1] = (byte)((val >> 8) & 0xFF);
            buffer[offset + 2] = (byte)((val >> 16) & 0xFF);
            buffer[offset + 3] = (byte)((val >> 24) & 0xFF);
            buffer[offset + 4] = (byte)((val >> 32) & 0xFF);
            buffer[offset + 5] = (byte)((val >> 40) & 0xFF);
            buffer[offset + 6] = (byte)((val >> 48) & 0xFF);
            buffer[offset + 7] = (byte)((val >> 56) & 0xFF);
        }

        /// <summary>
        /// Write the given bytes in the short into the given buffer (by least significant byte)
        /// </summary>
        /// <param name="val"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public static void WriteBytesLittleEndian(short val, byte[] buffer, int offset)
        {
            WriteBytesLittleEndian((ushort)val, buffer, offset);
        }

        /// <summary>
        /// Write the given bytes in the integer into the given buffer (by least significant byte)
        /// </summary>
        /// <param name="val"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public static void WriteBytesLittleEndian(int val, byte[] buffer, int offset)
        {
            WriteBytesLittleEndian((uint)val, buffer, offset);
        }

        /// <summary>
        /// Write the given bytes in the long into the given buffer (by least significant byte)
        /// </summary>
        /// <param name="val"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public static void WriteBytesLittleEndian(long val, byte[] buffer, int offset)
        {
            WriteBytesLittleEndian((ulong)val, buffer, offset);
        }

        /// <summary>
        /// Write the given bytes in the Guid into the given buffer (by least significant byte)
        /// </summary>
        /// <param name="val"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public static void WriteBytesLittleEndian(Guid val, byte[] buffer, int offset)
        {
            byte[] le = val.ToByteArray();
            Array.Copy(le, 0, buffer, offset, 16);
        }

        /// <summary>
        /// Write the given bytes in the unsigned short into the given buffer (by most significant byte)
        /// </summary>
        /// <param name="val"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public static void WriteBytesBigEndian(ushort val, byte[] buffer, int offset)
        {
            buffer[offset] = (byte)(val >> 8);
            buffer[offset + 1] = (byte)(val & 0xFF);
        }

        /// <summary>
        /// Write the given bytes in the unsigned integer into the given buffer (by most significant byte)
        /// </summary>
        /// <param name="val"></param>
        public static void Write3BytesBigEndian(uint val, ref byte byte1, ref byte byte2, ref byte byte3)
        {
            byte1 = (byte)((val >> 16) & 0xFF);
            byte2 = (byte)((val >> 8) & 0xFF);
            byte3 = (byte)(val & 0xFF);
        }

        /// <summary>
        /// Write the given bytes in the unsigned integer into the given buffer (by most significant byte)
        /// </summary>
        /// <param name="val"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public static void WriteBytesBigEndian(uint val, byte[] buffer, int offset)
        {
            buffer[offset] = (byte)((val >> 24) & 0xFF);
            buffer[offset + 1] = (byte)((val >> 16) & 0xFF);
            buffer[offset + 2] = (byte)((val >> 8) & 0xFF);
            buffer[offset + 3] = (byte)(val & 0xFF);
        }

        /// <summary>
        /// Write the given bytes in the unsigned long into the given buffer (by most significant byte)
        /// </summary>
        /// <param name="val"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public static void WriteBytesBigEndian(ulong val, byte[] buffer, int offset)
        {
            buffer[offset] = (byte)((val >> 56) & 0xFF);
            buffer[offset + 1] = (byte)((val >> 48) & 0xFF);
            buffer[offset + 2] = (byte)((val >> 40) & 0xFF);
            buffer[offset + 3] = (byte)((val >> 32) & 0xFF);
            buffer[offset + 4] = (byte)((val >> 24) & 0xFF);
            buffer[offset + 5] = (byte)((val >> 16) & 0xFF);
            buffer[offset + 6] = (byte)((val >> 8) & 0xFF);
            buffer[offset + 7] = (byte)(val & 0xFF);
        }

        /// <summary>
        /// Write the given bytes in the short into the given buffer (by most significant byte)
        /// </summary>
        /// <param name="val"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public static void WriteBytesBigEndian(short val, byte[] buffer, int offset)
        {
            WriteBytesBigEndian((ushort)val, buffer, offset);
        }

        /// <summary>
        /// Write the given bytes in the integer into the given buffer (by most significant byte)
        /// </summary>
        /// <param name="val"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public static void WriteBytesBigEndian(int val, byte[] buffer, int offset)
        {
            WriteBytesBigEndian((uint)val, buffer, offset);
        }

        /// <summary>
        /// Write the given bytes in the long into the given buffer (by most significant byte)
        /// </summary>
        /// <param name="val"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public static void WriteBytesBigEndian(long val, byte[] buffer, int offset)
        {
            WriteBytesBigEndian((ulong)val, buffer, offset);
        }

        /// <summary>
        /// Write the given bytes in the Guid into the given buffer (by most significant byte)
        /// </summary>
        /// <param name="val"></param>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        public static void WriteBytesBigEndian(Guid val, byte[] buffer, int offset)
        {
            byte[] le = val.ToByteArray();
            WriteBytesBigEndian(ToUInt32LittleEndian(le, 0), buffer, offset + 0);
            WriteBytesBigEndian(ToUInt16LittleEndian(le, 4), buffer, offset + 4);
            WriteBytesBigEndian(ToUInt16LittleEndian(le, 6), buffer, offset + 6);
            Array.Copy(le, 8, buffer, offset + 8, 8);
        }

        /// <summary>
        /// Get an unsigned short value from the given bytes. (by least significant byte)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static ushort ToUInt16LittleEndian(byte[] buffer, int offset)
        {
            return (ushort)(((buffer[offset + 1] << 8) & 0xFF00) | ((buffer[offset + 0] << 0) & 0x00FF));
        }

        /// <summary>
        /// Get an unsigned integer value from the given bytes. (by least significant byte)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static uint ToUInt32LittleEndian(byte[] buffer, int offset)
        {
            return (uint)(((buffer[offset + 3] << 24) & 0xFF000000U) | ((buffer[offset + 2] << 16) & 0x00FF0000U)
                | ((buffer[offset + 1] << 8) & 0x0000FF00U) | ((buffer[offset + 0] << 0) & 0x000000FFU));
        }

        /// <summary>
        /// Get an unsigned long value from the given bytes. (by least significant byte)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static ulong ToUInt64LittleEndian(byte[] buffer, int offset)
        {
            return (((ulong)ToUInt32LittleEndian(buffer, offset + 4)) << 32) | ToUInt32LittleEndian(buffer, offset + 0);
        }

        /// <summary>
        /// Get a signed short value from the given bytes. (by least significant byte)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static short ToInt16LittleEndian(byte[] buffer, int offset)
        {
            return (short)ToUInt16LittleEndian(buffer, offset);
        }

        /// <summary>
        /// Get a signed integer value from the given bytes. (by least significant byte)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int ToInt32LittleEndian(byte[] buffer, int offset)
        {
            return (int)ToUInt32LittleEndian(buffer, offset);
        }

        /// <summary>
        /// Get a signed long value from the given bytes. (by least significant byte)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static long ToInt64LittleEndian(byte[] buffer, int offset)
        {
            return (long)ToUInt64LittleEndian(buffer, offset);
        }

        /// <summary>
        /// Get an unsigned short value from the given bytes. (by most significant byte)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static ushort ToUInt16BigEndian(byte[] buffer, int offset)
        {
            return (ushort)(((buffer[offset] << 8) & 0xFF00) | ((buffer[offset + 1] << 0) & 0x00FF));
        }

        /// <summary>
        /// Get an unsigned integer value from the given bytes. (by most significant byte)
        /// </summary>
        /// <returns></returns>
        public static uint Bytes3ToUInt32BigEndian(byte byte1, byte byte2, byte byte3)
        {
            uint val = (uint)(((byte1 << 16) & 0x00FF0000U) | ((byte2 << 8) & 0x0000FF00U) |
                ((byte3 << 0) & 0x000000FFU));
            return val;
        }

        /// <summary>
        /// Get an unsigned integer value from the given bytes. (by most significant byte)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static uint ToUInt32BigEndian(byte[] buffer, int offset)
        {
            uint val = (uint)(((buffer[offset + 0] << 24) & 0xFF000000U) | ((buffer[offset + 1] << 16) & 0x00FF0000U)
                | ((buffer[offset + 2] << 8) & 0x0000FF00U) | ((buffer[offset + 3] << 0) & 0x000000FFU));
            return val;
        }

        /// <summary>
        /// Get an unsigned long value from the given bytes. (by most significant byte)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static ulong ToUInt64BigEndian(byte[] buffer, int offset)
        {
            return (((ulong)ToUInt32BigEndian(buffer, offset + 0)) << 32) | ToUInt32BigEndian(buffer, offset + 4);
        }

        /// <summary>
        /// Get an signed short value from the given bytes. (by most significant byte)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static short ToInt16BigEndian(byte[] buffer, int offset)
        {
            return (short)ToUInt16BigEndian(buffer, offset);
        }

        /// <summary>
        /// Get a signed integer value from the given bytes. (by most significant byte)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int ToInt32BigEndian(byte[] buffer, int offset)
        {
            return (int)ToUInt32BigEndian(buffer, offset);
        }

        /// <summary>
        /// Get a signed long value from the given bytes. (by most significant byte)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static long ToInt64BigEndian(byte[] buffer, int offset)
        {
            return (long)ToUInt64BigEndian(buffer, offset);
        }

        /// <summary>
        /// Get a Guid from the given bytes. (by least significant byte)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Guid ToGuidLittleEndian(byte[] buffer, int offset)
        {
            byte[] temp = new byte[16];
            Array.Copy(buffer, offset, temp, 0, 16);
            return new Guid(temp);
        }

        /// <summary>
        /// Get a Guid from the given bytes. (by most significant byte)
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Guid ToGuidBigEndian(byte[] buffer, int offset)
        {
            return new Guid(
                ToUInt32BigEndian(buffer, offset + 0),
                ToUInt16BigEndian(buffer, offset + 4),
                ToUInt16BigEndian(buffer, offset + 6),
                buffer[offset + 8],
                buffer[offset + 9],
                buffer[offset + 10],
                buffer[offset + 11],
                buffer[offset + 12],
                buffer[offset + 13],
                buffer[offset + 14],
                buffer[offset + 15]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(byte[] buffer, int offset, int length)
        {
            byte[] result = new byte[length];
            Array.Copy(buffer, offset, result, 0, length);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static T ToStruct<T>(byte[] buffer, int offset) where T : IByteArraySerializable, new()
        {
            T result = new T();
            result.ReadFrom(buffer, offset);
            return result;
        }

        /// <summary>
        /// Get a byte value for only the given bits.
        /// </summary>
        /// <param name="source">Source byte to get bits from</param>
        /// <param name="shiftValue">Number of bits to shift</param>
        /// <param name="mask">Bits to mask (out) of resulting value</param>
        /// <param name="rightShift">Flag indicating whether or not we want to right shift the result</param>
        /// <returns></returns>
        public static byte GetValueMask(byte source, int shiftValue, byte mask, bool rightShift = false)
        {
            if (!rightShift)
            {
                // bitwise AND mask, then return left shifted result
                source &= mask;
                return (byte)(source << shiftValue);
            }
            else
            {
                source &= mask;
                return (byte)(source >> shiftValue);
            }
        }

        /// <summary>
        /// Sets a byte value for the given bits.
        /// </summary>
        /// <param name="target">Target to set bits for</param>
        /// <param name="value">Source byte to get bits to set</param>
        /// <param name="shiftValue">Number of bits to shift</param>
        /// <param name="mask">Bits to mask (out) of resulting value</param>
        /// <returns></returns>
        public static void PutValueMask(ref byte target, byte value, int shiftValue, byte mask)
        {
            // left-shift value, bitwise AND mask on value and target, then OR value bytes on target
            value = (byte)(value << shiftValue);

            target &= (byte)~mask;
            value &= mask;
            target |= value;
        }

        /// <summary>
        /// Get individual bit value from a byte.
        /// </summary>
        /// <param name="b">Byte to get bit value for</param>
        /// <param name="i">Bit to get value for</param>
        /// <returns></returns>
        public static bool GetBit(byte b, int i)
        {
            // left-shift 1, then bitwise AND, then check for non-zero
            return ((b & (1 << i)) != 0);
        }

        /// <summary>
        /// Set individual bit value from a byte.
        /// </summary>
        /// <param name="b">Byte to get bit value for</param>
        /// <param name="i">Bit to set value for</param>
        /// <param name="value">Value to set bit to</param>
        /// <returns></returns>
        public static void SetBit(ref byte b, int i, bool value)
        {
            if (value)
            {
                // left-shift 1, then bitwise OR
                b = (byte)(b | (1 << i));
            }
            else
            {
                // left-shift 1, then take complement, then bitwise AND
                b = (byte)(b & ~(1 << i));
            }
        }

        /**
         * Stream Manipulation Methods
         */
        /// <summary>
        /// Read bytes until buffer filled or EOF.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="buffer">The buffer to populate.</param>
        /// <param name="offset">Offset in the buffer to start.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>The number of bytes actually read.</returns>
        public static int ReadFully(Stream stream, byte[] buffer, int offset, int length)
        {
            int totalRead = 0;
            int numRead = stream.Read(buffer, offset, length);
            while (numRead > 0)
            {
                totalRead += numRead;
                if (totalRead == length)
                    break;

                numRead = stream.Read(buffer, offset + totalRead, length - totalRead);
            }

            return totalRead;
        }

        /// <summary>
        /// Read bytes until buffer filled or throw IOException.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The data read from the stream.</returns>
        public static byte[] ReadFully(Stream stream, int count)
        {
            byte[] buffer = new byte[count];
            if (ReadFully(stream, buffer, 0, count) == count)
                return buffer;
            else
                throw new IOException("Unable to complete read of " + count + " bytes");
        }

        /// <summary>
        /// Reads a structure from a stream.
        /// </summary>
        /// <typeparam name="T">The type of the structure.</typeparam>
        /// <param name="stream">The stream to read.</param>
        /// <returns>The structure.</returns>
        public static T ReadStruct<T>(Stream stream) where T : IByteArraySerializable, new()
        {
            T result = new T();
            byte[] buffer = ReadFully(stream, result.Size);
            result.ReadFrom(buffer, 0);
            return result;
        }

        /// <summary>
        /// Reads a structure from a stream.
        /// </summary>
        /// <typeparam name="T">The type of the structure.</typeparam>
        /// <param name="stream">The stream to read.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>The structure.</returns>
        public static T ReadStruct<T>(Stream stream, int length) where T : IByteArraySerializable, new()
        {
            T result = new T();
            byte[] buffer = ReadFully(stream, length);
            result.ReadFrom(buffer, 0);
            return result;
        }

        /// <summary>
        /// Writes a structure to a stream.
        /// </summary>
        /// <typeparam name="T">The type of the structure.</typeparam>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="obj">The structure to write.</param>
        public static void WriteStruct<T>(Stream stream, T obj) where T : IByteArraySerializable
        {
            byte[] buffer = new byte[obj.Size];
            obj.WriteTo(buffer, 0);
            stream.Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Copies the contents of one stream to another.
        /// </summary>
        /// <param name="source">The stream to copy from.</param>
        /// <param name="dest">The destination stream.</param>
        /// <remarks>Copying starts at the current stream positions.</remarks>
        public static void PumpStreams(Stream source, Stream dest)
        {
            byte[] buffer = new byte[64 * 1024];

            int numRead = source.Read(buffer, 0, buffer.Length);
            while (numRead != 0)
            {
                dest.Write(buffer, 0, numRead);
                numRead = source.Read(buffer, 0, buffer.Length);
            }
        }
        
        /**
         * Path Manipulation
         */
        /// <summary>
        /// Extracts the directory part of a path.
        /// </summary>
        /// <param name="path">The path to process.</param>
        /// <returns>The directory part.</returns>
        public static string GetDirectoryFromPath(string path)
        {
            string trimmed = path.TrimEnd(Path.DirectorySeparatorChar);

            int index = trimmed.LastIndexOf(Path.DirectorySeparatorChar);
            if (index < 0)
                return string.Empty; // No directory, just a file name

            return trimmed.Substring(0, index);
        }

        /// <summary>
        /// Extracts the file part of a path.
        /// </summary>
        /// <param name="path">The path to process.</param>
        /// <returns>The file part of the path.</returns>
        public static string GetFileFromPath(string path)
        {
            string trimmed = path.Trim(Path.DirectorySeparatorChar);

            int index = trimmed.LastIndexOf(Path.DirectorySeparatorChar);
            if (index < 0)
                return trimmed; // No directory, just a file name

            return trimmed.Substring(index + 1);
        }

        /// <summary>
        /// Combines two paths.
        /// </summary>
        /// <param name="a">The first part of the path.</param>
        /// <param name="b">The second part of the path.</param>
        /// <returns>The combined path.</returns>
        public static string CombinePaths(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || (b.Length > 0 && b[0] == Path.DirectorySeparatorChar))
                return b;
            else if (string.IsNullOrEmpty(b))
                return a;
            else
                return a.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar + 
                    b.TrimStart(Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Resolves a relative path into an absolute one.
        /// </summary>
        /// <param name="basePath">The base path to resolve from.</param>
        /// <param name="relativePath">The relative path.</param>
        /// <returns>The absolute path, so far as it can be resolved.  If the
        /// <paramref name="relativePath"/> contains more '..' characters than the
        /// base path contains levels of directory, the resultant string will be relative.
        /// For example: (TEMP\Foo.txt, ..\..\Bar.txt) gives (..\Bar.txt).</returns>
        public static string ResolveRelativePath(string basePath, string relativePath)
        {
            List<string> pathElements = new List<string>(basePath.Split(new char[] { Path.DirectorySeparatorChar }, 
                StringSplitOptions.RemoveEmptyEntries));
            if (!basePath.EndsWith(string.Empty + Path.DirectorySeparatorChar, StringComparison.Ordinal) && pathElements.Count > 0)
                pathElements.RemoveAt(pathElements.Count - 1);

            pathElements.AddRange(relativePath.Split(new char[] { Path.DirectorySeparatorChar }, 
                StringSplitOptions.RemoveEmptyEntries));

            int pos = 1;
            while (pos < pathElements.Count)
            {
                if (pathElements[pos] == ".")
                {
                    pathElements.RemoveAt(pos);
                }
                else if (pathElements[pos] == ".." && pos > 0 && pathElements[pos - 1][0] != '.')
                {
                    pathElements.RemoveAt(pos);
                    pathElements.RemoveAt(pos - 1);
                    pos--;
                }
                else
                {
                    pos++;
                }
            }

            string merged = string.Join(string.Empty + Path.DirectorySeparatorChar, pathElements.ToArray());
            if (relativePath.EndsWith(string.Empty + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                merged += string.Empty + Path.DirectorySeparatorChar;

            if (basePath.StartsWith(string.Empty + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                merged = string.Empty + Path.DirectorySeparatorChar + merged;
            else if (basePath.StartsWith(string.Empty + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                merged = string.Empty + Path.DirectorySeparatorChar + merged;

            return merged;
        }

        /// <summary>
        /// Resolve the given path.
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ResolvePath(string basePath, string path)
        {
            if (!path.StartsWith(string.Empty + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                return ResolveRelativePath(basePath, path);
            else
                return path;
        }

        /// <summary>
        /// Make a relative path to the given base path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public static string MakeRelativePath(string path, string basePath)
        {
            List<string> pathElements = new List<string>(path.Split(new char[] { Path.DirectorySeparatorChar }, 
                StringSplitOptions.RemoveEmptyEntries));
            List<string> basePathElements = new List<string>(basePath.Split(new char[] { Path.DirectorySeparatorChar }, 
                StringSplitOptions.RemoveEmptyEntries));

            if (!basePath.EndsWith(string.Empty + Path.DirectorySeparatorChar, StringComparison.Ordinal) && basePathElements.Count > 0)
                basePathElements.RemoveAt(basePathElements.Count - 1);

            // find first part of paths that don't match
            int i = 0;
            while (i < Math.Min(pathElements.Count - 1, basePathElements.Count))
            {
                if (pathElements[i].ToUpperInvariant() != basePathElements[i].ToUpperInvariant())
                    break;

                ++i;
            }

            // for each remaining part of the base path, insert '..'
            StringBuilder result = new StringBuilder();
            if (i == basePathElements.Count)
                result.Append(@".\");
            else if (i < basePathElements.Count)
            {
                for (int j = 0; j < basePathElements.Count - i; ++j)
                    result.Append(@"..\");
            }

            // for each remaining part of the path, add the path element
            for (int j = i; j < pathElements.Count - 1; ++j)
            {
                result.Append(pathElements[j]);
                result.Append(string.Empty + Path.DirectorySeparatorChar);
            }

            result.Append(pathElements[pathElements.Count - 1]);

            // if the target was a directory, put the terminator back
            if (path.EndsWith(string.Empty + Path.DirectorySeparatorChar, StringComparison.Ordinal))
                result.Append(string.Empty + Path.DirectorySeparatorChar);

            return result.ToString();
        }
    } // public static class Util
} // namespace EightSixBoxVPN
