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

namespace EightSixBoxVPN
{
    /// <summary>
    /// Interface defines a class structure that can locate files.
    /// </summary>
    public abstract class IFileLocator
    {
        /**
         * Methods
         */
        /// <summary>
        /// Checks if the base path exists.
        /// </summary>
        /// <returns></returns>
        public abstract bool Exists();

        /// <summary>
        /// Checks if the given file exists.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public abstract bool Exists(string fileName);

        /// <summary>
        /// Checks if the given path exists.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public abstract bool PathExists(string directoryName);

        /// <summary>
        /// Opens the given file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="mode"></param>
        /// <param name="access"></param>
        /// <param name="share"></param>
        /// <returns></returns>
        public abstract Stream Open(string fileName, FileMode mode, FileAccess access, FileShare share);

        /// <summary>
        /// Returns the relative file locator.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract IFileLocator GetRelativeLocator(string path);

        /// <summary>
        /// Returns the full path.
        /// </summary>
        /// <returns></returns>
        public abstract string GetFullPath();

        /// <summary>
        /// Returns the full path (including file component).
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract string GetFullPath(string path);

        /// <summary>
        /// Gets the directory portion of the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract string GetDirectoryFromPath(string path);

        /// <summary>
        /// Gets the file portion of the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract string GetFileFromPath(string path);

        /// <summary>
        /// Returns the last modified/write time of the given file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract DateTime GetLastWriteTimeUtc(string path);

        /// <summary>
        /// Checks if this locator has a common root with another locator.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public abstract bool HasCommonRoot(IFileLocator other);

        /// <summary>
        /// Resolves a relative path to a full path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract string ResolveRelativePath(string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileLocator"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public string MakeRelativePath(IFileLocator fileLocator, string path)
        {
            if (!HasCommonRoot(fileLocator))
                return null;

            string ourFullPath = GetFullPath(string.Empty) + @"\";
            string otherFullPath = fileLocator.GetFullPath(path);

            return Util.MakeRelativePath(otherFullPath, ourFullPath);
        }
    } // public abstract class FileLocator
} // namespace EightSixBoxVPN
