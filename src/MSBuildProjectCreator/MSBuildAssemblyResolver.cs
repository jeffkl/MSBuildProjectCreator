// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Resolves MSBuild assemblies.
    /// </summary>
    public static class MSBuildAssemblyResolver
    {
        /// <summary>
        /// The MSBuild public key token b03f5f7f11d50a3a.
        /// </summary>
        private static readonly byte[] MicrosoftPublicKeyToken = { 0xB0, 0x3F, 0x5F, 0x7F, 0x11, 0xD5, 0x0A, 0x3A };

        private static readonly Lazy<string> MSBuildDirectoryLazy = new Lazy<string>(
            () =>
            {
                string visualStudioDirectory;

                if (!String.IsNullOrWhiteSpace(visualStudioDirectory = Environment.GetEnvironmentVariable("VSINSTALLDIR")))
                {
                    return Path.Combine(visualStudioDirectory, "MSBuild", "15.0", "Bin");
                }

                if (!String.IsNullOrWhiteSpace(visualStudioDirectory = Environment.GetEnvironmentVariable("VSAPPIDDIR")))
                {
                    return Path.Combine(visualStudioDirectory, "..", "..", "MSBuild", "15.0", "Bin");
                }

                return null;
            },
            isThreadSafe: true);

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

            if (!assemblyName.Name.StartsWith("Microsoft.Build") || !MicrosoftPublicKeyToken.SequenceEqual(assemblyName.GetPublicKeyToken()) || String.IsNullOrWhiteSpace(MSBuildDirectoryLazy.Value))
            {
                return null;
            }

            FileInfo fileInfo = new FileInfo(Path.Combine(MSBuildDirectoryLazy.Value, $"{assemblyName.Name}.dll"));

            if (!fileInfo.Exists)
            {
                return null;
            }

            return Assembly.LoadFrom(fileInfo.FullName);
        }
    }
}