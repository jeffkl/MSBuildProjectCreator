﻿// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class ProjectCreatorTemplates
    {
        /// <summary>
        /// Creates an SDK-style C# project that targets a single framework.
        /// </summary>
        /// <param name="path">An optional relative or full path for the project.</param>
        /// <param name="sdk">An optional SDK for the project.</param>
        /// <param name="targetFramework">An optional target framework for the project.</param>
        /// <param name="outputType">An optional output type for the project.</param>
        /// <param name="projectCreator">An optional <see cref="Action{ProjectCreator}"/> delegate to call in the body of the project.</param>
        /// <param name="defaultTargets">An optional list of default targets for the project.</param>
        /// <param name="initialTargets">An optional list of initial targets for the project.</param>
        /// <param name="treatAsLocalProperty">An optional list of properties to treat as local properties.</param>
        /// <param name="projectCollection">An optional <see cref="ProjectCollection"/> to use when loading the project.</param>
        /// <param name="projectFileOptions">An optional <see cref="NewProjectFileOptions"/> specifying options when creating a new file.</param>
        /// <returns>A <see cref="ProjectCreator"/> object that is used to construct an MSBuild project.</returns>
        public ProjectCreator SdkCsproj(
            string path = null,
            string sdk = ProjectCreatorConstants.SdkCsprojDefaultSdk,
            string targetFramework = ProjectCreatorConstants.SdkCsprojDefaultTargetFramework,
            string outputType = null,
            Action<ProjectCreator> projectCreator = null,
            string defaultTargets = null,
            string initialTargets = null,
            string treatAsLocalProperty = null,
            ProjectCollection projectCollection = null,
            NewProjectFileOptions? projectFileOptions = NewProjectFileOptions.None)
        {
            ProjectCreator creator = ProjectCreator.Create(
                path,
                sdk: sdk,
                defaultTargets: defaultTargets,
                initialTargets: initialTargets,
                treatAsLocalProperty: treatAsLocalProperty,
                projectCollection: projectCollection,
                projectFileOptions: projectFileOptions);

            if (targetFramework != null)
            {
                creator.Property("TargetFramework", targetFramework);
            }

            if (outputType != null)
            {
                creator.Property("OutputType", outputType);
            }

            projectCreator?.Invoke(creator);

            return creator;
        }

        /// <summary>
        /// Creates an SDK-style C# project that target multiple frameworks.
        /// </summary>
        /// <param name="targetFrameworks">An <see cref="IEnumerable{String}"/> that specifies the target frameworks for the project.</param>
        /// <param name="path">An optional relative or full path for the project.</param>
        /// <param name="sdk">An optional SDK for the project.</param>
        /// <param name="outputType">An optional output type for the project.</param>
        /// <param name="projectCreator">An optional <see cref="Action{ProjectCreator}"/> delegate to call in the body of the project.</param>
        /// <param name="defaultTargets">An optional list of default targets for the project.</param>
        /// <param name="initialTargets">An optional list of initial targets for the project.</param>
        /// <param name="treatAsLocalProperty">An optional list of properties to treat as local properties.</param>
        /// <param name="projectCollection">An optional <see cref="ProjectCollection"/> to use when loading the project.</param>
        /// <param name="projectFileOptions">An optional <see cref="NewProjectFileOptions"/> specifying options when creating a new file.</param>
        /// <returns>A <see cref="ProjectCreator"/> object that is used to construct an MSBuild project.</returns>
        public ProjectCreator SdkCsproj(
            IEnumerable<string> targetFrameworks,
            string path = null,
            string sdk = "Microsoft.NET.Sdk",
            string outputType = null,
            Action<ProjectCreator> projectCreator = null,
            string defaultTargets = null,
            string initialTargets = null,
            string treatAsLocalProperty = null,
            ProjectCollection projectCollection = null,
            NewProjectFileOptions? projectFileOptions = NewProjectFileOptions.None)
        {
            ProjectCreator creator = SdkCsproj(
                path: path,
                sdk: sdk,
                targetFramework: null,
                outputType: outputType,
                defaultTargets: defaultTargets,
                initialTargets: initialTargets,
                treatAsLocalProperty: treatAsLocalProperty,
                projectCollection: projectCollection,
                projectFileOptions: projectFileOptions);

            if (targetFrameworks != null)
            {
                creator.Property("TargetFrameworks", String.Join(";", targetFrameworks));
            }

            projectCreator?.Invoke(creator);

            return creator;
        }
    }
}