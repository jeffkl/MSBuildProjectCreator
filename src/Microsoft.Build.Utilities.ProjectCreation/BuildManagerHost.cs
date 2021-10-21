// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Provides access to builds.
    /// </summary>
    internal static class BuildManagerHost
    {
        /// <summary>
        /// Gets a value indicating if the current runtime is .NET Core or .NET Framework.
        /// </summary>
        private static readonly Lazy<bool> IsDotNetCoreLazy = new Lazy<bool>(() => !RuntimeInformation.FrameworkDescription.Contains("Framework"));

        private static readonly object LockObject = new object();

        /// <summary>
        /// Gets a value indicating the full path to the loaded Microsoft.Build.dll
        /// </summary>
        private static readonly Lazy<FileInfo> MSBuildAssemblyFullPathLazy = new Lazy<FileInfo>(() => new FileInfo(typeof(BuildManager).Assembly.Location));

        /// <summary>
        /// Executes a build for the specified project.
        /// </summary>
        /// <param name="projectFullPath">The full path to the project.</param>
        /// <param name="targets">An optional list of targets to execute.</param>
        /// <param name="globalProperties">An optional <see cref="IDictionary{TKey,TValue}" /> containing global properties to use when building.</param>
        /// <param name="loggers">An <see cref="IEnumerable{T}" /> containing <see cref="ILogger" /> items to use.</param>
        /// <param name="buildRequestDataFlags">The <see cref="BuildRequestDataFlags" /> to use.</param>
        /// <returns>A <see cref="BuildResult" /> containing details about the result of the build.</returns>
        public static BuildResult Build(string projectFullPath, string[] targets, IDictionary<string, string> globalProperties, IEnumerable<ILogger> loggers, BuildRequestDataFlags buildRequestDataFlags)
        {
            BuildRequestData buildRequestData = new BuildRequestData(
                projectFullPath,
                globalProperties ?? new Dictionary<string, string>(),
                toolsVersion: null,
                targets ?? Array.Empty<string>(),
                hostServices: null,
                buildRequestDataFlags);

            return Build(buildRequestData, loggers);
        }

        /// <summary>
        /// Executes a build for the specified project.
        /// </summary>
        /// <param name="projectInstance">A <see cref="ProjectInstance" /> representing the project.</param>
        /// <param name="targets">An optional list of targets to execute.</param>
        /// <param name="globalProperties">An optional <see cref="IDictionary{TKey,TValue}" /> containing global properties to use when building.</param>
        /// <param name="loggers">An <see cref="IEnumerable{T}" /> containing <see cref="ILogger" /> items to use.</param>
        /// <param name="buildRequestDataFlags">The <see cref="BuildRequestDataFlags" /> to use.</param>
        /// <returns>A <see cref="BuildResult" /> containing details about the result of the build.</returns>
        public static BuildResult Build(ProjectInstance projectInstance, string[] targets, IDictionary<string, string> globalProperties, IEnumerable<ILogger> loggers, BuildRequestDataFlags buildRequestDataFlags)
        {
            BuildRequestData buildRequestData = new BuildRequestData(
                projectInstance,
                targets ?? Array.Empty<string>(),
                hostServices: null,
                buildRequestDataFlags);

            return Build(buildRequestData, loggers);
        }

        /// <summary>
        /// Sets the host for MSBuild to launch nodes.  For .NET Core this is dotnet.exe and for .NET Framework this is MSBuild.exe.
        /// This is a workaround for https://github.com/dotnet/msbuild/issues/6782
        /// </summary>
        /// <param name="buildManager">The current <see cref="BuildManager" />.</param>
        public static void SetCurrentHost(BuildManager buildManager)
        {
            if (!IsDotNetCoreLazy.Value)
            {
                return;
            }

            string hostExePath = Path.Combine(Path.GetFullPath(Path.Combine(MSBuildAssemblyFullPathLazy.Value.DirectoryName!, "..", "..")), RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "dotnet.exe" : "dotnet");

            if (!File.Exists(hostExePath))
            {
                return;
            }

            Type buildManagerType = buildManager.GetType();

            FieldInfo? nodeManagerFieldInfo = buildManagerType.GetField("_nodeManager", BindingFlags.Instance | BindingFlags.NonPublic);

            if (nodeManagerFieldInfo == null)
            {
                return;
            }

            object? nodeManager = nodeManagerFieldInfo.GetValue(buildManager);

            if (nodeManager == null)
            {
                return;
            }

            Type nodeManagerType = nodeManager.GetType();

            FieldInfo? outOfProcNodeProviderFieldInfo = nodeManagerType.GetField("_outOfProcNodeProvider", BindingFlags.Instance | BindingFlags.NonPublic);

            if (outOfProcNodeProviderFieldInfo == null)
            {
                return;
            }

            object? outOfProcNodeProvider = outOfProcNodeProviderFieldInfo.GetValue(nodeManager);

            if (outOfProcNodeProvider == null)
            {
                return;
            }

            Type? nodeProviderOutOfProcBaseType = outOfProcNodeProvider.GetType().BaseType;

            if (nodeProviderOutOfProcBaseType == null)
            {
                return;
            }

            FieldInfo? currentHostFieldInfo = nodeProviderOutOfProcBaseType.GetField("CurrentHost", BindingFlags.Static | BindingFlags.NonPublic);

            if (currentHostFieldInfo == null)
            {
                return;
            }

            currentHostFieldInfo.SetValue(outOfProcNodeProvider, hostExePath);
        }

        private static BuildResult Build(BuildRequestData buildRequestData, IEnumerable<ILogger> loggers)
        {
            lock (LockObject)
            {
                MuxLogger muxLogger = new MuxLogger
                {
                    Verbosity = LoggerVerbosity.Diagnostic,
                };

                BuildParameters buildParameters = new BuildParameters
                {
                    EnableNodeReuse = false,
                    MaxNodeCount = Environment.ProcessorCount,
                    ResetCaches = true,
                };

                LoggerDescription forwardingLoggerDescription = new LoggerDescription(
                    loggerClassName: typeof(ConfigurableForwardingLogger).FullName,
                    loggerAssemblyName: typeof(ConfigurableForwardingLogger).Assembly.GetName().FullName,
                    loggerAssemblyFile: null,
                    loggerSwitchParameters: string.Empty,
                    verbosity: muxLogger.Verbosity);

                ForwardingLoggerRecord forwardingLoggerRecord = new ForwardingLoggerRecord(
                    muxLogger,
                    forwardingLoggerDescription);

                buildParameters.ForwardingLoggers = new[]
                {
                    forwardingLoggerRecord,
                };

                BuildManager.DefaultBuildManager.BeginBuild(buildParameters);

                try
                {
                    BuildSubmission buildSubmission = BuildManager.DefaultBuildManager.PendBuildRequest(buildRequestData);

                    foreach (ILogger logger in loggers)
                    {
                        muxLogger.RegisterLogger(buildSubmission.SubmissionId, logger);
                    }

                    try
                    {
                        SetCurrentHost(BuildManager.DefaultBuildManager);

                        BuildResult buildResult = buildSubmission.Execute();

                        if (buildResult.Exception != null)
                        {
                            throw buildResult.Exception;
                        }

                        return buildResult;
                    }
                    finally
                    {
                        muxLogger.UnregisterLoggers(buildSubmission.SubmissionId);
                    }
                }
                finally
                {
                    BuildManager.DefaultBuildManager.EndBuild();
                }
            }
        }
    }
}