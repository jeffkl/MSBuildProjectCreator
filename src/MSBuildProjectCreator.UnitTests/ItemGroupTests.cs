// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Shouldly;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class ItemGroupTests : MSBuildTestBase
    {
        [Fact]
        public void ItemGroupCondition()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .ItemGroup(condition: "4AEF424E1AFB46DF844A29AC9663329E")
                .Xml
                .ShouldBe(
                    @"<Project>
  <ItemGroup Condition=""4AEF424E1AFB46DF844A29AC9663329E"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void ItemGroupOrder()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .ItemGroup(condition: "C5381C0D244D49CCBCFB512A434D2B9E")
                .PropertyGroup()
                .ItemGroup(condition: "F31A4B051DFB414684BA368068067EEA")
                .ItemGroup(condition: "C8DDD08765A740589ECF938DB7BC5755")
                .Xml
                .ShouldBe(
                    @"<Project>
  <ItemGroup Condition=""C5381C0D244D49CCBCFB512A434D2B9E"" />
  <PropertyGroup />
  <ItemGroup Condition=""F31A4B051DFB414684BA368068067EEA"" />
  <ItemGroup Condition=""C8DDD08765A740589ECF938DB7BC5755"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void ItemGroupSimple()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .ItemGroup()
                .Xml
                .ShouldBe(
                    @"<Project>
  <ItemGroup />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }
    }
}