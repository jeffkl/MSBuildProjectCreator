// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Frameworks;
using NuGet.LibraryModel;
using NuGet.Versioning;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class PackageFeed
    {
        /// <summary>
        /// Adds a dependency to the current package.
        /// </summary>
        /// <param name="targetFramework">The target framework of the dependency.</param>
        /// <param name="id">The package ID of the dependency.</param>
        /// <param name="version">The minimum version of the dependency.</param>
        /// <param name="include">An optional comma delimited list of assets to include.  The default value is All.</param>
        /// <param name="exclude">An optional comma delimited list of assets to exclude.  The default value is None.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed Dependency(string targetFramework, string id, string version, string include = "All", string exclude = "None")
        {
            return Dependency(NuGetFramework.Parse(targetFramework), id, VersionRange.Parse(version), LibraryIncludeFlagUtils.GetFlags(include, LibraryIncludeFlags.All), LibraryIncludeFlagUtils.GetFlags(exclude, LibraryIncludeFlags.None));
        }

        /// <summary>
        /// Adds a dependency to the current package.
        /// </summary>
        /// <param name="targetFramework">The <see cref="NuGetFramework" /> of the dependency.</param>
        /// <param name="id">The package ID of the dependency.</param>
        /// <param name="versionRange">The <see cref="VersionRange" /> of the dependency.</param>
        /// <param name="include">A <see cref="LibraryIncludeFlags" /> value indicating which assets to include.  The default value is <see cref="LibraryIncludeFlags.All" />.</param>
        /// <param name="exclude">A <see cref="LibraryIncludeFlags" /> value indicating which assets to exclude.  The default value is <see cref="LibraryIncludeFlags.None" />.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed Dependency(NuGetFramework targetFramework, string id, VersionRange versionRange, LibraryIncludeFlags include = LibraryIncludeFlags.All, LibraryIncludeFlags exclude = LibraryIncludeFlags.None)
        {
            LastPackage.AddDependency(targetFramework, id, versionRange, include, exclude);

            return this;
        }
    }
}