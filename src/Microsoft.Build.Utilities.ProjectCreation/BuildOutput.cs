// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Stores and makes available the logged output of MSBuild when building a project.
    /// </summary>
    public sealed class BuildOutput : BuildEventArgsCollection, ILogger
    {
        /// <summary>
        /// Stores the results by project.
        /// </summary>
        private readonly ConcurrentDictionary<string, bool> _resultsByProject = new ConcurrentDictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Stores the <see cref="BuildFinishedEventArgs" /> that were logged when the build finished.
        /// </summary>
        private BuildFinishedEventArgs? _buildFinished;

        private BuildOutput()
        {
            Parameters = string.Empty;
        }

        /// <summary>
        /// Gets a value indicating whether or not the build output is shut down.
        /// </summary>
        public bool IsShutdown { get; private set; }

        /// <inheritdoc cref="ILogger.Parameters" />
        public string? Parameters { get; set; }

        /// <summary>
        /// Gets the results by project path.
        /// </summary>
        public IReadOnlyDictionary<string, bool> ProjectResults => _resultsByProject;

        /// <summary>
        /// Gets a value indicating if the build succeeded.
        /// </summary>
        public bool? Succeeded => _buildFinished?.Succeeded;

        /// <inheritdoc cref="ILogger.Verbosity" />
        public LoggerVerbosity Verbosity { get; set; }

        /// <summary>
        /// Creates an instance of the <see cref="BuildOutput" /> class.
        /// </summary>
        /// <returns>A <see cref="BuildOutput" /> instance.</returns>
        public static BuildOutput Create()
        {
            return new BuildOutput();
        }

        /// <inheritdoc cref="IDisposable.Dispose" />
        public override void Dispose()
        {
            _buildFinished = null;

            base.Dispose();
        }

        /// <inheritdoc cref="ILogger.Initialize" />
        public void Initialize(IEventSource eventSource)
        {
            eventSource.BuildFinished += OnBuildFinished;
            eventSource.ProjectFinished += OnProjectFinished;
            eventSource.AnyEventRaised += OnAnyEventRaised;
        }

        /// <inheritdoc cref="ILogger.Shutdown" />
        public void Shutdown()
        {
            IsShutdown = true;
        }

        private void OnAnyEventRaised(object sender, BuildEventArgs e)
        {
            Add(e);
        }

        private void OnBuildFinished(object sender, BuildFinishedEventArgs args) => _buildFinished = args;

        private void OnProjectFinished(object sender, ProjectFinishedEventArgs e)
        {
            if (e.ProjectFile != null)
            {
                _resultsByProject.AddOrUpdate(e.ProjectFile, e.Succeeded, (projectFile, succeeded) => succeeded && e.Succeeded);
            }
        }
    }
}