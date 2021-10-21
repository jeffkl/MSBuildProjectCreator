// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using System;
using System.IO;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class PackageFeed
    {
        private const string Build = "build";
        private const string BuildMultiTargeting = "buildMultiTargeting";
        private const string BuildTransitive = "buildTransitive";
        private const string PropsExtension = ".props";
        private const string TargetsExtension = ".targets";

        /// <summary>
        /// Adds a .props file to the buildMultitargeting directory.
        /// </summary>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .props file.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed BuildMultiTargetingProps(NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildMultiTargetingProps(null, projectFileOptions);
        }

        /// <summary>
        /// Adds a .props file to the buildMultitargeting directory.
        /// </summary>
        /// <param name="creator">An <see cref="Action" /> to generate the .props file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .props file.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed BuildMultiTargetingProps(Action<ProjectCreator>? creator, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            CreateBuildFile(PropsExtension, BuildMultiTargeting, creator, projectFileOptions);

            return this;
        }

        /// <summary>
        /// Adds a .targets file to the buildMultitargeting directory.
        /// </summary>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .targets file.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed BuildMultiTargetingTargets(NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildMultiTargetingTargets(null, projectFileOptions);
        }

        /// <summary>
        /// Adds a .targets file to the buildMultitargeting directory.
        /// </summary>
        /// <param name="creator">An <see cref="Action{ProjectCreator}" /> to generate the .targets file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .targets file.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed BuildMultiTargetingTargets(Action<ProjectCreator>? creator, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            CreateBuildFile(TargetsExtension, BuildMultiTargeting, creator, projectFileOptions);

            return this;
        }

        /// <summary>
        /// Adds a .props file to the build directory.
        /// </summary>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .props file.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed BuildProps(NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildProps(null, projectFileOptions);
        }

        /// <summary>
        /// Adds a .props file to the build directory.
        /// </summary>
        /// <param name="creator">An <see cref="Action{ProjectCreator}" /> to generate the .props file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .props file.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed BuildProps(Action<ProjectCreator>? creator, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            CreateBuildFile(PropsExtension, Build, creator, projectFileOptions);

            return this;
        }

        /// <summary>
        /// Adds a .targets file to the build directory.
        /// </summary>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .targets file.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed BuildTargets(NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildTargets(null, projectFileOptions);
        }

        /// <summary>
        /// Adds a .targets file to the build directory.
        /// </summary>
        /// <param name="creator">An <see cref="Action{ProjectCreator}" /> to generate the .targets file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .targets file.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed BuildTargets(Action<ProjectCreator>? creator, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            CreateBuildFile(TargetsExtension, Build, creator, projectFileOptions);

            return this;
        }

        /// <summary>
        /// Adds a .props file to the buildTransitive directory.
        /// </summary>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .props file.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed BuildTransitiveProps(NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildTransitiveProps(null, projectFileOptions);
        }

        /// <summary>
        /// Adds a .props file to the buildTransitive directory.
        /// </summary>
        /// <param name="creator">An <see cref="Action{ProjectCreator}" /> to generate the .props file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .props file.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed BuildTransitiveProps(Action<ProjectCreator>? creator, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            CreateBuildFile(PropsExtension, BuildTransitive, creator, projectFileOptions);

            return this;
        }

        /// <summary>
        /// Adds a .targets file to the buildTransitive directory.
        /// </summary>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .targets file.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed BuildTransitiveTargets(NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            return BuildTransitiveTargets(null, projectFileOptions);
        }

        /// <summary>
        /// Adds a .targets file to the buildTransitive directory.
        /// </summary>
        /// <param name="creator">An <see cref="Action{ProjectCreator}" /> to generate the .targets file.</param>
        /// <param name="projectFileOptions">Optional <see cref="NewProjectFileOptions" /> for the .targets file.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed BuildTransitiveTargets(Action<ProjectCreator>? creator, NewProjectFileOptions projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            CreateBuildFile(TargetsExtension, BuildTransitive, creator, projectFileOptions);

            return this;
        }

        private void CreateBuildFile(string extension, string folderName, Action<ProjectCreator>? creator, NewProjectFileOptions projectFileOptions)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new ArgumentNullException(extension);
            }

            if (string.IsNullOrWhiteSpace(folderName))
            {
                throw new ArgumentNullException(folderName);
            }

            ProjectCreator project = ProjectCreator.Create(projectFileOptions: projectFileOptions);

            creator?.Invoke(project);

            FileText(Path.Combine(folderName, $"{LastPackage.Id}{extension}"), project.Xml);
        }
    }
}