// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class SolutionCreator
    {
        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(string target, out bool result)
        {
            return TryBuild(target, globalProperties: null, out result);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(string target, IDictionary<string, string>? globalProperties, out bool result)
        {
            return TryBuild(restore: false, target, globalProperties, out result);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the Visual Studio solution should be restored before building.</param>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(bool restore, string target, out bool result)
        {
            return TryBuild(restore, target, null, out result);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the Visual Studio solution should be restored before building.</param>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(bool restore, string target, IDictionary<string, string>? globalProperties, out bool result)
        {
            if (restore)
            {
                TryRestore(out result);

                if (!result)
                {
                    return this;
                }
            }

            result = Build([target], globalProperties, out _, out _);

            return this;
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(string target, out bool result, out BuildOutput buildOutput)
        {
            return TryBuild(target, null, out result, out buildOutput);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(string target, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput)
        {
            return TryBuild(restore: false, target, globalProperties, out result, out buildOutput);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the Visual Studio solution should be restored before building.</param>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(bool restore, string target, out bool result, out BuildOutput buildOutput)
        {
            return TryBuild(restore, target, null, out result, out buildOutput);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the Visual Studio solution should be restored before building.</param>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(bool restore, string target, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput)
        {
            buildOutput = BuildOutput.Create();

            result = Build(restore, [target], globalProperties, buildOutput, out _);

            return this;
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(out bool result)
        {
            return TryBuild(globalProperties: null, out result);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(Dictionary<string, string>? globalProperties, out bool result)
        {
            return TryBuild(restore: false, globalProperties: globalProperties, out result);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the Visual Studio solution should be restored before building.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(bool restore, out bool result)
        {
            return TryBuild(restore, globalProperties: null, out result);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the Visual Studio solution should be restored before building.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(bool restore, IDictionary<string, string>? globalProperties, out bool result)
        {
            if (restore)
            {
                TryRestore(out result);

                if (!result)
                {
                    return this;
                }
            }

            result = Build(null, globalProperties: globalProperties, out _, out _);

            return this;
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(out bool result, out BuildOutput buildOutput)
        {
            return TryBuild(globalProperties: null, out result, out buildOutput);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput)
        {
            return TryBuild(restore: false, globalProperties, out result, out buildOutput);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the Visual Studio solution should be restored before building.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(bool restore, out bool result, out BuildOutput buildOutput)
        {
            return TryBuild(restore, globalProperties: null, out result, out buildOutput);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the Visual Studio solution should be restored before building.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(bool restore, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput)
        {
            buildOutput = BuildOutput.Create();

            result = Build(restore, null, globalProperties, buildOutput, out _);

            return this;
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(string target, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(target, null, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(string target, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(restore: false, target, globalProperties, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the Visual Studio solution should be restored before building.</param>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(bool restore, string target, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(restore, target, null, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the Visual Studio solution should be restored before building.</param>
        /// <param name="target">The name of the target to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(bool restore, string target, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(restore, new[] { target }, globalProperties, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="targets">The names of the targets to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(string[] targets, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(targets, null, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="targets">The names of the targets to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(string[] targets, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(restore: false, targets, globalProperties, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the Visual Studio solution should be restored before building.</param>
        /// <param name="targets">The names of the targets to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(bool restore, string[] targets, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(restore, targets, null, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the Visual Studio solution should be restored before building.</param>
        /// <param name="targets">The names of the targets to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(bool restore, string[] targets, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            buildOutput = BuildOutput.Create();

            result = Build(restore, targets, globalProperties, buildOutput, out targetOutputs);

            return this;
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="targets">The names of the targets to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(IEnumerable<string> targets, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(targets, null, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="targets">The names of the targets to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(IEnumerable<string> targets, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(restore: false, targets, globalProperties, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the Visual Studio solution should be restored before building.</param>
        /// <param name="targets">The names of the targets to build.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(bool restore, IEnumerable<string> targets, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(restore, targets, null, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to build it.
        /// </summary>
        /// <param name="restore">A value indicating whether or not the Visual Studio solution should be restored before building.</param>
        /// <param name="targets">The names of the targets to build.</param>
        /// <param name="globalProperties">Global properties to use when building the target.</param>
        /// <param name="result">A value indicating the result of the build.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the build.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryBuild(bool restore, IEnumerable<string> targets, IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryBuild(restore, targets.ToArray(), globalProperties, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to run the Restore target with a unique evaluation context.
        /// </summary>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryRestore(out bool result)
        {
            return TryRestore(null, out result);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to run the Restore target with a unique evaluation context.
        /// </summary>
        /// <param name="globalProperties">Global properties to use when running the Restore the target.</param>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryRestore(IDictionary<string, string>? globalProperties, out bool result)
        {
            return TryRestore(globalProperties, out result, out BuildOutput _, out IDictionary<string, TargetResult>? _);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to run the Restore target with a unique evaluation context.
        /// </summary>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the restore.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryRestore(out bool result, out BuildOutput buildOutput)
        {
            return TryRestore(null, out result, out buildOutput);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to run the Restore target with a unique evaluation context.
        /// </summary>
        /// <param name="globalProperties">Global properties to use when running the Restore the target.</param>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the restore.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryRestore(IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput)
        {
            return TryRestore(globalProperties, out result, out buildOutput, out IDictionary<string, TargetResult>? _);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to run the Restore target with a unique evaluation context.
        /// </summary>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the restore.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryRestore(out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            return TryRestore(null, out result, out buildOutput, out targetOutputs);
        }

        /// <summary>
        /// Saves the current Visual Studio solution and attempts to run the Restore target with a unique evaluation context.
        /// </summary>
        /// <param name="globalProperties">Global properties to use when running the Restore the target.</param>
        /// <param name="result">A value indicating the result of the restore.</param>
        /// <param name="buildOutput">A <see cref="BuildOutput" /> object that captured the logging from the restore.</param>
        /// <param name="targetOutputs">A <see cref="IDictionary{String,TargetResult}" /> containing the target outputs.</param>
        /// <returns>The current <see cref="SolutionCreator" />.</returns>
        public SolutionCreator TryRestore(IDictionary<string, string>? globalProperties, out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            buildOutput = BuildOutput.Create();

            result = Restore(globalProperties, buildOutput, out targetOutputs);

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

            if (restore)
            {
                if (!Restore(globalProperties, buildOutput, out targetOutputs))
                {
                    return false;
                }
            }
            else
            {
                Save();
            }

            bool result = BuildHost.TryBuild(FullPath, ProjectCollection, buildOutput, out targetOutputs, targets: targets);

            return result;
        }

        private bool Restore(IDictionary<string, string>? globalProperties, BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs)
        {
            Save();

            return BuildHost.Restore(FullPath, ProjectCollection, globalProperties ?? _globalProperties, buildOutput, out targetOutputs);
        }
    }
}
