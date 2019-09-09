// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

#if !NETCORE
using System.Linq;
using Microsoft.VisualStudio.Setup.Configuration;
#endif

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Resolves MSBuild assemblies.
    /// </summary>
    public static class MSBuildAssemblyResolver
    {
#if NETCORE
        private static readonly Regex DotNetBasePathRegex = new Regex(@"^ Base Path:\s+(?<Path>.*)$");
#endif

        private static readonly Lazy<string> MSBuildDirectoryLazy = new Lazy<string>(
            () =>
            {
#if NETCORE
                string basePath;

                if (!string.IsNullOrWhiteSpace(basePath = GetDotNetBasePath()))
                {
                    return basePath;
                }

                if (!string.IsNullOrWhiteSpace(basePath = Environment.GetEnvironmentVariable("MSBuildExtensionsPath")))
                {
                    return basePath;
                }
#else
                string visualStudioDirectory;

                if (!string.IsNullOrWhiteSpace(visualStudioDirectory = Environment.GetEnvironmentVariable("VSINSTALLDIR")))
                {
                    return Path.Combine(visualStudioDirectory, "MSBuild", GetMSBuildVersionDirectory(Environment.GetEnvironmentVariable("VISUALSTUDIOVERSION") ?? "15.0"), "Bin");
                }

                if (!string.IsNullOrWhiteSpace(visualStudioDirectory = Environment.GetEnvironmentVariable("VSAPPIDDIR")))
                {
                    return Path.GetFullPath(Path.Combine(visualStudioDirectory, "..", "..", "MSBuild", GetMSBuildVersionDirectory(Environment.GetEnvironmentVariable("VISUALSTUDIOVERSION") ?? "15.0"), "Bin"));
                }

                foreach (string path in (Environment.GetEnvironmentVariable("PATH") ?? string.Empty).Split(PathSplitChars, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (File.Exists(Path.Combine(path, "MSBuild.exe")))
                    {
                        return path;
                    }
                }

                if (!string.IsNullOrWhiteSpace(visualStudioDirectory = MSBuildAssemblyResolver.GetPathOfFirstInstalledVisualStudioInstance()))
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

#if NETCORE

        private static string GetDotNetBasePath()
        {
            string basePath = null;

            using (ManualResetEvent processExited = new ManualResetEvent(false))
            using (Process process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo
                {
                    Arguments = "--info",
                    CreateNoWindow = true,
                    FileName = "dotnet",
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                },
            })
            {
                process.StartInfo.EnvironmentVariables["DOTNET_CLI_UI_LANGUAGE"] = "en-US";

                process.ErrorDataReceived += (sender, args) => { };

                process.OutputDataReceived += (sender, args) =>
                {
                    if (!String.IsNullOrWhiteSpace(args?.Data))
                    {
                        Match match = DotNetBasePathRegex.Match(args.Data);

                        if (match.Success && match.Groups["Path"].Success)
                        {
                            basePath = match.Groups["Path"].Value.Trim();
                        }
                    }
                };

                process.Exited += (sender, args) => { processExited.Set(); };

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

                process.StandardInput.Close();

                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                switch (WaitHandle.WaitAny(new WaitHandle[] { processExited }, TimeSpan.FromSeconds(5)))
                {
                    case WaitHandle.WaitTimeout:
                        break;

                    case 0:
                        break;
                }

                if (!process.HasExited)
                {
                    try
                    {
                        process.Kill();
                    }
                    catch
                    {
                    }
                }

                return basePath;
            }
        }

#endif

        private static string GetMSBuildVersionDirectory(string version)
        {
            if (Version.TryParse(version, out Version visualStudioVersion) && visualStudioVersion.Major >= 16)
            {
                return "Current";
            }

            return version;
        }

#if !NETCORE
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