// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Represents Visual Studio solution templates.
    /// </summary>
    public partial class SolutionCreatorTemplates
    {
        /// <summary>
        /// Gets a Visual Studio solution for .NET projects that has "Debug" and "Release" configurations as well as an "Any CPU" platform.
        /// </summary>
        /// <param name="path">The path to the Visual Studio solution.</param>
        /// <returns>A <see cref="SolutionCreator" /> instance configured for .NET projects with "Debug" and "Release" configurations as well as an "Any CPU" platform.</returns>
        public SolutionCreator DotNet(string path)
        {
            return SolutionCreator.Create(path)
                .Configuration("Debug")
                .Configuration("Release")
                .Platform("Any CPU");
        }
    }
}
