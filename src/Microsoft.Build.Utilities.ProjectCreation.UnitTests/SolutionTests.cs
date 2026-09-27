// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.VisualStudio.SolutionPersistence.Model;
using Shouldly;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class SolutionTests : TestBase
    {
        [Fact]
        public void BasicTest()
        {
            string solutionFileFullPath = Path.Combine(TestRootPath, "solution1.sln");

            string project1Name = "project1";

            string project1FullPath = Path.Combine(TestRootPath, project1Name, "project1.csproj");

            ProjectCreator project1 = ProjectCreator.Templates.SdkCsproj(project1FullPath);

            SolutionCreator solution = SolutionCreator.Create(solutionFileFullPath)
                .TryProject(project1, projectInSolution: out SolutionProjectModel projectInSolution)
                .Save();

            File.ReadAllText(solutionFileFullPath).ShouldBe(
                @$"Microsoft Visual Studio Solution File, Format Version 12.00
Project(""{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}"") = ""{project1Name}"", ""{project1FullPath.Replace('/', '\\')}"", ""{{{projectInSolution.Id.ToString().ToUpperInvariant()}}}""
EndProject
Global
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
",
                StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void CanBuild()
        {
            ProjectCreator project1 = ProjectCreator.Templates.SdkCsproj(path: Path.Combine(TestRootPath, "project1", "project1.csproj"));

            SolutionCreator.Create(Path.Combine(TestRootPath, "solution1.sln"))
                .Configuration("Debug")
                .Configuration("Release")
                .Platform("Any CPU")
                .Project(project1)
                .TryBuild(out _);
        }

        [Fact]
        public void CanBuildWithGlobalProperties()
        {
            ProjectCreator project1 = ProjectCreator.Templates.SdkCsproj(path: Path.Combine(TestRootPath, "project1", "project1.csproj"));

            Directory.CreateDirectory(Path.Combine(TestRootPath, "project1"));

            File.WriteAllText(
                Path.Combine(TestRootPath, "project1", "Xyz.cs"),
                """
                public class Xyz
                {
                    public void Boo(int value)
                    {
                        // This switch statement has no fallthrough, which triggers warning CS1522 normally.
                        switch (value)
                        {
                        }
                    }
                }
                """);

            SolutionCreator solution = SolutionCreator.Create(Path.Combine(TestRootPath, "solution1.sln"))
                .Configuration("Debug")
                .Configuration("Release")
                .Platform("Any CPU")
                .Project(project1);

            // When built normally, we should see a warning.
            solution.TryBuild(restore: true, out _, out BuildOutput output);
            output.Warnings.Count.ShouldBe(1);

            // When built with a global property turning that warning off, we shouldn't see any.
            IDictionary<string, string> globalProperties = new Dictionary<string, string>()
            {
                { "NoWarn", "CS1522" },
            };

            solution.TryBuild(restore: true, globalProperties, out _, out output);
            output.Warnings.ShouldBeEmpty();
        }
    }
}
