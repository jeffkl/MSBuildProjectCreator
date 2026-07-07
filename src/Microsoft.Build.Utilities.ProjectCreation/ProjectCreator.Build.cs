// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
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
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryBuild(string target, out bool result)
        {
            return TryBuild(target, globalProperties: null, out result);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
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
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryBuild(bool restore, string target, out bool result)
        {
            return TryBuild(restore, target, globalProperties: null, out result);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the project should be restored before building.</param>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryBuild(bool restore, string target, IDictionary<string, string>? globalProperties, out bool result)
        {
            BuildOutput buildOutput = BuildOutput.Create();

            result = Build(restore, [target], globalProperties, buildOutput, out _);

            return this;
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryBuild(string target, out bool result, out BuildOutput buildOutput)
        {
            return TryBuild(target, globalProperties: null, out result, out buildOutput);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
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
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryBuild(bool restore, string target, out bool result, out BuildOutput buildOutput)
        {
            return TryBuild(restore, target, globalProperties: null, out result, out buildOutput);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the project should be restored before building.</param>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryBuild(bool restore, string target, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput)
        {
            buildOutput = BuildOutput.Create();

            result = Build(restore, [target], globalProperties, buildOutput, out _);

            return this;
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryBuild(out bool result)
        {
            return TryBuild(globalProperties: null, out result);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryBuild(IDictionary<string, string>? globalProperties, out bool result)
        {
            return TryBuild(restore: false, globalProperties: globalProperties, out result);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the project should be restored before building.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
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
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryBuild(bool restore, IDictionary<string, string>? globalProperties, out bool result)
        {
            BuildOutput buildOutput = BuildOutput.Create();

            result = Build(restore, targets: null, globalProperties, buildOutput, out _);

            return this;
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryBuild(out bool result, out BuildOutput buildOutput)
        {
            return TryBuild(globalProperties: null, out result, out buildOutput);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryBuild(IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput)
        {
            return TryBuild(restore: false, globalProperties, out result, out buildOutput);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the project should be restored before building.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
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
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryBuild(bool restore, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput)
        {
            buildOutput = BuildOutput.Create();

            result = Build(restore, null, globalProperties, buildOutput, out _);

            return this;
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
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
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
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
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
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
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryBuild(bool restore, string target, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(restore, new[] { target }, globalProperties, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="targets">The names of the targets to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
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
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
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
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
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
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryBuild(bool restore, string[] targets, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            buildOutput = BuildOutput.Create();

            result = Build(restore, targets, globalProperties, buildOutput, out targetOutputs);

            return this;
        }

        /// <summary>
        /// Attempts to build the current project.
        /// </summary>
        /// <param name="targets">The names of the targets to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
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
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
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
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
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
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryBuild(bool restore, IEnumerable<string> targets, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(restore, targets.ToArray(), globalProperties, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Attempts to run the Restore target for the current project with a unique evaluation context.
        /// </summary>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryRestore(out bool result)
        {
            return TryRestore(null, out result);
        }

        /// <summary>
        /// Attempts to run the Restore target for the current project with a unique evaluation context.
        /// </summary>
        /// <param name="globalProperties">Global properties to use when running the Restore the target.</param>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryRestore(IDictionary<string, string>? globalProperties, out bool result)
        {
            return TryRestore(globalProperties, out result, out BuildOutput _, out IDictionary<string, TargetResult>? _);
        }

        /// <summary>
        /// Attempts to run the Restore target for the current project with a unique evaluation context.
        /// </summary>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the restore.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryRestore(out bool result, out BuildOutput buildOutput)
        {
            return TryRestore(null, out result, out buildOutput);
        }

        /// <summary>
        /// Attempts to run the Restore target for the current project with a unique evaluation context.
        /// </summary>
        /// <param name="globalProperties">Global properties to use when running the Restore the target.</param>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the restore.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryRestore(IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput)
        {
            return TryRestore(globalProperties, out result, out buildOutput, out IDictionary<string, TargetResult>? _);
        }

        /// <summary>
        /// Attempts to run the Restore target for the current project with a unique evaluation context.
        /// </summary>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the restore.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryRestore(out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryRestore(null, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Attempts to run the Restore target for the current project with a unique evaluation context.
        /// </summary>
        /// <param name="globalProperties">Global properties to use when running the Restore the target.</param>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the restore.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TryRestore(IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            buildOutput = BuildOutput.Create();

            IEnumerable<ILogger> loggers = [.. ProjectCollection.Loggers, buildOutput];

            lock (BuildManagerHost.LockObject)
            {
                BuildManagerHost.BeginBuild(loggers);

                try
                {
                    result = Restore(globalProperties, buildOutput, out targetOutputs);
                }
                finally
                {
                    BuildManagerHost.EndBuild();
                }
            }

            return this;
        }

        private bool Build(string[]? targets, IDictionary<string, string>? globalProperties, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            buildOutput = BuildOutput.Create();

            return Build(restore: false, targets, globalProperties, buildOutput, out targetOutputs);
        }

        private bool Build(bool restore, string[]? targets, IDictionary<string, string>? globalProperties, BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            targetOutputs = null;

            lock (BuildManagerHost.LockObject)
            {
                BuildManagerHost.BeginBuild([.. ProjectCollection.Loggers, buildOutput]);

                try
                {
                    if (restore)
                    {
                        if (!Restore(globalProperties, buildOutput, out targetOutputs))
                        {
                            return false;
                        }

                        bool buildResult = BuildHost.TryBuild(FullPath, ProjectCollection, buildOutput, out targetOutputs, targets: targets, globalProperties: globalProperties);

                        ResetProjectInstance();

                        return buildResult;
                    }

                    ProjectInstance projectInstance;

                    if (globalProperties != null)
                    {
                        TryGetProject(out Project project, globalProperties);

                        projectInstance = project.CreateProjectInstance();
                    }
                    else
                    {
                        projectInstance = ProjectInstance;
                    }

                    bool result = BuildHost.TryBuild(projectInstance, ProjectCollection, buildOutput, out targetOutputs, targets: targets);

                    ResetProjectInstance();

                    return result;
                }
                finally
                {
                    BuildManagerHost.EndBuild();
                }
            }
        }

        private bool Restore(IDictionary<string, string>? globalProperties, BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            Save();

            return BuildHost.Restore(FullPath, ProjectCollection, globalProperties ?? _globalProperties, buildOutput, out targetOutputs);
        }
    }
}
