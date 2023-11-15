// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

#if !NETFRAMEWORK
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.RegularExpressions;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Resolves MSBuild assemblies.
    /// </summary>
    public static partial class MSBuildAssemblyResolver
    {
        private static readonly Regex DotNetListSdksRegex = new Regex("^(?<Name>(?<Version>\\d+\\.\\d+\\.\\d{3,4})(?<Tag>[-\\.\\w]*)) \\[(?<Directory>.*)\\]\\r?$", RegexOptions.Multiline);

        private static readonly Lazy<string?> DotNetSdksPathLazy = new Lazy<string?>(GetDotNetBasePath);

        private static readonly Lazy<(string[]? SearchPaths, string? MSBuildExePath)> MSBuildDirectoryLazy = new Lazy<(string[]?, string?)>(
            () =>
            {
                if (!string.IsNullOrWhiteSpace(DotNetSdksPath))
                {
                    return (
                        new[]
                        {
                            DotNetSdksPath,
                            Path.Combine(DotNetSdksPathLazy.Value!, "Roslyn", "bincore"),
                        },
                        Path.Combine(DotNetSdksPath, "MSBuild.dll"));
                }

                return (null, null);
            });

        /// <inheritdoc cref="AssemblyLoadContext.Resolving" />
        public static Assembly? AssemblyResolve(AssemblyLoadContext assemblyLoadContext, AssemblyName requestedAssemblyName)
        {
            return AssemblyResolve(requestedAssemblyName, assemblyLoadContext.LoadFromAssemblyPath);
        }

        private static string? GetDotNetBasePath()
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

            DirectoryInfo? basePath = GetFirstMatchingSdk(process.StandardOutput.ReadToEnd());

            return basePath?.FullName;

            // Gets the highest version SDK that is the same major version as the runtime
            // You cannot always evaluate MSBuild projects using a different version of MSBuild than your runtime.  This is because
            // if you're running as .NET 5.0, the .NET 6.0 MSBuild has dependencies that your app doesn't supply.
            // This means that if your app is .NET Core 3.1, you can only use MSBuild from the .NET Core 3.1 SDK
            DirectoryInfo? GetFirstMatchingSdk(string output)
            {
                Match match = DotNetListSdksRegex.Match(output);

                DirectoryInfo? directoryInfo = null;

                Version? highestVersion = null;

                while (match.Success)
                {
                    string versionString = match.Groups["Version"].Value.Trim();

                    if (Version.TryParse(versionString, out Version? version) && Environment.Version.Major == version.Major && (highestVersion == null || version > highestVersion))
                    {
                        highestVersion = version;

                        directoryInfo = new DirectoryInfo(Path.Combine(match.Groups["Directory"].Value.Trim(), match.Groups["Name"].Value.Trim()));
                    }

                    match = match.NextMatch();
                }

                return directoryInfo;
            }
        }
    }
}
#endif