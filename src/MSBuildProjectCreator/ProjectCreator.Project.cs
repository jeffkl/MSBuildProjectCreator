// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class ProjectCreator
    {
        /// <summary>
        /// Stores the <see cref="Lazy{Project}"/> instance used to create a <see cref="Project"/> object lazily.
        /// </summary>
        private Project _project;

        /// <summary>
        /// Gets the <see cref="Project"/> instance for the current project.  The project is re-evaluated if necessary every time this property is accessed.
        /// </summary>
        public Project Project
        {
            get
            {
                if (_project == null)
                {
                    TryGetProject(out _project, ProjectCollection.GlobalProperties, string.IsNullOrEmpty(RootElement.ToolsVersion) ? null : RootElement.ToolsVersion, ProjectCollection);
                }

                _project.ReevaluateIfNecessary();

                return _project;
            }
        }

        /// <summary>
        /// Gets a <see cref="Project"/> instance from the current project.
        /// </summary>
        /// <param name="project">Receives the <see cref="Project"/> instance.</param>
        /// <param name="globalProperties">Optional <see cref="IDictionary{String, String}"/> containing global properties.</param>
        /// <param name="toolsVersion">Optional tools version.</param>
        /// <param name="projectCollection">Optional <see cref="ProjectCollection"/> to use.  Defaults to <code>ProjectCollection.GlobalProjectCollection</code>.</param>
        /// <param name="projectLoadSettings">Optional <see cref="ProjectLoadSettings"/> to use.  Defaults to <see cref="ProjectLoadSettings.Default"/>.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryGetProject(
            out Project project,
            IDictionary<string, string> globalProperties = null,
            string toolsVersion = null,
            ProjectCollection projectCollection = null,
            ProjectLoadSettings projectLoadSettings = ProjectLoadSettings.Default)
        {
            project = new Project(
                RootElement,
                globalProperties ?? projectCollection?.GlobalProperties,
                toolsVersion,
                projectCollection ?? ProjectCollection.GlobalProjectCollection,
                projectLoadSettings);

            return this;
        }

        /// <summary>
        /// Gets a <see cref="Project"/> instance from the current project.
        /// </summary>
        /// <param name="project">Receives the <see cref="Project"/> instance.</param>
        /// <param name="buildOutput">Receives <see cref="BuildOutput"/> instance.</param>
        /// <param name="globalProperties">Optional <see cref="IDictionary{String, String}"/> containing global properties.</param>
        /// <param name="toolsVersion">Optional tools version.</param>
        /// <param name="projectCollection">Optional <see cref="ProjectCollection"/> to use.  Defaults to <code>ProjectCollection.GlobalProjectCollection</code>.</param>
        /// <param name="projectLoadSettings">Optional <see cref="ProjectLoadSettings"/> to use.  Defaults to <see cref="ProjectLoadSettings.Default"/>.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryGetProject(
            out Project project,
            out BuildOutput buildOutput,
            IDictionary<string, string> globalProperties = null,
            string toolsVersion = null,
            ProjectCollection projectCollection = null,
            ProjectLoadSettings projectLoadSettings = ProjectLoadSettings.Default)
        {
            buildOutput = BuildOutput.Create();

            projectCollection = projectCollection ?? ProjectCollection ?? new ProjectCollection();

            projectCollection.RegisterLogger(buildOutput);

            project = new Project(
                RootElement,
                globalProperties ?? projectCollection.GlobalProperties,
                toolsVersion,
                projectCollection,
                projectLoadSettings);

            return this;
        }
    }
}