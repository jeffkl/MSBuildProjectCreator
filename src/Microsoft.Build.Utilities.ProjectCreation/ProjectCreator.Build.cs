// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Execution;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class ProjectCreator
    {
        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(string target, out bool result)
        {
            return TryBuild(target, null, out result);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(string target, IDictionary<string, string>? globalProperties, out bool result)
        {
            return TryBuild(restore: false, target, globalProperties, out result);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the project should be restored before building.</param>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(bool restore, string target, out bool result)
        {
            return TryBuild(restore, target, null, out result);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the project should be restored before building.</param>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(bool restore, string target, IDictionary<string, string>? globalProperties, out bool result)
        {
            if (restore)
            {
                TryRestore(out result);

                if (!result)
                {
                    return this;
                }
            }

            Build(target.ToArrayWithSingleElement(), globalProperties, out result, out _, out _);

            return this;
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(string target, out bool result, out BuildOutput buildOutput)
        {
            return TryBuild(target, null, out result, out buildOutput);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(string target, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput)
        {
            return TryBuild(restore: false, target, globalProperties, out result, out buildOutput);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the project should be restored before building.</param>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(bool restore, string target, out bool result, out BuildOutput buildOutput)
        {
            return TryBuild(restore, target, null, out result, out buildOutput);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the project should be restored before building.</param>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(bool restore, string target, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput)
        {
            buildOutput = BuildOutput.Create();

            Build(restore, target.ToArrayWithSingleElement(), globalProperties, buildOutput, out result, out _);

            return this;
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(out bool result)
        {
            return TryBuild(globalProperties: null, out result);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(Dictionary<string, string>? globalProperties, out bool result)
        {
            return TryBuild(restore: false, globalProperties: globalProperties, out result);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the project should be restored before building.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(bool restore, out bool result)
        {
            return TryBuild(restore, globalProperties: null, out result);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the project should be restored before building.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(bool restore, IDictionary<string, string>? globalProperties, out bool result)
        {
            if (restore)
            {
                TryRestore(out result);

                if (!result)
                {
                    return this;
                }
            }

            Build(null, globalProperties: globalProperties, out result, out _, out _);

            return this;
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(out bool result, out BuildOutput buildOutput)
        {
            return TryBuild(globalProperties: null, out result, out buildOutput);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput)
        {
            return TryBuild(restore: false, globalProperties, out result, out buildOutput);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the project should be restored before building.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(bool restore, out bool result, out BuildOutput buildOutput)
        {
            return TryBuild(restore, globalProperties: null, out result, out buildOutput);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the project should be restored before building.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(bool restore, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput)
        {
            buildOutput = BuildOutput.Create();

            Build(restore, null, globalProperties, buildOutput, out result, out _);

            return this;
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(string target, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(target, null, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(string target, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(restore: false, target, globalProperties, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the project should be restored before building.</param>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(bool restore, string target, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(restore, target, null, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the project should be restored before building.</param>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(bool restore, string target, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(restore, new[] { target }, globalProperties, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="targets">The names of the targets to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(string[] targets, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(targets, null, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="targets">The names of the targets to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(string[] targets, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(restore: false, targets, globalProperties, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the project should be restored before building.</param>
        /// <param name="targets">The names of the targets to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(bool restore, string[] targets, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(restore, targets, null, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the project should be restored before building.</param>
        /// <param name="targets">The names of the targets to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(bool restore, string[] targets, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            buildOutput = BuildOutput.Create();

            Build(restore, targets, globalProperties, buildOutput, out result, out targetOutputs);

            return this;
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="targets">The names of the targets to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(IEnumerable<string> targets, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(targets, null, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="targets">The names of the targets to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(IEnumerable<string> targets, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(restore: false, targets, globalProperties, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the project should be restored before building.</param>
        /// <param name="targets">The names of the targets to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(bool restore, IEnumerable<string> targets, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(restore, targets, null, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the project should be restored before building.</param>
        /// <param name="targets">The names of the targets to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(bool restore, IEnumerable<string> targets, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(restore, targets.ToArray(), globalProperties, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Attempts to run the Restore target for the current project with a unique evaluation context.
        /// </summary>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryRestore(out bool result)
        {
            return TryRestore(null, out result);
        }

        /// <summary>
        /// Attempts to run the Restore target for the current project with a unique evaluation context.
        /// </summary>
        /// <param name="globalProperties">Global properties to use when running the Restore the target.</param>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryRestore(IDictionary<string, string>? globalProperties, out bool result)
        {
            return TryRestore(globalProperties, out result, out BuildOutput _, out IDictionary<string, TargetResult> _);
        }

        /// <summary>
        /// Attempts to run the Restore target for the current project with a unique evaluation context.
        /// </summary>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the restore.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryRestore(out bool result, out BuildOutput buildOutput)
        {
            return TryRestore(null, out result, out buildOutput);
        }

        /// <summary>
        /// Attempts to run the Restore target for the current project with a unique evaluation context.
        /// </summary>
        /// <param name="globalProperties">Global properties to use when running the Restore the target.</param>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the restore.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryRestore(IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput)
        {
            return TryRestore(globalProperties, out result, out buildOutput, out IDictionary<string, TargetResult> _);
        }

        /// <summary>
        /// Attempts to run the Restore target for the current project with a unique evaluation context.
        /// </summary>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the restore.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryRestore(out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryRestore(null, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Attempts to run the Restore target for the current project with a unique evaluation context.
        /// </summary>
        /// <param name="globalProperties">Global properties to use when running the Restore the target.</param>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the restore.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryRestore(IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            buildOutput = BuildOutput.Create();

            Restore(globalProperties, buildOutput, out result, out targetOutputs);

            return this;
        }

        private void Build(string[]? targets, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            buildOutput = BuildOutput.Create();

            Build(restore: false, targets, globalProperties, buildOutput, out result, out targetOutputs);
        }

        private void Build(bool restore, string[]? targets, IDictionary<string, string>? globalProperties, BuildOutput buildOutput, out bool result, out IDictionary<string, TargetResult>? targetOutputs)
        {
            targetOutputs = null;

            Save();

            if (restore)
            {
                Restore(globalProperties, buildOutput, out result, out targetOutputs);

                if (!result)
                {
                    return;
                }
            }

            BuildResult buildResult = BuildManagerHost.Build(
                FullPath,
                targets!,
                globalProperties!,
                new List<Framework.ILogger>(ProjectCollection.Loggers.Concat(buildOutput.AsEnumerable())),
                BuildRequestDataFlags.None);

            result = buildResult.OverallResult == BuildResultCode.Success;

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
        }

        private void Restore(IDictionary<string, string>? globalProperties, BuildOutput buildOutput, out bool result, out IDictionary<string, TargetResult> targetOutputs)
        {
            Save();

            // IMPORTANT: Make a copy of the global properties here so as not to modify the ones passed in
            Dictionary<string, string> restoreGlobalProperties = new Dictionary<string, string>(globalProperties ?? ProjectCollection.GlobalProperties);

            restoreGlobalProperties["ExcludeRestorePackageImports"] = "true";
            restoreGlobalProperties["MSBuildRestoreSessionId"] = Guid.NewGuid().ToString("D");

            BuildResult buildResult = BuildManagerHost.Build(
                FullPath,
                new[] { "Restore" },
                restoreGlobalProperties,
                new List<Framework.ILogger>(ProjectCollection.Loggers.Concat(buildOutput.AsEnumerable())),
                BuildRequestDataFlags.ClearCachesAfterBuild | BuildRequestDataFlags.SkipNonexistentTargets | BuildRequestDataFlags.IgnoreMissingEmptyAndInvalidImports);

            targetOutputs = buildResult.ResultsByTarget;

            if (buildResult.Exception != null)
            {
                throw buildResult.Exception;
            }

            result = buildResult.OverallResult == BuildResultCode.Success;
        }
    }
}