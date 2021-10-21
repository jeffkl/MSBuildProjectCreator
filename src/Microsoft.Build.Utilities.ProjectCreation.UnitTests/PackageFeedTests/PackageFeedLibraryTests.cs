// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Frameworks;
using NuGet.Packaging;
using Shouldly;
using System.Reflection;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests.PackageFeedTests
{
    public class PackageFeedLibraryTests : PackageFeedTestBase
    {
        [Fact]
        public void LibraryCustom()
        {
            PackageFeed.Create(FeedRootPath)
                .Package("PackageA", "1.0.0", out Package packageA)
                    .Library("net45", "CustomFile.dll", "Custom.Namespace", "CustomClass", "2.3.4.5")
                .Save();

            ValidateAssembly(packageA, @"lib/net45/CustomFile.dll", "CustomFile, Version=2.3.4.5, Culture=neutral, PublicKeyToken=null", "Custom.Namespace.CustomClass");
        }

        [Theory]
        [InlineData("PackageA")]
        [InlineData("Package.A")]
        public void LibraryDefault(string packageName)
        {
            PackageFeed.Create(FeedRootPath)
                .Package(packageName, "1.0.0", out Package packageA)
                    .Library(FrameworkConstants.CommonFrameworks.Net45.GetShortFolderName())
                .Save();

            ValidateAssembly(packageA, $@"lib/net45/{packageName}.dll", $"{packageName}, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", $"{packageName}.{packageName.Replace(".", "_")}_Class");
        }

        [Fact]
        public void LibraryThrowsIfNoPackageAdded()
        {
            ShouldThrowExceptionIfNoPackageAdded(() =>
            {
                PackageFeed.Create(FeedRootPath)
                    .Library(FrameworkConstants.CommonFrameworks.Net45.GetShortFolderName());
            });
        }

        [Fact]
        public void MultipleTargetFrameworks()
        {
            string[] targetFrameworks =
            {
                FrameworkConstants.CommonFrameworks.Net45.GetShortFolderName(),
                FrameworkConstants.CommonFrameworks.Net46.GetShortFolderName(),
            };

            PackageFeed.Create(FeedRootPath)
                .Package("PackageA", "1.0.0", out Package packageA)
                .ForEach(
                    targetFrameworks,
                    (targetFramework, feed) =>
                    {
                        feed.Library(targetFramework)
                            .ReferenceAssembly(targetFramework);
                    })
                .Save();

            foreach (string targetFramework in targetFrameworks)
            {
                ValidateAssembly(packageA, $@"lib/{targetFramework}/PackageA.dll", "PackageA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "PackageA.PackageA_Class");
                ValidateAssembly(packageA, $@"ref/{targetFramework}/PackageA.dll", "PackageA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "PackageA.PackageA_Class");
            }
        }

        [Fact]
        public void ReferenceAssemblyCustom()
        {
            PackageFeed.Create(FeedRootPath)
                .Package("PackageA", "1.0.0", out Package packageA)
                    .ReferenceAssembly("net45", "CustomFile.dll", "Custom.Namespace", "CustomClass", "2.3.4.5")
                .Save();

            ValidateAssembly(packageA, @"ref/net45/CustomFile.dll", "CustomFile, Version=2.3.4.5, Culture=neutral, PublicKeyToken=null", "Custom.Namespace.CustomClass");
        }

        [Fact]
        public void ReferenceAssemblyDefault()
        {
            PackageFeed.Create(FeedRootPath)
                .Package("PackageA", "1.0.0", out Package packageA)
                    .ReferenceAssembly(FrameworkConstants.CommonFrameworks.Net45.GetShortFolderName())
                .Save();

            ValidateAssembly(packageA, @"ref/net45/PackageA.dll", "PackageA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "PackageA.PackageA_Class");
        }

        [Fact]
        public void ReferenceAssemblyThrowsIfNoPackageAdded()
        {
            ShouldThrowExceptionIfNoPackageAdded(() =>
            {
                PackageFeed.Create(FeedRootPath)
                    .ReferenceAssembly(FrameworkConstants.CommonFrameworks.Net45.GetShortFolderName());
            });
        }

        private void ValidateAssembly(Package package, string filePath, string expectedAssemblyFullName, string expectedTypeFullName)
        {
            Assembly assembly = LoadAssembly(package.FullPath, filePath);

            assembly.FullName.ShouldBe(expectedAssemblyFullName);

            assembly.GetTypes().ShouldHaveSingleItem().FullName.ShouldBe(expectedTypeFullName);
        }
    }
}