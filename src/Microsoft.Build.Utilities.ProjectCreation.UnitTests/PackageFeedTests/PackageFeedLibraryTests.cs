// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Shouldly;
using System.IO;
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

            ValidateAssembly(packageA, Path.Combine("lib", "net45", "CustomFile.dll"), "CustomFile, Version=2.3.4.5, Culture=neutral, PublicKeyToken=null", "Custom.Namespace.CustomClass");
        }

        [Theory]
        [InlineData("PackageA")]
        [InlineData("Package.A")]
        public void LibraryDefault(string packageName)
        {
            PackageFeed.Create(FeedRootPath)
                .Package(packageName, "1.0.0", out Package packageA)
                    .Library("net45")
                .Save();

            ValidateAssembly(packageA, Path.Combine("lib", "net45", $"{packageName}.dll"), $"{packageName}, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", $"{packageName}.{packageName.Replace(".", "_")}_Class");
        }

        [Fact]
        public void LibraryThrowsIfNoPackageAdded()
        {
            ShouldThrowExceptionIfNoPackageAdded(() =>
            {
                PackageFeed.Create(FeedRootPath)
                    .Library("net45");
            });
        }

        [Fact]
        public void MultipleTargetFrameworks()
        {
            string[] targetFrameworks =
            {
                "net45",
                "net46",
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
                ValidateAssembly(packageA, Path.Combine("lib", targetFramework, "PackageA.dll"), "PackageA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "PackageA.PackageA_Class");
                ValidateAssembly(packageA, Path.Combine("ref", targetFramework, "PackageA.dll"), "PackageA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "PackageA.PackageA_Class");
            }
        }

        [Fact]
        public void ReferenceAssemblyCustom()
        {
            PackageFeed.Create(FeedRootPath)
                .Package("PackageA", "1.0.0", out Package packageA)
                    .ReferenceAssembly("net45", "CustomFile.dll", "Custom.Namespace", "CustomClass", "2.3.4.5")
                .Save();

            ValidateAssembly(packageA, Path.Combine("ref", "net45", "CustomFile.dll"), "CustomFile, Version=2.3.4.5, Culture=neutral, PublicKeyToken=null", "Custom.Namespace.CustomClass");
        }

        [Fact]
        public void ReferenceAssemblyDefault()
        {
            PackageFeed.Create(FeedRootPath)
                .Package("PackageA", "1.0.0", out Package packageA)
                    .ReferenceAssembly("net45")
                .Save();

            ValidateAssembly(packageA, Path.Combine("ref", "net45", "PackageA.dll"), "PackageA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "PackageA.PackageA_Class");
        }

        [Fact]
        public void ReferenceAssemblyThrowsIfNoPackageAdded()
        {
            ShouldThrowExceptionIfNoPackageAdded(() =>
            {
                PackageFeed.Create(FeedRootPath)
                    .ReferenceAssembly("net45");
            });
        }

        private void ValidateAssembly(Package package, string filePath, string expectedAssemblyFullName, string expectedTypeFullName)
        {
            Assembly assembly = LoadAssembly(package.FullPath, filePath);

            assembly.FullName.ShouldBe(expectedAssemblyFullName);

            assembly.GetTypes().ShouldContain(i => i.FullName.Equals(expectedTypeFullName));
        }
    }
}