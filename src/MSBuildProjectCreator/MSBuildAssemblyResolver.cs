// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
#if NETCORE
using System.Diagnostics;
#endif
using System.IO;
using System.Linq;
using System.Reflection;
#if NETCORE
using System.Text.RegularExpressions;
using System.Threading;
#endif
#if !NETCORE
using Microsoft.VisualStudio.Setup.Configuration;
#endif

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Resolves MSBuild assemblies.
    /// </summary>
    public static class MSBuildAssemblyResolver
    {
        private static readonly Lazy<string[]> MSBuildDirectoryLazy = new Lazy<string[]>(
            () =>
            {
#if NETCORE
                string basePath = GetDotNetBasePath();

                if (!string.IsNullOrWhiteSpace(basePath))
                {
                    return new[]
                    {
                        basePath,
                        Path.Combine(basePath, "Roslyn", "bincore"),
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

        private static readonly char[] PathSplitChars = { Path.PathSeparator };

        private static readonly string[] AssemblyExtensions = { ".dll", ".exe" };

#if NETCORE
        private static readonly Regex DotNetBasePathRegex = new Regex(@"^ Base Path:\s+(?<Path>.*)$");
#endif

        /// <summary>
        /// Gets the full path to the MSBuild directory used.
        /// </summary>
        public static string[] SearchPaths => MSBuildDirectoryLazy.Value;

        /// <summary>
        /// A <see cref="ResolveEventHandler"/> for MSBuild related assemblies.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event data.</param>
        /// <returns>An MSBuild assembly if one could be located, otherwise <code>null</code>.</returns>
        public static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (SearchPaths == null)
            {
                return null;
            }

            AssemblyName requestedAssemblyName = new AssemblyName(args.Name);

            foreach (FileInfo candidateAssemblyFile in SearchPaths.SelectMany(searchPath => AssemblyExtensions.Select(extension => new FileInfo(Path.Combine(searchPath, $"{requestedAssemblyName.Name}{extension}")))))
            {
                if (!candidateAssemblyFile.Exists)
                {
                    continue;
                }

                AssemblyName candidateAssemblyName = AssemblyName.GetAssemblyName(candidateAssemblyFile.FullName);

                if (requestedAssemblyName.ProcessorArchitecture != ProcessorArchitecture.None && requestedAssemblyName.ProcessorArchitecture != candidateAssemblyName.ProcessorArchitecture)
                {
                    // The requested assembly has a processor architecture and the candidate assembly has a different value
                    continue;
                }

                if (requestedAssemblyName.Flags.HasFlag(AssemblyNameFlags.PublicKey) && !requestedAssemblyName.GetPublicKeyToken().SequenceEqual(candidateAssemblyName.GetPublicKeyToken()))
                {
                    // Requested assembly has a public key but it doesn't match the candidate assembly public key
                    continue;
                }

                return Assembly.LoadFrom(candidateAssemblyFile.FullName);
            }

            return null;
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