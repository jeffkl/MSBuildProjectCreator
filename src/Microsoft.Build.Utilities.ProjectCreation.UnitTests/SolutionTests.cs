// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.VisualStudio.SolutionPersistence.Model;
using Shouldly;
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

            SolutionCreator solution = SolutionCreator.Create()
                .TryProject(project1, projectInSolution: out SolutionProjectModel projectInSolution)
                .Save(solutionFileFullPath);

            File.ReadAllText(solutionFileFullPath).ShouldBe(@$"Microsoft Visual Studio Solution File, Format Version 12.00
Project(""{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}"") = ""{project1Name}"", ""{project1FullPath}"", ""{{{projectInSolution.Id.ToString().ToUpperInvariant()}}}""
EndProject
Global
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
", StringCompareShould.IgnoreLineEndings);
        }
    }
}
