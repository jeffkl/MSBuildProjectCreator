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
            return TryBuild(restore: false, target, out result);
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
            if (restore)
            {
                TryRestore(out result);

                if (!result)
                {
                    return this;
                }
            }

            lock (BuildManager.DefaultBuildManager)
            {
                result = Project.Build(target);
            }

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
            return TryBuild(restore: false, target, out result, out buildOutput);
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
            if (restore)
            {
                TryRestore(out result, out buildOutput);

                if (!result)
                {
                    return this;
                }
            }
            else
            {
                buildOutput = BuildOutput.Create();
            }

            lock (BuildManager.DefaultBuildManager)
            {
                result = Project.Build(target, buildOutput.AsEnumerable());
            }

            return this;
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(out bool result)
        {
            return TryBuild(restore: false, out result);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the project should be restored before building.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(bool restore, out bool result)
        {
            if (restore)
            {
                TryRestore(out result);

                if (!result)
                {
                    return this;
                }
            }

            lock (BuildManager.DefaultBuildManager)
            {
                result = Project.Build();
            }

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
            return TryBuild(restore: false, out result, out buildOutput);
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
            if (restore)
            {
                TryRestore(out result, out buildOutput);

                if (!result)
                {
                    return this;
                }
            }
            else
            {
                buildOutput = BuildOutput.Create();
            }

            lock (BuildManager.DefaultBuildManager)
            {
                result = Project.Build(buildOutput.AsEnumerable());
            }

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
        public ProjectCreator TryBuild(string target, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult> targetOutputs)
        {
            return TryBuild(restore: false, target, out result, out buildOutput, out targetOutputs);
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
        public ProjectCreator TryBuild(bool restore, string target, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult> targetOutputs)
        {
            return TryBuild(restore, new[] { target }, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="targets">The names of the targets to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryBuild(string[] targets, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult> targetOutputs)
        {
            return TryBuild(restore: false, targets, out result, out buildOutput, out targetOutputs);
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
        public ProjectCreator TryBuild(bool restore, string[] targets, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult> targetOutputs)
        {
            if (restore)
            {
                TryRestore(out result, out buildOutput, out targetOutputs);

                if (!result)
                {
                    return this;
                }
            }
            else
            {
                buildOutput = BuildOutput.Create();
            }

            lock (BuildManager.DefaultBuildManager)
            {
                ProjectInstance projectInstance = Project.CreateProjectInstance();

                result = projectInstance.Build(targets, buildOutput.AsEnumerable(), out targetOutputs);
            }

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
        public ProjectCreator TryBuild(IEnumerable<string> targets, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult> targetOutputs)
        {
            return TryBuild(restore: false, targets, out result, out buildOutput, out targetOutputs);
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
        public ProjectCreator TryBuild(bool restore, IEnumerable<string> targets, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult> targetOutputs)
        {
            return TryBuild(restore, targets.ToArray(), out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Attempts to run the Restore target for the current project with a unique evaluation context.
        /// </summary>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryRestore(out bool result)
        {
            return TryRestore(out result, out BuildOutput _, out IDictionary<string, TargetResult> _);
        }

        /// <summary>
        /// Attempts to run the Restore target for the current project with a unique evaluation context.
        /// </summary>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the restore.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryRestore(out bool result, out BuildOutput buildOutput)
        {
            return TryRestore(out result, out buildOutput, out IDictionary<string, TargetResult> _);
        }

        /// <summary>
        /// Attempts to run the Restore target for the current project with a unique evaluation context.
        /// </summary>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput"/> object that captured the logging from the restore.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryRestore(out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult> targetOutputs)
        {
            Save();

            buildOutput = BuildOutput.Create();

            lock (BuildManager.DefaultBuildManager)
            {
                BuildRequestData restoreRequest = new BuildRequestData(
                    FullPath,
                    new Dictionary<string, string>
                    {
                        ["ExcludeRestorePackageImports"] = "true",
                        ["MSBuildRestoreSessionId"] = Guid.NewGuid().ToString("D"),
                    },
                    ProjectCollection.DefaultToolsVersion,
                    targetsToBuild: new[] { "Restore" },
                    hostServices: null,
                    flags: BuildRequestDataFlags.ClearCachesAfterBuild | BuildRequestDataFlags.SkipNonexistentTargets | BuildRequestDataFlags.IgnoreMissingEmptyAndInvalidImports);

                BuildParameters buildParameters = new BuildParameters
                {
                    Loggers = new List<Framework.ILogger>
                    {
                        buildOutput,
                    },
                };

                BuildManager.DefaultBuildManager.BeginBuild(buildParameters);
                try
                {
                    BuildSubmission buildSubmission = BuildManager.DefaultBuildManager.PendBuildRequest(restoreRequest);

                    BuildResult buildResult = buildSubmission.Execute();

                    result = buildResult.OverallResult == BuildResultCode.Success;

                    targetOutputs = buildResult.ResultsByTarget;
                }
                finally
                {
                    BuildManager.DefaultBuildManager.EndBuild();
                }
            }

            Project.MarkDirty();

            Project.ReevaluateIfNecessary();

            return this;
        }
    }
}