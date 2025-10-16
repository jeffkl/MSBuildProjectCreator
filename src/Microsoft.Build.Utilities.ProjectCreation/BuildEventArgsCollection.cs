// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
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
    /// Represents a collection of <see cref="BuildEventArgs" /> objects.
    /// </summary>
    public abstract class BuildEventArgsCollection : IDisposable
    {
        /// <summary>
        /// Stores all build events.
        /// </summary>
        private readonly ConcurrentQueue<BuildEventArgs> _allEvents = new ConcurrentQueue<BuildEventArgs>();

        /// <summary>
        /// Stores the errors that were logged.
        /// </summary>
        private readonly List<BuildErrorEventArgs> _errorEvents = new List<BuildErrorEventArgs>();

        /// <summary>
        /// Stores the messages that were logged.
        /// </summary>
        private readonly List<BuildMessageEventArgs> _messageEvents = new List<BuildMessageEventArgs>(50);

        /// <summary>
        /// Stores the warnings that were logged.
        /// </summary>
        private readonly List<BuildWarningEventArgs> _warningEvents = new List<BuildWarningEventArgs>();

        /// <summary>
        /// Stores the last console output.
        /// </summary>
        private string? _lastConsoleOutput = null;

        /// <summary>
        ///  Stores the <see cref="LoggerVerbosity" /> of the last console output.
        /// </summary>
        private LoggerVerbosity _lastVerbosity = LoggerVerbosity.Normal;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildEventArgsCollection" /> class.
        /// </summary>
        protected BuildEventArgsCollection()
        {
            MessageEvents = new BuildMessageEventArgsCollection(_messageEvents);
            Messages = new BuildMessageCollection(this);
        }

        /// <summary>
        /// Occurs when console output is created and the cache is not used.
        /// </summary>
        internal event EventHandler<LoggerVerbosity>? ConsoleOutputCreated;

        /// <summary>
        /// Gets all events that were logged.
        /// </summary>
        public IReadOnlyCollection<BuildEventArgs> AllEvents => _allEvents!;

        /// <summary>
        /// Gets the error events that were logged.
        /// </summary>
        public IReadOnlyCollection<BuildErrorEventArgs> ErrorEvents => _errorEvents;

        /// <summary>
        /// Gets the error messages that were logged.
        /// </summary>
        public IReadOnlyCollection<string> Errors => _errorEvents.Select(i => i.Message).ToList()!;

        /// <summary>
        /// Gets the messages that were logged.
        /// </summary>
        public BuildMessageEventArgsCollection MessageEvents { get; }

        /// <summary>
        /// Gets a <see cref="BuildMessageCollection" /> object that gets the messages from the build.
        /// </summary>
        public BuildMessageCollection Messages { get; }

        /// <summary>
        /// Gets the warning events that were logged.
        /// </summary>
        public IReadOnlyCollection<BuildWarningEventArgs> WarningEvents => _warningEvents;

        /// <summary>
        /// Gets the warning messages that were logged.
        /// </summary>
        public IReadOnlyCollection<string> Warnings => _warningEvents.Select(i => i.Message).ToList()!;

        /// <inheritdoc cref="IDisposable.Dispose" />
        public virtual void Dispose()
        {
            _errorEvents.Clear();
            _messageEvents.Clear();
            _warningEvents.Clear();
        }

        /// <summary>
        /// Gets the current build output in the format of a console log.
        /// </summary>
        /// <param name="verbosity">The logger verbosity to use.</param>
        /// <param name="showSummary">Optional parameter indicating whether or not to show an error and warning summary at the end.</param>
        /// <param name="performanceSummary">Optional parameter indicating whether or not to show time spent in tasks, targets and projects.</param>
        /// <param name="errorsOnly">Optional parameter indicating whether or not to show only errors.</param>
        /// <param name="warningsOnly">Optional parameter indicating whether or not to show only warnings.</param>
        /// <param name="showItemAndPropertyList">Optional parameter indicating whether or not to show a list of items and properties at the start of each project build.</param>
        /// <param name="showCommandLine">Optional parameter indicating whether or not to show <see cref="TaskCommandLineEventArgs" /> messages.</param>
        /// <param name="showTimestamp">Optional parameter indicating whether or not to show the timestamp as a prefix to any message.</param>
        /// <param name="showEventId">Optional parameter indicating whether or not to show eventId for started events, finished events, and messages.</param>
        /// <returns>The build output in the format of a console log.</returns>
        public string GetConsoleLog(LoggerVerbosity verbosity = LoggerVerbosity.Normal, bool showSummary = true, bool performanceSummary = false, bool errorsOnly = false, bool warningsOnly = false, bool showItemAndPropertyList = true, bool showCommandLine = false, bool showTimestamp = false, bool showEventId = false)
        {
            if (_lastConsoleOutput != null && verbosity == _lastVerbosity)
            {
                return _lastConsoleOutput;
            }

            _lastVerbosity = verbosity;

            _lastConsoleOutput = ConsoleLoggerStringBuilder.GetConsoleLogAsString(AllEvents, verbosity, showSummary, performanceSummary, errorsOnly, warningsOnly, showItemAndPropertyList, showCommandLine, showTimestamp, showEventId);

            ConsoleOutputCreated?.Invoke(this, verbosity);

            return _lastConsoleOutput;
        }

        /// <summary>
        /// Adds a build event.
        /// </summary>
        /// <param name="buildEventArgs">A <see cref="BuildEventArgs" /> object to add.</param>
        protected void Add(BuildEventArgs buildEventArgs)
        {
            _lastConsoleOutput = null;

            _allEvents.Enqueue(buildEventArgs);

            switch (buildEventArgs)
            {
                case BuildMessageEventArgs buildMessageEventArgs:
                    _messageEvents.Add(buildMessageEventArgs);
                    break;

                case BuildWarningEventArgs buildWarningEventArgs:
                    _warningEvents.Add(buildWarningEventArgs);
                    break;

                case BuildErrorEventArgs buildErrorEventArgs:
                    _errorEvents.Add(buildErrorEventArgs);
                    break;
            }
        }
    }
}
