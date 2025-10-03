// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System.IO;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class SolutionTests : TestBase
    {
        [Fact]
        public void Test1()
        {
            ProjectCreator project1 = ProjectCreator.Templates.SdkCsproj()
                .Save(Path.Combine(TestRootPath, "project1", "project1.csproj"));

            SolutionCreator solution = SolutionCreator.Create(Path.Combine(TestRootPath, "solution1.sln"))
                .Project(project1)
                .Save();
        }
    }
}
