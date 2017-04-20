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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace EightSixBoxVPN
{
    /// <summary>
    /// Implements the management core logger system.
    /// </summary>
    public class Messages
    {
        /**
         * Fields
         */
        public static bool DisplayToConsole = false;
        private static TextWriter tw;
        private const string LOG_TIME_FORMAT = "MM/dd/yyyy HH:mm:ss";

        private static ConsoleColor defColor = Console.ForegroundColor;

        /**
         * Methods
         */
        /// <summary>
        /// Sets up the trace logging.
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void SetupTextWriter(string directoryPath, string logFile)
        {
            tw = new StreamWriter(directoryPath + Path.DirectorySeparatorChar + logFile, false);
        }

        /// <summary>
        /// Writes a log entry to the text log.
        /// </summary>
        /// <param name="message"></param>
        public static void WriteLog(string message)
        {
            string logTime = DateTime.Now.ToString(LOG_TIME_FORMAT) + " ";
            if (tw != null)
            {
                tw.WriteLine(logTime + message);
                tw.Flush();
            }

#if TRACE
            System.Diagnostics.Trace.WriteLine(logTime + message);
#else
            if (DisplayToConsole)
            {
                Console.Write(logTime);
                
                if (message.StartsWith("MGT_WARN"))
                    Console.ForegroundColor = ConsoleColor.Yellow;
                else if (message.StartsWith("MGT_FAIL") || message.StartsWith("MGT_ERROR"))
                    Console.ForegroundColor = ConsoleColor.Red;
                
                Console.WriteLine(message);

                Console.ForegroundColor = defColor;
            }
#endif
        }

        /// <summary>
        /// Dumps all variables stored in a object.
        /// </summary>
        /// <param name="obj">Object to dump</param>
        /// <param name="recursion">Level of recursion</param>
        /// <param name="frame">StackTrace frame count</param>
        public static void var_dump(object obj, int recursion, int frame)
        {
            // protect the method against endless recursion
            if (recursion < 5)
            {
                // determine object type
                Type t = obj.GetType();

                // get array with properties for this object
                PropertyInfo[] properties = t.GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    try
                    {
                        // get the property value
                        object value = property.GetValue(obj, null);

                        // create indenting string to put in front of properties of a deeper level
                        // we'll need this when we display the property name and value
                        string indent = String.Empty;
                        string spaces = "|   ";
                        string trail = "|...";

                        if (recursion > 0)
                            indent = new StringBuilder(trail).Insert(0, spaces, recursion - 1).ToString();

                        if (value != null)
                        {
                            // if the value is a string, add quotation marks
                            string displayValue = value.ToString();
                            if (value is string) 
                                displayValue = String.Concat('"', displayValue, '"');

                            // add property name and value to return string
                            Trace(indent + property.Name + " = " + displayValue, frame);

                            try
                            {
                                if (!(value is ICollection))
                                {
                                    // call var_dump() again to list child properties
                                    // this throws an exception if the current property value
                                    // is of an unsupported type (eg. it has not properties)
                                    var_dump(value, recursion + 1, frame + 1);
                                }
                                else
                                {
                                    // the value is a collection (eg. it's an arraylist or generic list) 
                                    // so loop through its elements and dump their properties
                                    int elementCount = 0;
                                    foreach (object element in ((ICollection)value))
                                    {
                                        string elementName = String.Format("{0}[{1}]", property.Name, elementCount);
                                        indent = new StringBuilder(trail).Insert(0, spaces, recursion).ToString();

                                        // display the collection element name and type
                                        Trace(indent + elementName + " = " + element.ToString(), frame);

                                        // display the child properties
                                        var_dump(element, recursion + 2, frame + 1);
                                        elementCount++;
                                    }

                                    var_dump(value, recursion + 1, frame + 1);
                                }
                            }
                            catch { }
                        }
                        else
                        {
                            // add empty (null) property to return string
                            Trace(indent + property.Name + " = null", frame);
                        }
                    }
                    catch
                    {
                        // some properties will throw an exception on property.GetValue()
                        // i don't know exactly why this happens, so for now i will ignore them...
                    }
                }
            }
        }

        /// <summary>
        /// Writes the exception stack trace to the console/trace log
        /// </summary>
        /// <param name="throwable">Exception to obtain information from</param>
        /// <param name="reThrow"></param>
        public static void StackTrace(Exception throwable, bool reThrow = true)
        {
            StackTrace(string.Empty, throwable, reThrow);
        }

        /// <summary>
        /// Writes the exception stack trace to the console/trace log
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="throwable">Exception to obtain information from</param>
        /// <param name="reThrow"></param>
        public static void StackTrace(string msg, Exception throwable, bool reThrow = true)
        {
            MethodBase mb = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod();
            ParameterInfo[] param = mb.GetParameters();
            string funcParams = string.Empty;
            for (int i = 0; i < param.Length; i++)
                if (i < param.Length - 1)
                    funcParams += param[i].ParameterType.Name + ", ";
                else
                    funcParams += param[i].ParameterType.Name;

            Exception inner = throwable.InnerException;

            WriteLog("MGT_FAIL: caught an unrecoverable exception! " + msg);
            WriteLog("---- TRACE SNIP ----");
            WriteLog(throwable.Message + (inner != null ? " (Inner: " + inner.Message + ")" : ""));
            WriteLog(throwable.GetType().ToString());

            WriteLog("<" + mb.ReflectedType.Name + "::" + mb.Name + "(" + funcParams + ")>");
            WriteLog(throwable.Source);
            foreach (string str in throwable.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                WriteLog(str);
            if (inner != null)
                foreach (string str in throwable.StackTrace.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                    WriteLog("inner trace: " + str);
            WriteLog("---- TRACE SNIP ----");

            if (reThrow)
                throw throwable;
        }

        /// <summary>
        /// Writes a error trace message w/ calling function information.
        /// </summary>
        /// <param name='message'>Message to print</param>
        public static void WriteWarning(string message)
        {
            WriteLog("WARN: " + message);
        }

        /// <summary>
        /// Writes a error trace message w/ calling function information.
        /// </summary>
        /// <param name='message'>Message to print</param>
        public static void WriteError(string message)
        {
            WriteLog("ERROR: " + message);
        }

        /// <summary>
        /// Writes a trace message w/ calling function information.
        /// </summary>
        /// <param name="message">Message to print to debug window</param>
        public static void Trace(string message, int frame = 1)
        {
            string trace = string.Empty;
            MethodBase mb = new System.Diagnostics.StackTrace().GetFrame(frame).GetMethod();
            ParameterInfo[] param = mb.GetParameters();
            string funcParams = string.Empty;
            for (int i = 0; i < param.Length; i++)
                if (i < param.Length - 1)
                    funcParams += param[i].ParameterType.Name + ", ";
                else
                    funcParams += param[i].ParameterType.Name;

            trace += "<" + mb.ReflectedType.Name + "::" + mb.Name + "(" + funcParams + ")> ";
            trace += message;

            WriteLog(trace);
        }

        /// <summary>
        /// Perform a hex dump of a buffer.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buffer"></param>
        /// <param name="maxLength"></param>
        /// <param name="myTraceFilter"></param>
        /// <param name="startOffset"></param>
        public static void TraceHex(string message, byte[] buffer, int maxLength = 32, int startOffset = 0)
        {
            int bCount = 0, j = 0, lenCount = 0;

            // iterate through buffer printing all the stored bytes
            string traceMsg = message + " Off [" + j.ToString("X4") + "] -> [";
            for (int i = startOffset; i < buffer.Length; i++)
            {
                byte b = buffer[i];

                // split the message every 16 bytes...
                if (bCount == 16)
                {
                    traceMsg += "]";
                    Trace(traceMsg, 2);

                    bCount = 0;
                    j += 16;
                    traceMsg = message + " Off [" + j.ToString("X4") + "] -> [";
                }
                else
                    traceMsg += (bCount > 0) ? " " : "";

                traceMsg += b.ToString("X2");

                bCount++;
                    
                // increment the length counter, and check if we've exceeded the specified
                // maximum, then break the loop
                lenCount++;
                if (lenCount > maxLength)
                    break;
            }

            // if the byte count at this point is non-zero print the message
            if (bCount != 0)
            {
                traceMsg += "]";
                Trace(traceMsg, 2);
            }
        }
    } // public class Messages
} // namespace EightSixBoxVPN
