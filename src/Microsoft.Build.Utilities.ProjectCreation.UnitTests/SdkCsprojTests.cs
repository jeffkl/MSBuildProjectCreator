// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class SdkCsprojTests : TestBase
    {
        [Fact]
        public void CanBuild()
        {
            ProjectCreator.Templates.SdkCsproj(
                    targetFramework: TargetFramework,
                    path: GetTempFileName(".csproj"))
                .Save()
                .TryBuild(restore: true, "Build", out bool result, out BuildOutput buildOutput);

            result.ShouldBeTrue(string.Join(Environment.NewLine, AppDomain.CurrentDomain.GetAssemblies().Where(i => !i.IsDynamic && !string.IsNullOrEmpty(i.Location)).OrderBy(i => i.FullName).Select(i => $"{i.FullName} / {i.Location}")) + Environment.NewLine + buildOutput.GetConsoleLog());
        }

        [Fact]
        public void CustomSdk()
        {
            const string sdk = "Foo/1.2.3";

            ProjectCreator.Templates.SdkCsproj(sdk: sdk)
                .Xml
                .ShouldBe(
                    $@"<Project Sdk=""{sdk}"">
  <PropertyGroup>
    <TargetFramework>{ProjectCreatorConstants.SdkCsprojDefaultTargetFramework}</TargetFramework>
  </PropertyGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void Default()
        {
            ProjectCreator.Templates.SdkCsproj()
                .Xml
                .ShouldBe(
                    $@"<Project Sdk=""{ProjectCreatorConstants.SdkCsprojDefaultSdk}"">
  <PropertyGroup>
    <TargetFramework>{ProjectCreatorConstants.SdkCsprojDefaultTargetFramework}</TargetFramework>
  </PropertyGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void TargetFrameworksNullDoesNothing()
        {
            ProjectCreator.Templates.SdkCsproj(
                    targetFrameworks: null)
                .Xml
                .ShouldBe(
                    $@"<Project Sdk=""{ProjectCreatorConstants.SdkCsprojDefaultSdk}"">
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void TargetFrameworksWithMultipleValues()
        {
            ProjectCreator.Templates.SdkCsproj(
                    targetFrameworks: new[]
                    {
                        "86A865B7391B4AFBA7466B3882CB21BD",
                        "475340B7B92C4E35A9503B747996F5F6",
                        "9266B04DA648433F9BB76BBF42474545",
                    })
                .Xml
                .ShouldBe(
                    $@"<Project Sdk=""{ProjectCreatorConstants.SdkCsprojDefaultSdk}"">
  <PropertyGroup>
    <TargetFrameworks>86A865B7391B4AFBA7466B3882CB21BD;475340B7B92C4E35A9503B747996F5F6;9266B04DA648433F9BB76BBF42474545</TargetFrameworks>
  </PropertyGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void TargetFrameworksWithSingleValue()
        {
            ProjectCreator.Templates.SdkCsproj(
                    targetFrameworks: new[]
                    {
                        "8A683D24C9CE489C804C79897BC1A44C",
                    })
                .Xml
                .ShouldBe(
                    $@"<Project Sdk=""{ProjectCreatorConstants.SdkCsprojDefaultSdk}"">
  <PropertyGroup>
    <TargetFramework>8A683D24C9CE489C804C79897BC1A44C</TargetFramework>
  </PropertyGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }
    }
}