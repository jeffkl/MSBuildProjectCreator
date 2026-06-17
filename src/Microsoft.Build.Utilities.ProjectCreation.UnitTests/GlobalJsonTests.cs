// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Shouldly;
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
    }
}
