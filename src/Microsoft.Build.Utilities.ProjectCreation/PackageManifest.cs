// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Common;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Represents the manifest of a package.
    /// </summary>
    internal class PackageManifest : Manifest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PackageManifest"/> class.
        /// </summary>
        /// <param name="fullPath">The full path to the manifest file.</param>
        /// <param name="name">The name or ID of the package.</param>
        /// <param name="version">The semantic version of the package.</param>
        /// <param name="authors">An optional semicolon delimited list of authors of the package.  The default value is &quot;UserA&quot;</param>
        /// <param name="description">An optional description of the package.  The default value is &quot;Description&quot;</param>
        /// <param name="copyright">An optional copyright of the package.</param>
        /// <param name="developmentDependency">An optional value indicating whether or not the package is a development dependency.  The default value is <code>false</code>.</param>
        /// <param name="icon">An optional path in the package that should be used for the icon of the package.</param>
        /// <param name="iconUrl">An optional URL to the icon of the package.</param>
        /// <param name="language">An optional language of the package.</param>
        /// <param name="licenseUrl">An optional URL to the license of the package.</param>
        /// <param name="licenseMetadata">An optional <see cref="LicenseMetadata" /> of the package.</param>
        /// <param name="owners">An optional semicolon delimited list of owners of the package.</param>
        /// <param name="packageTypes">An optional <see cref="IEnumerable{PackageType}" /> containing the package types of the package.</param>
        /// <param name="projectUrl">An optional URL to the project of the package.</param>
        /// <param name="releaseNotes">An optional value specifying release notes of the package.</param>
        /// <param name="repositoryType">An optional value specifying the type of source code repository of the package.</param>
        /// <param name="repositoryUrl">An optional value specifying the URL of the source code repository of the package.</param>
        /// <param name="repositoryBranch">An optional value specifying the branch of the source code repository of the package.</param>
        /// <param name="repositoryCommit">An optional value specifying the commit of the source code repository of the package.</param>
        /// <param name="requireLicenseAcceptance">An optional value indicating whether or not the package requires license acceptance  The default value is <code>false</code>.</param>
        /// <param name="serviceable">An option value indicating whether or not the package is serviceable.  The default value is <code>false</code>.</param>
        /// <param name="summary">An optional summary of the package.</param>
        /// <param name="tags">An optional set of tags of the package.</param>
        /// <param name="title">An optional title of the package.</param>
        public PackageManifest(
            string fullPath,
            string name,
            string version,
            string? authors = null,
            string? description = null,
            string? copyright = null,
            bool developmentDependency = false,
            string? icon = null,
            string? iconUrl = null,
            string? language = null,
            string? licenseUrl = null,
            LicenseMetadata? licenseMetadata = null,
            string? owners = null,
            IEnumerable<PackageType>? packageTypes = null,
            string? projectUrl = null,
            string? releaseNotes = null,
            string? repositoryType = null,
            string? repositoryUrl = null,
            string? repositoryBranch = null,
            string? repositoryCommit = null,
            bool requireLicenseAcceptance = false,
            bool serviceable = false,
            string? summary = null,
            string? tags = null,
            string? title = null)
            : base(
                GetManifestMetadata(
                    name,
                    version,
                    authors,
                    description,
                    copyright,
                    developmentDependency,
                    icon,
                    iconUrl,
                    language,
                    licenseUrl,
                    licenseMetadata,
                    owners,
                    packageTypes,
                    projectUrl,
                    releaseNotes,
                    repositoryType,
                    repositoryUrl,
                    repositoryBranch,
                    repositoryCommit,
                    requireLicenseAcceptance,
                    serviceable,
                    summary,
                    tags,
                    title))
        {
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                throw new ArgumentNullException(nameof(fullPath));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentNullException(nameof(version));
            }

            FullPath = fullPath;

            Directory = Path.GetDirectoryName(fullPath)!;

            Save();

            NupkgMetadataFileFormat.Write(
                Path.Combine(Directory, PackagingCoreConstants.NupkgMetadataFileExtension),
                new NupkgMetadataFile
                {
                    ContentHash = string.Empty,
                    Version = NupkgMetadataFileFormat.Version,
                });
        }

        /// <summary>
        /// Gets the full path to the directory of the package.
        /// </summary>
        public string Directory { get; }

        /// <summary>
        /// Gets the full path to the package manifest file.
        /// </summary>
        public string FullPath { get; }

        /// <summary>
        /// Adds a <see cref="PackageDependencyGroup" /> to the package manifest.
        /// </summary>
        /// <param name="targetFramework">The <see cref="NuGetFramework" /> of the package dependency group.</param>
        public void AddDependencyGroup(NuGetFramework targetFramework)
        {
            Metadata.DependencyGroups = Metadata.DependencyGroups.Concat(new List<PackageDependencyGroup>
            {
                new PackageDependencyGroup(targetFramework, Enumerable.Empty<PackageDependency>()),
            });

            Save();
        }

        /// <summary>
        /// Saves the package manifest file.
        /// </summary>
        public void Save()
        {
            System.IO.Directory.CreateDirectory(Directory);

            using (Stream stream = new FileStream(FullPath, FileMode.Create))
            {
                Save(stream);
            }
        }

        /// <summary>
        /// Gets the <see cref="ManifestMetadata" /> for a package.
        /// </summary>
        /// <summary>
        /// Initializes a new instance of the <see cref="PackageManifest"/> class.
        /// </summary>
        /// <param name="name">The name or ID of the package.</param>
        /// <param name="version">The semantic version of the package.</param>
        /// <param name="authors">An optional semicolon delimited list of authors of the package.  The default value is &quot;UserA&quot;</param>
        /// <param name="description">An optional description of the package.  The default value is &quot;Description&quot;</param>
        /// <param name="copyright">An optional copyright of the package.</param>
        /// <param name="developmentDependency">An optional value indicating whether or not the package is a development dependency.  The default value is <code>false</code>.</param>
        /// <param name="icon">An optional path in the package that should be used for the icon of the package.</param>
        /// <param name="iconUrl">An optional URL to the icon of the package.</param>
        /// <param name="language">An optional language of the package.</param>
        /// <param name="licenseUrl">An optional URL to the license of the package.</param>
        /// <param name="licenseMetadata">An optional <see cref="LicenseMetadata" /> of the package.</param>
        /// <param name="owners">An optional semicolon delimited list of owners of the package.</param>
        /// <param name="packageTypes">An optional <see cref="IEnumerable{PackageType}" /> containing the package types of the package.</param>
        /// <param name="projectUrl">An optional URL to the project of the package.</param>
        /// <param name="releaseNotes">An optional value specifying release notes of the package.</param>
        /// <param name="repositoryType">An optional value specifying the type of source code repository of the package.</param>
        /// <param name="repositoryUrl">An optional value specifying the URL of the source code repository of the package.</param>
        /// <param name="repositoryBranch">An optional value specifying the branch of the source code repository of the package.</param>
        /// <param name="repositoryCommit">An optional value specifying the commit of the source code repository of the package.</param>
        /// <param name="requireLicenseAcceptance">An optional value indicating whether or not the package requires license acceptance  The default value is <code>false</code>.</param>
        /// <param name="serviceable">An option value indicating whether or not the package is serviceable.  The default value is <code>false</code>.</param>
        /// <param name="summary">An optional summary of the package.</param>
        /// <param name="tags">An optional set of tags of the package.</param>
        /// <param name="title">An optional title of the package.</param>
        /// <returns>The <see cref="ManifestMetadata" /> for the package.</returns>
        private static ManifestMetadata GetManifestMetadata(
            string name,
            string version,
            string? authors = null,
            string? description = null,
            string? copyright = null,
            bool developmentDependency = false,
            string? icon = null,
            string? iconUrl = null,
            string? language = null,
            string? licenseUrl = null,
            LicenseMetadata? licenseMetadata = null,
            string? owners = null,
            IEnumerable<PackageType>? packageTypes = null,
            string? projectUrl = null,
            string? releaseNotes = null,
            string? repositoryType = null,
            string? repositoryUrl = null,
            string? repositoryBranch = null,
            string? repositoryCommit = null,
            bool requireLicenseAcceptance = false,
            bool serviceable = false,
            string? summary = null,
            string? tags = null,
            string? title = null)
        {
            ManifestMetadata metadata = new ManifestMetadata
            {
                Authors = MSBuildStringUtility.Split(authors ?? "UserA"),
                Copyright = copyright,
                Description = description ?? "Description",
                DevelopmentDependency = developmentDependency,
                Icon = icon,
                Id = name,
                Language = language,
                LicenseMetadata = licenseMetadata,
                Owners = string.IsNullOrWhiteSpace(owners) ? null : MSBuildStringUtility.Split(owners),
                PackageTypes = packageTypes ?? new[] { PackageType.Dependency },
                ReleaseNotes = releaseNotes,
                Repository = new RepositoryMetadata(repositoryType, repositoryUrl, repositoryBranch, repositoryCommit),
                RequireLicenseAcceptance = requireLicenseAcceptance,
                Serviceable = serviceable,
                Summary = summary,
                Tags = tags,
                Title = title,
                Version = NuGetVersion.Parse(version),
            };

            if (!string.IsNullOrWhiteSpace(iconUrl))
            {
                metadata.SetIconUrl(iconUrl);
            }

            if (!string.IsNullOrWhiteSpace(licenseUrl))
            {
                metadata.SetLicenseUrl(licenseUrl);
            }

            if (!string.IsNullOrWhiteSpace(projectUrl))
            {
                metadata.SetProjectUrl(projectUrl);
            }

            return metadata;
        }
    }
}