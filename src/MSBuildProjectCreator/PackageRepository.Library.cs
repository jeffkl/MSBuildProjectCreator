// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Utilities.ProjectCreation.Resources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

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

            if (string.IsNullOrWhiteSpace(className))
            {
                className = $"{_packageManifest.Metadata.Id}_Class";
            }

            _packageManifest.AddDependencyGroup(targetFramework);

            return File(
                Path.Combine("lib", targetFramework.GetShortFolderName(), filename),
                fileInfo => CreateAssembly(fileInfo, @namespace, className, assemblyVersion, targetFramework.GetDotNetFrameworkName(DefaultFrameworkNameProvider.Instance)));
        }

        private void CreateAssembly(FileInfo fileInfo, string @namespace, string className, string version, string targetFramework)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            if (fileInfo.Directory == null)
            {
                throw new ArgumentNullException(nameof(fileInfo.Directory));
            }

            fileInfo.Directory.Create();

            string name = string.IsNullOrWhiteSpace(@namespace) ? Path.GetFileNameWithoutExtension(fileInfo.Name) : @namespace;

            CreateAssembly(
                fileInfo,
                name,
                $@"
[assembly: System.Reflection.AssemblyVersionAttribute(""{version}"")]
[assembly: System.Runtime.Versioning.TargetFramework(""{targetFramework}"")]
namespace {name}
{{
    public class {className}
    {{
    }}
}}",
                new[]
                {
                    typeof(object).Assembly.Location,
                });
        }

        private void CreateAssembly(FileInfo fileInfo, string name, string code, IEnumerable<string> references, OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary)
        {
            CSharpCompilation compilation = CSharpCompilation.Create(
                name,
                new[]
                {
                    CSharpSyntaxTree.ParseText(code, null, string.Empty, Encoding.Default, CancellationToken.None),
                },
                references.Select(i => MetadataReference.CreateFromFile(i)),
                GetCSharpCompilationOptions(outputKind));

            EmitResult result = compilation.Emit(fileInfo.FullName);

            if (!result.Success)
            {
            }
        }

        private CSharpCompilationOptions GetCSharpCompilationOptions(OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary)
        {
            ConstructorInfo ctor = typeof(CSharpCompilationOptions).GetConstructors().FirstOrDefault();

            if (ctor == null)
            {
                return null;
            }

            object[] parameters = new object[ctor.GetParameters().Length];

            parameters[0] = outputKind;

            return (CSharpCompilationOptions)ctor.Invoke(parameters);
        }
    }
}