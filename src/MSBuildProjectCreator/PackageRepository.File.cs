// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Utilities.ProjectCreation.Resources;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class PackageRepository
    {
        /// <summary>
        /// Adds a text file to the package.
        /// </summary>
        /// <param name="relativePath">The relative path of the text file within the package.</param>
        /// <param name="contents">The contents of the text file.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository FileText(string relativePath, string contents)
        {
            return File(relativePath, file => System.IO.File.WriteAllText(file.FullName, contents));
        }

        private PackageRepository File(string relativePath, Action<FileInfo> writeAction)
        {
            FileInfo fileInfo = new FileInfo(Path.Combine(_packageManifest.Directory, relativePath));

            if (fileInfo.Exists)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ErrorFileAlreadyCreated, relativePath));
            }

            if (fileInfo.Directory == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.ErrorFilePathMustBeInADirectory, relativePath));
            }

            fileInfo.Directory.Create();

            writeAction(fileInfo);

            return this;
        }
    }
}