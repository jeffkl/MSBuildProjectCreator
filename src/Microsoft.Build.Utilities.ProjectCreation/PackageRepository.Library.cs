// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Frameworks;
using System;
using System.IO;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class PackageRepository
    {
        /// <summary>
        /// Adds a library to the package.
        /// </summary>
        /// <param name="targetFramework">The target framework of the library.</param>
        /// <param name="filename">An optional filename for the library.  The default value is &lt;PackageId&gt;.dll.</param>
        /// <param name="namespace">An optional namespace for the library.  The default value is &lt;PackageId&gt;.</param>
        /// <param name="className">An optional class name for the library.  The default value is &lt;PackageId&gt;_Class.</param>
        /// <param name="assemblyVersion">An optional assembly version for the library.  The default value is &quot;1.0.0.0&quot;</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Library(string targetFramework, string filename = null, string @namespace = null, string className = null, string assemblyVersion = "1.0.0.0")
        {
            return Library(NuGetFramework.Parse(targetFramework), filename, @namespace, className, assemblyVersion);
        }

        /// <summary>
        /// Adds a library to the package.
        /// </summary>
        /// <param name="targetFramework">The <see cref="NuGetFramework" /> of the library.</param>
        /// <param name="filename">An optional filename for the library.  The default value is &lt;PackageId&gt;.dll.</param>
        /// <param name="namespace">An optional namespace for the library.  The default value is &lt;PackageId&gt;.</param>
        /// <param name="className">An optional class name for the library.  The default value is &lt;PackageId&gt;_Class.</param>
        /// <param name="assemblyVersion">An optional assembly version for the library.  The default value is &quot;1.0.0.0&quot;</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Library(NuGetFramework targetFramework, string filename = null, string @namespace = null, string className = null, string assemblyVersion = "1.0.0.0")
        {
            if (_packageManifest == null)
            {
                throw new InvalidOperationException(Strings.ErrorWhenAddingLibraryRequiresPackage);
            }

            if (string.IsNullOrWhiteSpace(filename))
            {
                filename = $"{_packageManifest.Metadata.Id}.dll";
            }

            if (string.IsNullOrWhiteSpace(@namespace))
            {
                @namespace = _packageManifest.Metadata.Id;
            }

            if (string.IsNullOrWhiteSpace(className))
            {
                className = $"{_packageManifest.Metadata.Id}_Class";
            }

            _packageManifest.AddDependencyGroup(targetFramework);

            return File(
                Path.Combine("lib", targetFramework.GetShortFolderName(), filename),
                fileInfo =>
                {
                    fileInfo.Directory.Create();

                    using (Stream stream = System.IO.File.Create(fileInfo.FullName))
                    {
                        AssemblyCreator.Create(stream, Path.GetFileNameWithoutExtension(filename), @namespace, className, assemblyVersion, targetFramework);
                    }
                });
        }
    }
}