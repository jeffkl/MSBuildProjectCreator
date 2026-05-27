// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
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
        private static readonly object LockObject = new object();

        /// <summary>
        /// Executes a build for the specified project.
        /// </summary>
        /// <param name="projectFullPath">The full path to the project.</param>
        /// <param name="targets">An optional list of targets to execute.</param>
        /// <param name="globalProperties">An optional <see cref="IDictionary{TKey,TValue}" /> containing global properties to use when building.</param>
        /// <param name="loggers">An <see cref="IEnumerable{T}" /> containing <see cref="ILogger" /> items to use.</param>
        /// <param name="buildRequestDataFlags">The <see cref="BuildRequestDataFlags" /> to use.</param>
        /// <returns>A <see cref="BuildResult" /> containing details about the result of the build.</returns>
        public static BuildResult Build(
            string projectFullPath,
            string[] targets,
#if NET8_0
            IDictionary<string, string> globalProperties,
#else
            IDictionary<string, string?> globalProperties,
#endif
            IEnumerable<ILogger> loggers,
            BuildRequestDataFlags buildRequestDataFlags)
        {
            BuildRequestData buildRequestData = new BuildRequestData(
                projectFullPath,
#if NET8_0
                globalProperties ?? new Dictionary<string, string>(),
#else
                globalProperties ?? new Dictionary<string, string?>(),
#endif
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

        private static BuildResult Build(BuildRequestData buildRequestData, IEnumerable<ILogger> loggers)
        {
            lock (LockObject)
            {
                BuildParameters buildParameters = new BuildParameters
                {
                    EnableNodeReuse = false,
                    MaxNodeCount = Environment.ProcessorCount,
                    ResetCaches = true,
                    Loggers = loggers,
                };

                BuildManager.DefaultBuildManager.BeginBuild(buildParameters);

                try
                {
                    BuildSubmission buildSubmission = BuildManager.DefaultBuildManager.PendBuildRequest(buildRequestData);

                    BuildResult buildResult = buildSubmission.Execute();

                    if (buildResult.Exception != null)
                    {
                        throw buildResult.Exception;
                    }

                    return buildResult;
                }
                finally
                {
                    BuildManager.DefaultBuildManager.EndBuild();
                }
            }
        }
    }
}