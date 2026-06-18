// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Shouldly;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class DirectoryPackagesPropsTests : TestBase
    {
        [Fact]
        public void Default()
        {
            ProjectCreator creator = ProjectCreator.Templates.DirectoryPackagesProps();

            creator.Xml.ShouldBe(
@"<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
</Project>",
StringCompareShould.IgnoreLineEndings);

            creator.FullPath.ShouldBe(Path.Combine(Directory.GetCurrentDirectory(), "Directory.Packages.props"));
        }

        [Fact]
        public void PackageVersions()
        {
            ProjectCreator creator = ProjectCreator.Templates.DirectoryPackagesProps(
                    new Dictionary<string, string>
                    {
                        ["Newtonsoft.Json"] = "13.0.1",
                        ["Serilog"] = "3.1.0",
                    },
                    directory: TestRootPath);

            creator.Xml.ShouldBe(
@"<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <PackageVersion Include=""Newtonsoft.Json"">
      <Version>13.0.1</Version>
    </PackageVersion>
    <PackageVersion Include=""Serilog"">
      <Version>3.1.0</Version>
    </PackageVersion>
  </ItemGroup>
</Project>",
StringCompareShould.IgnoreLineEndings);

            creator.FullPath.ShouldBe(Path.Combine(TestRootPath, "Directory.Packages.props"));
        }
    }
}
