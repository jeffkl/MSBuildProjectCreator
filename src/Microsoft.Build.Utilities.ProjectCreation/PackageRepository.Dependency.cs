// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class PackageRepository
    {
        /// <summary>
        /// Adds a dependency to the current package.
        /// </summary>
        /// <param name="package">The <see cref="Microsoft.Build.Utilities.ProjectCreation.Package" /> representing the package.</param>
        /// <param name="targetFramework">The target framework for the dependency.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Dependency(Package package, string targetFramework)
        {
            return Dependency(package, targetFramework, null, null);
        }

        /// <summary>
        /// Adds a dependency to the current package.
        /// </summary>
        /// <param name="package">The <see cref="Microsoft.Build.Utilities.ProjectCreation.Package" /> representing the package.</param>
        /// <param name="targetFramework">The target framework for the dependency.</param>
        /// <param name="include">An optional array of strings representing the assets to include for the dependency.</param>
        /// <param name="exclude">An optional array of strings representing the assets to exclude from the dependency.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Dependency(Package package, string targetFramework, string[]? include, string[]? exclude)
        {
            return Dependency(package.Id, package.Version, targetFramework, include, exclude);
        }

        /// <summary>
        /// Adds a dependency to the current package.
        /// </summary>
        /// <param name="id">The identifier of the dependency.</param>
        /// <param name="version">The <see cref="VersionRange" /> of the dependency.</param>
        /// <param name="targetFramework">The target framework for the dependency.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Dependency(string id, string version, string targetFramework)
        {
            return Dependency(id, version, targetFramework, null, null);
        }

        /// <summary>
        /// Adds a dependency to the current package.
        /// </summary>
        /// <param name="id">The identifier of the dependency.</param>
        /// <param name="version">The <see cref="VersionRange" /> of the dependency.</param>
        /// <param name="targetFramework">The target framework for the dependency.</param>
        /// <param name="include">An optional array of strings representing the assets to include for the dependency.</param>
        /// <param name="exclude">An optional array of strings representing the assets to exclude from the dependency.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Dependency(string id, string version, string targetFramework, string[]? include, string[]? exclude)
        {
            if (_packageManifest == null)
            {
                throw new InvalidOperationException(Strings.ErrorWhenAddingLibraryRequiresPackage);
            }

            NuGetFramework nuGetFramework = string.IsNullOrWhiteSpace(targetFramework) ? NuGetFramework.AnyFramework : NuGetFramework.Parse(targetFramework);

            _packageManifest.Metadata.DependencyGroups = _packageManifest.Metadata.DependencyGroups.Concat(new List<PackageDependencyGroup>
            {
                new PackageDependencyGroup(
                    nuGetFramework,
                    new List<PackageDependency>
                    {
                        new PackageDependency(
                            id,
                            VersionRange.Parse(version),
                            include,
                            exclude),
                    }),
            });

            _packageManifest.Save();

            return this;
        }
    }
}