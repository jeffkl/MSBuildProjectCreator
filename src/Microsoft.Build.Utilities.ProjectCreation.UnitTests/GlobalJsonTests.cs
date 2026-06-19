// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Shouldly;
using System;
using System.IO;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class GlobalJsonTests : TestBase
    {
        public GlobalJsonTests()
        {
            TestDirectory = new DirectoryInfo(Path.Combine(TestRootPath, Path.GetRandomFileName()));
        }

        public DirectoryInfo TestDirectory { get; }

        [Fact]
        public void BasicGlobalJson()
        {
            GlobalJsonCreator globalJson = GlobalJsonCreator.Create(TestDirectory, "8.0.100")
                .Save();

            globalJson.FullPath.ShouldNotBeNull();

            new FileInfo(globalJson.FullPath).ShouldExist();

            string json = File.ReadAllText(globalJson.FullPath);

            json.ShouldBe(
@"{
  ""sdk"": {
    ""version"": ""8.0.100""
  }
}",
StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void GlobalJsonCreateWithoutDirectoryCanSaveToDirectoryInfo()
        {
            GlobalJsonCreator globalJson = GlobalJsonCreator
                .Create(sdkVersion: "10.0.100")
                .Save(TestDirectory);

            globalJson.FullPath.ShouldBe(Path.Combine(TestDirectory.FullName, "global.json"));
            new FileInfo(globalJson.FullPath!).ShouldExist();
        }

        [Fact]
        public void GlobalJsonCreateWithoutDirectoryCanSaveToStringPath()
        {
            GlobalJsonCreator globalJson = GlobalJsonCreator
                .Create(sdkVersion: "10.0.100")
                .Save(TestDirectory.FullName);

            globalJson.FullPath.ShouldBe(Path.Combine(TestDirectory.FullName, "global.json"));
            new FileInfo(globalJson.FullPath!).ShouldExist();
        }

        [Fact]
        public void GlobalJsonImplicitConversionToString()
        {
            string json = GlobalJsonCreator
                .Create(TestDirectory, "8.0.100");

            json.ShouldBe(
@"{
  ""sdk"": {
    ""version"": ""8.0.100""
  }
}",
StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void GlobalJsonSaveWithoutDirectoryThrows()
        {
            Should.Throw<InvalidOperationException>(
                () => GlobalJsonCreator.Create().Save());
        }

        [Fact]
        public void GlobalJsonSdkRollForwardEnumValuesAreStable()
        {
            ((int)GlobalJsonSdkRollForward.LatestPatch).ShouldBe(0);
            ((int)GlobalJsonSdkRollForward.LatestFeature).ShouldBe(1);
            ((int)GlobalJsonSdkRollForward.LatestMinor).ShouldBe(2);
            ((int)GlobalJsonSdkRollForward.LatestMajor).ShouldBe(3);
            ((int)GlobalJsonSdkRollForward.Disable).ShouldBe(4);
            ((int)GlobalJsonSdkRollForward.Patch).ShouldBe(5);
            ((int)GlobalJsonSdkRollForward.Feature).ShouldBe(6);
            ((int)GlobalJsonSdkRollForward.Minor).ShouldBe(7);
            ((int)GlobalJsonSdkRollForward.Major).ShouldBe(8);
        }

        [Fact]
        public void GlobalJsonToJson()
        {
            string json = GlobalJsonCreator
                .Create(TestDirectory, "8.0.100", rollForward: GlobalJsonSdkRollForward.LatestMinor)
                .ToJson();

            json.ShouldBe(
@"{
  ""sdk"": {
    ""version"": ""8.0.100"",
    ""rollForward"": ""latestMinor""
  }
}",
StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void GlobalJsonWithAllowPrerelease()
        {
            GlobalJsonCreator globalJson = GlobalJsonCreator
                .Create(TestDirectory, "8.0.100", allowPrerelease: true)
                .Save();

            globalJson.FullPath.ShouldNotBeNull();

            new FileInfo(globalJson.FullPath).ShouldExist();

            string json = File.ReadAllText(globalJson.FullPath);

            json.ShouldBe(
@"{
  ""sdk"": {
    ""version"": ""8.0.100"",
    ""allowPrerelease"": true
  }
}",
StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void GlobalJsonWithMsbuildSdks()
        {
            GlobalJsonCreator globalJson = GlobalJsonCreator
                .Create(TestDirectory, "8.0.100")
                .MSBuildSdk("Microsoft.Build.NoTargets", "3.7.0")
                .MSBuildSdk("Microsoft.Build.Traversal", "4.1.0")
                .Save();

            globalJson.FullPath.ShouldNotBeNull();

            new FileInfo(globalJson.FullPath).ShouldExist();

            string json = File.ReadAllText(globalJson.FullPath);

            json.ShouldBe(
@"{
  ""sdk"": {
    ""version"": ""8.0.100""
  },
  ""msbuild-sdks"": {
    ""Microsoft.Build.NoTargets"": ""3.7.0"",
    ""Microsoft.Build.Traversal"": ""4.1.0""
  }
}",
StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void GlobalJsonWithRollForward()
        {
            GlobalJsonCreator globalJson = GlobalJsonCreator
                .Create(TestDirectory, "10.0.100", GlobalJsonSdkRollForward.LatestMinor)
                .Save();

            globalJson.FullPath.ShouldNotBeNull();

            new FileInfo(globalJson.FullPath).ShouldExist();

            string json = File.ReadAllText(globalJson.FullPath);

            json.ShouldBe(
@"{
  ""sdk"": {
    ""version"": ""10.0.100"",
    ""rollForward"": ""latestMinor""
  }
}",
StringCompareShould.IgnoreLineEndings);
        }

        [Theory]
        [InlineData(GlobalJsonSdkRollForward.Patch, "patch")]
        [InlineData(GlobalJsonSdkRollForward.Feature, "feature")]
        [InlineData(GlobalJsonSdkRollForward.Minor, "minor")]
        [InlineData(GlobalJsonSdkRollForward.Major, "major")]
        [InlineData(GlobalJsonSdkRollForward.LatestPatch, "latestPatch")]
        [InlineData(GlobalJsonSdkRollForward.LatestFeature, "latestFeature")]
        [InlineData(GlobalJsonSdkRollForward.LatestMinor, "latestMinor")]
        [InlineData(GlobalJsonSdkRollForward.LatestMajor, "latestMajor")]
        [InlineData(GlobalJsonSdkRollForward.Disable, "disable")]
        public void GlobalJsonWithRollForwardValues(GlobalJsonSdkRollForward rollForward, string expectedRollForwardValue)
        {
            string json = GlobalJsonCreator
                .Create(TestDirectory, "10.0.100", rollForward)
                .ToJson();

            json.ShouldBe(
$@"{{
  ""sdk"": {{
    ""version"": ""10.0.100"",
    ""rollForward"": ""{expectedRollForwardValue}""
  }}
}}",
StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void GlobalJsonWithSdkErrorMessagePathsAndTestRunner()
        {
            string json = GlobalJsonCreator
                .Create(sdkVersion: "10.0.100")
                .SdkErrorMessage("Install .NET SDK 10.0.100")
                .SdkPath("$host$")
                .SdkPath(".dotnet")
                .TestRunner("vstest")
                .ToJson();

            json.ShouldBe(
@"{
  ""sdk"": {
    ""version"": ""10.0.100"",
    ""paths"": [
      ""$host$"",
      "".dotnet""
    ],
    ""errorMessage"": ""Install .NET SDK 10.0.100""
  },
  ""test"": {
    ""runner"": ""vstest""
  }
}",
StringCompareShould.IgnoreLineEndings);
        }
    }
}
