// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class BuildTests : TestBase
    {
        [Fact]
        public void BasicBuild()
        {
            ProjectCreator
                .Create(
                    Path.Combine(TestRootPath, "project2.proj"),
                    defaultTargets: "Build")
                .Target("Build")
                .Target("Restore")
                .TryBuild("Build", out bool result1)
                .TryBuild(restore: true, "Build", out bool result2)
                .TryBuild(restore: true, out bool result3);

            result1.ShouldBeTrue();
            result2.ShouldBeTrue();
            result3.ShouldBeTrue();
        }

        [Fact]
        public void BuildTargetOutputsTest()
        {
            ProjectCreator
                .Create(Path.Combine(TestRootPath, "project1.proj"))
                .Target("Build", returns: "@(MyItems)")
                .TargetItemInclude("MyItems", "E32099C7AF4E481885B624E5600C718A")
                .TargetItemInclude("MyItems", "7F38E64414104C6182F492B535926187")
                .Save()
                .TryBuild("Build", out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult> targetOutputs);

            result.ShouldBeTrue(buildOutput.GetConsoleLog());

            KeyValuePair<string, TargetResult> item = targetOutputs.ShouldHaveSingleItem(buildOutput.GetConsoleLog());

            item.Key.ShouldBe("Build", buildOutput.GetConsoleLog());

            item.Value.Items.Select(i => i.ItemSpec).ShouldBe(new[] { "E32099C7AF4E481885B624E5600C718A", "7F38E64414104C6182F492B535926187" }, buildOutput.GetConsoleLog());
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
                .TryBuild("Build", globalProperties, out bool resultWithGlobalProperties, out BuildOutput buildOutputWithGlobalProperties, out IDictionary<string, TargetResult> targetOutputs);

            resultWithoutGlobalProperties.ShouldBeTrue(buildOutputWithoutGlobalProperties.GetConsoleLog());

            buildOutputWithoutGlobalProperties.MessageEvents.High.ShouldHaveSingleItem(buildOutputWithoutGlobalProperties.GetConsoleLog()).Message.ShouldBe("Value = ", buildOutputWithoutGlobalProperties.GetConsoleLog());

            resultWithGlobalProperties.ShouldBeTrue(buildOutputWithGlobalProperties.GetConsoleLog());

            buildOutputWithGlobalProperties.MessageEvents.High.ShouldHaveSingleItem(buildOutputWithGlobalProperties.GetConsoleLog()).Message.ShouldBe("Value = D7BBABDFB2D142D3A75E0C1A33E33780", buildOutputWithGlobalProperties.GetConsoleLog());
        }

        [Fact]
        public void BuildOutputContainsOutOfProcMessages()
        {
            const int messageCount = 100;

            List<ProjectCreator> projects = new List<ProjectCreator>(messageCount);

            for (int i = 0; i < messageCount; i++)
            {
                FileInfo projectPath = new FileInfo(Path.Combine(TestRootPath, $"Project{i}", $"Project{i}.proj"));

                projectPath.Directory!.Create();

                projects.Add(
                    ProjectCreator.Create()
                        .Target("Build")
                        .TaskMessage($"Message {i}", MessageImportance.High)
                        .Save(projectPath.FullName));
            }

            ProjectCreator.Create(path: GetTempProjectPath())
                .ForEach(projects, (project, creator) => creator.ItemProjectReference(project))
                .Target("Build")
                .Task("MSBuild", parameters: new Dictionary<string, string>
                {
                    ["Projects"] = "@(ProjectReference)",
                    ["BuildInParallel"] = bool.TrueString,
                })
                .Save()
                .TryBuild(out bool result, out BuildOutput buildOutput);

            result.ShouldBeTrue(buildOutput.GetConsoleLog());

            buildOutput.IsShutdown.ShouldBeTrue(buildOutput.GetConsoleLog());

            buildOutput.MessageEvents.High.Count.ShouldBeGreaterThanOrEqualTo(messageCount, buildOutput.GetConsoleLog());
        }

        [Fact]
        public void BuildOutputIsComplete()
        {
            ProjectCreator.Create()
                .Target("Build")
                    .For(100, (i, creator) => creator.TaskMessage($"Message {i}", MessageImportance.High))
                .TryBuild(out bool result, out BuildOutput buildOutput);

            result.ShouldBeTrue(buildOutput.GetConsoleLog());

            buildOutput.IsShutdown.ShouldBeTrue();

            buildOutput.MessageEvents.High.Count.ShouldBeGreaterThanOrEqualTo(100, buildOutput.GetConsoleLog());
        }

        [Fact]
        public void CanBuildLotsOfProjects()
        {
            int maxBuilds = Environment.ProcessorCount * 2;

            List<ProjectCreator> projects = new List<ProjectCreator>(maxBuilds);

            for (int i = 0; i < maxBuilds; i++)
            {
                projects.Add(
                    ProjectCreator.Create(GetTempProjectPath())
                        .Target("Build")
                        .Task(
                            "Exec",
                            parameters: new Dictionary<string, string>
                            {
                                ["Command"] = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ping 127.0.0.1 -n 2 >NUL" : "sleep 2",
                            })
                        .Save());
            }

            ProjectCreator.Create(path: GetTempProjectPath())
                .ForEach(projects, (project, creator) => creator.ItemProjectReference(project))
                .Target("Build")
                .Task("MSBuild", parameters: new Dictionary<string, string>
                {
                    ["Projects"] = "@(ProjectReference)",
                    ["BuildInParallel"] = bool.TrueString,
                })
                .Save()
                .TryBuild(out bool result, out BuildOutput buildOutput);

            result.ShouldBeTrue(buildOutput.GetConsoleLog());
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
        public void ProjectWithGlobalPropertiesUsedDuringBuild()
        {
            ProjectCollection projectCollection = new ProjectCollection(new Dictionary<string, string>
            {
                ["Property1"] = "F6EBAC88A10E453B9AF8FA656A574737",
            });

            ProjectCreator creator = ProjectCreator.Create(projectCollection: projectCollection)
                .TaskMessage("$(Property1)", MessageImportance.High)
                .TryBuild(out bool result, out BuildOutput buildOutput);

            result.ShouldBeTrue(buildOutput.GetConsoleLog());

            buildOutput.Messages.Last().ShouldBe("F6EBAC88A10E453B9AF8FA656A574737");
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

                result.ShouldBeTrue(buildOutput.GetConsoleLog());

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
        public void ProjectWithNoPathRestoreThrowsInvalidOperationException()
        {
            InvalidOperationException exception = Should.Throw<InvalidOperationException>(() =>
            {
                ProjectCreator.Templates.LogsMessage("6E83EF78-959F-45A2-9FE3-08BAD99C0F92")
                    .TryRestore(out bool _);
            });

            exception.Message.ShouldBe("Project has not been given a path to save to.");
        }

        [Fact]
        public void RestoreAndBuildUseDifferentGlobalPropertiesWhenGlobalPropertiesSpecified()
        {
            Dictionary<string, string> globalProperties = new Dictionary<string, string>
            {
                ["Something"] = bool.TrueString,
            };

            ProjectCreator.Create(
                    path: GetTempFileName(".csproj"))
                .Target("Restore")
                    .TaskMessage("Restore $(ExcludeRestorePackageImports)", MessageImportance.High)
                    .TaskMessage("Restore Something $(Something)", MessageImportance.High)
                .Target("Build")
                    .TaskMessage("Build $(ExcludeRestorePackageImports)", MessageImportance.High)
                    .TaskMessage("Build Something $(Something)", MessageImportance.High)
                .Save()
                .TryBuild(restore: true, "Build", globalProperties, out bool result, out BuildOutput buildOutput);

            result.ShouldBeTrue(buildOutput.GetConsoleLog());

            buildOutput.MessageEvents.High.ShouldContain(i => i.Message == "Restore true", buildOutput.GetConsoleLog());
            buildOutput.MessageEvents.High.ShouldContain(i => i.Message == "Restore Something True", buildOutput.GetConsoleLog());

            buildOutput.MessageEvents.High.ShouldNotContain(i => i.Message == "Build true", buildOutput.GetConsoleLog());
            buildOutput.MessageEvents.High.ShouldContain(i => i.Message == "Build Something True", buildOutput.GetConsoleLog());
        }

        [Fact]
        public void RestoreAndBuildUseDifferentGlobalPropertiesWhenProjectCollectionSpecified()
        {
            Dictionary<string, string> globalProperties = new Dictionary<string, string>
            {
                ["Something"] = bool.TrueString,
            };

            using ProjectCollection projectCollection = new ProjectCollection(globalProperties);

            ProjectCreator.Create(
                    path: GetTempFileName(".csproj"),
                    projectCollection: projectCollection)
                .Target("Restore")
                    .TaskMessage("Restore $(ExcludeRestorePackageImports)", MessageImportance.High)
                    .TaskMessage("Restore Something $(Something)", MessageImportance.High)
                .Target("Build")
                    .TaskMessage("Build $(ExcludeRestorePackageImports)", MessageImportance.High)
                    .TaskMessage("Build Something $(Something)", MessageImportance.High)
                .Save()
                .TryBuild(restore: true, "Build", globalProperties, out bool result, out BuildOutput buildOutput);

            result.ShouldBeTrue(buildOutput.GetConsoleLog());

            buildOutput.MessageEvents.High.ShouldContain(i => i.Message == "Restore true", buildOutput.GetConsoleLog());
            buildOutput.MessageEvents.High.ShouldContain(i => i.Message == "Restore Something True", buildOutput.GetConsoleLog());

            buildOutput.MessageEvents.High.ShouldNotContain(i => i.Message == "Build true", buildOutput.GetConsoleLog());
            buildOutput.MessageEvents.High.ShouldContain(i => i.Message == "Build Something True", buildOutput.GetConsoleLog());
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