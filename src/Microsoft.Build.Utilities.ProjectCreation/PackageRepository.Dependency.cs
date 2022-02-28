// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class PackageRepository
    {
        /// <summary>
        /// Adds a dependency to the current package.
        /// </summary>
        /// <param name="package">The <see cref="Microsoft.Build.Utilities.ProjectCreation.Package" /> representing the package.</param>
        /// <param name="targetFramework">The target framework for the dependency.</param>
        /// <param name="includeAssets">An optional array of strings representing the assets to include for the dependency.</param>
        /// <param name="excludeAssets">An optional array of strings representing the assets to exclude from the dependency.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Dependency(Package package, string targetFramework, string? includeAssets = "All", string? excludeAssets = "None")
        {
            return Dependency(package.Id, package.Version, targetFramework, includeAssets, excludeAssets);
        }

        /// <summary>
        /// Adds a dependency to the current package.
        /// </summary>
        /// <param name="id">The identifier of the dependency.</param>
        /// <param name="version">The version of the dependency.</param>
        /// <param name="targetFramework">The target framework for the dependency.</param>
        /// <param name="includeAssets">An optional array of strings representing the assets to include for the dependency.</param>
        /// <param name="excludeAssets">An optional array of strings representing the assets to exclude from the dependency.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Dependency(string id, string version, string targetFramework, string? includeAssets = "All", string? excludeAssets = "None")
        {
            if (LastPackage == null)
            {
                throw new InvalidOperationException(Strings.ErrorWhenAddingLibraryRequiresPackage);
            }

            LastPackage.AddTargetFramework(targetFramework);

            LastPackage.AddDependency(targetFramework, id, version, includeAssets, excludeAssets);

            SavePackageManifest();

            return this;
        }
    }
}