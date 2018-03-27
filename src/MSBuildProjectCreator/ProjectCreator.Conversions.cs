// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class ProjectCreator
    {
        /// <summary>
        /// Defines an implicit conversion of a <see cref="ProjectCreator"/> to a <see cref="Project"/>.
        /// </summary>
        /// <param name="creator">A <see cref="ProjectCreator"/> to convert.</param>
        public static implicit operator Project(ProjectCreator creator)
        {
            return creator?.Project;
        }

        /// <summary>
        /// Defines an implicit conversion of a <see cref="ProjectCreator"/> to a <see cref="ProjectRootElement"/>.
        /// </summary>
        /// <param name="creator">A <see cref="ProjectCreator"/> to convert.</param>
        public static implicit operator ProjectRootElement(ProjectCreator creator)
        {
            return creator.RootElement;
        }
    }
}