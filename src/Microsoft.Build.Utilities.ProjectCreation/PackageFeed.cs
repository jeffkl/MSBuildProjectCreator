// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

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
        /// Converts the current <see cref="PackageFeed" /> to a <see cref="Uri" />.
        /// </summary>
        /// <param name="packageFeed">The <see cref="PackageFeed" /> to convert.</param>
        public static implicit operator Uri(PackageFeed packageFeed) => new Uri(packageFeed._rootPath.FullName);

        /// <summary>
        /// Creates a new <see cref="PackageFeed" /> instance at the specified path.
        /// </summary>
        /// <param name="rootPath">The root directory to create the feed at.</param>
        /// <returns>A <see cref="PackageFeed" /> object used to create a package feed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="rootPath" /> is <see langword="null" />.</exception>
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
        /// /// <exception cref="ArgumentNullException"><paramref name="rootPath" /> is <see langword="null" /> or a string that is empty or consists only of whitespace.</exception>
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
                if (package.Saved)
                {
                    continue;
                }

                _rootPath.Create();

                package.FullPath = Path.Combine(_rootPath.FullName, package.FileName);

                using (FileStream fileStream = System.IO.File.OpenWrite(package.FullPath))
                {
                    using (ZipArchive nupkg = new ZipArchive(fileStream, ZipArchiveMode.Create))
                    {
                        foreach (KeyValuePair<string, Func<Stream>> file in package.Files)
                        {
                            using (Stream? stream = file.Value())
                            {
                                ZipArchiveEntry? entry = nupkg.CreateEntryFromStream(file.Key, stream);
                            }
                        }

                        ZipArchiveEntry? nuspec = nupkg.CreateEntry($"{package.Id.ToLowerInvariant()}.nuspec");

                        using (Stream nuspecStream = nuspec.Open())
                        {
                            package.WriteNuspec(nuspecStream);
                        }
                    }
                }

                package.Saved = true;
            }

            return this;
        }
    }
}