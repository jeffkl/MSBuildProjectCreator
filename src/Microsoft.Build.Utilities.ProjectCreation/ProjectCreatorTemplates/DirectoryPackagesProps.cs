// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class ProjectCreatorTemplates
    {
        /// <summary>
        /// Creates a Directory.Packages.props file with central package management enabled.
        /// </summary>
        /// <param name="packageVersions">An optional collection of package IDs and versions to write to the file.</param>
        /// <param name="directory">An optional relative or full directory for the file. The default is the current directory.</param>
        /// <param name="projectCreator">An optional <see cref="Action{ProjectCreator}" /> delegate to call in the body of the file.</param>
        /// <param name="projectCollection">An optional <see cref="ProjectCollection" /> to use when loading the file.</param>
        /// <param name="projectFileOptions">An optional <see cref="NewProjectFileOptions" /> specifying options when creating a new file.</param>
        /// <param name="globalProperties">An optional <see cref="IDictionary{String,String}" /> containing global properties for the file.</param>
        /// <returns>A <see cref="ProjectCreator" /> object that is used to construct an MSBuild file.</returns>
        public ProjectCreator DirectoryPackagesProps(
            IDictionary<string, string>? packageVersions = null,
            string? directory = null,
            Action<ProjectCreator>? projectCreator = null,
            ProjectCollection? projectCollection = null,
            NewProjectFileOptions? projectFileOptions = NewProjectFileOptions.None,
            IDictionary<string, string>? globalProperties = null)
        {
            string directoryPath = directory ?? Directory.GetCurrentDirectory();

            ProjectCreator project = ProjectCreator.Create(
                    Path.Combine(directoryPath, "Directory.Packages.props"),
                    projectCollection: projectCollection,
                    projectFileOptions: projectFileOptions,
                    globalProperties: globalProperties)
                .Property("ManagePackageVersionsCentrally", "true");

            if (packageVersions != null)
            {
                foreach (KeyValuePair<string, string> packageVersion in packageVersions)
                {
                    project.ItemPackageVersion(packageVersion.Key, packageVersion.Value);
                }
            }

            return project.CustomAction(projectCreator);
        }
    }
}
