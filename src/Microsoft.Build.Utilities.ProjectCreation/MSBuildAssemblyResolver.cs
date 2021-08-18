// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Concurrent;
#if !NETFRAMEWORK
using System.Diagnostics;
#endif

using System.IO;
using System.Linq;
using System.Reflection;
#if NETCOREAPP || NET5_0_OR_GREATER
using System.Runtime.Loader;
#endif
#if !NETFRAMEWORK
using System.Text.RegularExpressions;
#endif
#if NETFRAMEWORK
using Microsoft.VisualStudio.Setup.Configuration;
#endif

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Resolves MSBuild assemblies.
    /// </summary>
    public static class MSBuildAssemblyResolver
    {
        private static readonly string[] AssemblyExtensions = { ".dll", ".exe" };

        private static readonly ConcurrentDictionary<string, Lazy<Assembly>> LoadedAssemblies = new ConcurrentDictionary<string, Lazy<Assembly>>(StringComparer.OrdinalIgnoreCase);

#if !NETFRAMEWORK
        private static readonly Regex DotNetListSdksRegex = new Regex("^(?<Name>(?<Version>\\d+\\.\\d+\\.\\d{3,4})(?<Tag>[-\\.\\w]*)) \\[(?<Directory>.*)\\]\\r?$", RegexOptions.Multiline);
#endif

#if NETCOREAPP || NET5_0_OR_GREATER
        private static readonly AssemblyLoadContext LoadContext = new AssemblyLoadContext(nameof(MSBuildAssemblyResolver));
#endif

        private static readonly Lazy<string> DotNetSdksPathLazy = new Lazy<string>(
        () =>
        {
#if NETFRAMEWORK
            return null;
#else
            return GetDotNetBasePath();
#endif
        });

        private static readonly Lazy<string[]> MSBuildDirectoryLazy = new Lazy<string[]>(
            () =>
            {
#if !NETFRAMEWORK
                if (!string.IsNullOrWhiteSpace(DotNetSdksPath))
                {
                    return new[]
                    {
                        DotNetSdksPath,
                        Path.Combine(DotNetSdksPathLazy.Value, "Roslyn", "bincore"),
                    };
                }
#else
                string msbuildBinPath = null;
                string visualStudioDirectory;

                if (!string.IsNullOrWhiteSpace(visualStudioDirectory = Environment.GetEnvironmentVariable("VSINSTALLDIR")))
                {
                    msbuildBinPath = Path.Combine(visualStudioDirectory, "MSBuild", GetMSBuildVersionDirectory(Environment.GetEnvironmentVariable("VISUALSTUDIOVERSION") ?? "15.0"), "Bin");
                }
                else if (!string.IsNullOrWhiteSpace(visualStudioDirectory = Environment.GetEnvironmentVariable("VSAPPIDDIR")))
                {
                    msbuildBinPath = Path.Combine(visualStudioDirectory, "..", "..", "MSBuild", GetMSBuildVersionDirectory(Environment.GetEnvironmentVariable("VISUALSTUDIOVERSION") ?? "15.0"), "Bin");
                }
                else
                {
                    foreach (string path in (Environment.GetEnvironmentVariable("PATH") ?? string.Empty).Split(PathSplitChars, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (File.Exists(Path.Combine(path, "MSBuild.exe")))
                        {
                            msbuildBinPath = path;
                            break;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(msbuildBinPath))
                    {
                        msbuildBinPath = GetPathOfFirstInstalledVisualStudioInstance();
                    }
                }

                if (!string.IsNullOrWhiteSpace(msbuildBinPath))
                {
                    return new[]
                    {
                        Path.GetFullPath(msbuildBinPath),
                        Path.Combine(msbuildBinPath, "Roslyn"),
                        Path.GetFullPath(Path.Combine(msbuildBinPath, @"..\..\..\Common7\IDE\CommonExtensions\Microsoft\NuGet")),
                    };
                }
#endif
                return null;
            });

#if NETFRAMEWORK
        private static readonly char[] PathSplitChars = { Path.PathSeparator };
#endif

        /// <summary>
        /// Gets the path to the .NET SDKs.
        /// </summary>
        public static string DotNetSdksPath => DotNetSdksPathLazy.Value;

        /// <summary>
        /// Gets the full path to the MSBuild directory used.
        /// </summary>
        public static string[] SearchPaths => MSBuildDirectoryLazy.Value;

        /// <summary>
        /// A <see cref="ResolveEventHandler"/> for MSBuild related assemblies.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event data.</param>
        /// <returns>An MSBuild assembly if one could be located, otherwise <c>null</c>.</returns>
        public static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (SearchPaths == null)
            {
                return null;
            }

            AssemblyName requestedAssemblyName = new AssemblyName(args.Name);

            foreach (FileInfo candidateAssemblyFile in SearchPaths.SelectMany(searchPath => AssemblyExtensions.Select(extension => new FileInfo(Path.Combine(searchPath, $"{requestedAssemblyName.Name}{extension}")))))
            {
                Lazy<Assembly> assemblyLazy = LoadedAssemblies.GetOrAdd(
                    candidateAssemblyFile.FullName,
                    _ =>
                    {
                        return new Lazy<Assembly>(() =>
                        {
                            if (!candidateAssemblyFile.Exists)
                            {
                                return null;
                            }

                            AssemblyName candidateAssemblyName = AssemblyName.GetAssemblyName(candidateAssemblyFile.FullName);

                            if (requestedAssemblyName.ProcessorArchitecture != ProcessorArchitecture.None && requestedAssemblyName.ProcessorArchitecture != candidateAssemblyName.ProcessorArchitecture)
                            {
                                // The requested assembly has a processor architecture and the candidate assembly has a different value
                                return null;
                            }

                            if (requestedAssemblyName.Flags.HasFlag(AssemblyNameFlags.PublicKey) && !requestedAssemblyName.GetPublicKeyToken() !.SequenceEqual(candidateAssemblyName.GetPublicKeyToken() !))
                            {
                                // Requested assembly has a public key but it doesn't match the candidate assembly public key
                                return null;
                            }

#if NETCOREAPP || NET5_0_OR_GREATER
                            return LoadContext.LoadFromAssemblyPath(candidateAssemblyFile.FullName);
#else
                            return Assembly.LoadFile(candidateAssemblyFile.FullName);
#endif
                        });
                    });

                if (assemblyLazy.Value != null)
                {
                    return assemblyLazy.Value;
                }
            }

            return null;
        }

#if !NETFRAMEWORK
        private static string GetDotNetBasePath()
        {
            using Process process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    Arguments = "--list-sdks",
                    CreateNoWindow = true,
                    FileName = "dotnet",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                },
            };

            process.StartInfo.EnvironmentVariables["DOTNET_CLI_UI_LANGUAGE"] = "en-US";
            process.StartInfo.EnvironmentVariables["DOTNET_MULTILEVEL_LOOKUP"] = "0";
            process.StartInfo.EnvironmentVariables["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";

            try
            {
                if (!process.Start())
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }

            process.WaitForExit();

            // Gets the highest version SDK that is the same major version as the runtime
            // You cannot always evaluate MSBuild projects using a different version of MSBuild than your runtime.  This is because
            // if you're running as .NET 5.0, the .NET 6.0 MSBuild has dependencies that your app doesn't supply.
            // This means that if your app is .NET Core 3.1, you can only use MSBuild from the .NET Core 3.1 SDK
            DirectoryInfo GetFirstMatchingSdk(string output)
            {
                Match match = DotNetListSdksRegex.Match(output);

                DirectoryInfo directoryInfo = null;

                Version highestVersion = null;

                while (match.Success)
                {
                    string versionString = match.Groups["Version"].Value.Trim();

                    if (Version.TryParse(versionString, out Version version) && Environment.Version.Major == version.Major && (highestVersion == null || version > highestVersion))
                    {
                        highestVersion = version;

                        directoryInfo = new DirectoryInfo(Path.Combine(match.Groups["Directory"].Value.Trim(), match.Groups["Name"].Value.Trim()));
                    }

                    match = match.NextMatch();
                }

                return directoryInfo;
            }

            DirectoryInfo basePath = GetFirstMatchingSdk(process.StandardOutput.ReadToEnd());

            return basePath?.FullName;
        }
#endif

#if NETFRAMEWORK
        private static string GetMSBuildVersionDirectory(string version)
        {
            if (Version.TryParse(version, out Version visualStudioVersion) && visualStudioVersion.Major >= 16)
            {
                return "Current";
            }

            return version;
        }

        private static string GetPathOfFirstInstalledVisualStudioInstance()
        {
            Tuple<Version, string> highestVersion = null;

            try
            {
                IEnumSetupInstances setupInstances = new SetupConfiguration().EnumAllInstances();

                ISetupInstance[] instances = new ISetupInstance[1];

                for (setupInstances.Next(1, instances, out int fetched); fetched > 0; setupInstances.Next(1, instances, out fetched))
                {
                    ISetupInstance2 instance = (ISetupInstance2)instances.First();

                    if (instance.GetState() == InstanceState.Complete)
                    {
                        string installationPath = instance.GetInstallationPath();

                        if (!string.IsNullOrWhiteSpace(installationPath) && Version.TryParse(instance.GetInstallationVersion(), out Version version))
                        {
                            if (highestVersion == null || version > highestVersion.Item1)
                            {
                                highestVersion = new Tuple<Version, string>(version, installationPath);
                            }
                        }
                    }
                }

                if (highestVersion != null)
                {
                    return Path.Combine(highestVersion.Item2, "MSBuild", GetMSBuildVersionDirectory($"{highestVersion.Item1.Major}.0"), "Bin");
                }
            }
            catch
            {
                // Ignored
            }

            return null;
        }
#endif
    }
}