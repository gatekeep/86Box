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
    /// This class implements a local file locator.
    /// </summary>
    public sealed class LocalFileLocator : IFileLocator
    {
        /**
         * Fields
         */
        private string dir;

        /**
         * Methods
         */
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileLocator"/> class.
        /// </summary>
        /// <param name="dir"></param>
        public LocalFileLocator(string dir)
        {
            this.dir = dir;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileLocator"/> class.
        /// </summary>
        /// <param name="dir"></param>
        public LocalFileLocator(IFileLocator basePath, string dir)
        {
            this.dir = basePath.GetFullPath(dir);
        }

        /// <summary>
        /// Checks if the base path exists.
        /// </summary>
        /// <returns></returns>
        public override bool Exists()
        {
            return Directory.Exists(dir);
        }

        /// <summary>
        /// Checks if the given file exists.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override bool Exists(string fileName)
        {
            return File.Exists(Path.Combine(dir, fileName));
        }

        /// <summary>
        /// Checks if the given path exists.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override bool PathExists(string directoryName)
        {
            return Directory.Exists(Path.Combine(dir, directoryName));
        }

        /// <summary>
        /// Opens the given file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="mode"></param>
        /// <param name="access"></param>
        /// <param name="share"></param>
        /// <returns></returns>
        public override Stream Open(string fileName, FileMode mode, FileAccess access, FileShare share)
        {
            return new FileStream(Path.Combine(dir, fileName), mode, access, share);
        }

        /// <summary>
        /// Returns the relative file locator.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override IFileLocator GetRelativeLocator(string path)
        {
            return new LocalFileLocator(Path.Combine(dir, path));
        }

        /// <summary>
        /// Returns the full path.
        /// </summary>
        /// <returns></returns>
        public override string GetFullPath()
        {
            return Path.GetFullPath(dir);
        }

        /// <summary>
        /// Returns the full path (including file component).
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override string GetFullPath(string path)
        {
            string combinedPath = Path.Combine(dir, path);
            if (string.IsNullOrEmpty(combinedPath))
                return Environment.CurrentDirectory;
            else
                return Path.GetFullPath(combinedPath);
        }

        /// <summary>
        /// Gets the directory portion of the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override string GetDirectoryFromPath(string path)
        {
            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// Gets the file portion of the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override string GetFileFromPath(string path)
        {
            return Path.GetFileName(path);
        }

        /// <summary>
        /// Returns the last modified/write time of the given file path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override DateTime GetLastWriteTimeUtc(string path)
        {
            return File.GetLastWriteTimeUtc(Path.Combine(dir, path));
        }

        /// <summary>
        /// Checks if this locator has a common root with another locator.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool HasCommonRoot(IFileLocator other)
        {
            LocalFileLocator otherLocal = other as LocalFileLocator;
            if (otherLocal == null)
                return false;

            // if the paths have drive specifiers, then common root depends on them having a common
            // drive letter.
            string otherDir = otherLocal.dir;
            if (otherDir.Length >= 2 && dir.Length >= 2)
                if (otherDir[1] == ':' && dir[1] == ':')
                    return Char.ToUpperInvariant(otherDir[0]) == Char.ToUpperInvariant(dir[0]);

            return true;
        }

        /// <summary>
        /// Resolves a relative path to a full path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public override string ResolveRelativePath(string path)
        {
            return Util.ResolveRelativePath(dir, path);
        }
    } // public sealed class LocalFileLocator : IFileLocator
} // namespace EightSixBoxVPN
