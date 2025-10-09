// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio.SolutionPersistence;
using Microsoft.VisualStudio.SolutionPersistence.Model;
using Microsoft.VisualStudio.SolutionPersistence.Serializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// A fluent API for generating Visual Studio solutions.
    /// </summary>
    public partial class SolutionCreator
    {
        private readonly IDictionary<string, string>? _globalProperties = default;

        /// <summary>
        /// Stores the last project added to the Visual Studio solution.
        /// </summary>
        private SolutionProjectModel? _lastProject = default;

        /// <summary>
        /// Stores the last solution folder added to the Visual Studio solution.
        /// </summary>
        private SolutionFolderModel? _lastSolutionFolder = default;

        /// <summary>
        /// Stores the <see cref="SolutionModel" /> representing the Visual Studio solution being created.
        /// </summary>
        private SolutionModel _solutionModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionCreator" /> class.
        /// </summary>
        /// <param name="path">The path to solution where it will be saved.</param>
        /// <param name="projectCollection">An optional <see cref="ProjectCollection" /> to use when loading the project.</param>
        /// <param name="globalProperties">An optional dictionary of global properties to use when evaluating projects.</param>
        private SolutionCreator(
            string path,
            ProjectCollection? projectCollection = null,
            IDictionary<string, string>? globalProperties = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            FullPath = Path.GetFullPath(path);

            ProjectCollection = projectCollection ?? ProjectCollection.GlobalProjectCollection;

            _globalProperties = globalProperties;

            _solutionModel = new SolutionModel();
        }

        /// <summary>
        /// Gets the full path to the Visual Studio solution.
        /// </summary>
        public string FullPath { get; private set; }

        /// <summary>
        /// Gets the <see cref="ProjectCollection" /> for the current Visual Studio solution.
        /// </summary>
        public ProjectCollection ProjectCollection { get; private set; }

        /// <summary>
        /// Gets the last project added to the Visual Studio solution.
        /// </summary>
        protected SolutionProjectModel? LastProject => _lastProject;

        /// <summary>
        /// Gets the last solution folder added to the Visual Studio solution.
        /// </summary>
        protected SolutionFolderModel? LastSolutionFolder => _lastSolutionFolder;

        /// <summary>
        /// Creates a new <see cref="SolutionCreator" /> instance.
        /// </summary>
        /// <param name="path">The path to use when saving the Visual Studio solution.</param>
        /// <param name="projectCollection">An optional <see cref="ProjectCollection" /> to use when evaluating projects.</param>
        /// <param name="globalProperties">An optional dictionary of global properties to use when evaluating projects.</param>
        /// <returns>A <see cref="SolutionCreator" /> object used to construct a Visual Studio solution.</returns>
        public static SolutionCreator Create(
            string path,
            ProjectCollection? projectCollection = null,
            IDictionary<string, string>? globalProperties = null)
        {
            return new SolutionCreator(path, projectCollection, globalProperties);
        }

        /// <summary>
        /// Adds a build configuration to the Visual Studio solution.
        /// </summary>
        /// <param name="configuration">The name of the build configuration to add like &quot;Debug&quot; or &quot;Release&quot;</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator Configuration(string configuration)
        {
            _solutionModel.AddBuildType(configuration);

            return this;
        }

        /// <summary>
        /// Adds a folder to the Visual Studio solution. This folder becomes the current context for adding projects.
        /// </summary>
        /// <param name="path">A path to the folder.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator Folder(string path)
        {
            _lastSolutionFolder = _solutionModel.AddFolder(path);

            return this;
        }

        /// <summary>
        /// Adds a build platform to the Visual Studio solution.
        /// </summary>
        /// <param name="platform">The name of the build platform to add like &quot;Any CPU&quot; or &quot;x64&quot;.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator Platform(string platform)
        {
            _solutionModel.AddPlatform(platform);

            return this;
        }

        /// <summary>
        /// Adds the specified projects to the Visual Studio solution. The projects are added to the last solution folder added, if any.
        /// </summary>
        /// <param name="projects">One or more <see cref="ProjectCreator" /> instances representing the projects to add.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator Project(params ProjectCreator[] projects)
        {
            foreach (ProjectCreator project in projects)
            {
                Project(project, projectTypeName: null);
            }

            return this;
        }

        /// <summary>
        /// Adds a project to the Visual Studio solution. The project is added to the last solution folder added, if any.
        /// </summary>
        /// <param name="project">The <see cref="ProjectCreator" /> representing the project to add.</param>
        /// <param name="projectTypeName">An optional type name for the project. By default, the project type is detected automatically.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator Project(ProjectCreator project, string? projectTypeName = null)
        {
            return Project(project, _lastSolutionFolder, projectTypeName);
        }

        /// <summary>
        /// Adds a project to the Visual Studio solution. The project is added to the specified solution folder, if any.
        /// </summary>
        /// <param name="project">The <see cref="ProjectCreator" /> representing the project to add.</param>
        /// <param name="folder">The <see cref="SolutionFolderModel" /> representing the folder to add the project to.</param>
        /// <param name="projectTypeName">An optional type name for the project. By default, the project type is detected automatically.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator Project(ProjectCreator project, SolutionFolderModel? folder, string? projectTypeName = null)
        {
            return TryProject(project, folder, out _, projectTypeName);
        }

        /// <summary>
        /// Saves the Visual Studio solution.
        /// </summary>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator Save()
        {
            ISolutionSerializer? serializer = SolutionSerializers.GetSerializerByMoniker(FullPath);

            if (serializer == null)
            {
                throw new InvalidOperationException($"No solution serializer found for path '{FullPath}'.");
            }

            Directory.CreateDirectory(Path.GetDirectoryName(FullPath)!);

            serializer.SaveAsync(FullPath, _solutionModel, CancellationToken.None).GetAwaiter().GetResult();

            return this;
        }

        /// <summary>
        /// Adds a project to the Visual Studio solution and outputs the <see cref="SolutionProjectModel" /> representing the project in the Visual Studio solution. The project is added to the last solution folder added, if any.
        /// </summary>
        /// <param name="project">The <see cref="ProjectCreator" /> representing the project to add.</param>
        /// <param name="projectInSolution">Receives a <see cref="SolutionProjectModel" /> representing the project in the Visual Studio solution.</param>
        /// <param name="projectTypeName">An optional type name for the project. By default, the project type is detected automatically.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryProject(ProjectCreator project, out SolutionProjectModel projectInSolution, string? projectTypeName = null)
        {
            return TryProject(project, _lastSolutionFolder, out projectInSolution, projectTypeName);
        }

        /// <summary>
        /// Adds a project to the Visual Studio solution and outputs the <see cref="SolutionProjectModel" /> representing the project in the Visual Studio solution. The project is added to the specified solution folder, if any.
        /// </summary>
        /// <param name="project">The <see cref="ProjectCreator" /> representing the project to add.</param>
        /// <param name="folder">The <see cref="SolutionFolderModel" /> representing the folder to add the project to.</param>
        /// <param name="projectInSolution">Receives a <see cref="SolutionProjectModel" /> representing the project in the Visual Studio solution.</param>
        /// <param name="projectTypeName">An optional type name for the project. By default, the project type is detected automatically.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryProject(ProjectCreator project, SolutionFolderModel? folder, out SolutionProjectModel projectInSolution, string? projectTypeName = null)
        {
            if (string.IsNullOrWhiteSpace(project.FullPath))
            {
                throw new InvalidOperationException("The project must have a valid path before it can be added to the solution.");
            }

            project.Save();

            projectInSolution = _lastProject = _solutionModel.AddProject(project.FullPath, projectTypeName, folder);

            _solutionModel.DistillProjectConfigurations();

            return this;
        }
    }
}
