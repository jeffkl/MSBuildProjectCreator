// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Represents a utility class for generating assemblies.
    /// </summary>
    internal static class AssemblyCreator
    {
        /// <summary>
        /// Creates an assembly and returns a <see cref="MemoryStream" />.
        /// </summary>
        /// <param name="name">The name of the assembly.</param>
        /// <param name="namespace">The namespace of the class in the assembly.</param>
        /// <param name="className">The name of the class in the library.</param>
        /// <param name="assemblyVersion">The version of the assembly.</param>
        /// <param name="targetFramework">The target framework of the assembly.</param>
        /// <returns>A <see cref="MemoryStream" /> that represents the assembly.</returns>
        public static MemoryStream Create(string name, string @namespace, string className, string assemblyVersion, string targetFramework)
        {
            MemoryStream stream = new MemoryStream();

            Create(stream, name, @namespace, className, assemblyVersion, targetFramework);

            return stream;
        }

        /// <summary>
        /// Creates an assembly at the specified stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream" /> to create the assembly to.</param>
        /// <param name="name">The name of the assembly.</param>
        /// <param name="namespace">The namespace of the class in the assembly.</param>
        /// <param name="className">The name of the class in the library.</param>
        /// <param name="assemblyVersion">The version of the assembly.</param>
        /// <param name="targetFramework">The target framework of the assembly.</param>
        public static void Create(Stream stream, string name, string @namespace, string className, string assemblyVersion, string targetFramework)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            CreateAssembly(
                stream,
                name,
                $@"
[assembly: System.Reflection.AssemblyVersion(""{assemblyVersion}"")]
[assembly: System.Runtime.Versioning.TargetFramework(""{targetFramework}"")]
namespace {@namespace}
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

        private static void CreateAssembly(Stream stream, string name, string code, IEnumerable<string> references, OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary)
        {
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName: name,
                syntaxTrees: new[]
                {
                    CSharpSyntaxTree.ParseText(code),
                },
                references: references.Select(i => MetadataReference.CreateFromFile(i)),
                options: new CSharpCompilationOptions(outputKind));

            EmitResult result = compilation.Emit(stream);

            stream.Seek(0, SeekOrigin.Begin);

            if (!result.Success)
            {
                throw new Exception($@"Failed to create assembly due to the following errors:{Environment.NewLine}{string.Join(Environment.NewLine, result.Diagnostics.Where(i => i.Severity == DiagnosticSeverity.Error))}");
            }
        }
    }
}