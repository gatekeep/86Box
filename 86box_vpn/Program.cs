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
using System.IO;
using System.ServiceProcess;
using System.Windows.Forms;

namespace EightSixBoxVPN
{
    /// <summary>
    /// This class serves as the entry point for the application.
    /// </summary>
    public sealed class Program
    {
        /**
         * Fields
         */
        public static IFileLocator CommonAppData = new LocalFileLocator(Environment.GetFolderPath(
            Environment.SpecialFolder.CommonApplicationData));
        public static IFileLocator ProgramCommonData = new LocalFileLocator(CommonAppData,
            EmuService.ServiceInstallName);

        /**
         * Methods
         */
        /// <summary>
        /// Internal helper to prints the program usage.
        /// </summary>
        private static void Usage(OptionSet p)
        {
            Console.WriteLine("usage: 86BoxVPN [--force-debug]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }

        /// <summary>
        /// Internal helper to execute the service in forced debug mode.
        /// </summary>
        /// <param name="args"></param>
        private static void ForceDebug(string[] args)
        {
            EmuService svc = new EmuService();
            svc.ForceStartCLI(args);

            Console.WriteLine(">>> Press Q to quit");

            while (Console.ReadKey().KeyChar.ToString().ToUpper() != "Q") ;

            svc.ForceStopCLI();
            Environment.Exit(0);
        }
#if WIN32
        /// <summary>
        /// Internal helper to install the server as a service on Windows computers.
        /// </summary>
        private static void InstallService()
        {
            ServiceInstaller installer = new ServiceInstaller();

            Console.WriteLine("Installing the 86BoxVPN Service");
            string programExe = AppDomain.CurrentDomain.FriendlyName;
            try
            {
                installer.InstallService(Environment.CurrentDirectory + Path.DirectorySeparatorChar +
                    programExe, EmuService.ServiceInstallName, "86Box VPN Server");
            }
            catch (Exception e)
            {
                Messages.WriteError("failed to install service");
                Messages.StackTrace(e, false);
            }
        }

        /// <summary>
        /// Internal helper to uninstall the server as a service on Windows computers.
        /// </summary>
        private static void UninstallService()
        {
            ServiceInstaller installer = new ServiceInstaller();

            Console.WriteLine("Uninstalling the 86BoxVPN Service");
            try
            {
                installer.UninstallService(EmuService.ServiceInstallName);
            }
            catch (Exception e)
            {
                Messages.WriteError("failed to uninstall service");
                Messages.StackTrace(e, false);
            }
        }
#endif
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main(string[] args)
        {
            List<string> extraArgs = new List<string>();
            bool showHelp = false, runForceDebug = false;

            // setup the common app path
            if (!CommonAppData.PathExists(EmuService.ServiceInstallName))
                Directory.CreateDirectory(CommonAppData.GetFullPath(EmuService.ServiceInstallName));

            // configure trace logger
            Messages.SetupTextWriter(ProgramCommonData.GetFullPath(), EmuService.ServiceInstallName + "-Trace.log");

            Console.WriteLine(AssemblyVersion._VERSION_STRING + " (Built: " + AssemblyVersion._BUILD_DATE + ")");
            Console.WriteLine(AssemblyVersion._COPYRIGHT + "., All Rights Reserved.");
            Console.WriteLine();

            if (args.Length == 0)
            {
                Console.Error.WriteLine("Do not start the 86BoxVPN as a normal program!");
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                { 
                    new EmuService() 
                };

                ServiceBase.Run(ServicesToRun);
            }

            // command line parameters
            OptionSet options = new OptionSet()
            {
                { "h|help", "show this message and exit", v => showHelp = v != null },
#if WIN32
                { "install-service", "install the server as a service on Windows computers", v => InstallService() },
                { "uninstall-service", "uninstall the server as a service on Windows computers", v => UninstallService() },
#endif
                { "force-debug", "force the server to run in debug mode", v => runForceDebug = v != null },
            };

            // attempt to parse the commandline
            try
            {
                extraArgs = options.Parse(args);
            }
            catch (OptionException)
            {
                Console.WriteLine("error: invalid arguments");
                Usage(options);
                Environment.Exit(1);
            }

            // show help
            if (showHelp)
                Usage(options);

            if (runForceDebug)
                ForceDebug(args);

            Environment.Exit(0);
        }
    } // public class Program
} // namespace 86BoxVPN
