// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Construction;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class ProjectCreator
    {
        /// <summary>
        /// Stores the last import that was added.
        /// </summary>
        private ProjectImportElement _lastImport;

        /// <summary>
        /// Adds an &lt;Import /&gt; element to the current project.
        /// </summary>
        /// <param name="project">The path to the project to import.</param>
        /// <param name="condition">An optional condition to add to the import.</param>
        /// <param name="sdk">An optional SDK to add to the import.</param>
        /// <param name="sdkVersion">An optional SDK version to add to the import.</param>
        /// <param name="conditionOnExistence">An optional value indicating if a condition should be automatically added that checks if the specified project exists.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator Import(string project, string condition = null, string sdk = null, string sdkVersion = null, bool conditionOnExistence = false)
        {
            _lastImport = AddTopLevelElement(RootElement.CreateImportElement(project));

            _lastImport.Condition = conditionOnExistence ? $"Exists('{project}')" : condition;

            if (sdk != null)
            {
                _lastImport.Sdk = sdk;
            }

            if (sdkVersion != null)
            {
                _lastImport.Version = sdkVersion;
            }

            return this;
        }
    }
}