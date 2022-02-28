// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System.IO;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Represents templates for package feeds.
    /// </summary>
    public class PackageFeedTemplates
    {
        /// <summary>
        /// Creates a package feed with a single package.
        /// </summary>
        /// <param name="rootPath">The root path to the package feed.</param>
        /// <param name="package">Receives the <see cref="Package" />in the feed.</param>
        /// <param name="id">An optional ID for the package.  The default is &quot;SomePackage&quot;.</param>
        /// <param name="version">An optional version for the package.  The default is &quot;1.0.0&quot;.</param>
        /// <param name="targetFramework">An optional version for the package.  The default is &quot;netstandard2.0&quot;</param>
        /// <returns>The <see cref="PackageFeed" /> containing the package.</returns>
        public PackageFeed SinglePackage(string rootPath, out Package package, string id = "SomePackage", string version = "1.0.0", string targetFramework = "netstandard2.0")
        {
            return SinglePackage(new DirectoryInfo(rootPath), out package, id, version, targetFramework);
        }

        /// <summary>
        /// Creates a package feed with a single package.
        /// </summary>
        /// <param name="rootPath">The root path to the package feed.</param>
        /// <param name="package">Receives the <see cref="Package" />in the feed.</param>
        /// <param name="id">An optional ID for the package.  The default is &quot;SomePackage&quot;.</param>
        /// <param name="version">An optional version for the package.  The default is &quot;1.0.0&quot;.</param>
        /// <param name="targetFramework">An optional version for the package.  The default is &quot;netstandard2.0&quot;</param>
        /// <returns>The <see cref="PackageFeed" /> containing the package.</returns>
        public PackageFeed SinglePackage(DirectoryInfo rootPath, out Package package, string id = "SomePackage", string version = "1.0.0", string targetFramework = "netstandard2.0")
        {
            return PackageFeed.Create(rootPath)
                .Package(id, version, out package)
                    .Library(targetFramework)
                .Save();
        }
    }
}
