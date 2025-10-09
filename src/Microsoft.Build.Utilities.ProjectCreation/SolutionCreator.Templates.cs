// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class SolutionCreator
    {
        /// <summary>
        /// Gets a set of templates that be used to generate complete Visual Studio solutions.
        /// </summary>
        public static SolutionCreatorTemplates Templates { get; } = new SolutionCreatorTemplates();
    }
}
