// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Framework;
using System;
using System.Collections;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Represents an implementation of <see cref="IBuildEngine" /> that allows for capturing logged events in tasks.
    /// </summary>
    public sealed class BuildEngine : BuildEventArgsCollection, IBuildEngine
    {
        private BuildEngine()
        {
        }

        /// <inheritdoc cref="IBuildEngine.ColumnNumberOfTaskNode" />
        public int ColumnNumberOfTaskNode => 0;

        /// <inheritdoc cref="IBuildEngine.ContinueOnError" />
        public bool ContinueOnError => false;

        /// <inheritdoc cref="IBuildEngine.LineNumberOfTaskNode" />
        public int LineNumberOfTaskNode => 0;

        /// <inheritdoc cref="IBuildEngine.ProjectFileOfTaskNode" />
        public string? ProjectFileOfTaskNode => null;

        /// <summary>
        /// Creates an instance of the <see cref="BuildEngine" /> class.
        /// </summary>
        /// <returns>A <see cref="BuildEngine" /> instance.</returns>
        public static BuildEngine Create()
        {
            return new BuildEngine();
        }

        /// <inheritdoc cref="IBuildEngine.BuildProjectFile" />
        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="IBuildEngine.LogCustomEvent" />
        public void LogCustomEvent(CustomBuildEventArgs e) => Add(e);

        /// <inheritdoc cref="IBuildEngine.LogErrorEvent" />
        public void LogErrorEvent(BuildErrorEventArgs e) => Add(e);

        /// <inheritdoc cref="IBuildEngine.LogMessageEvent" />
        public void LogMessageEvent(BuildMessageEventArgs e) => Add(e);

        /// <inheritdoc cref="IBuildEngine.LogWarningEvent" />
        public void LogWarningEvent(BuildWarningEventArgs e) => Add(e);
    }
}