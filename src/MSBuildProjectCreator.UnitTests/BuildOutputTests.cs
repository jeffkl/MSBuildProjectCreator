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
    public class BuildOutputTests : MSBuildTestBase
    {
        [Fact]
        public void ConsoleLog()
        {
            BuildOutput buildOutput = GetProjectLoggerWithEvents(eventSource =>
            {
                eventSource.OnErrorRaised("FDC8FB4F8E084055974580DF7CD7531E", "6496288436BE4E7CAE014F163914063C", "7B07B020E38343A89B3FA844A40895E4", 1, 2, 0, 0);
                eventSource.OnWarningRaised("E00BBDAEEFAB45949AFEE1BF792B1691", "56206897E63F44159603D22BB7C08145", "C455F26F4D4543E78F109BCB00F02BE2", 1, 2, 0, 0);
                eventSource.OnMessageRaised("55B991507D52403295E92E4FFA8704F3", MessageImportance.High);
                eventSource.OnMessageRaised("FA7FCCBE43B741998BAB399E74F2997D", MessageImportance.Normal);
                eventSource.OnMessageRaised("67C0E0E52F2A45A981F3143BAF00A4A3", MessageImportance.Low);
            });

            buildOutput.GetConsoleLog()
#pragma warning disable SA1116 // Split parameters must start on line after declaration
                .ShouldBe(@"7B07B020E38343A89B3FA844A40895E4(1,2): error 6496288436BE4E7CAE014F163914063C: FDC8FB4F8E084055974580DF7CD7531E
C455F26F4D4543E78F109BCB00F02BE2(1,2): warning 56206897E63F44159603D22BB7C08145: E00BBDAEEFAB45949AFEE1BF792B1691
55B991507D52403295E92E4FFA8704F3
FA7FCCBE43B741998BAB399E74F2997D
",
#pragma warning restore SA1116 // Split parameters must start on line after declaration
                    StringCompareShould.IgnoreLineEndings);
        }

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