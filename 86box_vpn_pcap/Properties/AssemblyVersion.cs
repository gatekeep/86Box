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
using System.Reflection;

namespace EightSixBoxVPN
{
    /// <summary>
    /// Static class to build the engine version string.
    /// </summary>
    public class AssemblyVersion
    {
        /**
         * Fields
         */
        public static string _NAME;
        public static string _VERSION;
        public static string _BUILD;
        public static string _BUILD_DATE;
        public static string _COPYRIGHT;
        public static string _VERSION_STRING;

        /**
         * Methods
         */
        /// <summary>
        /// Initializes static members of the AssemblyVersion class.
        /// </summary>
        static AssemblyVersion()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            AssemblyDescriptionAttribute asmDesc = asm.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0] as AssemblyDescriptionAttribute;
            AssemblyCopyrightAttribute asmCopyright = asm.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0] as AssemblyCopyrightAttribute;
            DateTime buildDate = new DateTime(2000, 1, 1).AddDays(asm.GetName().Version.Build).AddSeconds(asm.GetName().Version.Revision * 2);
            _NAME = asmDesc.Description;
            _VERSION = asm.GetName().Version.Major + "." + asm.GetName().Version.Minor;
            _BUILD = string.Empty + "B" + asm.GetName().Version.Build + "R" + asm.GetName().Version.Revision;
            _BUILD_DATE = buildDate.ToShortDateString() + " at " + buildDate.ToShortTimeString();
            _COPYRIGHT = asmCopyright.Copyright;

            _VERSION_STRING = AssemblyVersion._NAME + " " + AssemblyVersion._VERSION + " build " + AssemblyVersion._BUILD;
        }
    } // public class AssemblyVersion
} // namespace EightSixBoxVPN
