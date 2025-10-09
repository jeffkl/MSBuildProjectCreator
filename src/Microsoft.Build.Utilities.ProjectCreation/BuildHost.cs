// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    internal static class BuildHost
    {
        private static readonly IDictionary<string, string> EmptyGlobalProperties = new Dictionary<string, string>(capacity: 0);

#if !NET8_0
        private static readonly IDictionary<string, string?> EmptyGlobalPropertiesWithNull = new Dictionary<string, string?>(capacity: 0);
#endif

        public static bool Restore(
            string projectFullPath,
            ProjectCollection projectCollection,
            IDictionary<string, string>? globalProperties,
            BuildOutput buildOutput,
            out IDictionary<string, TargetResult>? targetOutputs)
        {
            Dictionary<string, string> restoreGlobalProperties = new(globalProperties ?? projectCollection.GlobalProperties); // IMPORTANT: Make a copy of the global properties here so as not to modify the ones passed in

            restoreGlobalProperties["ExcludeRestorePackageImports"] = "true";
            restoreGlobalProperties["MSBuildRestoreSessionId"] = Guid.NewGuid().ToString("D");

            BuildRequestDataFlags buildRequestDataFlags = BuildRequestDataFlags.ClearCachesAfterBuild | BuildRequestDataFlags.SkipNonexistentTargets | BuildRequestDataFlags.IgnoreMissingEmptyAndInvalidImports;

            return BuildProjectFromFullPath(projectFullPath, ["Restore"], restoreGlobalProperties, [.. projectCollection.Loggers, buildOutput], buildRequestDataFlags, out targetOutputs);
        }

        public static bool TryBuild(
            string projectFullPath,
            ProjectCollection projectCollection,
            BuildOutput buildOutput,
            out IDictionary<string, TargetResult>? targetOutputs,
            bool restore = false,
            string? target = null,
            IDictionary<string, string>? globalProperties = null)
        {
            return Build(projectFullPath, restore, target is null ? Array.Empty<string>() : [target], projectCollection, globalProperties, buildOutput, out targetOutputs);
        }

        public static bool TryBuild(
            string projectFullPath,
            ProjectCollection projectCollection,
            BuildOutput buildOutput,
            out IDictionary<string, TargetResult>? targetOutputs,
            bool restore = false,
            string[]? targets = null,
            IDictionary<string, string>? globalProperties = null)
        {
            return Build(projectFullPath, restore, targets ?? Array.Empty<string>(), projectCollection, globalProperties, buildOutput, out targetOutputs);
        }

        public static bool TryBuild(
            ProjectInstance projectInstance,
            ProjectCollection projectCollection,
            BuildOutput buildOutput,
            out IDictionary<string, TargetResult>? targetOutputs,
            string? target = null,
            IDictionary<string, string>? globalProperties = null)
        {
            return Build(projectInstance, target is null ? Array.Empty<string>() : [target], projectCollection, globalProperties, buildOutput, out targetOutputs);
        }

        public static bool TryBuild(
            ProjectInstance projectInstance,
            ProjectCollection projectCollection,
            BuildOutput buildOutput,
            out IDictionary<string, TargetResult>? targetOutputs,
            string[]? targets = null,
            IDictionary<string, string>? globalProperties = null)
        {
            return Build(projectInstance, targets ?? Array.Empty<string>(), projectCollection, globalProperties, buildOutput, out targetOutputs);
        }

        private static bool Build(
            string projectFullPath,
            bool restore,
            string[] targets,
            ProjectCollection projectCollection,
            IDictionary<string, string>? globalProperties,
            BuildOutput buildOutput,
            out IDictionary<string, TargetResult>? targetOutputs)
        {
            targetOutputs = null;

            if (restore)
            {
                if (!Restore(projectFullPath, projectCollection, globalProperties, buildOutput, out _))
                {
                    return false;
                }
            }

            return BuildProjectFromFullPath(projectFullPath, targets, globalProperties, [.. projectCollection.Loggers, buildOutput], BuildRequestDataFlags.None, out targetOutputs);
        }

        private static bool Build(
            ProjectInstance projectInstance,
            string[] targets,
            ProjectCollection projectCollection,
            IDictionary<string, string>? globalProperties,
            BuildOutput buildOutput,
            out IDictionary<string, TargetResult>? targetOutputs)
        {
            targetOutputs = null;

            return BuildProjectFromProjectInstance(projectInstance, targets, globalProperties, [.. projectCollection.Loggers, buildOutput], BuildRequestDataFlags.None, out targetOutputs);
        }

        private static bool BuildProjectFromProjectInstance(
            ProjectInstance projectInstance,
            string[] targets,
            IDictionary<string, string>? globalProperties,
            IEnumerable<ILogger> loggers,
            BuildRequestDataFlags buildRequestDataFlags,
            out IDictionary<string, TargetResult>? targetOutputs)
        {
            targetOutputs = null;

            BuildResult buildResult = BuildManagerHost.Build(
                projectInstance,
                targets,
                globalProperties ?? EmptyGlobalProperties,
                loggers,
                buildRequestDataFlags);

            if (buildResult.Exception != null)
            {
                throw buildResult.Exception;
            }

            targetOutputs = buildResult.ResultsByTarget;

            return buildResult.OverallResult == BuildResultCode.Success;
        }

        private static bool BuildProjectFromFullPath(
            string projectFullPath,
            string[] targets,
            IDictionary<string, string>? globalProperties,
            IEnumerable<ILogger> loggers,
            BuildRequestDataFlags buildRequestDataFlags,
            out IDictionary<string, TargetResult>? targetOutputs)
        {
            targetOutputs = null;

            BuildResult buildResult = BuildManagerHost.Build(
                projectFullPath,
                targets,
                GetGlobalProperties(globalProperties),
                loggers,
                buildRequestDataFlags);

            if (buildResult.Exception != null)
            {
                throw buildResult.Exception;
            }

            if (targetOutputs != null)
            {
                foreach (KeyValuePair<string, TargetResult> targetResult in buildResult.ResultsByTarget)
                {
                    targetOutputs[targetResult.Key] = targetResult.Value;
                }
            }
            else
            {
                targetOutputs = buildResult.ResultsByTarget;
            }

            return buildResult.OverallResult == BuildResultCode.Success;

#if NET8_0
            IDictionary<string, string>
#else
            IDictionary<string, string?>
#endif
            GetGlobalProperties(IDictionary<string, string>? globalProperties)
            {
#if NET8_0
                return globalProperties ?? EmptyGlobalProperties;
#else
                if (globalProperties is null)
                {
                    return EmptyGlobalPropertiesWithNull;
                }

                Dictionary<string, string?> finalGlobalProperties = new(globalProperties.Count);

                foreach (var kvp in globalProperties)
                {
                    finalGlobalProperties[kvp.Key] = kvp.Value;
                }

                return finalGlobalProperties;
#endif
            }
        }
    }
}
