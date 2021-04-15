// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Configuration;
using NuGet.Packaging;
using System;
using System.IO;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Represents a NuGet package repository.
    /// </summary>
    public partial class PackageRepository
    {
        private readonly VersionFolderPathResolver _versionFolderPathResolver;

        private PackageManifest _packageManifest;

        private PackageRepository(string rootPath)
        {
            GlobalPackagesFolder = Path.Combine(rootPath, ".nuget", SettingsUtility.DefaultGlobalPackagesFolderPath);

            _versionFolderPathResolver = new VersionFolderPathResolver(GlobalPackagesFolder);

            Environment.SetEnvironmentVariable("NUGET_PACKAGES", GlobalPackagesFolder);
        }

        /// <summary>
        /// Gets the full path to the global packages folder.
        /// </summary>
        public string GlobalPackagesFolder { get; }

        /// <summary>
        /// Creates a new <see cref="PackageRepository" /> instance.
        /// </summary>
        /// <param name="rootPath">The root directory to create a package repository directory in.</param>
        /// <returns>A <see cref="PackageRepository" /> object that is used to construct an NuGet package repository.</returns>
        public static PackageRepository Create(string rootPath)
        {
            return new PackageRepository(rootPath);
        }
    }
}