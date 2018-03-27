// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

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
        /// <param name="path">An optional relative or full path for the project.</param>
        /// <param name="importance">An optional <see cref="MessageImportance"/> to use.</param>
        /// <param name="condition">An optional condition to add to the message task.</param>
        /// <returns>A <see cref="ProjectCreator"/> object that is used to construct an MSBuild project.</returns>
        public ProjectCreator LogsMessage(string text, string path = null, MessageImportance? importance = null, string condition = null)
        {
            return ProjectCreator.Create(path)
                .TaskMessage(text, importance, condition);
        }
    }
}