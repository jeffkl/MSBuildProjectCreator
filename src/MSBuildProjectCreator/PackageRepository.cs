// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Configuration;
using NuGet.Packaging;
using System.IO;
using System.Linq;

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

            ISettings settings = new Settings(rootPath, Settings.DefaultSettingsFileName);

            SettingsUtility.SetConfigValue(settings, ConfigurationConstants.GlobalPackagesFolder, GlobalPackagesFolder);

            settings.Remove(ConfigurationConstants.PackageSources, settings.GetSection(ConfigurationConstants.PackageSources).Items.First());

            settings.AddOrUpdate(ConfigurationConstants.PackageSources, new ClearItem());

            settings.SaveToDisk();

            _versionFolderPathResolver = new VersionFolderPathResolver(GlobalPackagesFolder);
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