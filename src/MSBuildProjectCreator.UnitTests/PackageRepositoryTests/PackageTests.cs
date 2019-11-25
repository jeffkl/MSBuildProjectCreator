// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests.PackageRepositoryTests
{
    public class PackageTests : TestBase
    {
        [Fact]
        public void BasicPackage()
        {
            PackageRepository.Create(TestRootPath)
                .Package("PackageD", "1.2.3-beta", out PackageIdentity package);

            package.ShouldNotBeNull();

            package.Id.ShouldBe("PackageD");
            package.Version.ShouldBe(NuGetVersion.Parse("1.2.3-beta"));

            FileInfo manifestFilePath = new FileInfo(VersionFolderPathResolver.GetManifestFilePath(package.Id, package.Version))
                .ShouldExist();

            using (Stream stream = File.OpenRead(manifestFilePath.FullName))
            {
                Manifest manifest = Manifest.ReadFrom(stream, validateSchema: true);

                manifest.Metadata.Authors.ShouldBe(new[] { "UserA" });
                manifest.Metadata.Description.ShouldBe("Description");
                manifest.Metadata.DevelopmentDependency.ShouldBeFalse();
                manifest.Metadata.Id.ShouldBe("PackageD");
                manifest.Metadata.RequireLicenseAcceptance.ShouldBeFalse();
            }
        }

        [Fact]
        public void CanSetAllPackageProperties()
        {
            PackageRepository.Create(TestRootPath)
                .Package(
                    name: "PackageD",
                    version: "1.2.3",
                    package: out PackageIdentity package,
                    authors: "UserA;UserB",
                    description: "Custom description",
                    copyright: "Copyright 2000",
                    developmentDependency: true,
#if !NET46
                    icon: @"some\icon.jpg",
#endif
                    iconUrl: "https://icon.url",
                    language: "Pig latin",
                    licenseMetadata: new LicenseMetadata(LicenseType.Expression, "MIT", null, null, Version.Parse("1.0.0")),
                    owners: "Owner1;Owner2",
                    packageTypes: new List<PackageType> { PackageType.Dependency, PackageType.DotnetCliTool },
                    projectUrl: "https://project.url",
                    releaseNotes: "Release notes for PackageD",
                    repositoryType: "Git",
                    repositoryUrl: "https://repository.url",
                    repositoryBranch: "Branch1000",
                    repositoryCommit: "Commit14",
                    requireLicenseAcceptance: true,
                    serviceable: true,
                    summary: "Summary of PackageD",
                    tags: "Tag1 Tag2 Tag3",
                    title: "Title of PackageD");

            package.ShouldNotBeNull();

            package.Id.ShouldBe("PackageD");
            package.Version.ShouldBe(NuGetVersion.Parse("1.2.3"));

            FileInfo manifestFilePath = new FileInfo(VersionFolderPathResolver.GetManifestFilePath(package.Id, package.Version))
                .ShouldExist();

            using (Stream stream = File.OpenRead(manifestFilePath.FullName))
            {
                Manifest manifest = Manifest.ReadFrom(stream, validateSchema: true);

                manifest.Metadata.Authors.ShouldBe(new[] { "UserA", "UserB" });
                manifest.Metadata.Copyright.ShouldBe("Copyright 2000");
                manifest.Metadata.Description.ShouldBe("Custom description");
                manifest.Metadata.DevelopmentDependency.ShouldBeTrue();
#if !NET46
                manifest.Metadata.Icon.ShouldBe(@"some\icon.jpg");
#endif
                manifest.Metadata.IconUrl.ShouldBe(new Uri("https://icon.url"));
                manifest.Metadata.Id.ShouldBe("PackageD");
                manifest.Metadata.Language.ShouldBe("Pig latin");
                manifest.Metadata.LicenseMetadata.License.ShouldBe("MIT");
                manifest.Metadata.LicenseMetadata.Type.ShouldBe(LicenseType.Expression);
                manifest.Metadata.LicenseMetadata.Version.ShouldBe(Version.Parse("1.0.0"));
                manifest.Metadata.Owners.ShouldBe(new[] { "Owner1", "Owner2" });
                manifest.Metadata.PackageTypes.ShouldBe(new[] { PackageType.Dependency, PackageType.DotnetCliTool });
                manifest.Metadata.ProjectUrl.ShouldBe(new Uri("https://project.url"));
                manifest.Metadata.ReleaseNotes.ShouldBe("Release notes for PackageD");
                manifest.Metadata.Repository.Branch.ShouldBe("Branch1000");
                manifest.Metadata.Repository.Commit.ShouldBe("Commit14");
                manifest.Metadata.Repository.Type.ShouldBe("Git");
                manifest.Metadata.Repository.Url.ShouldBe("https://repository.url");
                manifest.Metadata.RequireLicenseAcceptance.ShouldBeTrue();
                manifest.Metadata.Serviceable.ShouldBeTrue();
                manifest.Metadata.Summary.ShouldBe("Summary of PackageD");
                manifest.Metadata.Tags.ShouldBe("Tag1 Tag2 Tag3");
                manifest.Metadata.Title.ShouldBe("Title of PackageD");
            }
        }
    }
}