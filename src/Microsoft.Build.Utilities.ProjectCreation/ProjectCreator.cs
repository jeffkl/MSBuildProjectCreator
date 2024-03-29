﻿// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.IO;

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
        /// Stores the last top-level element added to the project XML.
        /// </summary>
        private ProjectElement? _lastTopLevelElement;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectCreator" /> class.
        /// </summary>
        /// <param name="rootElement">The <see cref="ProjectRootElement" /> of the backing MSBuild project.</param>
        /// <param name="globalProperties">An <see cref="IDictionary{String,String}" /> containing global properties to use when creating the project.</param>
        private ProjectCreator(ProjectRootElement rootElement, IDictionary<string, string>? globalProperties)
        {
            RootElement = rootElement;

            _globalProperties = globalProperties;

            ProjectCollection = ProjectCollection.GlobalProjectCollection;
        }

        /// <summary>
        /// Gets the full path to the project file. If the project has not been loaded from disk and has not been given a path, returns null. If the project has not been loaded from disk but has been given a path, this path may not exist. Setter renames the project, if it already had a name.
        /// </summary>
        public string FullPath => RootElement.FullPath;

        /// <summary>
        /// Gets the <see cref="ProjectCollection" /> for the current project.
        /// </summary>
        public ProjectCollection ProjectCollection { get; private set; }

        /// <summary>
        /// Gets the <see cref="ProjectRootElement" /> instance for the current project.
        /// </summary>
        public ProjectRootElement RootElement { get; }

        /// <summary>
        /// Gets the XML representing this project as a string.
        /// </summary>
        public string Xml => RootElement.RawXml;

        /// <summary>
        /// Creates a new <see cref="ProjectCreator" /> instance.
        /// </summary>
        /// <param name="path">An optional relative or full path for the project.</param>
        /// <param name="defaultTargets">An optional list of default targets for the project.</param>
        /// <param name="initialTargets">An optional list of initial targets for the project.</param>
        /// <param name="sdk">An optional SDK for the project.</param>
        /// <param name="toolsVersion">An optional tools version for the project.</param>
        /// <param name="treatAsLocalProperty">An optional list of properties to treat as local properties.</param>
        /// <param name="projectCollection">An optional <see cref="ProjectCollection" /> to use when loading the project.</param>
        /// <param name="projectFileOptions">An optional <see cref="NewProjectFileOptions" /> specifying options when creating a new file.</param>
        /// <param name="globalProperties">An optional <see cref="IDictionary{String,String}" /> containing global properties for the project.</param>
        /// <returns>A <see cref="ProjectCreator" /> object that is used to construct an MSBuild project.</returns>
        public static ProjectCreator Create(
            string? path = null,
            string? defaultTargets = null,
            string? initialTargets = null,
            string? sdk = null,
            string? toolsVersion = null,
            string? treatAsLocalProperty = null,
            ProjectCollection? projectCollection = null,
            NewProjectFileOptions? projectFileOptions = null,
            IDictionary<string, string>? globalProperties = null)
        {
            projectCollection ??= ProjectCollection.GlobalProjectCollection;

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

            rootElement.ToolsVersion = toolsVersion ?? string.Empty;

            if (treatAsLocalProperty != null)
            {
                rootElement.TreatAsLocalProperty = treatAsLocalProperty;
            }

            return new ProjectCreator(rootElement, globalProperties == null ? null : new Dictionary<string, string>(globalProperties))
            {
                ProjectCollection = projectCollection,
            };
        }

        /// <summary>
        /// Executes custom actions against the current <see cref="ProjectCreator" />.
        /// </summary>
        /// <param name="projectCreator">An <see cref="Action{ProjectCreator}" /> delegate to execute against the current <see cref="ProjectCreator" />.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator CustomAction(Action<ProjectCreator>? projectCreator)
        {
            projectCreator?.Invoke(this);

            return this;
        }

        /// <summary>
        /// Saves the project to disk.
        /// </summary>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator Save()
        {
            RootElement.Save();

            return this;
        }

        /// <summary>
        /// Saves the project to the specified path.
        /// </summary>
        /// <param name="path">The path to save the file to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator Save(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            RootElement.Save(path);

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