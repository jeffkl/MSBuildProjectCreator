// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Stores and makes available the logged output of MSBuild when building a project.
    /// </summary>
    public sealed class BuildOutput : ILogger, IDisposable
    {
        /// <summary>
        /// Stores the errors that were logged.
        /// </summary>
        private readonly List<BuildErrorEventArgs> _errorEvents = new List<BuildErrorEventArgs>();

        /// <summary>
        /// Stores the messages that were logged.
        /// </summary>
        private readonly List<BuildMessageEventArgs> _messageEvents = new List<BuildMessageEventArgs>(50);

        /// <summary>
        /// Stores the results by project.
        /// </summary>
        private readonly ConcurrentDictionary<string, bool> _resultsByProject = new ConcurrentDictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Stores the warnings that were logged.
        /// </summary>
        private readonly List<BuildWarningEventArgs> _warningEvents = new List<BuildWarningEventArgs>();

        /// <summary>
        /// Stores all build events.
        /// </summary>
        private ConcurrentQueue<BuildEventArgs> _allEvents = new ConcurrentQueue<BuildEventArgs>();

        /// <summary>
        /// Stores the <see cref="BuildFinishedEventArgs"/> that were logged when the build finished.
        /// </summary>
        private BuildFinishedEventArgs _buildFinished;

        private BuildOutput()
        {
            MessageEvents = new BuildOutputMessageEvents(_messageEvents);
            Messages = new BuildOutputMessages(this);
        }

        /// <summary>
        /// Gets all events that were logged.
        /// </summary>
        public IReadOnlyCollection<BuildEventArgs> AllEvents => _allEvents;

        /// <summary>
        /// Gets the error events that were logged.
        /// </summary>
        public IReadOnlyCollection<BuildErrorEventArgs> ErrorEvents => _errorEvents;

        /// <summary>
        /// Gets the error messages that were logged.
        /// </summary>
        public IReadOnlyCollection<string> Errors => _errorEvents.Select(i => i.Message).ToList();

        /// <summary>
        /// Gets the messages that were logged.
        /// </summary>
        public BuildOutputMessageEvents MessageEvents { get; }

        /// <summary>
        /// Gets a <see cref="BuildOutputMessages"/> object that gets the messages from the build.
        /// </summary>
        public BuildOutputMessages Messages { get; }

        /// <inheritdoc cref="ILogger.Parameters"/>
        public string Parameters { get; set; }

        /// <summary>
        /// Gets the results by project path.
        /// </summary>
        public IReadOnlyDictionary<string, bool> ProjectResults => _resultsByProject;

        /// <summary>
        /// Gets a value indicating if the build succeeded.
        /// </summary>
        public bool? Succeeded => _buildFinished?.Succeeded;

        /// <inheritdoc cref="ILogger.Verbosity"/>
        public LoggerVerbosity Verbosity { get; set; }

        /// <summary>
        /// Gets the warning events that were logged.
        /// </summary>
        public IReadOnlyCollection<BuildWarningEventArgs> WarningEvents => _warningEvents;

        /// <summary>
        /// Gets the warning messages that were logged.
        /// </summary>
        public IReadOnlyCollection<string> Warnings => _warningEvents.Select(i => i.Message).ToList();

        /// <summary>
        /// Creates an instance of the <see cref="BuildOutput"/> class.
        /// </summary>
        /// <returns>A <see cref="BuildOutput"/> instance.</returns>
        public static BuildOutput Create()
        {
            return new BuildOutput();
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            _buildFinished = null;
            _errorEvents.Clear();
            _messageEvents.Clear();
            _warningEvents.Clear();
            _allEvents = null;
        }

        /// <summary>
        /// Gets the current build output in the format of a console log.
        /// </summary>
        /// <param name="verbosity">The logger verbosity to use.</param>
        /// <returns>The build output in the format of a console log.</returns>
        public string GetConsoleLog(LoggerVerbosity verbosity = LoggerVerbosity.Normal)
        {
            StringBuilder sb = new StringBuilder(_allEvents.Count * 300);

            ConsoleLogger logger = new ConsoleLogger(verbosity, message => sb.Append(message), color => { }, () => { });

            foreach (BuildEventArgs buildEventArgs in _allEvents)
            {
                switch (buildEventArgs)
                {
                    case BuildMessageEventArgs buildMessageEventArgs:
                        logger.MessageHandler(logger, buildMessageEventArgs);
                        break;

                    case BuildErrorEventArgs buildErrorEventArgs:
                        logger.ErrorHandler(logger, buildErrorEventArgs);
                        break;

                    case BuildWarningEventArgs buildWarningEventArgs:
                        logger.WarningHandler(logger, buildWarningEventArgs);
                        break;

                    case BuildStartedEventArgs buildStartedEventArgs:
                        logger.BuildStartedHandler(logger, buildStartedEventArgs);
                        break;

                    case BuildFinishedEventArgs buildFinishedEventArgs:
                        logger.BuildFinishedHandler(logger, buildFinishedEventArgs);
                        break;

                    case ProjectStartedEventArgs projectStartedEventArgs:
                        logger.ProjectStartedHandler(logger, projectStartedEventArgs);
                        break;

                    case ProjectFinishedEventArgs projectFinishedEventArgs:
                        logger.ProjectFinishedHandler(logger, projectFinishedEventArgs);
                        break;

                    case TargetStartedEventArgs targetStartedEventArgs:
                        logger.TargetStartedHandler(logger, targetStartedEventArgs);
                        break;

                    case TargetFinishedEventArgs targetFinishedEventArgs:
                        logger.TargetFinishedHandler(logger, targetFinishedEventArgs);
                        break;

                    case TaskStartedEventArgs taskStartedEventArgs:
                        logger.TaskStartedHandler(logger, taskStartedEventArgs);
                        break;

                    case TaskFinishedEventArgs taskFinishedEventArgs:
                        logger.TaskFinishedHandler(logger, taskFinishedEventArgs);
                        break;

                    case CustomBuildEventArgs customBuildEventArgs:
                        logger.CustomEventHandler(logger, customBuildEventArgs);
                        break;
                }
            }

            return sb.ToString();
        }

        /// <inheritdoc cref="ILogger.Initialize"/>
        public void Initialize(IEventSource eventSource)
        {
            eventSource.BuildFinished += OnBuildFinished;
            eventSource.ErrorRaised += OnErrorRaised;
            eventSource.MessageRaised += OnMessageRaised;
            eventSource.ProjectFinished += OnProjectFinished;
            eventSource.WarningRaised += OnWarningRaised;
            eventSource.AnyEventRaised += OnAnyEventRaised;
        }

        /// <inheritdoc cref="ILogger.Shutdown"/>
        public void Shutdown()
        {
        }

        private void OnAnyEventRaised(object sender, BuildEventArgs e)
        {
            _allEvents.Enqueue(e);
        }

        private void OnBuildFinished(object sender, BuildFinishedEventArgs args) => _buildFinished = args;

        private void OnErrorRaised(object sender, BuildErrorEventArgs args) => _errorEvents.Add(args);

        private void OnMessageRaised(object sender, BuildMessageEventArgs args) => _messageEvents.Add(args);

        private void OnProjectFinished(object sender, ProjectFinishedEventArgs e) => _resultsByProject.AddOrUpdate(e.ProjectFile, e.Succeeded, (projectFile, succeeded) => succeeded && e.Succeeded);

        private void OnWarningRaised(object sender, BuildWarningEventArgs args) => _warningEvents.Add(args);
    }
}