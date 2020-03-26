// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Microsoft.Build.Exceptions;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class ProjectTests : MSBuildTestBase
    {
        [Fact]
        public void ProjectInstanceGetsCorrectObject()
        {
            ProjectCreator.Create()
                .Property("FA7AA8A112DA480AA2591A7FF619B05A", "43EEA58D938C4C6A9060816155FD5FA9")
                .ProjectInstance
                .GetPropertyValue("FA7AA8A112DA480AA2591A7FF619B05A")
                .ShouldBe("43EEA58D938C4C6A9060816155FD5FA9");
        }

        [Fact]
        public void ProjectIsReEvaluated()
        {
            ProjectCollection projectCollection = new ProjectCollection();

            ProjectCreator creator = ProjectCreator.Create(projectCollection: projectCollection);

            creator.Project.GetPropertyValue("Property1").ShouldBe(string.Empty);

            creator.Project.SetGlobalProperty("Property1", "8AD6F0530E774E468DBBD5B4143A1B1D");

            creator.Project.GetPropertyValue("Property1").ShouldBe("8AD6F0530E774E468DBBD5B4143A1B1D");
        }

        [Fact]
        public void ProjectWithGlobalProperties()
        {
            IDictionary<string, string> globalProperties = new Dictionary<string, string>
            {
                ["Property1"] = "0945E8894F5B46C3894158EAE2815DF3",
            };

            ProjectCreator creator = ProjectCreator.Create(globalProperties: globalProperties);

            creator.ProjectCollection.ShouldBeSameAs(ProjectCollection.GlobalProjectCollection);

            creator.Project.ProjectCollection.ShouldBeSameAs(ProjectCollection.GlobalProjectCollection);

            creator.Project.GlobalProperties.ShouldBe(globalProperties);

            creator.Project.GetPropertyValue("Property1").ShouldBe("0945E8894F5B46C3894158EAE2815DF3");
        }

        [Fact]
        public void ProjectWithGlobalPropertiesFromProjectCollection()
        {
            ProjectCollection projectCollection = new ProjectCollection(new Dictionary<string, string>
            {
                ["Property1"] = "5DFF776EBCFF4173B0E14160C2191402",
            });

            ProjectCreator creator = ProjectCreator.Create(projectCollection: projectCollection);

            creator.ProjectCollection.ShouldBeSameAs(projectCollection);

            creator.Project.ProjectCollection.ShouldBeSameAs(projectCollection);

            creator.Project.GlobalProperties.ShouldBe(projectCollection.GlobalProperties);

            creator.Project.GetPropertyValue("Property1").ShouldBe("5DFF776EBCFF4173B0E14160C2191402");
        }

        [Fact]
        public void TryGetProjectBuildOutput()
        {
            ProjectCreator.Create()
                .Property("ImportDirectoryBuildTargets", "false")
                .Import(@"$(MSBuildBinPath)\Microsoft.Common.targets")
                .Import(@"$(MSBuildBinPath)\Microsoft.Common.targets")
                .TryGetProject(out Project _, out BuildOutput buildOutput);

            buildOutput.WarningEvents.ShouldHaveSingleItem(buildOutput.GetConsoleLog()).Code.ShouldBe("MSB4011", buildOutput.GetConsoleLog());
        }

        [Fact]
        public void TryGetProjectWithGlobalProperties()
        {
            ProjectCreator.Create()
                .Property("Foo", "E82055CD4BBE40E58DF224A8734E75AC", setIfEmpty: true)
                .TryGetProject(out Project project, new Dictionary<string, string>
                {
                    ["Foo"] = "CF8CBA9CEA034D2AB1704B11287579C8",
                });

            project.GetPropertyValue("Foo").ShouldBe("CF8CBA9CEA034D2AB1704B11287579C8");
        }

        [Fact]
        public void TryGetProjectWithProjectCollection()
        {
            ProjectCollection expectedProjectCollection = new ProjectCollection(new Dictionary<string, string>
            {
                ["Foo"] = "CF3478738DC04B3C9358FE0D23456BCD",
            });

            ProjectCreator.Create()
                .Property("Foo", "4458994367D741719B24DE003EE4F541", setIfEmpty: true)
                .TryGetProject(out Project project, projectCollection: expectedProjectCollection);

            project.ProjectCollection.ShouldBeSameAs(expectedProjectCollection);

            project.GetPropertyValue("Foo").ShouldBe("CF3478738DC04B3C9358FE0D23456BCD");
        }

        [Fact]
        public void TryGetProjectWithToolsVersion()
        {
            Should.Throw<InvalidProjectFileException>(
                () => ProjectCreator.Create().TryGetProject(out _, toolsVersion: "624491E368E24CE38F51D9D620685809"))
                .Message
                .ShouldStartWith("The tools version \"624491E368E24CE38F51D9D620685809\" is unrecognized. Available tools versions are");
        }
    }
}