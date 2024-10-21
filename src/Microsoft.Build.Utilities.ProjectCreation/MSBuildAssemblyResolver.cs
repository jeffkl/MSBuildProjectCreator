// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
#if NET8_0_OR_GREATER
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

        private static readonly ConcurrentDictionary<string, Lazy<Assembly?>> LoadedAssemblies = new ConcurrentDictionary<string, Lazy<Assembly?>>(StringComparer.OrdinalIgnoreCase);

        private static readonly Lazy<object> RegisterLazy = new Lazy<object>(() =>
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

            foreach (FileInfo candidateAssemblyFile in SearchPaths.SelectMany(searchPath => AssemblyExtensions.Select(extension => new FileInfo(Path.Combine(searchPath, $"{requestedAssemblyName.Name}{extension}")))))
            {
                Lazy<Assembly?> assemblyLazy = LoadedAssemblies.GetOrAdd(
                    candidateAssemblyFile.FullName,
                    _ =>
                    {
                        return new Lazy<Assembly?>(() =>
                        {
                            if (!candidateAssemblyFile.Exists)
                            {
                                return null;
                            }

                            AssemblyName candidateAssemblyName = AssemblyName.GetAssemblyName(candidateAssemblyFile.FullName);
#if NETFRAMEWORK
                            if (requestedAssemblyName.ProcessorArchitecture != System.Reflection.ProcessorArchitecture.None && requestedAssemblyName.ProcessorArchitecture != candidateAssemblyName.ProcessorArchitecture)
                            {
                                // The requested assembly has a processor architecture and the candidate assembly has a different value
                                return null;
                            }

                            if (requestedAssemblyName.Flags.HasFlag(AssemblyNameFlags.PublicKey) && !requestedAssemblyName.GetPublicKeyToken()!.SequenceEqual(candidateAssemblyName.GetPublicKeyToken()!))
                            {
                                // Requested assembly has a public key but it doesn't match the candidate assembly public key
                                return null;
                            }
#endif

                            return assemblyLoader(candidateAssemblyFile.FullName);
                        });
                    });

                if (assemblyLazy.Value != null)
                {
                    return assemblyLazy.Value;
                }
            }

            return null;
        }
    }
}