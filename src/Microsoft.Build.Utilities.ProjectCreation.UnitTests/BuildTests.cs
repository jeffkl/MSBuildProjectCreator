// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Shouldly;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class BuildTests : TestBase
    {
        [Fact]
        public void BuildTargetOutputsTest()
        {
            ProjectCreator
                .Create(Path.Combine(TestRootPath, "project1.proj"))
                .Target("Build", returns: "@(MyItems)")
                .TargetItemInclude("MyItems", "E32099C7AF4E481885B624E5600C718A")
                .TargetItemInclude("MyItems", "7F38E64414104C6182F492B535926187")
                .Save()
                .TryBuild("Build", out bool result, out BuildOutput _, out IDictionary<string, TargetResult> targetOutputs);

            result.ShouldBeTrue();

            KeyValuePair<string, TargetResult> item = targetOutputs.ShouldHaveSingleItem();

            item.Key.ShouldBe("Build");

            item.Value.Items.Select(i => i.ItemSpec).ShouldBe(new[] { "E32099C7AF4E481885B624E5600C718A", "7F38E64414104C6182F492B535926187" });
        }

        [Fact]
        public void BuildWithGlobalProperties()
        {
            Dictionary<string, string> globalProperties = new Dictionary<string, string>
            {
                ["Property1"] = "D7BBABDFB2D142D3A75E0C1A33E33780",
            };

            ProjectCreator
                .Create(Path.Combine(TestRootPath, "project1.proj"))
                .Target("Build")
                .TaskMessage("Value = $(Property1)", MessageImportance.High)
                .TryBuild("Build", out bool resultWithoutGlobalProperties, out BuildOutput buildOutputWithoutGlobalProperties)
                .TryBuild("Build", globalProperties, out bool resultWithGlobalProperties, out BuildOutput buildOutputWithGlobalProperties);

            resultWithoutGlobalProperties.ShouldBeTrue();

            buildOutputWithoutGlobalProperties.MessageEvents.High.ShouldHaveSingleItem(buildOutputWithoutGlobalProperties.GetConsoleLog()).Message.ShouldBe("Value = ", buildOutputWithoutGlobalProperties.GetConsoleLog());

            resultWithGlobalProperties.ShouldBeTrue();

            buildOutputWithGlobalProperties.MessageEvents.High.ShouldHaveSingleItem(buildOutputWithGlobalProperties.GetConsoleLog()).Message.ShouldBe("Value = D7BBABDFB2D142D3A75E0C1A33E33780", buildOutputWithGlobalProperties.GetConsoleLog());
        }

        [Fact]
        public void CanRestoreAndBuild()
        {
            ProjectCreator.Create(
                    path: GetTempFileName(".csproj"))
                .Target("Restore")
                    .TaskMessage("Restoring...", MessageImportance.High)
                .Target("Build")
                    .TaskMessage("Building...", MessageImportance.High)
                .Save()
                .TryBuild(restore: true, "Build", out bool result, out BuildOutput buildOutput);

            result.ShouldBeTrue(buildOutput.GetConsoleLog());

            buildOutput.MessageEvents.High.ShouldContain(i => i.Message == "Restoring...", buildOutput.GetConsoleLog());

            buildOutput.MessageEvents.High.ShouldContain(i => i.Message == "Building...", buildOutput.GetConsoleLog());
        }

        [Fact]
        public void ProjectCollectionLoggersWork()
        {
            string binLogPath = Path.Combine(TestRootPath, "test.binlog");
            string fileLogPath = Path.Combine(TestRootPath, "test.log");

            using (ProjectCollection projectCollection = new ProjectCollection())
            {
                projectCollection.RegisterLogger(new BinaryLogger
                {
                    Parameters = $"LogFile={binLogPath}",
                });
                projectCollection.RegisterLogger(new FileLogger
                {
                    Parameters = $"LogFile={fileLogPath}",
                    Verbosity = LoggerVerbosity.Normal,
                    ShowSummary = true,
                });

                ProjectCreator.Templates
                    .LogsMessage(
                        text: "$(Property1)",
                        projectCollection: projectCollection)
                    .Property("Property1", "2AE492F6EEE04255B31B088051E9AF0F")
                    .Save(GetTempFileName(".proj"))
                    .TryBuild(out bool result, out BuildOutput buildOutput);

                result.ShouldBeTrue();

                buildOutput.MessageEvents.Normal.ShouldContain(i => i.Message == "2AE492F6EEE04255B31B088051E9AF0F", buildOutput.GetConsoleLog());
            }

            File.Exists(binLogPath).ShouldBeTrue();

            File.Exists(fileLogPath).ShouldBeTrue();

            string fileLogContents = File.ReadAllText(fileLogPath);

            fileLogContents.ShouldContain("2AE492F6EEE04255B31B088051E9AF0F", Case.Sensitive, fileLogContents);
        }

        [Fact]
        public void ProjectCollectionLoggersWorkWithRestore()
        {
            string binLogPath = Path.Combine(TestRootPath, "test.binlog");
            string fileLogPath = Path.Combine(TestRootPath, "test.log");

            using (ProjectCollection projectCollection = new ProjectCollection())
            {
                projectCollection.RegisterLogger(new BinaryLogger
                {
                    Parameters = $"LogFile={binLogPath}",
                });

                projectCollection.RegisterLogger(new FileLogger
                {
                    Parameters = $"LogFile={fileLogPath}",
                    Verbosity = LoggerVerbosity.Normal,
                    ShowSummary = true,
                });

                ProjectCreator.Templates
                    .LogsMessage(
                        text: "$(Property1)",
                        projectCollection: projectCollection)
                    .Property("Property1", "B7F9A257198D4A44A06BB6146AB27440")
                    .Target("Restore")
                        .TaskMessage("38EC33B686134B3C8DE4B8E571D4FB24", MessageImportance.High)
                    .Save(GetTempFileName(".proj"))
                    .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

                result.ShouldBeTrue();

                buildOutput.MessageEvents.High.ShouldContain(i => i.Message == "38EC33B686134B3C8DE4B8E571D4FB24", buildOutput.GetConsoleLog());
                buildOutput.MessageEvents.Normal.ShouldContain(i => i.Message == "B7F9A257198D4A44A06BB6146AB27440", buildOutput.GetConsoleLog());
            }

            File.Exists(binLogPath).ShouldBeTrue();

            File.Exists(fileLogPath).ShouldBeTrue();

            string fileLogContents = File.ReadAllText(fileLogPath);

            fileLogContents.ShouldContain("38EC33B686134B3C8DE4B8E571D4FB24", Case.Sensitive, fileLogContents);
            fileLogContents.ShouldContain("B7F9A257198D4A44A06BB6146AB27440", Case.Sensitive, fileLogContents);
        }

        [Fact]
        public void RestoreTargetCanBeRun()
        {
            ProjectCreator
                .Create(Path.Combine(TestRootPath, "project1.proj"))
                .Target("Restore")
                    .TaskMessage("312D2E6ABDDC4735B437A016CED1A68E", MessageImportance.High, condition: "'$(MSBuildRestoreSessionId)' != ''")
                    .TaskError("MSBuildRestoreSessionId was not defined", condition: "'$(MSBuildRestoreSessionId)' == ''")
                .TryRestore(out bool result, out BuildOutput buildOutput);

            result.ShouldBeTrue(buildOutput.GetConsoleLog());

            buildOutput.MessageEvents.High.ShouldContain(i => i.Message == "312D2E6ABDDC4735B437A016CED1A68E" && i.Importance == MessageImportance.High, buildOutput.GetConsoleLog());
        }
    }
}