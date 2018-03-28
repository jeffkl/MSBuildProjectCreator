// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;

// ReSharper disable once CheckNamespace
namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class ProjectCreatorTemplates
    {
        /// <summary>
        /// Creates a project that logs a single message.
        /// </summary>
        /// <param name="text">The message to display.</param>
        /// <param name="importance">An optional <see cref="MessageImportance"/> to use.</param>
        /// <param name="condition">An optional condition to add to the message task.</param>
        /// <param name="targetName">An optional target name to add the message task to.  Default is "Build".</param>
        /// <param name="path">An optional relative or full path for the project.</param>
        /// <param name="defaultTargets">An optional list of default targets for the project.</param>
        /// <param name="initialTargets">An optional list of initial targets for the project.</param>
        /// <param name="sdk">An optional SDK for the project.</param>
        /// <param name="toolsVersion">An optional tools version for the project.</param>
        /// <param name="treatAsLocalProperty">An optional list of properties to treat as local properties.</param>
        /// <param name="projectCollection">An optional <see cref="ProjectCollection"/> to use when loading the project.</param>
        /// <param name="projectFileOptions">An optional <see cref="NewProjectFileOptions"/> specifying options when creating a new file.</param>
        /// <returns>A <see cref="ProjectCreator"/> object that is used to construct an MSBuild project.</returns>
        public ProjectCreator LogsMessage(
            string text,
            MessageImportance? importance = null,
            string condition = null,
            string targetName = null,
            string path = null,
            string defaultTargets = null,
            string initialTargets = null,
            string sdk = null,
            string toolsVersion = null,
            string treatAsLocalProperty = null,
            ProjectCollection projectCollection = null,
            NewProjectFileOptions? projectFileOptions = null)
        {
            return ProjectCreator.Create(
                    path,
                    defaultTargets,
                    initialTargets,
                    sdk,
                    toolsVersion,
                    treatAsLocalProperty,
                    projectCollection,
                    projectFileOptions)
                .Target(targetName ?? ProjectCreatorConstants.DefaultTargetName)
                .TaskMessage(text, importance, condition);
        }
    }
}