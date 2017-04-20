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
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.ServiceProcess;
using System.Threading;

using EightSixBoxVPN.Comm;

namespace EightSixBoxVPN
{
    /// <summary>
    /// This class serves as the entry point for the application.
    /// </summary>
    public class EmuService : ServiceBase
    {
        /**
         * Fields
         */
        public const string _ServiceName = "86BoxVPN-Standalone-Service";
        public const string ServiceInstallName = "86BoxVPN";
        private const string ServiceNamespace = "86BoxVPN.Service";
        private const string ServiceAssembly = "86BoxVPN";

        private CommManager commManager;

        public const string lockFile = "86boxVPN.lock";

        public static bool HasCompletedStartup = false;

        private static EmuService instance;
        
        /**
         * Methods
         */
        /// <summary>
        /// Initializes a new instance of the <see cref="EmuService"/> class.
        /// </summary>
        public EmuService()
        {
            instance = this;
            ServiceName = _ServiceName;
        }

        /// <summary>
        /// Occurs when the service is started.
        /// </summary>
        /// <param name="args">Arguments service started with</param>
        protected override void OnStart(string[] args)
        {
            List<string> extraArgs = new List<string>();

            // command line parameters
            OptionSet options = new OptionSet()
            {
                
            };

            // attempt to parse the commandline
            try
            {
                extraArgs = options.Parse(args);
            }
            catch (OptionException)
            {
                /* stub */
            }

            try
            {
                StartService();
            }
            catch (Exception e)
            {
                Messages.StackTrace(e, false);
                OnStop();
            }
        }

        /// <summary>
        /// Starts the management service.
        /// </summary>
        public void StartService()
        {
            commManager = new CommManager();
            commManager.Start();

            // create service lock file
            FileStream _lock = File.Open(lockFile, FileMode.Create);

            // create stream writer and write this processes PID to the lock file
            StreamWriter stream = new StreamWriter(_lock);
            Process proc = Process.GetCurrentProcess();
            stream.WriteLine(proc.Id);
            stream.Flush();

            // dispose the file stream
            _lock.Dispose();
            HasCompletedStartup = true;
            Messages.Trace("started service");
        }

        /// <summary>
        /// Occurs when the service is stopped.
        /// </summary>
        protected override void OnStop()
        {
            HasCompletedStartup = false;
            try
            {
                StopService();
            }
            catch (Exception e)
            {
                StopService();
                Messages.StackTrace(e, false);
            }
        }

        /// <summary>
        /// Stops the management service.
        /// </summary>
        public void StopService()
        {
            // stop comm manager
            if (commManager != null)
            {
                try
                {
                    commManager.Stop();
                    commManager = null;
                }
                catch (ThreadAbortException tae)
                {
                    Messages.StackTrace(tae, false);
                }
            }

            // delete process lock file
            File.Delete(lockFile);
            Messages.Trace("stopped service");
        }

        /// <summary>
        /// Force starts the service on the CLI.
        /// </summary>
        /// <param name="args"></param>
        public void ForceStartCLI(string[] args)
        {
            Messages.DisplayToConsole = true;
            OnStart(args);
        }

        /// <summary>
        /// Force stops the service running on the CLI.
        /// </summary>
        public void ForceStopCLI()
        {
            OnStop();
        }
    } // public class EmuService : ServiceBase
} // namespace EightSixBoxVPN
