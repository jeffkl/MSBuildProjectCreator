// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Shouldly;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class SdkCsprojTests : TestBase
    {
        private readonly ITestOutputHelper _testOutput;

        public SdkCsprojTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public void CanBuild()
        {
            foreach (DictionaryEntry dictionaryEntry in Environment.GetEnvironmentVariables().Cast<DictionaryEntry>().OrderBy(i => (string)i.Key))
            {
                _testOutput.WriteLine($"{dictionaryEntry.Key}={dictionaryEntry.Value}");
            }

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies().Where(i => !i.IsDynamic).OrderBy(i => i.FullName))
            {
                _testOutput.WriteLine($"{assembly.FullName} / {assembly.Location}");
            }

            const string targetFramework = "net472";

            ProjectCreator.Templates.SdkCsproj(
                    targetFramework: targetFramework,
                    path: GetTempFileName(".csproj"))
                .Save()
                .TryBuild(restore: true, "Build", out bool result, out BuildOutput buildOutput);

            result.ShouldBeTrue(buildOutput.GetConsoleLog());
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
        public void TargetFrameworks()
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
    }
}