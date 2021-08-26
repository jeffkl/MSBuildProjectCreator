// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Represents a file-based NuGet feed.
    /// </summary>
    public partial class PackageFeed : IDisposable
    {
        private readonly DirectoryInfo _rootPath;

        private PackageFeed(DirectoryInfo rootPath)
        {
            _rootPath = rootPath;

            _rootPath.Create();
        }

        /// <summary>
        /// Creates a new <see cref="PackageFeed" /> instance at the specified path.
        /// </summary>
        /// <param name="rootPath">The root directory to create the feed at.</param>
        /// <returns>A <see cref="PackageFeed" /> object used to create a package feed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rootPath" /> is <c>null</c>.</exception>
        public static PackageFeed Create(DirectoryInfo rootPath)
        {
            if (rootPath == null)
            {
                throw new ArgumentNullException(nameof(rootPath));
            }

            return new PackageFeed(rootPath);
        }

        /// <summary>
        /// Creates a new <see cref="PackageFeed" /> instance at the specified path.
        /// </summary>
        /// <param name="rootPath">The root directory to create the feed at.</param>
        /// <returns>A <see cref="PackageFeed" /> object used to create a package feed.</returns>
        /// /// <exception cref="ArgumentNullException"><paramref name="rootPath" /> is <c>null</c> or a string that is empty or consists only of whitespace.</exception>
        public static PackageFeed Create(string rootPath)
        {
            if (string.IsNullOrEmpty(rootPath))
            {
                throw new ArgumentNullException(nameof(rootPath));
            }

            return new PackageFeed(Directory.CreateDirectory(rootPath));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Save();
        }

        /// <summary>
        /// Creates all of the packages in the feed.
        /// </summary>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed Save()
        {
            foreach (Package package in _packages)
            {
                package.Save(_rootPath);
            }

            return this;
        }
    }
}