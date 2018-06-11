// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Shouldly;
using System;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class TargetTests : MSBuildTestBase
    {
        [Fact]
        public void TargetComplex()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Target(
                    name: "3B2288C3ACA5460CBEEEC3EB914787EA",
                    condition: "84FB6783A56748AEA7024F648E520B5D",
                    afterTargets: "2C8FF9B75080400D920A8D5AF553AC65",
                    beforeTargets: "F04B583BD2B94B40A926568E27A056AA",
                    dependsOnTargets: "B7D5486A843348849ABBEF2A8161A37E",
                    inputs: "B7419E0377CE4FA6AEFCCFB3F69777FC",
                    outputs: "DA3C0A891FFD4D6CA0B883258E87D0CA",
                    returns: "D3091D945AF7479A8958BA05954626B2",
                    keepDuplicateOutputs: true)
                .Xml
                .ShouldBe(
                    @"<Project>
  <Target Name=""3B2288C3ACA5460CBEEEC3EB914787EA"" AfterTargets=""2C8FF9B75080400D920A8D5AF553AC65"" BeforeTargets=""F04B583BD2B94B40A926568E27A056AA"" Condition=""84FB6783A56748AEA7024F648E520B5D"" DependsOnTargets=""B7D5486A843348849ABBEF2A8161A37E"" Inputs=""B7419E0377CE4FA6AEFCCFB3F69777FC"" Outputs=""DA3C0A891FFD4D6CA0B883258E87D0CA"" Returns=""D3091D945AF7479A8958BA05954626B2"" KeepDuplicateOutputs=""True"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void TargetItemGroupOrder()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Target("175A0369651C48898794E469E86B662B")
                .TargetItemInclude("DE9A24B7C50448EDB7A5C61F4427EF72", "21C55C2DBCA04DE5BA3EEB9C2C824E80")
                .TargetItemGroup()
                .TargetItemInclude("E61F210199E74A65992BED10269D9140", "37EB1736F31E4EF6A5E8AF885C015BD7")
                .TargetItemGroup("6B08026617014A33A80D628FAEEFFC93")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Target Name=""175A0369651C48898794E469E86B662B"">
    <ItemGroup>
      <DE9A24B7C50448EDB7A5C61F4427EF72 Include=""21C55C2DBCA04DE5BA3EEB9C2C824E80"" />
    </ItemGroup>
    <ItemGroup>
      <E61F210199E74A65992BED10269D9140 Include=""37EB1736F31E4EF6A5E8AF885C015BD7"" />
    </ItemGroup>
    <ItemGroup Condition=""6B08026617014A33A80D628FAEEFFC93"" />
  </Target>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("85391B763FDB461C990A513E89813192")]
        public void TargetItemGroupSimple(string condition)
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .TargetItemGroup(condition)
                .Xml
                .ShouldBe(
                    $@"<Project>
  <Target Name=""{ProjectCreatorConstants.DefaultTargetName}"">
    <ItemGroup{(condition == null ? String.Empty : $@" Condition=""{condition}""")} />
  </Target>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void TargetItemIncludeSimple()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .TargetItemInclude("B35B5ED7AB7D449C99EDEBBF22580DBD", "9E9A83EB48C749B096EBD30268313749")
                .Xml
                .ShouldBe(
                    $@"<Project>
  <Target Name=""{ProjectCreatorConstants.DefaultTargetName}"">
    <ItemGroup>
      <B35B5ED7AB7D449C99EDEBBF22580DBD Include=""9E9A83EB48C749B096EBD30268313749"" />
    </ItemGroup>
  </Target>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void TargetPropertySimple()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Target("042C87104A064EA5A46F9F483F10251A")
                .TargetProperty("B0196D57D59A4F668B4764BC17A1F47C", "1F313116AF124FDDA6245E799A6E600C")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Target Name=""042C87104A064EA5A46F9F483F10251A"">
    <PropertyGroup>
      <B0196D57D59A4F668B4764BC17A1F47C>1F313116AF124FDDA6245E799A6E600C</B0196D57D59A4F668B4764BC17A1F47C>
    </PropertyGroup>
  </Target>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void TargetSimple()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Target("4C03DF113ECA46C4896CD199F4F41CB6")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Target Name=""4C03DF113ECA46C4896CD199F4F41CB6"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void TargetWithOnError()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Target("80A389DEE56C4DFE9D81CCC9B176C09E")
                .TargetOnError("CA51741A444D4B718A83E2364FE7DC98", "2F8BF0791F8F45AFB2F8D9338ECFDBA1")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Target Name=""80A389DEE56C4DFE9D81CCC9B176C09E"">
    <OnError ExecuteTargets=""CA51741A444D4B718A83E2364FE7DC98"" Condition=""2F8BF0791F8F45AFB2F8D9338ECFDBA1"" />
  </Target>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }
    }
}