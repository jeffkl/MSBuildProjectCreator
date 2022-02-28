// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.
/*
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests.PackageRepositoryTests
{
    public class FeedTests : TestBase
    {
        [Fact]
        public void AddFeedString()
        {
            const string uri = "https://custom.org/v3/index.json";

            using PackageRepository packageRepository = PackageRepository.Create(TestRootPath)
                .Feed(uri);

            packageRepository.NuGetConfigPath.ShouldBe(Path.Combine(TestRootPath, "NuGet.Config"));

            string actualPackageSourceUri = EnumeratePackageSourceUris(packageRepository).ShouldHaveSingleItem();

            actualPackageSourceUri.ShouldBe(uri);
        }

        [Fact]
        public void AddFeedUri()
        {
            Uri uri = new Uri("https://custom.org/v3/index.json");

            using PackageRepository packageRepository = PackageRepository.Create(TestRootPath)
                .Feed(uri);

            packageRepository.NuGetConfigPath.ShouldBe(Path.Combine(TestRootPath, "NuGet.Config"));

            PackageSource packageSource = EnumeratePackageSources(packageRepository).ShouldHaveSingleItem();

            packageSource.SourceUri.ShouldBe(uri);
        }

        [Fact]
        public void AddMultipleFeeds()
        {
            const string uri1 = "https://custom1.org/v3/index.json";
            const string uri2 = "https://custom2.org/v3/index.json";

            using PackageRepository packageRepository = PackageRepository.Create(TestRootPath)
                .Feed(uri1)
                .Feed(uri2);

            packageRepository.NuGetConfigPath.ShouldBe(Path.Combine(TestRootPath, "NuGet.Config"));

            EnumeratePackageSourceUris(packageRepository).ShouldBe(new[]
            {
                uri1,
                uri2,
            });
        }

        private IEnumerable<PackageSource> EnumeratePackageSources(PackageRepository packageRepository)
        {
            FileInfo nugetConfigFileInfo = new FileInfo(packageRepository.NuGetConfigPath);

            ISettings settings = Settings.LoadDefaultSettings(nugetConfigFileInfo.DirectoryName, nugetConfigFileInfo.Name, new XPlatMachineWideSetting());

            return PackageSourceProvider.LoadPackageSources(settings).ToList();
        }

        private IEnumerable<string> EnumeratePackageSourceUris(PackageRepository packageRepository)
        {
            return EnumeratePackageSources(packageRepository).Select(i => i.SourceUri.ToString());
        }
    }
}
*/