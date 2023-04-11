// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Represents a NuGet package repository.
    /// </summary>
    public partial class PackageRepository : IDisposable
    {
        private readonly string? _nugetPackagesGlobalFolderBackup = Environment.GetEnvironmentVariable("NUGET_PACKAGES");

        private PackageRepository(string rootPath, IEnumerable<Uri>? feeds = null)
        {
            GlobalPackagesFolder = Path.Combine(rootPath, ".nuget", "packages");

            _nugetPackagesGlobalFolderBackup = Environment.GetEnvironmentVariable("NUGET_PACKAGES");

            Environment.SetEnvironmentVariable("NUGET_PACKAGES", null);

            NuGetConfigPath = Path.Combine(rootPath, "NuGet.config");

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true,
            };

            using (XmlWriter? writer = XmlWriter.Create(NuGetConfigPath, settings))
            {
                writer.WriteStartElement("configuration");

                writer.WriteStartElement("config");
                writer.WriteStartElement("add");
                writer.WriteAttributeString("key", "globalPackagesFolder");
                writer.WriteAttributeString("value", GlobalPackagesFolder);
                writer.WriteEndElement(); // </add>
                writer.WriteEndElement(); // </config>

                writer.WriteStartElement("packageSources");
                writer.WriteElementString("clear", string.Empty);

                if (feeds != null)
                {
                    int i = 1;

                    foreach (Uri? feed in feeds.Where(i => i != null))
                    {
                        string feedName = feed.IsFile ? $"Local{i++}" : feed.Host;

                        writer.WriteStartElement("add");
                        writer.WriteAttributeString("key", feedName);
                        writer.WriteAttributeString("value", feed.IsFile ? feed.LocalPath : feed.ToString());
                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement(); // </packageSources>

                writer.WriteEndElement(); // </configuration>
            }
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
        /// Creates a new <see cref="PackageRepository" /> instance.
        /// </summary>
        /// <param name="rootPath">The root directory to create a package repository directory in.</param>
        /// <param name="feeds">Optional feeds to include in the configuration.</param>
        /// <returns>A <see cref="PackageRepository" /> object that is used to construct an NuGet package repository.</returns>
        public static PackageRepository Create(string rootPath, params Uri[] feeds) => new PackageRepository(rootPath, feeds);

        /// <inheritdoc cref="IDisposable.Dispose" />
        public void Dispose()
        {
            Environment.SetEnvironmentVariable("NUGET_PACKAGES", _nugetPackagesGlobalFolderBackup);
        }
    }
}