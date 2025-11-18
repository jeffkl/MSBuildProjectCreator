// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Represents a helper class to build a console log from a collection of <see cref="BuildEventArgs" />.
    /// </summary>
    internal sealed class ConsoleLoggerStringBuilder
    {
        private readonly StringBuilder _stringBuilder;

        private ConsoleLoggerStringBuilder(StringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;
        }

        internal static string GetConsoleLogAsString(IReadOnlyCollection<BuildEventArgs> events, LoggerVerbosity verbosity = LoggerVerbosity.Normal, bool showSummary = true, bool performanceSummary = false, bool errorsOnly = false, bool warningsOnly = false, bool showItemAndPropertyList = true, bool showCommandLine = false, bool showTimestamp = false, bool showEventId = false)
        {
            StringBuilder stringBuilder = new StringBuilder(events.Count * (int)verbosity * 100);

            ConsoleLoggerStringBuilder consoleLoggerStringBuilder = new(stringBuilder);

            return consoleLoggerStringBuilder.ReplayEventsAndGetLogString(events, verbosity, showSummary, performanceSummary, errorsOnly, warningsOnly, showItemAndPropertyList, showCommandLine, showTimestamp, showEventId);
        }

        private string GetLoggerParameters(bool showSummary, bool performanceSummary, bool errorsOnly, bool warningsOnly, bool showItemAndPropertyList, bool showCommandLine, bool showTimestamp, bool showEventId)
        {
            List<string> parameters = new(capacity: 10)
            {
                "ForceNoAlign",
                "DisableConsoleColor",
            };

            if (!showSummary)
            {
                parameters.Add("NoSummary");
            }

            if (performanceSummary)
            {
                parameters.Add("PerformanceSummary");
            }

            if (errorsOnly)
            {
                parameters.Add("ErrorsOnly");
            }

            if (warningsOnly)
            {
                parameters.Add("WarningsOnly");
            }

            if (!showItemAndPropertyList)
            {
                parameters.Add("NoItemAndPropertyList");
            }

            if (showCommandLine)
            {
                parameters.Add("ShowCommandLine");
            }

            if (showTimestamp)
            {
                parameters.Add("ShowTimestamp");
            }

            if (showEventId)
            {
                parameters.Add("ShowEventId");
            }

            return string.Join(";", parameters);
        }

        private void OnColorReset()
        {
        }

        private void OnColorSet(ConsoleColor color)
        {
        }

        private void OnWrite(string message)
        {
            _stringBuilder.Append(message);
        }

        private string ReplayEventsAndGetLogString(IReadOnlyCollection<BuildEventArgs> events, LoggerVerbosity verbosity = LoggerVerbosity.Normal, bool showSummary = true, bool performanceSummary = false, bool errorsOnly = false, bool warningsOnly = false, bool showItemAndPropertyList = true, bool showCommandLine = false, bool showTimestamp = false, bool showEventId = false)
        {
            ReplayEventSource eventSource = new ReplayEventSource();

            ConsoleLogger logger = new ConsoleLogger(verbosity, OnWrite, OnColorSet, OnColorReset)
            {
                Parameters = GetLoggerParameters(showSummary, performanceSummary, errorsOnly, warningsOnly, showItemAndPropertyList, showCommandLine, showTimestamp, showEventId),
            };

            logger.Initialize(eventSource);

            eventSource.ReplayEvents(events);

            logger.Shutdown();

            return _stringBuilder.ToString();
        }

        private class ReplayEventSource : IEventSource
        {
            public event AnyEventHandler? AnyEventRaised;

            public event BuildFinishedEventHandler? BuildFinished;

            public event BuildStartedEventHandler? BuildStarted;

            public event CustomBuildEventHandler? CustomEventRaised;

            public event BuildErrorEventHandler? ErrorRaised;

            public event BuildMessageEventHandler? MessageRaised;

            public event ProjectFinishedEventHandler? ProjectFinished;

            public event ProjectStartedEventHandler? ProjectStarted;

            public event BuildStatusEventHandler? StatusEventRaised;

            public event TargetFinishedEventHandler? TargetFinished;

            public event TargetStartedEventHandler? TargetStarted;

            public event TaskFinishedEventHandler? TaskFinished;

            public event TaskStartedEventHandler? TaskStarted;

            public event BuildWarningEventHandler? WarningRaised;

            public void ReplayEvents(IReadOnlyCollection<BuildEventArgs> events)
            {
                foreach (BuildEventArgs buildEventArgs in events)
                {
                    if (buildEventArgs.BuildEventContext is null)
                    {
                        buildEventArgs.BuildEventContext = BuildEventContext.Invalid;
                    }

                    AnyEventRaised?.Invoke(this, buildEventArgs);

                    switch (buildEventArgs)
                    {
                        case BuildMessageEventArgs buildMessageEventArgs:
                            MessageRaised?.Invoke(this, buildMessageEventArgs);
                            break;

                        case BuildErrorEventArgs buildErrorEventArgs:
                            ErrorRaised?.Invoke(this, buildErrorEventArgs);
                            break;

                        case BuildWarningEventArgs buildWarningEventArgs:
                            WarningRaised?.Invoke(this, buildWarningEventArgs);
                            break;

                        case BuildStartedEventArgs buildStartedEventArgs:
                            BuildStarted?.Invoke(this, buildStartedEventArgs);
                            break;

                        case BuildFinishedEventArgs buildFinishedEventArgs:
                            BuildFinished?.Invoke(this, buildFinishedEventArgs);
                            break;

                        case ProjectStartedEventArgs projectStartedEventArgs:
                            ProjectStarted?.Invoke(this, projectStartedEventArgs);
                            break;

                        case ProjectFinishedEventArgs projectFinishedEventArgs:
                            ProjectFinished?.Invoke(this, projectFinishedEventArgs);
                            break;

                        case TargetStartedEventArgs targetStartedEventArgs:
                            TargetStarted?.Invoke(this, targetStartedEventArgs);
                            break;

                        case TargetFinishedEventArgs targetFinishedEventArgs:
                            TargetFinished?.Invoke(this, targetFinishedEventArgs);
                            break;

                        case TaskStartedEventArgs taskStartedEventArgs:
                            TaskStarted?.Invoke(this, taskStartedEventArgs);
                            break;

                        case TaskFinishedEventArgs taskFinishedEventArgs:
                            TaskFinished?.Invoke(this, taskFinishedEventArgs);
                            break;

                        case CustomBuildEventArgs customBuildEventArgs:
                            CustomEventRaised?.Invoke(this, customBuildEventArgs);
                            break;

                        case BuildStatusEventArgs statusEventArgs:
                            StatusEventRaised?.Invoke(this, statusEventArgs);
                            break;
                    }
                }
            }
        }
    }
}
