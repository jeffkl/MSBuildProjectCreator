// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Versioning;
using System;
using System.Collections.Generic;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class PackageFeed
    {
        private readonly HashSet<Package> _packages = new HashSet<Package>();

        private Package? _lastPackage;

        /// <summary>
        /// Gets the packages in the feed.
        /// </summary>
        public IReadOnlyCollection<Package> Packages => _packages;

        /// <summary>
        /// Gets the last package added to the feed.
        /// </summary>
        internal Package LastPackage
        {
            get
            {
                if (_lastPackage == null)
                {
                    throw new InvalidOperationException(Strings.ErrorWhenAddingAnythingBeforePackage);
                }

                return _lastPackage;
            }
        }

        /// <summary>
        /// Adds a package to the current feed.
        /// </summary>
        /// <param name="id">The name or ID of the package</param>
        /// <param name="version">The version of the package.</param>
        /// <param name="author">The author of the package.</param>
        /// <param name="description">The description of the package.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        /// <exception cref="InvalidOperationException">A package with the same ID and version has already been added to the feed.</exception>
        public PackageFeed Package(string id, string version, string author = "Author", string description = "Description")
        {
            return Package(id, version, out _, author, description);
        }

        /// <summary>
        /// Adds a package to the current feed.
        /// </summary>
        /// <param name="id">The name or ID of the package</param>
        /// <param name="version">The version of the package.</param>
        /// <param name="package">Receives the <see cref="ProjectCreation.Package" /> object representing the package.</param>
        /// <param name="author">The author of the package.</param>
        /// <param name="description">The description of the package.</param>
        /// <param name="developmentDependency">A value indicating whether or not the package is a development dependency.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        /// <exception cref="InvalidOperationException">A package with the same ID and version has already been added to the feed.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="id" /> or <paramref name="version" /> are null.</exception>
        public PackageFeed Package(string id, string version, out Package package, string author = "Author", string description = "Description", bool developmentDependency = false)
        {
            package = new Package(
                id ?? throw new ArgumentNullException(nameof(id)),
                version ?? throw new ArgumentNullException(nameof(version)),
                author ?? throw new ArgumentNullException(nameof(author)),
                description ?? throw new ArgumentNullException(nameof(description)),
                developmentDependency);

            if (!_packages.Add(package))
            {
                throw new InvalidOperationException($"The NuGet feed already contains the package \"{package.Id}\" with version \"{package.Version}\"");
            }

            _lastPackage = package;

            return this;
        }
    }
}