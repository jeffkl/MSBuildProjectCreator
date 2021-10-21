// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using Shouldly;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests.PackageFeedTests
{
    public class PackageFeedFileTests : PackageFeedTestBase
    {
        [Fact]
        public void ContentFileTextCustom()
        {
            const string language = "cs";

            PackageFeed.Create(FeedRootPath)
                .Package("PackageA", "1.0.0", out Package packageA)
                .ContentFileText("file.cs", "A1CF42B9F20B4155B6B70753784615B5", "netstandard2.0", "Compile", copyToOutput: false, flatten: true, language)
                .Save();

            VerifyContentFile(
                packageA,
                "file.cs",
                "A1CF42B9F20B4155B6B70753784615B5",
                FrameworkConstants.CommonFrameworks.NetStandard20,
                expectedBuildAction: "Compile",
                expectedCopyToOutput: false,
                expectedFlatten: true,
                expectedLanguage: language);
        }

        [Fact]
        public void ContentFileTextDefault()
        {
            PackageFeed.Create(FeedRootPath)
                .Package("PackageA", "1.0.0", out Package packageA)
                    .ContentFileText("file.txt", "F1BE6E0E408141459C9728FBE0CD5751", "net45")
                .Save();

            VerifyContentFile(
                packageA,
                "file.txt",
                "F1BE6E0E408141459C9728FBE0CD5751",
                FrameworkConstants.CommonFrameworks.Net45,
                expectedBuildAction: "Content",
                expectedCopyToOutput: true);
        }

        [Fact]
        public void FileCustom()
        {
            string fileName = $"{Guid.NewGuid():N}.txt";

            FileInfo fileInfo = new FileInfo(Path.Combine(TestRootPath, fileName));

            File.WriteAllText(fileInfo.FullName, "585B55DD5AC54A10B841B3D9A00129D8");

            PackageFeed.Create(FeedRootPath)
                .Package("PackageA", "1.0.0", out Package packageA)
                    .FileCustom(Path.Combine("tools", "net46", fileName), fileInfo)
                .Save();

            GetFileContents(packageA.FullPath, $"tools/net46/{fileName}")
                .ShouldBe("585B55DD5AC54A10B841B3D9A00129D8");

            GetNuspecReader(packageA)
                .GetDependencyGroups().Select(i => i.TargetFramework.GetShortFolderName()).ToList()
                .ShouldContain("net46");
        }

        [Fact]
        public void FileCustomDoesNotExist()
        {
            Should.Throw<FileNotFoundException>(() =>
            {
                FileInfo fileInfo = new FileInfo(Path.Combine(TestRootPath, "foo.txt"));

                PackageFeed.Create(FeedRootPath)
                    .Package("PackageA", "1.0.0")
                        .FileCustom("foo.txt", fileInfo)
                    .Save();
            });
        }

        [Fact]
        public void FileText()
        {
            PackageFeed.Create(FeedRootPath)
                .Package("PackageA", "1.0.0", out Package packageA)
                    .FileText(@"something\nothing.txt", "607779BADE3645F8A288543213BFE948")
                .Save();

            GetFileContents(packageA.FullPath, "something/nothing.txt")
                .ShouldBe("607779BADE3645F8A288543213BFE948");

            GetNuspecReader(packageA)
                .GetDependencyGroups()
                .ShouldBeEmpty();
        }

        [Fact]
        public void FileTextThrowsIfNoPackageAdded()
        {
            ShouldThrowExceptionIfNoPackageAdded(() =>
            {
                PackageFeed.Create(FeedRootPath)
                    .FileText("something.txt", string.Empty);
            });
        }

        private void VerifyContentFile(Package package, string relativePath, string expectedContents, NuGetFramework expectedTargetFramework, string expectedExclude = null, string expectedBuildAction = null, bool expectedCopyToOutput = false, bool expectedFlatten = false, string expectedLanguage = "any")
        {
            GetFileContents(package.FullPath, $"contentFiles/{expectedLanguage}/{expectedTargetFramework.GetShortFolderName()}/{relativePath}").ShouldBe(expectedContents);

            NuspecReader nuspecReader = GetNuspecReader(package);

            ContentFilesEntry file = nuspecReader.GetContentFiles().ShouldHaveSingleItem();

            file.BuildAction.ShouldBe(expectedBuildAction, StringCompareShould.IgnoreCase);

            if (expectedCopyToOutput)
            {
                file.CopyToOutput.ShouldNotBeNull().ShouldBeTrue();
            }
            else
            {
                file.CopyToOutput.ShouldNotBeNull().ShouldBeFalse();
            }

            file.Include.ShouldBe(Path.Combine(expectedLanguage, expectedTargetFramework.GetShortFolderName(), relativePath));

            if (expectedExclude == null)
            {
                file.Exclude.ShouldBeNull();
            }
            else
            {
                file.Exclude.ShouldBe(expectedExclude);
            }

            if (expectedFlatten)
            {
                file.Flatten.ShouldNotBeNull().ShouldBeTrue();
            }
            else
            {
                file.Flatten.ShouldNotBeNull().ShouldBeFalse();
            }
        }
    }
}