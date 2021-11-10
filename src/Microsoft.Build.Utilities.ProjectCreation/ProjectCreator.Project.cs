// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Microsoft.Build.Evaluation.Context;
using Microsoft.Build.Execution;
using System;
using System.Collections.Generic;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class ProjectCreator
    {
        /// <summary>
        /// Stores the global properties for the project.
        /// </summary>
        private readonly IDictionary<string, string>? _globalProperties;

        /// <summary>
        /// Stores the <see cref="Lazy{Project}" /> instance used to create a <see cref="Project" /> object lazily.
        /// </summary>
        private Project? _project;

        /// <summary>
        /// Stores the <see cref="ProjectInstance" /> for the current project.
        /// </summary>
        private ProjectInstance? _projectInstance;

        /// <summary>
        /// Gets the <see cref="Project" /> instance for the current project.  The project is re-evaluated if necessary every time this property is accessed.
        /// </summary>
        public Project Project
        {
            get
            {
                if (_project == null)
                {
                    TryGetProject(out _project, _globalProperties);
                }

                _project.ReevaluateIfNecessary();

                return _project;
            }
        }

        /// <summary>
        /// Gets the <see cref="ProjectInstance" /> for the current project.
        /// </summary>
        public ProjectInstance ProjectInstance => _projectInstance ??= Project.CreateProjectInstance();

        /// <summary>
        /// Gets a <see cref="Project" /> instance from the current project.
        /// </summary>
        /// <param name="project">Receives the <see cref="Project" /> instance.</param>
        /// <param name="globalProperties">Optional <see cref="IDictionary{String, String}" /> containing global properties.</param>
        /// <param name="toolsVersion">Optional tools version.</param>
        /// <param name="projectCollection">Optional <see cref="ProjectCollection" /> to use.  Defaults to <code>ProjectCollection.GlobalProjectCollection</code>.</param>
        /// <param name="projectLoadSettings">Optional <see cref="ProjectLoadSettings" /> to use.  Defaults to <see cref="ProjectLoadSettings.Default" />.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryGetProject(
            out Project project,
            IDictionary<string, string>? globalProperties = null,
            string? toolsVersion = null,
            ProjectCollection? projectCollection = null,
            ProjectLoadSettings projectLoadSettings = ProjectLoadSettings.Default)
        {
            project = new Project(
                RootElement,
                globalProperties,
                toolsVersion ?? (string.IsNullOrEmpty(RootElement.ToolsVersion) ? null : RootElement.ToolsVersion),
                projectCollection ?? ProjectCollection,
                projectLoadSettings);

            return this;
        }

        /// <summary>
        /// Gets a <see cref="Project" /> instance from the current project.
        /// </summary>
        /// <param name="project">Receives the <see cref="Project" /> instance.</param>
        /// <param name="buildOutput">Receives <see cref="BuildOutput" /> instance.</param>
        /// <param name="globalProperties">Optional <see cref="IDictionary{String, String}" /> containing global properties.</param>
        /// <param name="toolsVersion">Optional tools version.</param>
        /// <param name="projectCollection">Optional <see cref="ProjectCollection" /> to use.  Defaults to <code>ProjectCollection.GlobalProjectCollection</code>.</param>
        /// <param name="projectLoadSettings">Optional <see cref="ProjectLoadSettings" /> to use.  Defaults to <see cref="ProjectLoadSettings.Default" />.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryGetProject(
            out Project project,
            out BuildOutput buildOutput,
            IDictionary<string, string>? globalProperties = null,
            string? toolsVersion = null,
            ProjectCollection? projectCollection = null,
            ProjectLoadSettings projectLoadSettings = ProjectLoadSettings.Default)
        {
            buildOutput = BuildOutput.Create();

            projectCollection = projectCollection ?? new ProjectCollection();

            projectCollection.RegisterLogger(buildOutput);

            project = new Project(
                RootElement,
                globalProperties,
                toolsVersion,
                projectCollection,
                projectLoadSettings);

            return this;
        }

        /// <summary>
        /// Gets a <see cref="ProjectInstance" /> from the current project.
        /// </summary>
        /// <param name="projectInstance">Receives the <see cref="ProjectInstance" />.</param>
        /// <param name="projectInstanceSettings">Optional <see cref="ProjectInstanceSettings" /> to use when creating the project instance.</param>
        /// <param name="evaluationContext">Optional <see cref="EvaluationContext" /> to use when creating the project instance.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryGetProjectInstance(
            out ProjectInstance projectInstance,
            ProjectInstanceSettings projectInstanceSettings = ProjectInstanceSettings.None,
            EvaluationContext? evaluationContext = null)
        {
            projectInstance = Project.CreateProjectInstance(projectInstanceSettings, evaluationContext);

            return this;
        }
    }
}