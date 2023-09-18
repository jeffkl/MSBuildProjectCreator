// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

#if NETFRAMEWORK
using Microsoft.VisualStudio.Setup.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Resolves MSBuild assemblies.
    /// </summary>
    public static partial class MSBuildAssemblyResolver
    {
        private static readonly Lazy<string?> DotNetSdksPathLazy = new Lazy<string?>(() => null);

        private static readonly Lazy<(string[]? SearchPaths, string? MSBuildExePath)> MSBuildDirectoryLazy = new Lazy<(string[]?, string?)>(
            () =>
            {
                string? msbuildBinPath = null;
                string visualStudioDirectory;

                if (!string.IsNullOrWhiteSpace(visualStudioDirectory = Environment.GetEnvironmentVariable("VSINSTALLDIR")!))
                {
                    msbuildBinPath = Path.Combine(visualStudioDirectory, "MSBuild", GetMSBuildVersionDirectory(Environment.GetEnvironmentVariable("VISUALSTUDIOVERSION") ?? "15.0"), "Bin");
                }
                else if (!string.IsNullOrWhiteSpace(visualStudioDirectory = Environment.GetEnvironmentVariable("VSAPPIDDIR")!))
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
                    return (
                        new[]
                        {
                            Path.GetFullPath(msbuildBinPath!),
                            Path.Combine(msbuildBinPath!, "Roslyn"),
                            Path.GetFullPath(Path.Combine(msbuildBinPath!, @"..\..\..\Common7\IDE\CommonExtensions\Microsoft\NuGet")),
                        },
                        Environment.Is64BitProcess ? Path.Combine(msbuildBinPath!, "amd64", "MSBuild.exe") : Path.Combine(msbuildBinPath!, "MSBuild.exe"));
                }

                return (null, null);
            });

        private static readonly char[] PathSplitChars = { Path.PathSeparator };

        /// <summary>
        /// A <see cref="ResolveEventHandler" /> for MSBuild related assemblies.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event data.</param>
        /// <returns>An MSBuild assembly if one could be located, otherwise <see langword="null" />.</returns>
        public static Assembly? AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            return AssemblyResolve(new AssemblyName(args.Name!), Assembly.LoadFrom);
        }

        private static string GetMSBuildVersionDirectory(string version)
        {
            if (Version.TryParse(version, out Version visualStudioVersion) && visualStudioVersion.Major >= 16)
            {
                return "Current";
            }

            return version;
        }

        private static string? GetPathOfFirstInstalledVisualStudioInstance()
        {
            Tuple<Version, string>? highestVersion = null;

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
    }
}
#endif