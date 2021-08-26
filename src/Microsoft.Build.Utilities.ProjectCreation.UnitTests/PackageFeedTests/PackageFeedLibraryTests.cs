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

        [Fact]
        public void LibraryDefault()
        {
            PackageFeed.Create(FeedRootPath)
                .Package("PackageA", "1.0.0", out Package packageA)
                    .Library(FrameworkConstants.CommonFrameworks.Net45)
                .Save();

            ValidateAssembly(packageA, @"lib/net45/PackageA.dll", "PackageA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "PackageA.PackageA_Class");
        }

        [Fact]
        public void LibraryThrowsIfNoPackageAdded()
        {
            ShouldThrowExceptionIfNoPackageAdded(() =>
            {
                PackageFeed.Create(FeedRootPath)
                    .Library(FrameworkConstants.CommonFrameworks.Net45);
            });
        }

        [Fact]
        public void MultipleTargetFrameworks()
        {
            NuGetFramework[] targetFrameworks =
            {
                FrameworkConstants.CommonFrameworks.Net45,
                FrameworkConstants.CommonFrameworks.Net46,
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

            foreach (NuGetFramework targetFramework in targetFrameworks)
            {
                ValidateAssembly(packageA, $@"lib/{targetFramework.GetShortFolderName()}/PackageA.dll", "PackageA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "PackageA.PackageA_Class");
                ValidateAssembly(packageA, $@"ref/{targetFramework.GetShortFolderName()}/PackageA.dll", "PackageA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "PackageA.PackageA_Class");
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
                    .ReferenceAssembly(FrameworkConstants.CommonFrameworks.Net45)
                .Save();

            ValidateAssembly(packageA, @"ref/net45/PackageA.dll", "PackageA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "PackageA.PackageA_Class");
        }

        [Fact]
        public void ReferenceAssemblyThrowsIfNoPackageAdded()
        {
            ShouldThrowExceptionIfNoPackageAdded(() =>
            {
                PackageFeed.Create(FeedRootPath)
                    .ReferenceAssembly(FrameworkConstants.CommonFrameworks.Net45);
            });
        }

        private void ValidateAssembly(Package package, string filePath, string expectedAssemblyFullName, string expectedTypeFullName)
        {
            PackageArchiveReader packageArchiveReader = GetPackageArchiveReader(package);

            Assembly assembly = LoadAssembly(packageArchiveReader, filePath);

            assembly.FullName.ShouldBe(expectedAssemblyFullName);

            assembly.GetTypes().ShouldHaveSingleItem().FullName.ShouldBe(expectedTypeFullName);
        }
    }
}