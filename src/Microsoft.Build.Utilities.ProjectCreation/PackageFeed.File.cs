// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

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
        /// <param name="targetFramework">The target framework for the content file.</param>
        /// <param name="copyToOutput"><c>true</c> to copy the content file to the build output folder, otherwise <c>false</c>.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed ContentFileText(string relativePath, string contents, string targetFramework, bool copyToOutput = true)
        {
            return ContentFileText(relativePath, contents, targetFramework, "content", copyToOutput);
        }

        /// <summary>
        /// Adds a text file to content files in the package.
        /// </summary>
        /// <param name="relativePath">The filename or relative path to the file.</param>
        /// <param name="contents">The contents of the file.</param>
        /// <param name="targetFramework">The target framework for the content file.</param>
        /// <param name="buildAction">The build action of the content file.</param>
        /// <param name="copyToOutput"><c>true</c> to copy the content file to the build output folder, otherwise <c>false</c>.</param>
        /// <param name="flatten"><c>true</c> to flatten the file structure by disregarding subfolders, otherwise <c>false</c></param>
        /// <param name="language">An optional language for the content file.  The default value is "any".</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed ContentFileText(string relativePath, string contents, string targetFramework, string buildAction, bool copyToOutput = false, bool flatten = false, string language = "any")
        {
            string contentFilePath = Path.Combine(language, targetFramework, relativePath);

            LastPackage.AddContentFile(new PackageContentFileEntry
            {
                BuildAction = buildAction,
                CopyToOutput = copyToOutput,
                Include = contentFilePath,
                Flatten = flatten,
            });

            return FileText(Path.Combine("contentFiles", contentFilePath), contents);
        }

        /// <summary>
        /// Adds a custom file to a package.
        /// </summary>
        /// <param name="relativePath">The relative path of the file within the package.</param>
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