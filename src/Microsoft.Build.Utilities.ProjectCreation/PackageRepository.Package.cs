// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class PackageRepository
    {
        private readonly HashSet<Package> _packages = new HashSet<Package>();

        /// <summary>
        /// Gets the current packages in the repository.
        /// </summary>
        public IReadOnlyCollection<Package> Packages => _packages;

        /// <summary>
        /// Gets or sets the most recent <see cref="ProjectCreation.Package" /> added to the repository.
        /// </summary>
        internal Package? LastPackage { get; set; }

        /// <summary>
        /// Gets the full path to the specified package in the current <see cref="PackageRepository" />.
        /// </summary>
        /// <param name="packageId">The ID of the package.</param>
        /// <param name="version">The version of the package.</param>
        /// <returns>The full path to the package in the repository.</returns>
        public string GetInstallPath(string packageId, string version)
        {
            return Path.Combine(GlobalPackagesFolder, packageId.ToLowerInvariant(), version.ToLowerInvariant());
        }

        /// <summary>
        /// Gets the full path to the specified package' <c>.nupsec</c> in the current <see cref="PackageRepository" />.
        /// </summary>
        /// <param name="packageId">The ID of the package.</param>
        /// <param name="version">The version of the package.</param>
        /// <returns>The full path to the package's <c>.nuspec</c> in the repository.</returns>
        public string GetManifestFilePath(string packageId, string version)
        {
            return Path.Combine(GetInstallPath(packageId, version), $"{packageId.ToLowerInvariant()}.nuspec");
        }

        /// <summary>
        /// Creates a new package.
        /// </summary>
        /// <param name="id">The name or ID of the package.</param>
        /// <param name="version">The semantic version of the package.</param>
        /// <param name="authors">An optional semicolon delimited list of authors of the package.  The default value is &quot;UserA&quot;</param>
        /// <param name="description">An optional description of the package.  The default value is &quot;Description&quot;</param>
        /// <param name="copyright">An optional copyright of the package.</param>
        /// <param name="developmentDependency">An optional value indicating whether or not the package is a development dependency.  The default value is <code>false</code>.</param>
        /// <param name="icon">An optional path in the package that should be used for the icon of the package.</param>
        /// <param name="iconUrl">An optional URL to the icon of the package.</param>
        /// <param name="language">An optional language of the package.</param>
        /// <param name="licenseUrl">An optional URL to the license of the package.</param>
        /// <param name="licenseExpression">An optional license expression.</param>
        /// <param name="licenseVersion">An optional license version.</param>
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
            string id,
            string version,
            string? authors = null,
            string? description = null,
            string? copyright = null,
            bool developmentDependency = false,
            string? icon = null,
            string? iconUrl = null,
            string? language = null,
            string? licenseUrl = null,
            string? licenseExpression = null,
            string? licenseVersion = null,
            string? owners = null,
            IEnumerable<string>? packageTypes = null,
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
            return Package(
                id,
                version,
                out Package _,
                authors,
                description,
                copyright,
                developmentDependency,
                icon,
                iconUrl,
                language,
                licenseUrl,
                licenseExpression,
                licenseVersion,
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
        /// <param name="id">The name or ID of the package.</param>
        /// <param name="version">The semantic version of the package.</param>
        /// <param name="package">Receives a <see cref="ProjectCreation.Package" /> object representing the package.</param>
        /// <param name="authors">An optional semicolon delimited list of authors of the package.  The default value is &quot;UserA&quot;</param>
        /// <param name="description">An optional description of the package.  The default value is &quot;Description&quot;</param>
        /// <param name="copyright">An optional copyright of the package.</param>
        /// <param name="developmentDependency">An optional value indicating whether or not the package is a development dependency.  The default value is <code>false</code>.</param>
        /// <param name="icon">An optional path in the package that should be used for the icon of the package.</param>
        /// <param name="iconUrl">An optional URL to the icon of the package.</param>
        /// <param name="language">An optional language of the package.</param>
        /// <param name="licenseUrl">An optional URL to the license of the package.</param>
        /// <param name="licenseExpression">An optional license expression.</param>
        /// <param name="licenseVersion">An optional license version.</param>
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
        /// <exception cref="ArgumentNullException"><paramref name="id" /> or <paramref name="version" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">A package with the same name and version has already been added to the repository.</exception>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Package(
            string id,
            string version,
            out Package package,
            string? authors = "UserA",
            string? description = "Description",
            string? copyright = null,
            bool developmentDependency = false,
            string? icon = null,
            string? iconUrl = null,
            string? language = null,
            string? licenseUrl = null,
            string? licenseExpression = null,
            string? licenseVersion = null,
            string? owners = null,
            IEnumerable<string>? packageTypes = null,
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
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentNullException(nameof(version));
            }

            string semVersion = SemVersion.Parse(version).ToString();

            DirectoryInfo directory = Directory.CreateDirectory(GetInstallPath(id, semVersion));

            package = new Package(id, semVersion, directory.FullName, authors, description!, copyright, developmentDependency, icon, iconUrl, language, licenseUrl, licenseExpression, licenseVersion, owners, packageTypes, projectUrl, releaseNotes, repositoryType, repositoryUrl, repositoryBranch, repositoryCommit, requireLicenseAcceptance, serviceable, summary, tags, title);

            if (!_packages.Add(package))
            {
                throw new InvalidOperationException($"A package with the ID '{id}' and version '{version}' has already been added to the repository");
            }

            LastPackage = package;

            SavePackageManifest();

            using (TextWriter? writer = System.IO.File.CreateText(Path.Combine(directory.FullName, ".nupkg.metadata")))
            {
                writer.WriteLine(
@"{ 
  ""version"": 2,
  ""contentHash"": """"
}");
            }

            return this;
        }

        private void SavePackageManifest()
        {
            if (LastPackage != null)
            {
                using (Stream? stream = System.IO.File.OpenWrite(GetManifestFilePath(LastPackage.Id, LastPackage.Version)))
                {
                    LastPackage.WriteNuspec(stream);
                }
            }
        }
    }
}