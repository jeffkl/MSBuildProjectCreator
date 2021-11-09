// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.LibraryModel;
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
    /// Represents a package in a feed.
    /// </summary>
    public class Package : IComparer<Package>, IEqualityComparer<Package>, IComparable<Package>
    {
        private readonly HashSet<ManifestContentFiles> _contentFiles = new HashSet<ManifestContentFiles>();
        private readonly Dictionary<NuGetFramework, HashSet<PackageDependency>> _dependencies = new Dictionary<NuGetFramework, HashSet<PackageDependency>>();

        private readonly Dictionary<string, IPackageFile> _files = new Dictionary<string, IPackageFile>(StringComparer.OrdinalIgnoreCase);
        private readonly PackageBuilder _packageBuilder;

        private readonly HashSet<NuGetFramework> _targetFrameworks = new HashSet<NuGetFramework>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Package" /> class.
        /// </summary>
        /// <param name="id">The name or ID of the package.</param>
        /// <param name="version">The version of the package.</param>
        /// <param name="author">The author of the package.</param>
        /// <param name="description">The description of the package.</param>
        /// <param name="developmentDependency">A value indicating whether or not the package is a development dependency.</param>
        internal Package(string id, string version, string author, string description, bool developmentDependency)
        {
            _packageBuilder = new PackageBuilder(deterministic: false)
            {
                Authors = { author },
                Description = description,
                DevelopmentDependency = developmentDependency,
                Id = id,
                Version = NuGetVersion.Parse(version),
            };

            FileName = $"{_packageBuilder.Id}.{_packageBuilder.Version.ToNormalizedString()}{NuGetConstants.PackageExtension}";

            FullPath = string.Empty;
        }

        /// <summary>
        /// Gets the author of the package.
        /// </summary>
        public string Author => _packageBuilder.Authors.First();

        /// <summary>
        /// Gets the description of the package.
        /// </summary>
        public string Description => _packageBuilder.Description;

        /// <summary>
        /// Gets a value indicating whether or not the package is a development dependency.
        /// </summary>
        public bool DevelopmentDependency => _packageBuilder.DevelopmentDependency;

        /// <summary>
        /// Gets the file name of the package.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets the full path to the package if it has been saved, otherwise <c>null</c>.
        /// </summary>
        public string FullPath { get; private set; }

        /// <summary>
        /// Gets the name of the package.
        /// </summary>
        public string Id => _packageBuilder.Id;

        /// <summary>
        /// Gets the version of the package.
        /// </summary>
        public string Version => _packageBuilder.Version.ToNormalizedString();

        /// <summary>
        /// Gets a value indicating whether or not the package is saved.
        /// </summary>
        internal bool Saved { get; private set; }

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
        /// <param name="manifestContentFiles">A <see cref="ManifestContentFiles" /> object containing details about the content file.</param>
        internal void AddContentFile(ManifestContentFiles manifestContentFiles)
        {
            _contentFiles.Add(manifestContentFiles);
        }

        /// <summary>
        /// Adds the specified dependency to the package.
        /// </summary>
        /// <param name="targetFramework">The <see cref="NuGetFramework" /> of the dependency.</param>
        /// <param name="id">The package ID of the dependency.</param>
        /// <param name="versionRange">The <see cref="VersionRange" /> of the dependency.</param>
        /// <param name="include">The <see cref="LibraryIncludeFlags" /> of the assets to include.</param>
        /// <param name="exclude">The <see cref="LibraryIncludeFlags" /> of the assets to exclude.</param>
        /// <param name="suppressParent">The <see cref="LibraryIncludeFlags" /> of the assets to suppress from dependents.  The default is <see cref="LibraryIncludeFlags.Analyzers" /> | <see cref="LibraryIncludeFlags.Build" /> | <see cref="LibraryIncludeFlags.ContentFiles" />.</param>
        internal void AddDependency(NuGetFramework targetFramework, string id, VersionRange versionRange, LibraryIncludeFlags include = LibraryIncludeFlags.All, LibraryIncludeFlags exclude = LibraryIncludeFlags.None, LibraryIncludeFlags suppressParent = LibraryIncludeFlags.Analyzers | LibraryIncludeFlags.Build | LibraryIncludeFlags.ContentFiles)
        {
            _targetFrameworks.Add(targetFramework);

            if (!_dependencies.TryGetValue(targetFramework, out HashSet<PackageDependency>? packageDependencies))
            {
                packageDependencies = new HashSet<PackageDependency>();

                _dependencies[targetFramework] = packageDependencies;
            }

            List<string> includes = new List<string>();
            List<string> excludes = new List<string>();

            LibraryIncludeFlags effectiveInclude = include & ~exclude & ~suppressParent;

            if (effectiveInclude == LibraryIncludeFlags.All)
            {
                includes.Add(LibraryIncludeFlags.All.ToString());
            }
            else if (effectiveInclude.HasFlag(LibraryIncludeFlags.ContentFiles))
            {
                includes.AddRange(effectiveInclude.ToString().Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                if ((LibraryIncludeFlagUtils.NoContent & ~effectiveInclude) != LibraryIncludeFlags.None)
                {
                    excludes.AddRange((LibraryIncludeFlagUtils.NoContent & ~effectiveInclude).ToString().Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
                }
            }

            packageDependencies.Add(
                new PackageDependency(
                    id,
                    versionRange,
                    includes,
                    excludes));

            Saved = false;
        }

        /// <summary>
        /// Adds a file to the package.
        /// </summary>
        /// <param name="relativePath">The relative path of the file to add.</param>
        /// <param name="streamFunc">A <see cref="Func{TResult}" /> that generates the file's content.</param>
        internal void AddFile(string relativePath, Func<Stream> streamFunc)
        {
            AddFile(
                relativePath,
                new PackageFileStream(relativePath, streamFunc)
                {
                    Path = relativePath,
                });
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
                new PackageFileStream(relativePath, fileInfo)
                {
                    Path = relativePath,
                });
        }

        /// <summary>
        /// Adds the specified target framework to a package.
        /// </summary>
        /// <param name="targetFramework">The <see cref="NuGetFramework" /> to add.</param>
        internal void AddTargetFramework(NuGetFramework targetFramework)
        {
            _targetFrameworks.Add(targetFramework);

            Saved = false;
        }

        /// <summary>
        /// Saves the package to the specified directory.
        /// </summary>
        /// <param name="rootPath">A <see cref="DirectoryInfo" /> representing the directory to save the package to.</param>
        internal void Save(DirectoryInfo rootPath)
        {
            if (Saved)
            {
                return;
            }

            rootPath.Create();

            FullPath = Path.Combine(rootPath.FullName, FileName);

            _packageBuilder.DependencyGroups.Clear();
            _packageBuilder.Files.Clear();

            // TODO: clean up streams?

            using (Stream stream = File.Create(FullPath))
            {
                foreach (IPackageFile file in _files.Values)
                {
                    _packageBuilder.Files.Add(file);
                }

                foreach (NuGetFramework targetFramework in _targetFrameworks)
                {
                    if (!_dependencies.TryGetValue(targetFramework, out HashSet<PackageDependency>? packageDependencies))
                    {
                        packageDependencies = new HashSet<PackageDependency>();
                    }

                    _packageBuilder.DependencyGroups.Add(new PackageDependencyGroup(targetFramework, packageDependencies));
                }

                _packageBuilder.ContentFiles.AddRange(_contentFiles);

                _packageBuilder.Save(stream);
            }

            Saved = true;
        }

        /// <summary>
        /// Adds a file to the package.
        /// </summary>
        /// <param name="relativePath">The relative path of the file to add.</param>
        /// <param name="packageFile">An <see cref="IPackageFile" /> that represents a file to add to the package.</param>
        private void AddFile(string relativePath, IPackageFile packageFile)
        {
            if (packageFile.NuGetFramework != null)
            {
                AddTargetFramework(packageFile.NuGetFramework);
            }

            _files[relativePath] = packageFile;

            Saved = false;
        }
    }
}