// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using System.Xml.Linq;

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
        /// Defines an implicit conversion of a <see cref="ProjectCreator"/> to a <see cref="ProjectCollection"/>.
        /// </summary>
        /// <param name="creator">A <see cref="ProjectCreator"/> to convert.</param>
        public static implicit operator ProjectCollection(ProjectCreator creator)
        {
            return creator.ProjectCollection;
        }

        /// <summary>
        /// Defines an implicit conversion of a <see cref="ProjectCreator"/> to a <see cref="ProjectRootElement"/>.
        /// </summary>
        /// <param name="creator">A <see cref="ProjectCreator"/> to convert.</param>
        public static implicit operator ProjectRootElement(ProjectCreator creator)
        {
            return creator.RootElement;
        }

        /// <summary>
        /// Defines an implicit conversion of a <see cref="ProjectCreator"/> to a <see cref="string"/>.
        /// </summary>
        /// <param name="creator">A <see cref="ProjectCreator"/> to convert.</param>
        public static implicit operator string(ProjectCreator creator)
        {
            return creator.FullPath;
        }

        /// <summary>
        /// Defines an implicit conversion of a <see cref="ProjectCreator"/> to an <see cref="XDocument"/>.
        /// </summary>
        /// <param name="creator">A <see cref="ProjectCreator"/> to convert.</param>
        public static implicit operator XDocument(ProjectCreator creator)
        {
            return XDocument.Parse(creator.Xml);
        }
    }
}