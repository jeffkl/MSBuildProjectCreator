// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Provides access to builds.
    /// </summary>
    internal static class BuildManagerHost
    {
        public static readonly object LockObject = new();

        public static void BeginBuild(IEnumerable<ILogger> loggers)
        {
            BuildParameters buildParameters = new()
            {
                EnableNodeReuse = false,
                MaxNodeCount = Environment.ProcessorCount,
                ResetCaches = true,
                Loggers = loggers,
                LogTaskInputs = true,
            };

            BuildManager.DefaultBuildManager.BeginBuild(buildParameters);
        }

        /// <summary>
        /// Executes a build for the specified project.
        /// </summary>
        /// <param name="projectFullPath">The full path to the project.</param>
        /// <param name="targets">An optional list of targets to execute.</param>
        /// <param name="globalProperties">An optional <see cref="IDictionary{TKey,TValue}" /> containing global properties to use when building.</param>
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
            BuildRequestDataFlags buildRequestDataFlags)
        {
            BuildRequestData buildRequestData = new(
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

            return Build(buildRequestData);
        }

        /// <summary>
        /// Executes a build for the specified project.
        /// </summary>
        /// <param name="projectInstance">A <see cref="ProjectInstance" /> representing the project.</param>
        /// <param name="targets">An optional list of targets to execute.</param>
        /// <param name="globalProperties">An optional <see cref="IDictionary{TKey,TValue}" /> containing global properties to use when building.</param>
        /// <param name="buildRequestDataFlags">The <see cref="BuildRequestDataFlags" /> to use.</param>
        /// <returns>A <see cref="BuildResult" /> containing details about the result of the build.</returns>
        public static BuildResult Build(ProjectInstance projectInstance, string[] targets, IDictionary<string, string> globalProperties, BuildRequestDataFlags buildRequestDataFlags)
        {
            BuildRequestData buildRequestData = new(
                projectInstance,
                targets ?? Array.Empty<string>(),
                hostServices: null,
                buildRequestDataFlags);

            return Build(buildRequestData);
        }

        public static void EndBuild()
        {
            BuildManager.DefaultBuildManager.EndBuild();
        }

        private static BuildResult Build(BuildRequestData buildRequestData)
        {
            BuildSubmission buildSubmission = BuildManager.DefaultBuildManager.PendBuildRequest(buildRequestData);

            BuildResult buildResult = buildSubmission.Execute();

            if (buildResult.Exception != null)
            {
                throw buildResult.Exception;
            }

            return buildResult;
        }
    }
}
