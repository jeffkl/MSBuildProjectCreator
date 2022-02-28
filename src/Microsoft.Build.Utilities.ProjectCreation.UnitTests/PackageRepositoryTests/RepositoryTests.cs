// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests.PackageRepositoryTests
{
    public class RepositoryTests : TestBase
    {
        [Fact]
        public void BasicPackage()
        {
            using (PackageRepository packageRepository = PackageRepository.Create(TestRootPath)
                .Package("PackageD", "1.2.3-beta", out Package package)
                    .Library("net45"))
            {
                package.ShouldNotBeNull();

                package.Id.ShouldBe("PackageD");
                package.Version.ShouldBe("1.2.3-beta");

                FileInfo nuspecFileInfo = new FileInfo(packageRepository.GetManifestFilePath(package.Id, package.Version)).ShouldExist();

                NuspecReader nuspec = new NuspecReader(nuspecFileInfo);

                nuspec.Authors.ShouldBe("UserA");
                nuspec.Description.ShouldBe("Description");
                nuspec.DevelopmentDependency.ShouldBeFalse();
                nuspec.Id.ShouldBe("PackageD");
                nuspec.RequireLicenseAcceptance.ShouldBeFalse();
            }
        }

        [Fact]
        public void BuildCanConsumePackage()
        {
            using (PackageRepository.Create(TestRootPath)
                .Package("PackageB", "1.0", out Package packageB)
                    .Library(TargetFramework)
                .Package("PackageA", "1.0.0", out Package packageA)
                    .Dependency(packageB, TargetFramework)
                    .Library(TargetFramework))
            {
                ProjectCreator.Templates.SdkCsproj(
                        path: Path.Combine(TestRootPath, "ClassLibraryA", "ClassLibraryA.csproj"),
                        targetFramework: TargetFramework)
                    .ItemPackageReference(packageA)
                    .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);

                result.ShouldBeTrue(buildOutput.GetConsoleLog());
            }
        }

        [Fact]
        public void CanSetAllPackageProperties()
        {
            using (PackageRepository packageRepository = PackageRepository.Create(TestRootPath)
                .Package(
                    id: "PackageD",
                    version: "1.2.3",
                    out Package package,
                    authors: "UserA;UserB",
                    description: "Custom description",
                    copyright: "Copyright 2000",
                    developmentDependency: true,
                    icon: Path.Combine("some", "icon.jpg"),
                    iconUrl: "https://icon.url",
                    language: "Pig latin",
                    licenseExpression: "MIT",
                    licenseVersion: "1.0.0",
                    owners: "Owner1;Owner2",
                    packageTypes: new List<string> { "Dependency", "DotnetCliTool" },
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
                package.Version.ShouldBe("1.2.3");

                FileInfo nuspecFileInfo = new FileInfo(packageRepository.GetManifestFilePath(package.Id, package.Version)).ShouldExist();

                NuspecReader nuspec = new NuspecReader(nuspecFileInfo);

                nuspec.Authors.ShouldBe("UserA;UserB");
                nuspec.Copyright.ShouldBe("Copyright 2000");
                nuspec.Description.ShouldBe("Custom description");
                nuspec.DevelopmentDependency.ShouldBeTrue();
                nuspec.Icon.ShouldBe(Path.Combine("some", "icon.jpg"));
                nuspec.IconUrl.ShouldBe("https://icon.url");
                nuspec.Id.ShouldBe("PackageD");
                nuspec.Language.ShouldBe("Pig latin");
                nuspec.License.ShouldBe("MIT");
                nuspec.LicenseType.ShouldBe("expression");
                nuspec.LicenseVersion.ShouldBe("1.0.0");
                nuspec.Owners.ShouldBe("Owner1;Owner2");
                nuspec.PackageTypes.ShouldBe(new[] { "Dependency", "DotnetCliTool" });
                nuspec.ProjectUrl.ShouldBe("https://project.url");
                nuspec.ReleaseNotes.ShouldBe("Release notes for PackageD");
                nuspec.RepositoryBranch.ShouldBe("Branch1000");
                nuspec.RepositoryCommit.ShouldBe("Commit14");
                nuspec.RepositoryType.ShouldBe("Git");
                nuspec.RepositoryUrl.ShouldBe("https://repository.url");
                nuspec.RequireLicenseAcceptance.ShouldBeTrue();
                nuspec.Serviceable.ShouldBeTrue();
                nuspec.Summary.ShouldBe("Summary of PackageD");
                nuspec.Tags.ShouldBe("Tag1 Tag2 Tag3");
                nuspec.Title.ShouldBe("Title of PackageD");
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
                    .Package("Foo.Bar", "1.2.3", out Package package)
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