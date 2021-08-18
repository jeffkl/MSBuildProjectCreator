// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Shouldly;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class SdkTests : MSBuildTestBase
    {
        [Fact]
        public void SdkComplex()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Sdk("3F86C1A37FFF45D698B5E496B0FF2096")
                .Property("D1290ECCFEF44C8A98EF6CBCC4CB2D72", "E4BB6358D96B4C0B926C76D0F1C7BE89")
                .Import("1A3971D93CF74C5EA7C952184159FD8C")
                .Sdk("FC46B7002BC443D8836D3566D76D362D")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Sdk Name=""3F86C1A37FFF45D698B5E496B0FF2096"" />
  <PropertyGroup>
    <D1290ECCFEF44C8A98EF6CBCC4CB2D72>E4BB6358D96B4C0B926C76D0F1C7BE89</D1290ECCFEF44C8A98EF6CBCC4CB2D72>
  </PropertyGroup>
  <Import Project=""1A3971D93CF74C5EA7C952184159FD8C"" />
  <Sdk Name=""FC46B7002BC443D8836D3566D76D362D"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void SdkSimple()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Sdk("8CFA5E8611DB4F12963FF495C43E015D")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Sdk Name=""8CFA5E8611DB4F12963FF495C43E015D"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void SdkWithVersion()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Sdk("9104FA3137FB4581B337262DCBDBAD6A", "AA99A15EABF84EB2A6F23A9495A1D160")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Sdk Name=""9104FA3137FB4581B337262DCBDBAD6A"" Version=""AA99A15EABF84EB2A6F23A9495A1D160"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }
    }
}