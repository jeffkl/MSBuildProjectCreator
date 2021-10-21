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
            LastPackage.AddDependency(NuGetFramework.Parse(targetFramework), id, VersionRange.Parse(version), LibraryIncludeFlagUtils.GetFlags(include, LibraryIncludeFlags.All), LibraryIncludeFlagUtils.GetFlags(exclude, LibraryIncludeFlags.None));

            return this;
        }
    }
}