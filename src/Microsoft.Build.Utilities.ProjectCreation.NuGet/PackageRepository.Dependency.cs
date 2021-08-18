// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Common;
using NuGet.Frameworks;
using NuGet.LibraryModel;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Build.Utilities.ProjectCreation.NuGet
{
    public partial class PackageRepository
    {
        /// <summary>
        /// Adds a dependency to the current package.
        /// </summary>
        /// <param name="packageIdentity">The <see cref="PackageIdentity" /> of the dependency.</param>
        /// <param name="targetFramework">An optional target framework for the dependency.</param>
        /// <param name="include">An optional <see cref="LibraryIncludeFlags" /> representing the assets to include for the dependency.</param>
        /// <param name="exclude">An optional <see cref="LibraryIncludeFlags" /> representing the assets to exclude from the dependency.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Dependency(PackageIdentity packageIdentity, string targetFramework, LibraryIncludeFlags? include = null, LibraryIncludeFlags? exclude = null)
        {
            return Dependency(
                packageIdentity.Id,
                new VersionRange(packageIdentity.Version),
                targetFramework,
                include,
                exclude);
        }

        /// <summary>
        /// Adds a dependency to the current package.
        /// </summary>
        /// <param name="packageIdentity">The <see cref="PackageIdentity" /> of the dependency.</param>
        /// <param name="targetFramework">The <see cref="NuGetFramework" /> for the dependency.</param>
        /// <param name="include">An optional <see cref="LibraryIncludeFlags" /> representing the assets to include for the dependency.</param>
        /// <param name="exclude">An optional <see cref="LibraryIncludeFlags" /> representing the assets to exclude from the dependency.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Dependency(PackageIdentity packageIdentity, NuGetFramework targetFramework, LibraryIncludeFlags? include = null, LibraryIncludeFlags? exclude = null)
        {
            return Dependency(packageIdentity.Id, new VersionRange(packageIdentity.Version), targetFramework, include, exclude);
        }

        /// <summary>
        /// Adds a dependency to the current package.
        /// </summary>
        /// <param name="id">The identifier of the dependency.</param>
        /// <param name="version">The version of the dependency.</param>
        /// <param name="targetFramework">An optional target framework for the dependency.</param>
        /// <param name="include">An optional <see cref="LibraryIncludeFlags" /> representing the assets to include for the dependency.</param>
        /// <param name="exclude">An optional <see cref="LibraryIncludeFlags" /> representing the assets to exclude from the dependency.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Dependency(string id, string version, string targetFramework, LibraryIncludeFlags? include = null, LibraryIncludeFlags? exclude = null)
        {
            return Dependency(
                id,
                VersionRange.Parse(version),
                targetFramework,
                include,
                exclude);
        }

        /// <summary>
        /// Adds a dependency to the current package.
        /// </summary>
        /// <param name="id">The identifier of the dependency.</param>
        /// <param name="version">The <see cref="VersionRange" /> of the dependency.</param>
        /// <param name="targetFramework">An optional target framework for the dependency.</param>
        /// <param name="include">An optional <see cref="LibraryIncludeFlags" /> representing the assets to include for the dependency.</param>
        /// <param name="exclude">An optional <see cref="LibraryIncludeFlags" /> representing the assets to exclude from the dependency.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Dependency(string id, VersionRange version, string targetFramework, LibraryIncludeFlags? include = null, LibraryIncludeFlags? exclude = null)
        {
            return Dependency(
                id,
                version,
                string.IsNullOrWhiteSpace(targetFramework) ? null : NuGetFramework.Parse(targetFramework),
                include,
                exclude);
        }

        /// <summary>
        /// Adds a dependency to the current package.
        /// </summary>
        /// <param name="id">The identifier of the dependency.</param>
        /// <param name="version">The <see cref="VersionRange" /> of the dependency.</param>
        /// <param name="targetFramework">The <see cref="NuGetFramework" /> for the dependency.</param>
        /// <param name="include">An optional <see cref="LibraryIncludeFlags" /> representing the assets to include for the dependency.</param>
        /// <param name="exclude">An optional <see cref="LibraryIncludeFlags" /> representing the assets to exclude from the dependency.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Dependency(string id, VersionRange version, NuGetFramework targetFramework, LibraryIncludeFlags? include = null, LibraryIncludeFlags? exclude = null)
        {
            _packageManifest.Metadata.DependencyGroups = _packageManifest.Metadata.DependencyGroups.Concat(new List<PackageDependencyGroup>
            {
                new PackageDependencyGroup(
                    targetFramework ?? NuGetFramework.AnyFramework,
                    new List<PackageDependency>
                    {
                        new PackageDependency(
                            id,
                            version,
                            include == null ? null : MSBuildStringUtility.Split(include.ToString(), ','),
                            exclude == null ? null : MSBuildStringUtility.Split(exclude.ToString(), ',')),
                    }),
            });

            _packageManifest.Save();

            return this;
        }
    }
}