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
        /// <param name="label">An optional label to add to the import.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator Import(string project, string condition = null, string sdk = null, string sdkVersion = null, bool conditionOnExistence = false, string label = null)
        {
            _lastImport = AddTopLevelElement(RootElement.CreateImportElement(project));

            _lastImport.Condition = conditionOnExistence ? $"Exists('{project}')" : condition;

            _lastImport.Label = label;

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

        /// <summary>
        /// Adds an &lt;Import /&gt; element to the current project.
        /// </summary>
        /// <param name="projectCreator">A <see cref="ProjectCreator"/> to import.</param>
        /// <param name="condition">An optional condition to add to the import.</param>
        /// <param name="conditionOnExistence">An optional value indicating if a condition should be automatically added that checks if the specified project exists.</param>
        /// <param name="label">An optional label to add to the import.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator Import(ProjectCreator projectCreator, string condition = null, bool conditionOnExistence = false, string label = null)
        {
            return Import(projectCreator.FullPath, condition, null, null, conditionOnExistence, label: label);
        }

        /// <summary>
        /// Adds an &lt;Import /&gt; element to the current project.
        /// </summary>
        /// <param name="project">A <see cref="Project"/> to import.</param>
        /// <param name="condition">An optional condition to add to the import.</param>
        /// <param name="conditionOnExistence">An optional value indicating if a condition should be automatically added that checks if the specified project exists.</param>
        /// <param name="label">An optional label to add to the import.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator Import(Project project, string condition = null, bool conditionOnExistence = false, string label = null)
        {
            return Import(project.FullPath, condition, null, null, conditionOnExistence, label: label);
        }

        /// <summary>
        /// Adds an &lt;Import /&gt; element to the current project.
        /// </summary>
        /// <param name="projectRootElement">A <see cref="ProjectRootElement"/> to import.</param>
        /// <param name="condition">An optional condition to add to the import.</param>
        /// <param name="conditionOnExistence">An optional value indicating if a condition should be automatically added that checks if the specified project exists.</param>
        /// <param name="label">An optional label to add to the import.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator Import(ProjectRootElement projectRootElement, string condition = null, bool conditionOnExistence = false, string label = null)
        {
            return Import(projectRootElement.FullPath, condition, null, null, conditionOnExistence, label: label);
        }

        /// <summary>
        /// Adds an &lt;Import /&gt; element to the current project for the specified SDK.
        /// </summary>
        /// <param name="project">The project to import, usually Sdk.props or Sdk.targets.</param>
        /// <param name="name">The name of the SDK.</param>
        /// <param name="version">An optional version of the SDK.</param>
        /// <param name="condition">An optional condition to add to the import.</param>
        /// <param name="label">An optional label to add to the import.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ImportSdk(string project, string name, string version = null, string condition = null, string label = null)
        {
            return Import(project, condition, name, version, label: label);
        }
    }
}