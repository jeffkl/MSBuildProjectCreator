// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System.IO;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class PackageFeed
    {
        /// <summary>
        /// Adds a library to the current package.
        /// </summary>
        /// <param name="targetFramework">The target framework of the assembly.</param>
        /// <param name="filename">An optional name for the library.  The default value is the ID of the package.</param>
        /// <param name="namespace">An optional namespace for the class in the library.  The default value is ID of the package.</param>
        /// <param name="className">An optional name for the class in the library.  The default value is the ID of the package with _Class appended.</param>
        /// <param name="assemblyVersion">An optional assembly version for the library.  The default value is <c>1.0.0.0</c></param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed Library(string targetFramework, string? filename = null, string? @namespace = null, string? className = null, string assemblyVersion = "1.0.0.0")
        {
            return Assembly("lib", targetFramework, filename, @namespace, className, assemblyVersion);
        }

        /// <summary>
        /// Adds a library to the current package.
        /// </summary>
        /// <param name="targetFramework">The target framework of the assembly.</param>
        /// <param name="filename">An optional name for the library.  The default value is the ID of the package.</param>
        /// <param name="namespace">An optional namespace for the class in the library.  The default value is ID of the package.</param>
        /// <param name="className">An optional name for the class in the library.  The default value is the ID of the package with _Class appended.</param>
        /// <param name="assemblyVersion">An optional assembly version for the library.  The default value is <c>1.0.0.0</c></param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed ReferenceAssembly(string targetFramework, string? filename = null, string? @namespace = null, string? className = null, string assemblyVersion = "1.0.0.0")
        {
            return Assembly("ref", targetFramework, filename, @namespace, className, assemblyVersion);
        }

        private PackageFeed Assembly(string rootFolderName, string targetFramework, string? filename = null, string? @namespace = null, string? className = null, string assemblyVersion = "1.0.0.0")
        {
            filename ??= $"{LastPackage.Id}.dll";

            if (string.IsNullOrWhiteSpace(@namespace))
            {
                @namespace = LastPackage.Id;
            }

            if (string.IsNullOrWhiteSpace(className))
            {
                className = $"{LastPackage.Id.Replace(".", "_")}_Class";
            }

            string relativePath = Path.Combine(rootFolderName, targetFramework, filename);

            return File(
                relativePath,
                () => AssemblyCreator.Create(Path.GetFileNameWithoutExtension(filename), @namespace!, className!, assemblyVersion, targetFramework));
        }
    }
}
