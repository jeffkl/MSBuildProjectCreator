// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Logging;
using Microsoft.Build.Logging.StructuredLogger;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public void BinaryLogContainsNuGetGeneratedFiles()
        {
            string binLogPath = Path.Combine(TestRootPath, "test.binlog");
            string projectPath = Path.Combine(TestRootPath, "ClassLibraryA", "ClassLibraryA.csproj");

            using (PackageRepository.Create(TestRootPath)
                    .Package("PackageA", "1.0.0", out Package packageA)
                        .Library(TargetFramework))
            {
                string projectExtensionsPath;

                using (ProjectCollection projectCollection = new ProjectCollection())
                {
                    projectCollection.RegisterLogger(new Logging.BinaryLogger
                    {
                        Parameters = $"LogFile={binLogPath}",
                    });

                    ProjectCreator.Templates.SdkCsproj(
                            path: projectPath,
                            targetFramework: TargetFramework,
                            projectCollection: projectCollection)
                        .ItemPackageReference(packageA)
                        .TryBuild(restore: true, out bool result, out BuildOutput buildOutput)
                        .TryGetPropertyValue("MSBuildProjectExtensionsPath", out projectExtensionsPath);

                    result.ShouldBeTrue(buildOutput.GetConsoleLog());
                }

                Logging.StructuredLogger.Build binaryLog = BinaryLog.ReadBuild(binLogPath);

                binaryLog.SourceFiles.ShouldContain(i => i.FullPath.Equals(Path.Combine(projectExtensionsPath, "ClassLibraryA.csproj.nuget.g.props")));
            }
        }

        [Fact]
        public void BuildCanConsumePackageWithGeneratePathProperty()
        {
            string projectPath = Path.Combine(TestRootPath, "ClassLibraryA", "ClassLibraryA.csproj");

            using (PackageRepository.Create(TestRootPath)
                    .Package("PackageB", "1.0", out Package packageB)
                        .Library(TargetFramework)
                    .Package("PackageA", "1.0.0", out Package packageA)
                        .Dependency(packageB, TargetFramework)
                        .Library(TargetFramework))
            {
                string projectExtensionsPath;

                using ProjectCollection projectCollection = new ProjectCollection();

                ProjectCreator.Templates.SdkCsproj(
                        path: projectPath,
                        targetFramework: TargetFramework,
                        projectCollection: projectCollection)
                    .ItemPackageReference(
                        packageA,
                        metadata: new Dictionary<string, string?>
                        {
                            ["GeneratePathProperty"] = bool.TrueString,
                        })
                    .TryBuild(restore: true, out bool result, out BuildOutput buildOutput)
                    .TryGetPropertyValue("PkgPackageA", out string packagePath)
                    .TryGetPropertyValue("MSBuildProjectExtensionsPath", out projectExtensionsPath);

                result.ShouldBeTrue(buildOutput.GetConsoleLog());

                packagePath.ShouldEndWith(Path.Combine(".nuget", "packages", "packagea", "1.0.0"));
            }
        }

        [Fact]
        public void BuildOutputContainsOutOfProcMessages()
        {
            const int messageCount = 100;

            List<ProjectCreator> projects = new(messageCount);

            for (int i = 0; i < messageCount; i++)
            {
                FileInfo projectPath = new(Path.Combine(TestRootPath, $"Project{i}", $"Project{i}.proj"));

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
                .Task("MSBuild", parameters: new Dictionary<string, string?>
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
        public void BuildTargetOutputsTest()
        {
            ProjectCreator
                .Create(Path.Combine(TestRootPath, "project1.proj"))
                .Target("Build", returns: "@(MyItems)")
                .TargetItemInclude("MyItems", "E32099C7AF4E481885B624E5600C718A")
                .TargetItemInclude("MyItems", "7F38E64414104C6182F492B535926187")
                .Save()
                .TryBuild("Build", out bool result, out BuildOutput buildOutput, out IDictionary<string, TargetResult>? targetOutputs);

            result.ShouldBeTrue(buildOutput.GetConsoleLog());

            KeyValuePair<string, TargetResult> item = targetOutputs.ShouldHaveSingleItem(buildOutput.GetConsoleLog());

            item.Key.ShouldBe("Build", buildOutput.GetConsoleLog());

            item.Value.Items.Select(i => i.ItemSpec).ShouldBe(new[] { "E32099C7AF4E481885B624E5600C718A", "7F38E64414104C6182F492B535926187" }, buildOutput.GetConsoleLog());
        }

        [Fact]
        public void BuildWithGlobalProperties()
        {
            Dictionary<string, string> globalProperties = new()
            {
                ["Property1"] = "D7BBABDFB2D142D3A75E0C1A33E33780",
            };

            ProjectCreator
                .Create(Path.Combine(TestRootPath, "project1.proj"))
                .Target("Build")
                .TaskMessage("Value = $(Property1)", MessageImportance.High)
                .TryBuild("Build", out bool resultWithoutGlobalProperties, out BuildOutput buildOutputWithoutGlobalProperties)
                .TryBuild("Build", globalProperties, out bool resultWithGlobalProperties, out BuildOutput buildOutputWithGlobalProperties, out _);

            resultWithoutGlobalProperties.ShouldBeTrue(buildOutputWithoutGlobalProperties.GetConsoleLog());

            buildOutputWithoutGlobalProperties.MessageEvents.High.ShouldHaveSingleItem(buildOutputWithoutGlobalProperties.GetConsoleLog()).Message.ShouldBe("Value = ", buildOutputWithoutGlobalProperties.GetConsoleLog());

            resultWithGlobalProperties.ShouldBeTrue(buildOutputWithGlobalProperties.GetConsoleLog());

            buildOutputWithGlobalProperties.MessageEvents.High.ShouldHaveSingleItem(buildOutputWithGlobalProperties.GetConsoleLog()).Message.ShouldBe("Value = D7BBABDFB2D142D3A75E0C1A33E33780", buildOutputWithGlobalProperties.GetConsoleLog());
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void BuildInheritsGlobalProperties(bool restore, bool useCollectionProperties)
        {
            Dictionary<string, string> globalProperties = new()
            {
                ["Property1"] = "inherited value",
            };
            using ProjectCollection projectCollection = new(useCollectionProperties ? globalProperties : new Dictionary<string, string>());

            ProjectCreator.Create(
                    path: GetTempProjectPath(),
                    projectCollection: projectCollection,
                    globalProperties: useCollectionProperties ? null : globalProperties)
                .Property("Property1", "project value")
                .Target("Restore")
                    .TaskMessage("Restore value = $(Property1)", MessageImportance.High)
                .Target("Build")
                    .TaskMessage("Build value = $(Property1)", MessageImportance.High)
                .Save()
                .TryBuild(restore: restore, target: "Build", out bool result, out BuildOutput buildOutput);

            result.ShouldBeTrue(buildOutput.GetConsoleLog());
            buildOutput.MessageEvents.High.ShouldContain(message => message.Message == "Build value = inherited value", buildOutput.GetConsoleLog());
            if (restore)
            {
                buildOutput.MessageEvents.High.ShouldContain(message => message.Message == "Restore value = inherited value", buildOutput.GetConsoleLog());
            }
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void BuildTimeGlobalPropertiesDoNotChangeCreatorDefaults(bool restore, bool useEmptyProperties)
        {
            Dictionary<string, string> creatorProperties = new()
            {
                ["Property1"] = "creator value",
            };
            Dictionary<string, string> buildProperties = new();
            if (!useEmptyProperties)
            {
                buildProperties["Property1"] = "build value";
            }

            using ProjectCollection projectCollection = new();
            ProjectCreator creator = ProjectCreator.Create(
                    path: GetTempProjectPath(),
                    projectCollection: projectCollection,
                    globalProperties: creatorProperties)
                .Property("Property1", "project value")
                .Target("Restore")
                    .TaskMessage("Restore value = $(Property1)", MessageImportance.High)
                .Target("Build")
                    .TaskMessage("Build value = $(Property1)", MessageImportance.High)
                .Save()
                .TryBuild(restore: restore, target: "Build", globalProperties: buildProperties, out bool overrideResult, out BuildOutput overrideOutput)
                .TryGetPropertyValue("Property1", out string propertyAfterOverride)
                .TryBuild(restore: restore, target: "Build", out bool defaultResult, out BuildOutput defaultOutput);

            string expectedOverride = useEmptyProperties ? "project value" : "build value";
            overrideResult.ShouldBeTrue(overrideOutput.GetConsoleLog());
            overrideOutput.MessageEvents.High.ShouldContain(message => message.Message == $"Build value = {expectedOverride}", overrideOutput.GetConsoleLog());
            if (restore)
            {
                overrideOutput.MessageEvents.High.ShouldContain(message => message.Message == $"Restore value = {expectedOverride}", overrideOutput.GetConsoleLog());
            }

            propertyAfterOverride.ShouldBe("creator value");
            defaultResult.ShouldBeTrue(defaultOutput.GetConsoleLog());
            defaultOutput.MessageEvents.High.ShouldContain(message => message.Message == "Build value = creator value", defaultOutput.GetConsoleLog());
            creator.Project.GlobalProperties.ShouldBe(creatorProperties);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void BuildWithGlobalPropertiesCanBeRepeated(bool evaluateBeforeBuild)
        {
            Dictionary<string, string> globalProperties = new()
            {
                ["Property1"] = "collection value",
            };
            using ProjectCollection projectCollection = new(globalProperties);
            string projectPath = GetTempProjectPath();
            ProjectCreator creator = ProjectCreator.Create(path: projectPath, projectCollection: projectCollection)
                .Target("Build")
                    .TaskMessage("Value = $(Property1)", MessageImportance.High)
                .Save();
            if (evaluateBeforeBuild)
            {
                creator.Project.GetPropertyValue("Property1").ShouldBe("collection value");
            }

            creator
                .TryBuild("Build", globalProperties, out bool firstResult, out BuildOutput firstOutput)
                .TryGetPropertyValue("Property1", out string propertyAfterBuild)
                .TryBuild("Build", globalProperties, out bool secondResult, out BuildOutput secondOutput);

            firstResult.ShouldBeTrue(firstOutput.GetConsoleLog());
            secondResult.ShouldBeTrue(secondOutput.GetConsoleLog());
            propertyAfterBuild.ShouldBe("collection value");
            projectCollection.GetLoadedProjects(projectPath).ShouldHaveSingleItem();
        }

        [Fact]
        public void CanBuildLotsOfProjects()
        {
            int maxBuilds = Environment.ProcessorCount * 2;

            List<ProjectCreator> projects = new(maxBuilds);

            for (int i = 0; i < maxBuilds; i++)
            {
                projects.Add(
                    ProjectCreator.Create(GetTempProjectPath())
                        .UsingTaskRoslynCodeTaskFactory(
                            taskName: "Sleep",
                            sourceCode: "System.Threading.Thread.Sleep(2000);")
                        .Target("Build")
                        .Task("Sleep")
                        .Save());
            }

            ProjectCreator.Create(path: GetTempProjectPath())
                .ForEach(projects, (project, creator) => creator.ItemProjectReference(project))
                .Target("Build")
                .Task("MSBuild", parameters: new Dictionary<string, string?>
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
        public void CanRestoreAndBuildMultipleTimes()
        {
            ProjectCreator projectCreator = ProjectCreator.Create(
                    path: GetTempFileName(".csproj"))
                .Target("Restore")
                    .TaskMessage("Restoring...", MessageImportance.High)
                .Target("Build")
                    .TaskMessage("Building...", MessageImportance.High)
                .Save()
                .TryBuild(restore: true, "Build", out bool result, out BuildOutput buildOutput);

            result.ShouldBeTrue(buildOutput.GetConsoleLog());

            projectCreator.TryBuild(out result, out buildOutput);

            result.ShouldBeTrue(buildOutput.GetConsoleLog());
        }

        [Fact]
        public void TryBuildWithRestoreAndTargetWithoutBuildOutputRestoresBeforeBuild()
        {
            string markerPath = Path.Combine(TestRootPath, $"{Guid.NewGuid():N}.restore-marker");

            ProjectCreator.Create(path: GetTempFileName(".proj"))
                .Target("Restore")
                    .Task(
                        "WriteLinesToFile",
                        parameters: new Dictionary<string, string?>
                        {
                            ["File"] = markerPath,
                            ["Lines"] = "restored",
                            ["Overwrite"] = bool.TrueString,
                        })
                .Target("Build")
                    .TaskError("Restore target did not run.", condition: $"!Exists('{markerPath}')")
                .Save()
                .TryBuild(restore: true, target: "Build", globalProperties: null, out bool result);

            result.ShouldBeTrue();
        }

        [Fact]
        public void TryBuildWithRestoreWithoutBuildOutputRestoresBeforeBuild()
        {
            string markerPath = Path.Combine(TestRootPath, $"{Guid.NewGuid():N}.restore-marker");

            ProjectCreator.Create(path: GetTempFileName(".proj"), defaultTargets: "Build")
                .Target("Restore")
                    .Task(
                        "WriteLinesToFile",
                        parameters: new Dictionary<string, string?>
                        {
                            ["File"] = markerPath,
                            ["Lines"] = "restored",
                            ["Overwrite"] = bool.TrueString,
                        })
                .Target("Build")
                    .TaskError("Restore target did not run.", condition: $"!Exists('{markerPath}')")
                .Save()
                .TryBuild(restore: true, globalProperties: null, out bool result);

            result.ShouldBeTrue();
        }

        [Fact]
        public void ProjectCollectionLoggersWork()
        {
            string binLogPath = Path.Combine(TestRootPath, "test.binlog");
            string fileLogPath = Path.Combine(TestRootPath, "test.log");

            using (ProjectCollection projectCollection = new())
            {
                projectCollection.RegisterLogger(new Logging.BinaryLogger
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

            using (ProjectCollection projectCollection = new())
            {
                projectCollection.RegisterLogger(new Logging.BinaryLogger
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
        public void ProjectWithGlobalPropertiesUsedDuringBuild()
        {
            ProjectCollection projectCollection = new(new Dictionary<string, string>
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
            Dictionary<string, string> globalProperties = new()
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
            Dictionary<string, string> globalProperties = new()
            {
                ["Something"] = bool.TrueString,
            };

            using ProjectCollection projectCollection = new(globalProperties);

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

        [Fact]
        public void RestoreUsesGlobalPropertiesFromCreate()
        {
            Dictionary<string, string> globalProperties = new()
            {
                ["SomeGlobalProperty"] = "04BB4AFE8AE14B7A8E3511B5F2CD442B",
            };

            ProjectCreator
                .Create(
                    path: Path.Combine(TestRootPath, "project1.proj"),
                    globalProperties: globalProperties)
                .Target("Restore")
                    .TaskMessage("$(SomeGlobalProperty)", MessageImportance.High)
                    .TaskError("SomeGlobalProperty was not defined", condition: "'$(SomeGlobalProperty)' == ''")
                .TryRestore(out bool result, out BuildOutput buildOutput);

            result.ShouldBeTrue(buildOutput.GetConsoleLog());

            buildOutput.MessageEvents.High.ShouldContain(i => i.Message == "04BB4AFE8AE14B7A8E3511B5F2CD442B" && i.Importance == MessageImportance.High, buildOutput.GetConsoleLog());
        }
    }
}
