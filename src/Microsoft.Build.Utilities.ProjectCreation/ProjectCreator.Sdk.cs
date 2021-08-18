// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class ProjectCreator
    {
        /// <summary>
        /// Adds an &lt;Sdk /&gt; element to the current project.
        /// </summary>
        /// <param name="name">The name of the SDK.</param>
        /// <param name="version">An optional version of the SDK.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator Sdk(string name, string version = null)
        {
            AddTopLevelElement(RootElement.CreateProjectSdkElement(name, version));

            return this;
        }
    }
}