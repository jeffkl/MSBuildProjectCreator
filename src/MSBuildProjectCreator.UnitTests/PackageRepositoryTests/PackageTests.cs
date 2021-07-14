// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
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
        public void BuildCanConsumePackage()
        {
            const string targetFramework =
#if NETCOREAPP3_1
                "netcoreapp3.1";
#elif NETFRAMEWORK
                "net472";
#else
                "net5.0";
#endif
            using (PackageRepository.Create(TestRootPath)
                .Package("PackageB", "1.0", out PackageIdentity packageB)
                    .Library("netstandard2.0")
                .Package("PackageA", "1.0.0", out PackageIdentity packageA)
                    .Dependency(packageB, "netstandard2.0")
                    .Library("netstandard2.0"))
            {
                ProjectCreator.Templates.SdkCsproj(
                        targetFramework: targetFramework)
                    .ItemPackageReference(packageA)
                    .Save(Path.Combine(TestRootPath, "ClassLibraryA", "ClassLibraryA.csproj"))
                    .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

                result.ShouldBeTrue(buildOutput.GetConsoleLog());
            }
        }

        [Fact]
        public void BasicPackage()
        {
            using (PackageRepository packageRepository = PackageRepository.Create(TestRootPath)
                .Package("PackageD", "1.2.3-beta", out PackageIdentity package))
            {
                package.ShouldNotBeNull();

                package.Id.ShouldBe("PackageD");
                package.Version.ShouldBe(NuGetVersion.Parse("1.2.3-beta"));

                FileInfo manifestFilePath = new FileInfo(packageRepository.GetManifestFilePath(package.Id, package.Version)).ShouldExist();

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
        }

        [Fact]
        public void CanSetAllPackageProperties()
        {
            using (PackageRepository packageRepository = PackageRepository.Create(TestRootPath)
                .Package(
                    name: "PackageD",
                    version: "1.2.3",
                    package: out PackageIdentity package,
                    authors: "UserA;UserB",
                    description: "Custom description",
                    copyright: "Copyright 2000",
                    developmentDependency: true,
                    icon: Path.Combine("some", "icon.jpg"),
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
                    title: "Title of PackageD"))
            {
                package.ShouldNotBeNull();

                package.Id.ShouldBe("PackageD");
                package.Version.ShouldBe(NuGetVersion.Parse("1.2.3"));

                FileInfo manifestFilePath = new FileInfo(packageRepository.GetManifestFilePath(package.Id, package.Version)).ShouldExist();

                using (Stream stream = File.OpenRead(manifestFilePath.FullName))
                {
                    Manifest manifest = Manifest.ReadFrom(stream, validateSchema: true);

                    manifest.Metadata.Authors.ShouldBe(new[] { "UserA", "UserB" });
                    manifest.Metadata.Copyright.ShouldBe("Copyright 2000");
                    manifest.Metadata.Description.ShouldBe("Custom description");
                    manifest.Metadata.DevelopmentDependency.ShouldBeTrue();
                    manifest.Metadata.Icon.ShouldBe(Path.Combine("some", "icon.jpg"));
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

        [Fact]
        public void CanUseNuGetSdkResolver()
        {
            using (ProjectCollection projectCollection = new ProjectCollection())
            {
                BuildOutput buildOutput = BuildOutput.Create();

                projectCollection.RegisterLogger(buildOutput);

                using (PackageRepository.Create(TestRootPath)
                    .Package("Foo.Bar", "1.2.3", out PackageIdentity package)
                    .FileText(Path.Combine("Sdk", "Sdk.props"), "<Project />")
                    .FileText(Path.Combine("Sdk", "Sdk.targets"), "<Project />"))
                {
                    ProjectCreator projectCreator = ProjectCreator
                        .Create(
                            sdk: $"{package.Id}/{package.Version}",
                            projectCollection: projectCollection)
                        .Save(GetTempFileName(".csproj"));

                    try
                    {
                        Project unused = projectCreator.Project;
                    }
                    catch (Exception e)
                    {
                        throw new Exception(buildOutput.GetConsoleLog(), e);
                    }
                }
            }
        }
    }
}