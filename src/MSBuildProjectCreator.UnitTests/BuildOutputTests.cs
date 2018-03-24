// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class BuildOutputTests
    {
        [Theory]
        [InlineData("5F56E5B72FDE4405A021F166D0E4D7A8", "B586E6DA25314DA8B6700CF798A88892")]
        public void Errors(string expectedMessage, string expectedCode)
        {
            BuildOutput buildOutput = GetProjectLoggerWithEvents(eventSource => { eventSource.OnErrorRaised(expectedMessage, expectedCode); });

            BuildErrorEventArgs args = buildOutput.Errors.ShouldHaveSingleItem();

            args.Message.ShouldBe(expectedMessage);

            args.Code.ShouldBe(expectedCode);
        }

        [Theory]
        [InlineData(MessageImportance.High)]
        [InlineData(MessageImportance.Normal)]
        [InlineData(MessageImportance.Low)]
        public void MessagesByImportance(MessageImportance importance)
        {
            const string expectedMessage = "A7E9F67E46A64181B25DC136A786F480";

            BuildOutput buildOutput = GetProjectLoggerWithEvents(eventSource => { eventSource.OnMessageRaised(expectedMessage, importance); });

            var actualItem = buildOutput.Messages.ShouldHaveSingleItem();

            actualItem.Message.ShouldBe(expectedMessage);

            switch (importance)
            {
                case MessageImportance.High:
                    buildOutput.MessagesHighImportance.ShouldHaveSingleItem().ShouldBe(actualItem);
                    break;

                case MessageImportance.Normal:
                    buildOutput.MessagesNormalImportance.ShouldHaveSingleItem().ShouldBe(actualItem);
                    break;

                case MessageImportance.Low:
                    buildOutput.MessagesLowImportance.ShouldHaveSingleItem().ShouldBe(actualItem);
                    break;
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void OverallResult(bool succeeded)
        {
            BuildOutput buildOutput = GetProjectLoggerWithEvents(eventSource => { eventSource.OnBuildFinished(succeeded); });

            buildOutput.Succeeded.ShouldBe(succeeded);
        }

        [Fact]
        public void ResultsByProject()
        {
            Dictionary<string, bool> projects = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase)
            {
                { @"DA920698\E40D\4D8F\89D8\B85D870C4214", true },
                { @"53C78698\F360\491F\8025\B323782DD912", false },
                { @"F42234CB\7504\4F23\ACD7\D58F5BCDD3C6", true }
            };

            BuildOutput buildOutput = GetProjectLoggerWithEvents(eventSource => { Parallel.ForEach(projects, project => { eventSource.OnProjectFinished(project.Key, project.Value); }); });

            buildOutput.ProjectResults.ShouldBe(projects, ignoreOrder: true);
        }

        [Theory]
        [InlineData("354D44C0FAC64155B1575C36696C1AB0", "079E33C0143E43E599F6ED421CFB519A")]
        public void Warnings(string expectedMessage, string expectedCode)
        {
            BuildOutput buildOutput = GetProjectLoggerWithEvents(eventSource => { eventSource.OnWarningRaised(expectedMessage, expectedCode); });

            BuildWarningEventArgs args = buildOutput.Warnings.ShouldHaveSingleItem();

            args.Message.ShouldBe(expectedMessage);

            args.Code.ShouldBe(expectedCode);
        }

        private BuildOutput GetProjectLoggerWithEvents(Action<MockEventSource> eventSourceActions)
        {
            MockEventSource eventSource = new MockEventSource();

            BuildOutput buildOutput = BuildOutput.Create();

            buildOutput.Initialize(eventSource);

            eventSourceActions(eventSource);

            return buildOutput;
        }
    }
}