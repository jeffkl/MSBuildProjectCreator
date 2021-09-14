// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Packaging;
using Shouldly;
using System.IO;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests.PackageFeedTests
{
    public class PackageFeedTests : PackageFeedTestBase
    {
        [Fact]
        public void BasicPackage()
        {
            using PackageFeed packageFeed = PackageFeed.Create(FeedRootPath)
                .Package("PackageA", "1.0.0", out _, "John Smith", "Custom Description", true)
                    .Library("net45")
                .Save();

            Package packageA = packageFeed.Packages.ShouldHaveSingleItem();

            packageA.Author.ShouldBe("John Smith");
            packageA.Description.ShouldBe("Custom Description");
            packageA.DevelopmentDependency.ShouldBeTrue();
            packageA.Id.ShouldBe("PackageA");
            packageA.Version.OriginalVersion.ShouldBe("1.0.0");

            using PackageArchiveReader reader = GetPackageArchiveReader(packageA);

            reader.NuspecReader.GetAuthors().ShouldBe("John Smith");
            reader.NuspecReader.GetDescription().ShouldBe("Custom Description");
            reader.NuspecReader.GetId().ShouldBe("PackageA");
            reader.NuspecReader.GetVersion().OriginalVersion.ShouldBe("1.0.0");
        }

        [Fact]
        public void Test1()
        {
            string[] targetFrameworks = new[]
            {
                "net45",
                "net46",
                "net5.0",
                "netstandard2.0",
                "netcoreapp3.1",
            };

            using PackageFeed packageFeed = PackageFeed.Create(FeedRootPath)
                .Package("PackageA", "1.0.0", out Package packageA)
                    .FileText("README.md", "This is a test")
                    .ForEach(targetFrameworks, (targetFramework, feed) => feed.Library(targetFramework))
                    .BuildProps(creator => creator.Property("Hello", "World"))
                    .BuildTargets()
                    .BuildMultiTargetingProps()
                    .BuildMultiTargetingTargets()
                    .BuildTransitiveProps()
                    .BuildTransitiveTargets()
                    .ContentFileText("test.txt", "Hello World", "net45")
                    .Dependency("net46", "Newtonsoft.Json", "1.0.0", "All", exclude: "runtime")
                .Save();

            using Stream stream = new FileStream(packageA.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096);
            using PackageArchiveReader reader = new PackageArchiveReader(stream);
        }
    }
}