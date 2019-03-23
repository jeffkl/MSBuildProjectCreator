// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
#if NET46
using Microsoft.VisualStudio.Setup.Configuration;
#endif

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Resolves MSBuild assemblies.
    /// </summary>
    public static class MSBuildAssemblyResolver
    {
        private static readonly Lazy<string> MSBuildDirectoryLazy = new Lazy<string>(
            () =>
            {
                string visualStudioDirectory;

                if (!String.IsNullOrWhiteSpace(visualStudioDirectory = Environment.GetEnvironmentVariable("VSINSTALLDIR")))
                {
                    return Path.Combine(visualStudioDirectory, "MSBuild", GetMSBuildVersionDirectory(Environment.GetEnvironmentVariable("VISUALSTUDIOVERSION") ?? "15.0"), "Bin");
                }

                if (!String.IsNullOrWhiteSpace(visualStudioDirectory = Environment.GetEnvironmentVariable("VSAPPIDDIR")))
                {
                    return Path.GetFullPath(Path.Combine(visualStudioDirectory, "..", "..", "MSBuild", GetMSBuildVersionDirectory(Environment.GetEnvironmentVariable("VISUALSTUDIOVERSION") ?? "15.0"), "Bin"));
                }

                foreach (string path in (Environment.GetEnvironmentVariable("PATH") ?? String.Empty).Split(PathSplitChars, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (File.Exists(Path.Combine(path, "MSBuild.exe")))
                    {
                        return path;
                    }
                }
#if NET46
                if (!String.IsNullOrWhiteSpace(visualStudioDirectory = MSBuildAssemblyResolver.GetPathOfFirstInstalledVisualStudioInstance()))
                {
                    return visualStudioDirectory;
                }
#endif
                return null;
            },
            isThreadSafe: true);

        private static readonly char[] PathSplitChars = { Path.PathSeparator };

        /// <summary>
        /// Gets the full path to the MSBuild directory used.
        /// </summary>
        public static string MSBuildPath => MSBuildDirectoryLazy.Value;

        /// <summary>
        /// A <see cref="ResolveEventHandler"/> for MSBuild related assemblies.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event data.</param>
        /// <returns>An MSBuild assembly if one could be located, otherwise <code>null</code>.</returns>
        public static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName assemblyName = new AssemblyName(args.Name);

            if (MSBuildPath == null)
            {
                return null;
            }

            FileInfo fileInfo = new FileInfo(Path.Combine(MSBuildPath, $"{assemblyName.Name}.dll"));

            if (!fileInfo.Exists)
            {
                return null;
            }

            return !assemblyName.FullName.Equals(AssemblyName.GetAssemblyName(fileInfo.FullName).FullName) ? null : Assembly.LoadFrom(fileInfo.FullName);
        }

        private static string GetMSBuildVersionDirectory(string version)
        {
            if (Version.TryParse(version, out Version visualStudioVersion) && visualStudioVersion.Major >= 16)
            {
                return "Current";
            }

            return version;
        }

#if NET46
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

                        if (!String.IsNullOrWhiteSpace(installationPath) && Version.TryParse(instance.GetInstallationVersion(), out Version version))
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