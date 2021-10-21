// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System.Collections.Generic;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class ProjectCreator
    {
        /// <summary>
        /// Adds a &lt;PackageReference /&gt; item to the current item group.
        /// </summary>
        /// <param name="package">The <see cref="Package" /> of the package to reference.</param>
        /// <param name="includeAssets">An optional value specifying which assets belonging to the package should be consumed.</param>
        /// <param name="excludeAssets">An optional value specifying which assets belonging to the package should be not consumed.</param>
        /// <param name="privateAssets">An optional value specifying which assets belonging to the package should not flow to dependent projects.</param>
        /// <param name="metadata">An optional <see cref="IDictionary{String,String}"/> containing metadata for the item.</param>
        /// <param name="condition">An optional condition to add to the item.</param>
        /// <param name="label">An optional label to add to the item.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemPackageReference(Package package, string? includeAssets = null, string? excludeAssets = null, string? privateAssets = null, IDictionary<string, string?>? metadata = null, string? condition = null, string? label = null)
        {
            return ItemPackageReference(package.Id, package.Version, includeAssets, excludeAssets, privateAssets, metadata, condition, label);
        }
    }
}