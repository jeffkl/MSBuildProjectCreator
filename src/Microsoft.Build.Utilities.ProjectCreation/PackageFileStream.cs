﻿// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Frameworks;
using NuGet.Packaging;
using System;
using System.IO;
using System.Runtime.Versioning;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Represents a file to add to a NuGet package.
    /// </summary>
    internal class PackageFileStream : IPackageFile
    {
        private readonly Func<Stream> _streamFunc;
        private string _path;

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageFileStream" /> class.
        /// </summary>
        /// <param name="targetPath">The relative path to the file in the package.</param>
        /// <param name="streamFunc">A <see cref="Func{TResult}" /> to be called to get a stream to the file.</param>
        public PackageFileStream(string targetPath, Func<Stream> streamFunc)
        {
            _path = string.Empty;
            EffectivePath = string.Empty;
            NuGetFramework = NuGetFramework.AnyFramework;
            TargetFramework = new FrameworkName(NuGetFramework.DotNetFrameworkName);

            Path = targetPath;
            _streamFunc = streamFunc ?? throw new ArgumentNullException(nameof(streamFunc));

            LastWriteTime = DateTimeOffset.Now;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PackageFileStream" /> class.
        /// </summary>
        /// <param name="targetPath">The relative path to the file in the package.</param>
        /// <param name="fileInfo">A <see cref="FileInfo" /> representing the file on disk.</param>
        public PackageFileStream(string targetPath, FileInfo fileInfo)
        : this(targetPath, fileInfo.OpenRead)
        {
            LastWriteTime = fileInfo.LastWriteTime;
        }

        /// <inheritdoc />
        public string EffectivePath { get; private set; }

        /// <inheritdoc />
        public DateTimeOffset LastWriteTime { get; }

        /// <inheritdoc />
        public NuGetFramework NuGetFramework { get; private set; }

        /// <inheritdoc />
        public string Path
        {
            get => _path;

            set
            {
                if (string.Equals(_path, value, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                _path = value;

                NuGetFramework = FrameworkNameUtility.ParseNuGetFrameworkFromFilePath(_path, out string effectivePath);

                if (NuGetFramework != null && NuGetFramework.Version.Major < 5)
                {
                    TargetFramework = new FrameworkName(NuGetFramework.DotNetFrameworkName);
                }

                EffectivePath = effectivePath;
            }
        }

        /// <inheritdoc />
        public FrameworkName TargetFramework { get; private set; }

        /// <inheritdoc />
        public Stream GetStream()
        {
            return _streamFunc();
        }
    }
}