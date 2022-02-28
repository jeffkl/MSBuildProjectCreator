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
    /// Represents a package in a feed.
    /// </summary>
    public class Package : IComparer<Package>, IEqualityComparer<Package>, IComparable<Package>
    {
        private readonly HashSet<PackageContentFileEntry> _contentFiles = new HashSet<PackageContentFileEntry>();

        private readonly Dictionary<string, HashSet<PackageDependency>> _dependencies = new Dictionary<string, HashSet<PackageDependency>>();

        private readonly Dictionary<string, Func<Stream>> _files = new Dictionary<string, Func<Stream>>(StringComparer.OrdinalIgnoreCase);

        private readonly HashSet<string> _targetFrameworks = new HashSet<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Package" /> class.
        /// </summary>
        /// <param name="id">The name or ID of the package.</param>
        /// <param name="version">The semantic version of the package.</param>
        /// <param name="directory">The directory of the package.</param>
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
        internal Package(
            string id,
            SemVersion version,
            string directory,
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
            Id = id;

            Version = version.ToString();

            Author = authors!;
            Description = description!;
            Copyright = copyright;
            DevelopmentDependency = developmentDependency;
            Icon = icon;
            IconUrl = iconUrl;
            Language = language;
            LicenseUrl = licenseUrl;
            LicenseExpression = licenseExpression;
            LicenseVersion = licenseVersion;
            Owners = owners;
            PackageTypes = packageTypes == null ? null : new List<string>(packageTypes);
            ProjectUrl = projectUrl;
            ReleaseNotes = releaseNotes;
            RepositoryType = repositoryType;
            RepositoryUrl = repositoryUrl;
            RepositoryBranch = repositoryBranch;
            RepositoryCommit = repositoryCommit;
            RequireLicenseAcceptance = requireLicenseAcceptance;
            Serviceable = serviceable;
            Summary = summary;
            Tags = tags;
            Title = title;

            Directory = directory;

            FileName = $"{Id.ToLowerInvariant()}.{Version.ToLowerInvariant()}.nupkg";

            FullPath = Path.Combine(directory, FileName);
        }

        /// <summary>
        /// Gets the author of the package.
        /// </summary>
        public string Author { get; }

        /// <summary>
        /// Gets the copyright of the package.
        /// </summary>
        public string? Copyright { get; }

        /// <summary>
        /// Gets the description of the package.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets a value indicating whether or not the package is a development dependency.
        /// </summary>
        public bool DevelopmentDependency { get; }

        /// <summary>
        /// Gets the directory of the package if its been saved, otherwise <c>null</c>.
        /// </summary>
        public string? Directory { get; }

        /// <summary>
        /// Gets the file name of the package.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets the full path to the package if it has been saved, otherwise <c>null</c>.
        /// </summary>
        public string? FullPath { get; internal set; }

        /// <summary>
        /// Gets the icon of the package.
        /// </summary>
        public string? Icon { get; }

        /// <summary>
        /// Gets the icon URL of the package.
        /// </summary>
        public string? IconUrl { get; }

        /// <summary>
        /// Gets the name of the package.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the language of the package.
        /// </summary>
        public string? Language { get; }

        /// <summary>
        /// Gets the license expression of the package.
        /// </summary>
        public string? LicenseExpression { get; }

        /// <summary>
        /// Gets the license URL of the package.
        /// </summary>
        public string? LicenseUrl { get; }

        /// <summary>
        /// Gets the license version of the package.
        /// </summary>
        public string? LicenseVersion { get; }

        /// <summary>
        /// Gets the owners of the package.
        /// </summary>
        public string? Owners { get; }

        /// <summary>
        /// Gets the package types of the package.
        /// </summary>
        public IReadOnlyCollection<string>? PackageTypes { get; }

        /// <summary>
        /// Gets the project URL of the package.
        /// </summary>
        public string? ProjectUrl { get; }

        /// <summary>
        /// Gets the release notes of the package.
        /// </summary>
        public string? ReleaseNotes { get; }

        /// <summary>
        /// Gets the repository branch of the package.
        /// </summary>
        public string? RepositoryBranch { get; }

        /// <summary>
        /// Gets the repository commit of the package.
        /// </summary>
        public string? RepositoryCommit { get; }

        /// <summary>
        /// Gets the repository type of the package.
        /// </summary>
        public string? RepositoryType { get; }

        /// <summary>
        /// Gets the repository URL of the package.
        /// </summary>
        public string? RepositoryUrl { get; }

        /// <summary>
        /// Gets a value indicating whether or not license acceptance is required.
        /// </summary>
        public bool RequireLicenseAcceptance { get; }

        /// <summary>
        /// Gets a value indicating whether or not the package is serviceable.
        /// </summary>
        public bool Serviceable { get; }

        /// <summary>
        /// Gets the summary of the package.
        /// </summary>
        public string? Summary { get; }

        /// <summary>
        /// Gets the tags of the package.
        /// </summary>
        public string? Tags { get; }

        /// <summary>
        /// Gets the title of the package.
        /// </summary>
        public string? Title { get; }

        /// <summary>
        /// Gets the version of the package.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets the files currently assocated with the package.
        /// </summary>
        internal IReadOnlyDictionary<string, Func<Stream>> Files => _files;

        /// <summary>
        /// Gets or sets a value indicating whether or not the package is saved.
        /// </summary>
        internal bool Saved { get; set; }

        /// <inheritdoc />
        public int Compare(Package? x, Package? y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (ReferenceEquals(null, y))
            {
                return 1;
            }

            if (ReferenceEquals(null, x))
            {
                return -1;
            }

            int idComparison = string.Compare(x.Id, y.Id, StringComparison.Ordinal);

            if (idComparison != 0)
            {
                return idComparison;
            }

            return x.Version.CompareTo(y.Version);
        }

        /// <inheritdoc />
        public int CompareTo(Package? other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (ReferenceEquals(null, other))
            {
                return 1;
            }

            int idComparison = string.Compare(Id, other.Id, StringComparison.Ordinal);

            if (idComparison != 0)
            {
                return idComparison;
            }

            return Version.CompareTo(other.Version);
        }

        /// <inheritdoc />
        public bool Equals(Package? x, Package? y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (ReferenceEquals(x, null))
            {
                return false;
            }

            if (ReferenceEquals(y, null))
            {
                return false;
            }

            if (x.GetType() != y.GetType())
            {
                return false;
            }

            return x.Id == y.Id && x.Version == y.Version;
        }

        /// <inheritdoc />
        public int GetHashCode(Package obj)
        {
            unchecked
            {
                return (obj.Id.GetHashCode() * 397) ^ obj.Version.GetHashCode();
            }
        }

        /// <summary>
        /// Adds a content file to the package.
        /// </summary>
        /// <param name="packageContentFile">A <see cref="PackageContentFileEntry" /> object containing details about the content file.</param>
        internal void AddContentFile(PackageContentFileEntry packageContentFile)
        {
            _contentFiles.Add(packageContentFile);
        }

        /// <summary>
        /// Adds the specified dependency to the package.
        /// </summary>
        /// <param name="targetFramework">The target framework of the dependency.</param>
        /// <param name="id">The package ID of the dependency.</param>
        /// <param name="version">The version of the dependency.</param>
        /// <param name="includeAssets">The assets to include.</param>
        /// <param name="excludeAssets">The assets to exclude.</param>
        /// <param name="privateAssets">The assets to suppress from dependents.  The default is Analyzers, Build, and ContentFiles.</param>
        internal void AddDependency(string targetFramework, string id, string version, string? includeAssets = "All", string? excludeAssets = "None", string privateAssets = PackageDependency.DefaultPrivateAssets)
        {
            _targetFrameworks.Add(targetFramework);

            if (!_dependencies.TryGetValue(targetFramework, out HashSet<PackageDependency>? packageDependencies))
            {
                packageDependencies = new HashSet<PackageDependency>();

                _dependencies[targetFramework] = packageDependencies;
            }

            packageDependencies.Add(
                new PackageDependency(
                    id,
                    version,
                    includeAssets,
                    excludeAssets,
                    privateAssets));

            Saved = false;
        }

        /// <summary>
        /// Adds a file to the package.
        /// </summary>
        /// <param name="relativePath">The relative path of the file to add.</param>
        /// <param name="streamFunc">A <see cref="Func{TResult}" /> that generates the file's content.</param>
        internal void AddFile(string relativePath, Func<Stream> streamFunc)
        {
            if (Path.IsPathRooted(relativePath))
            {
                throw new InvalidOperationException($"The specified path must be relative");
            }

            string[]? parts = relativePath.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, 3, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 3)
            {
                AddTargetFramework(parts[1]);
            }

            _files[relativePath] = streamFunc;

            Saved = false;
        }

        /// <summary>
        /// Adds a file to the package.
        /// </summary>
        /// <param name="relativePath">The relative path of the file to add.</param>
        /// <param name="fileInfo">A <see cref="FileInfo" /> that represents a file on disk.</param>
        internal void AddFile(string relativePath, FileInfo fileInfo)
        {
            AddFile(
                relativePath,
                () => File.OpenRead(fileInfo.FullName));
        }

        /// <summary>
        /// Adds the specified target framework to a package.
        /// </summary>
        /// <param name="targetFramework">The target framework to add.</param>
        internal void AddTargetFramework(string targetFramework)
        {
            if (!string.IsNullOrWhiteSpace(targetFramework))
            {
                _targetFrameworks.Add(targetFramework);

                Saved = false;
            }
        }

        internal void WriteNuspec(Stream stream)
        {
            using XmlWriter writer = XmlWriter.Create(
                stream,
                new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = true,
                });

            writer.WriteStartElement("package", "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd");
            writer.WriteStartElement("metadata");
            writer.WriteAttributeString("minClientVersion", "2.12");
            writer.WriteElementString("id", Id);
            writer.WriteElementString("version", Version);
            writer.WriteElementStringIfNotNull("title", Title);
            writer.WriteElementStringIfNotNull("authors", Author);
            writer.WriteElementStringIfNotNull("description", Description);
            writer.WriteElementStringIfNotNull("copyright", Copyright);
            writer.WriteElementStringIfNotNull("tags", Tags);
            writer.WriteElementStringIfNotNull("projectUrl", ProjectUrl);
            writer.WriteElementStringIfNotNull("requireLicenseAcceptance", RequireLicenseAcceptance.ToString());
            writer.WriteElementStringIfNotNull("licenseUrl", LicenseUrl);
            writer.WriteElementStringIfNotNull("icon", Icon);
            writer.WriteElementStringIfNotNull("iconUrl", IconUrl);
            writer.WriteElementStringIfNotNull("developmentDependency", DevelopmentDependency.ToString());
            writer.WriteElementStringIfNotNull("language", Language);
            writer.WriteElementStringIfNotNull("title", Title);
            writer.WriteElementStringIfNotNull("tags", Tags);
            writer.WriteElementStringIfNotNull("summary", Summary);
            writer.WriteElementStringIfNotNull("owners", Owners);
            writer.WriteElementStringIfNotNull("releaseNotes", ReleaseNotes);
            writer.WriteElementStringIfNotNull("serviceable", Serviceable.ToString());

            if (PackageTypes != null && PackageTypes.Any())
            {
                writer.WriteStartElement("packageTypes");

                foreach (string? packageType in PackageTypes)
                {
                    writer.WriteStartElement("packageType");
                    writer.WriteAttributeString("name", packageType);
                    writer.WriteEndElement(); // </packageType>
                }

                writer.WriteEndElement(); // </packageTypes>
            }

            if (LicenseExpression != null)
            {
                writer.WriteStartElement("license");
                writer.WriteAttributeString("type", "expression");
                writer.WriteAttributeStringIfNotNull("version", LicenseVersion);
                writer.WriteString(LicenseExpression);
                writer.WriteEndElement(); // </license>
            }

            if (RepositoryType != null)
            {
                writer.WriteStartElement("repository");
                writer.WriteAttributeString("type", RepositoryType);
                writer.WriteAttributeStringIfNotNull("url", RepositoryUrl);
                writer.WriteAttributeStringIfNotNull("commit", RepositoryCommit);
                writer.WriteAttributeStringIfNotNull("branch", RepositoryBranch);
                writer.WriteEndElement(); // </repository>
            }

            if (_targetFrameworks.Any())
            {
                writer.WriteStartElement("dependencies");

                foreach (string? targetFramework in _targetFrameworks)
                {
                    writer.WriteStartElement("group");
                    writer.WriteAttributeString("targetFramework", targetFramework);

                    if (_dependencies.TryGetValue(targetFramework, out HashSet<PackageDependency>? dependencies))
                    {
                        foreach (PackageDependency dependency in dependencies)
                        {
                            writer.WriteStartElement("dependency");
                            writer.WriteAttributeString("id", dependency.Id);
                            writer.WriteAttributeString("version", dependency.Version);
                            writer.WriteAttributeString("exclude", dependency.ExcludeAssets);
                            writer.WriteEndElement();
                        }
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement(); // </dependencies>
            }

            if (_contentFiles.Any())
            {
                writer.WriteStartElement("contentFiles");

                foreach (PackageContentFileEntry? contentFilesEntry in _contentFiles)
                {
                    writer.WriteStartElement("files");
                    writer.WriteAttributeStringIfNotNull("include", contentFilesEntry.Include);
                    writer.WriteAttributeStringIfNotNull("exclude", contentFilesEntry.Exclude);
                    writer.WriteAttributeString("copyToOutput", contentFilesEntry.CopyToOutput.ToString());
                    writer.WriteAttributeStringIfNotNull("flatten", contentFilesEntry.Flatten.ToString());
                    writer.WriteAttributeStringIfNotNull("buildAction", contentFilesEntry.BuildAction);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement(); // </contentFiles>
            }

            writer.WriteEndElement(); // </metadata>
            writer.WriteEndElement(); // </package>

            writer.Flush();
        }
    }
}