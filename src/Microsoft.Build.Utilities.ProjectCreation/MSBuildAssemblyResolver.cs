// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
#if !NETFRAMEWORK
using System.Runtime.Loader;
#endif

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Resolves MSBuild assemblies.
    /// </summary>
    public static partial class MSBuildAssemblyResolver
    {
        private static readonly string[] AssemblyExtensions = { ".dll", ".exe" };

        private static readonly Dictionary<string, Lazy<Assembly?>> LoadedAssemblies = new(StringComparer.OrdinalIgnoreCase);

        private static readonly Lazy<object> RegisterLazy = new(() =>
        {
            Environment.SetEnvironmentVariable("MSBUILD_EXE_PATH", MSBuildExePath);
            Environment.SetEnvironmentVariable("MSBuildExtensionsPath", DotNetSdksPath);
            Environment.SetEnvironmentVariable("MSBuildSDKsPath", string.IsNullOrWhiteSpace(DotNetSdksPath) ? null : Path.Combine(DotNetSdksPath, "Sdks"));

#if NETFRAMEWORK
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;

#else
            AssemblyLoadContext.Default.Resolving += AssemblyResolve;
#endif

            return new object();
        });

        /// <summary>
        /// Gets the path to the .NET SDKs.
        /// </summary>
        public static string? DotNetSdksPath => DotNetSdksPathLazy.Value;

        /// <summary>
        /// Gets the full path to the MSBuild directory used.
        /// </summary>
        public static string[]? SearchPaths => MSBuildDirectoryLazy.Value.SearchPaths;

        /// <summary>
        /// Gets the full path to the MSBuild executable used.
        /// </summary>
        public static string? MSBuildExePath => MSBuildDirectoryLazy.Value.MSBuildExePath;

        /// <summary>
        /// Register to resolve MSBuild related assemblies automatically.
        /// </summary>
        public static void Register() => _ = RegisterLazy.Value;

        private static Assembly? AssemblyResolve(AssemblyName requestedAssemblyName, Func<string, Assembly?> assemblyLoader)
        {
            if (SearchPaths == null)
            {
                return null;
            }

            lock (LoadedAssemblies)
            {
                if (!LoadedAssemblies.TryGetValue(requestedAssemblyName.FullName, out Lazy<Assembly?>? assemblyLazy))
                {
                    assemblyLazy = new Lazy<Assembly?>(() =>
                    {
                        foreach (FileInfo candidateAssemblyFile in SearchPaths.SelectMany(searchPath => AssemblyExtensions.Select(extension => new FileInfo(Path.Combine(searchPath, $"{requestedAssemblyName.Name}{extension}")))))
                        {
                            if (!candidateAssemblyFile.Exists)
                            {
                                continue;
                            }

                            return assemblyLoader(candidateAssemblyFile.FullName);
                        }

                        return null;
                    });

                    LoadedAssemblies.Add(requestedAssemblyName.FullName, assemblyLazy);
                }

                return assemblyLazy.Value;
            }
        }
    }
}
