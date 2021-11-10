// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Shouldly;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class ExtensibilityTests : MSBuildTestBase
    {
        /// <summary>
        /// Proves that <see cref="ProjectCreator" /> can be extended by and end user through extension methods.
        /// </summary>
        [Fact]
        public void CustomExtensionMethod()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .ForTestingOnly("4FCA06D98CEC4250B5E34FA66A85282B", "FAA8493986E442888CE8652BD7622B2C")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Import Project=""4FCA06D98CEC4250B5E34FA66A85282B"" Condition=""FAA8493986E442888CE8652BD7622B2C"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void CustomTemplate()
        {
            ProjectCreator
                .Templates
                .TestingOnlyTemplate(
                    projectFileOptions: NewProjectFileOptions.None,
                    param1: "DE77E0BD69324340926CFEDCAC76B070",
                    param2: "3E21C9488BE840A9846DBE7F6F9E7B19")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Import Project=""DE77E0BD69324340926CFEDCAC76B070"" Condition=""3E21C9488BE840A9846DBE7F6F9E7B19"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }
    }
}