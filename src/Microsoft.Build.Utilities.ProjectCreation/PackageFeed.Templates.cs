// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class PackageFeed
    {
        /// <summary>
        /// Gets a set of project templates that can be used to generate complete projects.
        /// </summary>
        public static PackageFeedTemplates Templates { get; } = new PackageFeedTemplates();
    }
}