// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Shouldly;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests.PackageFeedTests
{
    public class PackageFeedTests : PackageFeedTestBase
    {
        [Fact]
        public void BasicPackage()
        {
            using PackageFeed packageFeed = PackageFeed.Create(FeedRootPath)
                .Package("PackageA", "1.0.0", out _, "John Smith", "Custom Description", developmentDependency: true)
                    .Library("net45")
                .Save();

            Package packageA = packageFeed.Packages.ShouldHaveSingleItem();

            packageA.Author.ShouldBe("John Smith");
            packageA.Description.ShouldBe("Custom Description");
            packageA.DevelopmentDependency.ShouldBeTrue();
            packageA.Id.ShouldBe("PackageA");
            packageA.Version.ShouldBe("1.0.0");

            NuspecReader nuspec = GetNuspec(packageA);

            nuspec.Authors.ShouldBe("John Smith");
            nuspec.Description.ShouldBe("Custom Description");
            nuspec.Id.ShouldBe("PackageA");
            nuspec.Version.ShouldBe("1.0.0");

            (string targetFramework, System.Collections.Generic.IEnumerable<PackageDependency> dependencies) = nuspec.DependencyGroups.ToList().ShouldHaveSingleItem();

            targetFramework.ShouldBe("net45");

            dependencies.ShouldBeEmpty();
        }

        [Fact]
        public void BasicPackageWithDependency()
        {
            using PackageFeed packageFeed = PackageFeed.Create(FeedRootPath)
                .Package("PackageA", "1.0.0", out _, "John Smith", "Custom Description", developmentDependency: true)
                    .Library("net45")
                    .Dependency("net45", "PackageB", "2.0.0")
                .Save();

            Package packageA = packageFeed.Packages.ShouldHaveSingleItem();

            packageA.Author.ShouldBe("John Smith");
            packageA.Description.ShouldBe("Custom Description");
            packageA.DevelopmentDependency.ShouldBeTrue();
            packageA.Id.ShouldBe("PackageA");
            packageA.Version.ShouldBe("1.0.0");

            NuspecReader nuspec = GetNuspec(packageA);

            nuspec.Authors.ShouldBe("John Smith");
            nuspec.Description.ShouldBe("Custom Description");
            nuspec.Id.ShouldBe("PackageA");
            nuspec.Version.ShouldBe("1.0.0");

            (string targetFramework, System.Collections.Generic.IEnumerable<PackageDependency> dependencies) = nuspec.DependencyGroups.ToList().ShouldHaveSingleItem();

            targetFramework.ShouldBe("net45");

            PackageDependency dependency = dependencies.ShouldHaveSingleItem();

            dependency.Id.ShouldBe("PackageB");
            dependency.Version.ShouldBe("2.0.0");
            dependency.ExcludeAssets.ShouldBe("Build, Analyzers");
        }

        [Fact]
        public void RestoreCanConsumePackage()
        {
            using PackageFeed packageFeed = PackageFeed.Create(FeedRootPath)
                .Package("PackageA", "1.0.0", out Package packageA)
                    .Library(TargetFramework)
                .Save();

            using PackageRepository packageRepository = PackageRepository.Create(TestRootPath, feeds: packageFeed);

            ProjectCreator.Templates.SdkCsproj(
                        path: Path.Combine(TestRootPath, "ClassLibraryA", "ClassLibraryA.csproj"),
                        targetFramework: TargetFramework)
                    .ItemPackageReference(packageA)
                    .TryRestore(out bool result, out BuildOutput buildOutput);

            result.ShouldBeTrue(buildOutput.GetConsoleLog());
        }
    }
}