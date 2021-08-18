// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using System;
using System.IO;

namespace Microsoft.Build.Utilities.ProjectCreation.NuGet
{
    public partial class PackageRepository
    {
        /// <summary>
        /// Adds a .props file to the buildMultitargeting directory.
        /// </summary>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .props file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildMultiTargetingProps(NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildMultiTargetingProps(null, projectFileOptions);
        }

        /// <summary>
        /// Adds a .props file to the buildMultitargeting directory.
        /// </summary>
        /// <param name="creator">An <see cref="Action{ProjectCreator}" /> to generate the .props file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .props file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildMultiTargetingProps(Action<ProjectCreator> creator, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildMultiTargetingProps(creator, out _, projectFileOptions);
        }

        /// <summary>
        /// Adds a .props file to the buildMultitargeting directory.
        /// </summary>
        /// <param name="project">Receives the <see cref="ProjectCreator" /> of the created project file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .props file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildMultiTargetingProps(out ProjectCreator project, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildMultiTargetingProps(null, out project, projectFileOptions);
        }

        /// <summary>
        /// Adds a .props file to the buildMultitargeting directory.
        /// </summary>
        /// <param name="creator">An <see cref="Action{ProjectCreator}" /> to generate the .props file.</param>
        /// <param name="project">Receives the <see cref="ProjectCreator" /> of the created project file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .props file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildMultiTargetingProps(Action<ProjectCreator> creator, out ProjectCreator project, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            CreateBuildFile(".props", "buildMultiTargeting", creator, projectFileOptions, out project);

            return this;
        }

        /// <summary>
        /// Adds a .targets file to the buildMultitargeting directory.
        /// </summary>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .targets file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildMultiTargetingTargets(NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildMultiTargetingTargets(out _, projectFileOptions);
        }

        /// <summary>
        /// Adds a .targets file to the buildMultitargeting directory.
        /// </summary>
        /// <param name="project">Receives the <see cref="ProjectCreator" /> of the created project file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .targets file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildMultiTargetingTargets(out ProjectCreator project, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildMultiTargetingTargets(null, out project, projectFileOptions);
        }

        /// <summary>
        /// Adds a .targets file to the buildMultitargeting directory.
        /// </summary>
        /// <param name="creator">An <see cref="Action{ProjectCreator}" /> to generate the .targets file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .targets file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildMultiTargetingTargets(Action<ProjectCreator> creator, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildMultiTargetingTargets(creator, out _, projectFileOptions);
        }

        /// <summary>
        /// Adds a .targets file to the buildMultitargeting directory.
        /// </summary>
        /// <param name="creator">An <see cref="Action{ProjectCreator}" /> to generate the .targets file.</param>
        /// <param name="project">Receives the <see cref="ProjectCreator" /> of the created project file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .targets file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildMultiTargetingTargets(Action<ProjectCreator> creator, out ProjectCreator project, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            CreateBuildFile(".targets", "buildMultiTargeting", creator, projectFileOptions, out project);

            return this;
        }

        /// <summary>
        /// Adds a .props file to the build directory.
        /// </summary>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .props file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildProps(NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildProps(out ProjectCreator _, projectFileOptions);
        }

        /// <summary>
        /// Adds a .props file to the build directory.
        /// </summary>
        /// <param name="project">Receives the <see cref="ProjectCreator" /> of the created project file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .props file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildProps(out ProjectCreator project, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildProps(null, out project, projectFileOptions);
        }

        /// <summary>
        /// Adds a .props file to the build directory.
        /// </summary>
        /// <param name="creator">An <see cref="Action{ProjectCreator}" /> to generate the .props file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .props file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildProps(Action<ProjectCreator> creator, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildProps(creator, out ProjectCreator _, projectFileOptions);
        }

        /// <summary>
        /// Adds a .props file to the build directory.
        /// </summary>
        /// <param name="creator">An <see cref="Action{ProjectCreator}" /> to generate the .props file.</param>
        /// <param name="project">Receives the <see cref="ProjectCreator" /> of the created project file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .props file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildProps(Action<ProjectCreator> creator, out ProjectCreator project, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            CreateBuildFile(".props", "build", creator, projectFileOptions, out project);

            return this;
        }

        /// <summary>
        /// Adds a .targets file to the build directory.
        /// </summary>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .targets file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildTargets(NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildTargets(out _, projectFileOptions);
        }

        /// <summary>
        /// Adds a .targets file to the build directory.
        /// </summary>
        /// <param name="project">Receives the <see cref="ProjectCreator" /> of the created project file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .targets file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildTargets(out ProjectCreator project, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildTargets(null, out project, projectFileOptions);
        }

        /// <summary>
        /// Adds a .targets file to the build directory.
        /// </summary>
        /// <param name="creator">An <see cref="Action{ProjectCreator}" /> to generate the .targets file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .targets file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildTargets(Action<ProjectCreator> creator, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildTargets(creator, out _, projectFileOptions);
        }

        /// <summary>
        /// Adds a .targets file to the build directory.
        /// </summary>
        /// <param name="creator">An <see cref="Action{ProjectCreator}" /> to generate the .targets file.</param>
        /// <param name="project">Receives the <see cref="ProjectCreator" /> of the created project file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .targets file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildTargets(Action<ProjectCreator> creator, out ProjectCreator project, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            CreateBuildFile(".targets", "build", creator, projectFileOptions, out project);

            return this;
        }

        /// <summary>
        /// Adds a .props file to the buildTransitive directory.
        /// </summary>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .props file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildTransitiveProps(NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildTransitiveProps(out _, projectFileOptions);
        }

        /// <summary>
        /// Adds a .props file to the buildTransitive directory.
        /// </summary>
        /// <param name="project">Receives the <see cref="ProjectCreator" /> of the created project file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .props file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildTransitiveProps(out ProjectCreator project, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildTransitiveProps(null, out project, projectFileOptions);
        }

        /// <summary>
        /// Adds a .props file to the buildTransitive directory.
        /// </summary>
        /// <param name="creator">An <see cref="Action{ProjectCreator}" /> to generate the .props file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .props file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildTransitiveProps(Action<ProjectCreator> creator, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildTransitiveProps(creator, out _, projectFileOptions);
        }

        /// <summary>
        /// Adds a .props file to the buildTransitive directory.
        /// </summary>
        /// <param name="creator">An <see cref="Action{ProjectCreator}" /> to generate the .props file.</param>
        /// <param name="project">Receives the <see cref="ProjectCreator" /> of the created project file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .props file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildTransitiveProps(Action<ProjectCreator> creator, out ProjectCreator project, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            CreateBuildFile(".props", "buildTransitive", creator, projectFileOptions, out project);

            return this;
        }

        /// <summary>
        /// Adds a .targets file to the buildTransitive directory.
        /// </summary>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .targets file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildTransitiveTargets(NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildTransitiveTargets(out _, projectFileOptions);
        }

        /// <summary>
        /// Adds a .targets file to the buildTransitive directory.
        /// </summary>
        /// <param name="project">Receives the <see cref="ProjectCreator" /> of the created project file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .targets file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildTransitiveTargets(out ProjectCreator project, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildTransitiveTargets(null, out project, projectFileOptions);
        }

        /// <summary>
        /// Adds a .targets file to the buildTransitive directory.
        /// </summary>
        /// <param name="creator">An <see cref="Action{ProjectCreator}" /> to generate the .targets file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .targets file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildTransitiveTargets(Action<ProjectCreator> creator, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildTransitiveTargets(creator, out _, projectFileOptions);
        }

        /// <summary>
        /// Adds a .targets file to the buildTransitive directory.
        /// </summary>
        /// <param name="creator">An <see cref="Action{ProjectCreator}" /> to generate the .targets file.</param>
        /// <param name="project">Receives the <see cref="ProjectCreator" /> of the created project file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .targets file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository BuildTransitiveTargets(Action<ProjectCreator> creator, out ProjectCreator project, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            CreateBuildFile(".targets", "buildTransitive", creator, projectFileOptions, out project);

            return this;
        }

        private void CreateBuildFile(string extension, string folderName, Action<ProjectCreator> creator, NewProjectFileOptions projectFileOptions, out ProjectCreator project)
        {
            if (_packageManifest == null)
            {
                throw new InvalidOperationException(Strings.ErrorWhenAddingBuildLogicRequiresPackage);
            }

            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ArgumentNullException(extension);
            }

            if (string.IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException(folderName);
            }

            project = ProjectCreator.Create(
                path: Path.Combine(_packageManifest.Directory, folderName, $"{_packageManifest.Metadata.Id}{extension}"),
                projectFileOptions: projectFileOptions);

            creator?.Invoke(project);

            project.Save();
        }
    }
}