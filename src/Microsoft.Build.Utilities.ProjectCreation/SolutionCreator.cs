// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.VisualStudio.SolutionPersistence;
using Microsoft.VisualStudio.SolutionPersistence.Model;
using Microsoft.VisualStudio.SolutionPersistence.Serializer;
using System;
using System.Threading;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// A fluent API for generating Visual Studio solutions.
    /// </summary>
    public partial class SolutionCreator
    {
        /// <summary>
        /// Stores the last project added to the solution.
        /// </summary>
        private SolutionProjectModel? _lastProject = default;

        /// <summary>
        /// Stores the last solution folder added to the solution.
        /// </summary>
        private SolutionFolderModel? _lastSolutionFolder = default;

        /// <summary>
        /// Stores the path to save the solution to.
        /// </summary>
        private string? _path;

        /// <summary>
        /// Stores the <see cref="SolutionModel" /> representing the solution being created.
        /// </summary>
        private SolutionModel _solutionModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionCreator" /> class.
        /// </summary>
        /// <param name="path">An optional path to save the solution to.</param>
        private SolutionCreator(string? path)
        {
            _path = path;

            _solutionModel = new SolutionModel();
        }

        /// <summary>
        /// Gets the last project added to the solution.
        /// </summary>
        protected SolutionProjectModel? LastProject => _lastProject;

        /// <summary>
        /// Gets the last solution folder added to the solution.
        /// </summary>
        protected SolutionFolderModel? LastSolutionFolder => _lastSolutionFolder;

        /// <summary>
        /// Creates a new <see cref="SolutionCreator" /> instance.
        /// </summary>
        /// <param name="path">An optional path to use when saving the solution.</param>
        /// <returns>A <see cref="ProjectCreation" /> object used to construct a Visual Studio solution.</returns>
        public static SolutionCreator Create(string? path = null)
        {
            return new SolutionCreator(path);
        }

        /// <summary>
        /// Adds a folder to the solution. This folder becomes the current context for adding projects.
        /// </summary>
        /// <param name="path">A path to the folder.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator Folder(string path)
        {
            _lastSolutionFolder = _solutionModel.AddFolder(path);

            return this;
        }

        /// <summary>
        /// Adds a project to the solution. The project is added to the last solution folder added, if any.
        /// </summary>
        /// <param name="project">The <see cref="ProjectCreator" /> representing the project to add.</param>
        /// <param name="projectTypeName">An optional type name for the project. By default, the project type is detected automatically.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator Project(ProjectCreator project, string? projectTypeName = null)
        {
            return Project(project, _lastSolutionFolder, projectTypeName);
        }

        /// <summary>
        /// Adds a project to the solution. The project is added to the specified solution folder, if any.
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
        /// Saves the solution.
        /// </summary>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        /// <exception cref="InvalidOperationException">A path was not specified when the <see cref="SolutionCreator.Create(string?)" /> method was called.</exception>
        public SolutionCreator Save()
        {
            if (_path is null || string.IsNullOrEmpty(_path))
            {
                throw new InvalidOperationException("Path must be specified to save the solution.");
            }

            return Save(_path);
        }

        /// <summary>
        /// Saves the solution to the specified path.
        /// </summary>
        /// <param name="path">The path to save the solution to.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        /// <exception cref="InvalidOperationException">The format for the solution could not be detected because the path does not end in <c>.sln</c> or <c>.slnx</c>.</exception>
        public SolutionCreator Save(string path)
        {
            ISolutionSerializer? serializer = SolutionSerializers.GetSerializerByMoniker(path);

            if (serializer == null)
            {
                throw new InvalidOperationException($"No solution serializer found for path '{path}'.");
            }

            serializer.SaveAsync(path, _solutionModel, CancellationToken.None).GetAwaiter().GetResult();

            return this;
        }

        /// <summary>
        /// Adds a project to the solution and outputs the <see cref="SolutionProjectModel" /> representing the project in the solution. The project is added to the last solution folder added, if any.
        /// </summary>
        /// <param name="project">The <see cref="ProjectCreator" /> representing the project to add.</param>
        /// <param name="projectInSolution">Receives a <see cref="SolutionProjectModel" /> representing the project in the solution.</param>
        /// <param name="projectTypeName">An optional type name for the project. By default, the project type is detected automatically.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryProject(ProjectCreator project, out SolutionProjectModel projectInSolution, string? projectTypeName = null)
        {
            return TryProject(project, _lastSolutionFolder, out projectInSolution, projectTypeName);
        }

        /// <summary>
        /// Adds a project to the solution and outputs the <see cref="SolutionProjectModel" /> representing the project in the solution. The project is added to the specified solution folder, if any.
        /// </summary>
        /// <param name="project">The <see cref="ProjectCreator" /> representing the project to add.</param>
        /// <param name="folder">The <see cref="SolutionFolderModel" /> representing the folder to add the project to.</param>
        /// <param name="projectInSolution">Receives a <see cref="SolutionProjectModel" /> representing the project in the solution.</param>
        /// <param name="projectTypeName">An optional type name for the project. By default, the project type is detected automatically.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryProject(ProjectCreator project, SolutionFolderModel? folder, out SolutionProjectModel projectInSolution, string? projectTypeName = null)
        {
            project.Save();

            projectInSolution = _lastProject = _solutionModel.AddProject(project.FullPath, projectTypeName, folder);

            return this;
        }
    }
}
