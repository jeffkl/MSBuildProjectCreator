﻿// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

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

        /// <summary>
        /// Adds a custom file to the package.
        /// </summary>
        /// <param name="relativePath">The relative path of the file within the package.</param>
        /// <param name="sourceFileInfo">The <see cref="FileInfo" /> of the file to copy from.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository FileCustom(string relativePath, FileInfo sourceFileInfo)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new ArgumentNullException(nameof(relativePath));
            }

            if (Path.IsPathRooted(relativePath))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Strings.ErrorFilePathMustBeRelative, relativePath));
            }

            if (LastPackage == null)
            {
                throw new InvalidOperationException(Strings.ErrorWhenAddingLibraryRequiresPackage);
            }

            return File(relativePath, destinationFileInfo => sourceFileInfo.CopyTo(destinationFileInfo.FullName));
        }

        private PackageRepository File(string relativePath, Action<FileInfo> writeAction)
        {
            if (LastPackage == null)
            {
                throw new InvalidOperationException(Strings.ErrorWhenAddingLibraryRequiresPackage);
            }

            FileInfo fileInfo = new FileInfo(Path.Combine(LastPackage.Directory!, relativePath));

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