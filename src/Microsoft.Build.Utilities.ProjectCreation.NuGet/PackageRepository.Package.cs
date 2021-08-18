// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Build.Utilities.ProjectCreation.NuGet
{
    public partial class PackageRepository
    {
        private readonly HashSet<PackageIdentity> _packages = new HashSet<PackageIdentity>();

        /// <summary>
        /// Gets the current packages in the repository.
        /// </summary>
        public IReadOnlyCollection<PackageIdentity> Packages => _packages;

        /// <inheritdoc cref="global::NuGet.Packaging.VersionFolderPathResolver.GetInstallPath" />
        public string GetInstallPath(string packageId, NuGetVersion version)
        {
            return VersionFolderPathResolver.GetInstallPath(packageId, version);
        }

        /// <inheritdoc cref="global::NuGet.Packaging.VersionFolderPathResolver.GetManifestFilePath" />
        public string GetManifestFilePath(string packageId, NuGetVersion version)
        {
            return VersionFolderPathResolver.GetManifestFilePath(packageId, version);
        }

        /// <summary>
        /// Creates a new package.
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
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Package(
            string name,
            string version,
            string authors = null,
            string description = null,
            string copyright = null,
            bool developmentDependency = false,
            string icon = null,
            string iconUrl = null,
            string language = null,
            string licenseUrl = null,
            LicenseMetadata licenseMetadata = null,
            string owners = null,
            IEnumerable<PackageType> packageTypes = null,
            string projectUrl = null,
            string releaseNotes = null,
            string repositoryType = null,
            string repositoryUrl = null,
            string repositoryBranch = null,
            string repositoryCommit = null,
            bool requireLicenseAcceptance = false,
            bool serviceable = false,
            string summary = null,
            string tags = null,
            string title = null)
        {
            return Package(
                name,
                version,
                out PackageIdentity _,
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
                title);
        }

        /// <summary>
        /// Creates a new package.
        /// </summary>
        /// <param name="name">The name or ID of the package.</param>
        /// <param name="version">The semantic version of the package.</param>
        /// <param name="package">Receives the <see cref="PackageIdentity" /> of the package.</param>
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
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Package(
            string name,
            string version,
            out PackageIdentity package,
            string authors = null,
            string description = null,
            string copyright = null,
            bool developmentDependency = false,
            string icon = null,
            string iconUrl = null,
            string language = null,
            string licenseUrl = null,
            LicenseMetadata licenseMetadata = null,
            string owners = null,
            IEnumerable<PackageType> packageTypes = null,
            string projectUrl = null,
            string releaseNotes = null,
            string repositoryType = null,
            string repositoryUrl = null,
            string repositoryBranch = null,
            string repositoryCommit = null,
            bool requireLicenseAcceptance = false,
            bool serviceable = false,
            string summary = null,
            string tags = null,
            string title = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentNullException(nameof(version));
            }

            package = new PackageIdentity(name, NuGetVersion.Parse(version));

            string manifestFilePath = VersionFolderPathResolver.GetManifestFilePath(package.Id, package.Version);

            if (System.IO.File.Exists(manifestFilePath))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ErrorPackageAlreadyCreated, name, version));
            }

            _packageManifest = new PackageManifest(
                manifestFilePath,
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
                title);

            _packages.Add(package);

            return this;
        }
    }
}