// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class PackageRepository
    {
        private PackageRepository(string rootPath, IEnumerable<Uri> feeds = null)
                    : this(rootPath)
        {
            Settings = new Settings(rootPath, NuGet.Configuration.Settings.DefaultSettingsFileName);

            NuGetConfigPath = Settings.GetConfigFilePaths().First();

            PackageSourceProvider = new PackageSourceProvider(Settings);

            foreach (string packageSourceName in PackageSourceProvider.LoadPackageSources().Select(i => i.Name))
            {
                PackageSourceProvider.RemovePackageSource(packageSourceName);
            }

            SettingsUtility.SetConfigValue(Settings, ConfigurationConstants.GlobalPackagesFolder, GlobalPackagesFolder);

            Settings.AddOrUpdate(ConfigurationConstants.PackageSources, new ClearItem());

            if (feeds != null)
            {
                foreach (Uri feed in feeds)
                {
                    PackageSourceProvider.AddPackageSource(new PackageSource(feed.Host, feed.ToString()));
                }
            }

            Settings.SaveToDisk();
        }

        /// <summary>
        /// Gets the full path to the current NuGet.config.
        /// </summary>
        public string NuGetConfigPath { get; }

        /// <summary>
        /// Gets a <see cref="PackageSourceProvider" /> for the current NuGet configuration.
        /// </summary>
        public PackageSourceProvider PackageSourceProvider { get; }

        /// <summary>
        /// Gets the current NuGet configuration.
        /// </summary>
        public ISettings Settings { get; }

        /// <summary>
        /// Adds a feed to the current <see cref="PackageRepository" />.
        /// </summary>
        /// <param name="uri">The <see cref="Uri" /> of the feed.</param>
        /// <param name="name">An optional name for the feed.</param>
        /// <param name="additionalAttributes">Optional attributes for the feed.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Feed(Uri uri, string name = default, IReadOnlyDictionary<string, string> additionalAttributes = default)
        {
            Feed(new PackageSource(uri.ToString(), name ?? uri.Host));

            Settings.SaveToDisk();

            return this;
        }

        /// <summary>
        /// Adds a feed to the current <see cref="PackageRepository" />.
        /// </summary>
        /// <param name="packageSource">A <see cref="PackageSource" /> object containing details about the feed.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Feed(PackageSource packageSource)
        {
            PackageSourceProvider.AddPackageSource(packageSource);

            return this;
        }

        /// <summary>
        /// Adds a feed to the current <see cref="PackageRepository" />.
        /// </summary>
        /// <param name="directory">The <see cref="DirectoryInfo" /> of the feed.</param>
        /// <param name="name">An optional name for the feed.</param>
        /// <param name="additionalAttributes">Optional attributes for the feed.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Feed(DirectoryInfo directory, string name = default, IReadOnlyDictionary<string, string> additionalAttributes = default)
        {
            return Feed(new Uri(directory.FullName), name, additionalAttributes);
        }

        /// <summary>
        /// Adds a feed to the current <see cref="PackageRepository" />.
        /// </summary>
        /// <param name="uri">The URI of the feed.</param>
        /// <param name="name">An optional name for the feed.</param>
        /// <param name="additionalAttributes">Optional attributes for the feed.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Feed(string uri, string name = default, IReadOnlyDictionary<string, string> additionalAttributes = default)
        {
            return Feed(new Uri(uri), name, additionalAttributes);
        }
    }
}