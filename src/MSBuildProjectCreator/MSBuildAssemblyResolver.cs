// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Reflection;

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
                string msbuildVersionDirectory = Environment.GetEnvironmentVariable("VISUALSTUDIOVERSION") ?? "15.0";

                if (Version.TryParse(msbuildVersionDirectory, out Version visualStudioVersion) && visualStudioVersion.Major >= 16)
                {
                    msbuildVersionDirectory = "Current";
                }

                if (!String.IsNullOrWhiteSpace(visualStudioDirectory = Environment.GetEnvironmentVariable("VSINSTALLDIR")))
                {
                    return Path.Combine(visualStudioDirectory, "MSBuild", msbuildVersionDirectory, "Bin");
                }

                if (!String.IsNullOrWhiteSpace(visualStudioDirectory = Environment.GetEnvironmentVariable("VSAPPIDDIR")))
                {
                    return Path.GetFullPath(Path.Combine(visualStudioDirectory, "..", "..", "MSBuild", msbuildVersionDirectory, "Bin"));
                }

                foreach (string path in (Environment.GetEnvironmentVariable("PATH") ?? String.Empty).Split(PathSplitChars, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (File.Exists(Path.Combine(path, "MSBuild.exe")))
                    {
                        return path;
                    }
                }

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

            FileInfo fileInfo = new FileInfo(Path.Combine(MSBuildPath, $"{assemblyName.Name}.dll"));

            if (!fileInfo.Exists)
            {
                return null;
            }

            return !assemblyName.FullName.Equals(AssemblyName.GetAssemblyName(fileInfo.FullName).FullName) ? null : Assembly.LoadFrom(fileInfo.FullName);
        }
    }
}