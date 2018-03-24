// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Framework;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public sealed class MockEventSource : IEventSource2
    {
        public event AnyEventHandler AnyEventRaised;

        public event BuildFinishedEventHandler BuildFinished;

        public event BuildStartedEventHandler BuildStarted;

        public event CustomBuildEventHandler CustomEventRaised;

        public event BuildErrorEventHandler ErrorRaised;

        public event BuildMessageEventHandler MessageRaised;

        public event ProjectFinishedEventHandler ProjectFinished;

        public event ProjectStartedEventHandler ProjectStarted;

        public event BuildStatusEventHandler StatusEventRaised;

        public event TargetFinishedEventHandler TargetFinished;

        public event TargetStartedEventHandler TargetStarted;

        public event TaskFinishedEventHandler TaskFinished;

        public event TaskStartedEventHandler TaskStarted;

        public event TelemetryEventHandler TelemetryLogged;

        public event BuildWarningEventHandler WarningRaised;

        public void OnBuildFinished(bool succeeded, string message = null, string helpKeyword = null)
        {
            BuildFinished?.Invoke(this, new BuildFinishedEventArgs(message, helpKeyword, succeeded));
        }

        public void OnErrorRaised(string message, string code = null, string file = null, int lineNumber = -1, int columnNumber = -1, int endLineNumber = -1, int endColumnNumber = -1, string helpKeyword = null, string senderName = null)
        {
            ErrorRaised?.Invoke(this, new BuildErrorEventArgs(null, code, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message, helpKeyword, senderName));
        }

        public void OnMessageRaised(string message, MessageImportance importance = MessageImportance.Normal)
        {
            MessageRaised?.Invoke(this, new BuildMessageEventArgs(message, null, null, importance));
        }

        public void OnProjectFinished(string projectFile, bool succeeded, string message = null, string helpKeyword = null)
        {
            ProjectFinished?.Invoke(this, new ProjectFinishedEventArgs(message, helpKeyword, projectFile, succeeded));
        }

        public void OnWarningRaised(string message, string code = null, string file = null, int lineNumber = -1, int columnNumber = -1, int endLineNumber = -1, int endColumnNumber = -1, string helpKeyword = null, string senderName = null)
        {
            WarningRaised?.Invoke(this, new BuildWarningEventArgs(null, code, file, lineNumber, columnNumber, endLineNumber, endColumnNumber, message, helpKeyword, senderName));
        }

        // ReSharper disable once UnusedMember.Local
        private void OnAnyEventRaised(BuildEventArgs e)
        {
            AnyEventRaised?.Invoke(this, e);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnBuildStarted(BuildStartedEventArgs e)
        {
            BuildStarted?.Invoke(this, e);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnCustomEventRaised(CustomBuildEventArgs e)
        {
            CustomEventRaised?.Invoke(this, e);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnProjectStarted(ProjectStartedEventArgs e)
        {
            ProjectStarted?.Invoke(this, e);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnStatusEventRaised(BuildStatusEventArgs e)
        {
            StatusEventRaised?.Invoke(this, e);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnTargetFinished(TargetFinishedEventArgs e)
        {
            TargetFinished?.Invoke(this, e);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnTargetStarted(TargetStartedEventArgs e)
        {
            TargetStarted?.Invoke(this, e);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnTaskFinished(TaskFinishedEventArgs e)
        {
            TaskFinished?.Invoke(this, e);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnTaskStarted(TaskStartedEventArgs e)
        {
            TaskStarted?.Invoke(this, e);
        }

        // ReSharper disable once UnusedMember.Local
        private void OnTelemetryLogged(TelemetryEventArgs e)
        {
            TelemetryLogged?.Invoke(this, e);
        }
    }
}