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
    /// Represents a collection of <see cref="BuildEventArgs" /> objects.
    /// </summary>
    public abstract class BuildEventArgsCollection : IDisposable
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
        /// Stores the warnings that were logged.
        /// </summary>
        private readonly List<BuildWarningEventArgs> _warningEvents = new List<BuildWarningEventArgs>();

        /// <summary>
        /// Stores all build events.
        /// </summary>
        private readonly ConcurrentQueue<BuildEventArgs> _allEvents = new ConcurrentQueue<BuildEventArgs>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildEventArgsCollection" /> class.
        /// </summary>
        protected BuildEventArgsCollection()
        {
            MessageEvents = new BuildMessageEventArgsCollection(_messageEvents);
            Messages = new BuildMessageCollection(this);
        }

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
        /// <returns>The build output in the format of a console log.</returns>
        public string GetConsoleLog(LoggerVerbosity verbosity = LoggerVerbosity.Normal)
        {
            StringBuilder sb = new StringBuilder(AllEvents.Count * 300);

            ConsoleLogger logger = new ConsoleLogger(verbosity, message => sb.Append(message), _ => { }, () => { });

            foreach (BuildEventArgs buildEventArgs in AllEvents)
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

        /// <summary>
        /// Adds a build event.
        /// </summary>
        /// <param name="buildEventArgs">A <see cref="BuildEventArgs" /> object to add.</param>
        protected void Add(BuildEventArgs buildEventArgs)
        {
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