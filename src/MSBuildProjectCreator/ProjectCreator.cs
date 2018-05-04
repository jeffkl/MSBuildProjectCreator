// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using System;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// A fluent API for generating MSBuild projects.
    /// </summary>
    /// <remarks>
    /// This API is generally useful for unit test projects that are testing MSBuild logic and need generated projects
    /// as part of their test input.
    /// </remarks>
    public partial class ProjectCreator
    {
        /// <summary>
        /// Stores the <see cref="Lazy{Project}"/> instance used to create a <see cref="Project"/> object lazily.
        /// </summary>
        private readonly Lazy<Project> _projectLazy;

        /// <summary>
        /// Stores the last top-level element added to the project XML.
        /// </summary>
        private ProjectElement _lastTopLevelElement;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectCreator"/> class.
        /// </summary>
        /// <param name="rootElement">The <see cref="ProjectRootElement"/> of the backing MSBuild project.</param>
        private ProjectCreator(ProjectRootElement rootElement)
        {
            RootElement = rootElement;

            _projectLazy = new Lazy<Project>(
                () => new Project(
                    RootElement,
                    ProjectCollection.GlobalProperties,
                    String.IsNullOrEmpty(RootElement.ToolsVersion) ? null : RootElement.ToolsVersion,
                    ProjectCollection),
                isThreadSafe: true);
        }

        /// <summary>
        /// Gets the full path to the project file. If the project has not been loaded from disk and has not been given a path, returns null. If the project has not been loaded from disk but has been given a path, this path may not exist. Setter renames the project, if it already had a name.
        /// </summary>
        public string FullPath => RootElement.FullPath;

        /// <summary>
        /// Gets the <see cref="Project"/> instance for the current project.  The project is re-evaluated if necessary every time this property is accessed.
        /// </summary>
        public Project Project
        {
            get
            {
                _projectLazy.Value.ReevaluateIfNecessary();

                return _projectLazy.Value;
            }
        }

        /// <summary>
        /// Gets the <see cref="ProjectCollection"/> for the current project.
        /// </summary>
        public ProjectCollection ProjectCollection { get; private set; }

        /// <summary>
        /// Gets the <see cref="ProjectRootElement"/> instance for the current project.
        /// </summary>
        public ProjectRootElement RootElement { get; }

        /// <summary>
        /// Gets the XML representing this project as a string.
        /// </summary>
        public string Xml => RootElement.RawXml;

        /// <summary>
        /// Creates a new <see cref="ProjectCreator"/> instance.
        /// </summary>
        /// <param name="path">An optional relative or full path for the project.</param>
        /// <param name="defaultTargets">An optional list of default targets for the project.</param>
        /// <param name="initialTargets">An optional list of initial targets for the project.</param>
        /// <param name="sdk">An optional SDK for the project.</param>
        /// <param name="toolsVersion">An optional tools version for the project.</param>
        /// <param name="treatAsLocalProperty">An optional list of properties to treat as local properties.</param>
        /// <param name="projectCollection">An optional <see cref="ProjectCollection"/> to use when loading the project.</param>
        /// <param name="projectFileOptions">An optional <see cref="NewProjectFileOptions"/> specifying options when creating a new file.</param>
        /// <returns>A <see cref="ProjectCreator"/> object that is used to construct an MSBuild project.</returns>
        public static ProjectCreator Create(
            string path = null,
            string defaultTargets = null,
            string initialTargets = null,
            string sdk = null,
            string toolsVersion = null,
            string treatAsLocalProperty = null,
            ProjectCollection projectCollection = null,
            NewProjectFileOptions? projectFileOptions = null)
        {
            if (projectCollection == null)
            {
                projectCollection = ProjectCollection.GlobalProjectCollection;
            }

            ProjectRootElement rootElement = projectFileOptions == null ? ProjectRootElement.Create(projectCollection) : ProjectRootElement.Create(projectCollection, projectFileOptions.Value);

            if (path != null)
            {
                rootElement.FullPath = path;
            }

            if (defaultTargets != null)
            {
                rootElement.DefaultTargets = defaultTargets;
            }

            if (initialTargets != null)
            {
                rootElement.InitialTargets = initialTargets;
            }

            if (sdk != null)
            {
                rootElement.Sdk = sdk;
            }

            rootElement.ToolsVersion = toolsVersion ?? String.Empty;

            if (treatAsLocalProperty != null)
            {
                rootElement.TreatAsLocalProperty = treatAsLocalProperty;
            }

            return new ProjectCreator(rootElement)
            {
                ProjectCollection = projectCollection
            };
        }

        /// <summary>
        /// Executes custom actions against the current <see cref="ProjectCreator"/>.
        /// </summary>
        /// <param name="projectCreator">An <see cref="Action{ProjectCreator}"/> delegate to execute against the current <see cref="ProjectCreator"/>.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator CustomAction(Action<ProjectCreator> projectCreator)
        {
            projectCreator?.Invoke(this);

            return this;
        }

        /// <summary>
        /// Saves the project to disk.
        /// </summary>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator Save()
        {
            RootElement.Save();

            return this;
        }

        /// <summary>
        /// Adds a top-level element to the project after the most recently added top-level element.
        /// </summary>
        /// <typeparam name="T">The type of the element.</typeparam>
        /// <param name="element">The element to add.</param>
        /// <returns>The element that was added.</returns>
        protected T AddTopLevelElement<T>(T element)
            where T : ProjectElement
        {
            if (_lastTopLevelElement == null)
            {
                RootElement.AppendChild(element);
            }
            else
            {
                RootElement.InsertAfterChild(element, _lastTopLevelElement);
            }

            _lastTopLevelElement = element;

            return element;
        }
    }
}