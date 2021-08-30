// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.ProjectModel;
using System;
using System.IO;
using System.Text;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class PackageFeed
    {
        /// <summary>
        /// Adds a text file to content files in the package.
        /// </summary>
        /// <param name="relativePath">The filename or relative path to the file.</param>
        /// <param name="contents">The contents of the file.</param>
        /// <param name="targetFramework">The <see cref="NuGetFramework" /> for the content file.</param>
        /// <param name="copyToOutput"><c>true</c> to copy the content file to the build output folder, otherwise <c>false</c>.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed ContentFileText(string relativePath, string contents, string targetFramework, bool copyToOutput = true)
        {
            return ContentFileText(relativePath, contents, NuGetFramework.Parse(targetFramework), BuildAction.Content, copyToOutput);
        }

        /// <summary>
        /// Adds a text file to content files in the package.
        /// </summary>
        /// <param name="relativePath">The filename or relative path to the file.</param>
        /// <param name="contents">The contents of the file.</param>
        /// <param name="targetFramework">The <see cref="NuGetFramework" /> for the content file.</param>
        /// <param name="copyToOutput"><c>true</c> to copy the content file to the build output folder, otherwise <c>false</c>.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed ContentFileText(string relativePath, string contents, NuGetFramework targetFramework, bool copyToOutput = true)
        {
            return ContentFileText(relativePath, contents, targetFramework, BuildAction.Content, copyToOutput);
        }

        /// <summary>
        /// Adds a text file to content files in the package.
        /// </summary>
        /// <param name="relativePath">The filename or relative path to the file.</param>
        /// <param name="contents">The contents of the file.</param>
        /// <param name="targetFramework">The target framework for the content file.</param>
        /// <param name="buildAction">The <see cref="BuildAction" /> of the content file.</param>
        /// <param name="copyToOutput"><c>true</c> to copy the content file to the build output folder, otherwise <c>false</c>.</param>
        /// <param name="flatten"><c>true</c> to flatten the file structure by disregarding subfolders, otherwise <c>false</c></param>
        /// <param name="language">An optional language for the content file.  The default value is "any".</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed ContentFileText(string relativePath, string contents, string targetFramework, BuildAction buildAction, bool copyToOutput = false, bool flatten = false, string language = "any")
        {
            return ContentFileText(relativePath, contents, NuGetFramework.Parse(targetFramework), buildAction, copyToOutput, flatten, language);
        }

        /// <summary>
        /// Adds a text file to content files in the package.
        /// </summary>
        /// <param name="relativePath">The filename or relative path to the file.</param>
        /// <param name="contents">The contents of the file.</param>
        /// <param name="targetFramework">The <see cref="NuGetFramework" /> for the content file.</param>
        /// <param name="buildAction">The <see cref="BuildAction" /> of the content file.</param>
        /// <param name="copyToOutput"><c>true</c> to copy the content file to the build output folder, otherwise <c>false</c>.</param>
        /// <param name="flatten"><c>true</c> to flatten the file structure by disregarding subfolders, otherwise <c>false</c></param>
        /// <param name="language">An optional language for the content file.  The default value is "any".</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed ContentFileText(string relativePath, string contents, NuGetFramework targetFramework, BuildAction buildAction, bool copyToOutput = false, bool flatten = false, string language = "any")
        {
            string contentFilePath = Path.Combine(language, targetFramework.GetShortFolderName(), relativePath);

            LastPackage.AddContentFile(new ManifestContentFiles
            {
                BuildAction = buildAction.ToString(),
                CopyToOutput = copyToOutput.ToString(),
                Include = contentFilePath,
                Flatten = flatten.ToString(),
            });

            return FileText(Path.Combine("contentFiles", contentFilePath), contents);
        }

        /// <summary>
        /// Adds a custom file to a package.
        /// </summary>
        /// <param name="relativePath">The relative path of the text file within the package.</param>
        /// <param name="sourceFileInfo">The <see cref="FileInfo" /> of the file to copy from.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed FileCustom(string relativePath, FileInfo sourceFileInfo)
        {
            LastPackage.AddFile(relativePath, sourceFileInfo);

            return this;
        }

        /// <summary>
        /// Adds a text file to a package.
        /// </summary>
        /// <param name="relativePath">The relative path for the file.</param>
        /// <param name="contents">The contents of the file.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed FileText(string relativePath, string contents)
        {
            return File(relativePath, () => new MemoryStream(Encoding.UTF8.GetBytes(contents)));
        }

        private PackageFeed File(string relativePath, Func<MemoryStream> streamFunc)
        {
            LastPackage.AddFile(relativePath, streamFunc);

            return this;
        }
    }
}