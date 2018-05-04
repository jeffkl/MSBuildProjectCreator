// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class CreationTests : MSBuildTestBase
    {
        [Fact]
        public void GlobalPropertiesArePassedThrough()
        {
            ProjectCollection projectCollection = new ProjectCollection(new Dictionary<string, string>
            {
                ["Property1"] = "5DFF776EBCFF4173B0E14160C2191402"
            });

            ProjectCreator creator = ProjectCreator.Create(projectCollection: projectCollection);

            creator.ProjectCollection.ShouldBe(projectCollection);

            creator.Project.ProjectCollection.ShouldBe(projectCollection);

            creator.Project.GlobalProperties.ShouldBe(projectCollection.GlobalProperties);

            creator.Project.GetPropertyValue("Property1").ShouldBe("5DFF776EBCFF4173B0E14160C2191402");
        }
    }
}
