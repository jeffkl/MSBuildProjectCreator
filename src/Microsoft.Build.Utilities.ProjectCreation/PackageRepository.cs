// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Configuration;
using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Represents a NuGet package repository.
    /// </summary>
    public partial class PackageRepository : IDisposable
    {
        private readonly string? _nugetPackagesGlobalFolderBackup = Environment.GetEnvironmentVariable("NUGET_PACKAGES");

        private readonly PackageSourceProvider _packageSourceProvider;

        private readonly ISettings _settings;

        private PackageManifest? _packageManifest;

        private PackageRepository(string rootPath, IEnumerable<Uri>? feeds = null)
        {
            GlobalPackagesFolder = Path.Combine(rootPath, ".nuget", SettingsUtility.DefaultGlobalPackagesFolderPath);

            VersionFolderPathResolver = new VersionFolderPathResolver(GlobalPackagesFolder);

            Environment.SetEnvironmentVariable("NUGET_PACKAGES", null);

            _settings = new Settings(rootPath, Settings.DefaultSettingsFileName);

            NuGetConfigPath = _settings.GetConfigFilePaths().First();

            _packageSourceProvider = new PackageSourceProvider(_settings);

            foreach (string packageSourceName in _packageSourceProvider.LoadPackageSources().Select(i => i.Name))
            {
                _packageSourceProvider.RemovePackageSource(packageSourceName);
            }

            SettingsUtility.SetConfigValue(_settings, ConfigurationConstants.GlobalPackagesFolder, GlobalPackagesFolder);

            _settings.AddOrUpdate(ConfigurationConstants.PackageSources, new ClearItem());

            if (feeds != null)
            {
                foreach (Uri feed in feeds)
                {
                    _packageSourceProvider.AddPackageSource(new PackageSource(feed.Host, feed.ToString()));
                }
            }

            _settings.SaveToDisk();
        }

        /// <summary>
        /// Gets the full path to the global packages folder.
        /// </summary>
        public string GlobalPackagesFolder { get; }

        /// <summary>
        /// Gets the full path to the current NuGet.config.
        /// </summary>
        public string NuGetConfigPath { get; }

        /// <summary>
        /// Gets a <see cref="VersionFolderPathResolver" /> for the current package repository.
        /// </summary>
        private VersionFolderPathResolver VersionFolderPathResolver { get; }

        /// <summary>
        /// Creates a new <see cref="PackageRepository" /> instance.
        /// </summary>
        /// <param name="rootPath">The root directory to create a package repository directory in.</param>
        /// <param name="feeds">Optional feeds to include in the configuration.</param>
        /// <returns>A <see cref="PackageRepository" /> object that is used to construct an NuGet package repository.</returns>
        public static PackageRepository Create(string rootPath, IEnumerable<Uri>? feeds = null)
        {
            return new PackageRepository(rootPath, feeds);
        }

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            Environment.SetEnvironmentVariable("NUGET_PACKAGES", _nugetPackagesGlobalFolderBackup);
        }
    }
}