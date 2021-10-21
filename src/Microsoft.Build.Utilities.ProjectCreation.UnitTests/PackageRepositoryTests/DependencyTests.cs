// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using Shouldly;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests.PackageRepositoryTests
{
    public class DependencyTests : TestBase
    {
        [Fact]
        public void CanAddDependenciesToMultipleGroups()
        {
            using (PackageRepository packageRepository = PackageRepository.Create(TestRootPath)
                .Package("PackageA", "1.0.0", out Package package)
                .Dependency("PackageB", "1.0.0", "net45")
                .Dependency("PackageB", "1.0.0", "net46")
                .Dependency("PackageB", "1.0.0", "netstandard2.0"))
            {
                ValidatePackageDependencies(
                    packageRepository,
                    package,
                    new List<PackageDependencyGroup>
                    {
                        new PackageDependencyGroup(
                            FrameworkConstants.CommonFrameworks.Net45,
                            new List<PackageDependency>
                            {
                                new PackageDependency("PackageB", VersionRange.Parse("1.0.0")),
                            }),
                        new PackageDependencyGroup(
                            FrameworkConstants.CommonFrameworks.Net46,
                            new List<PackageDependency>
                            {
                                new PackageDependency("PackageB", VersionRange.Parse("1.0.0")),
                            }),
                        new PackageDependencyGroup(
                            FrameworkConstants.CommonFrameworks.NetStandard20,
                            new List<PackageDependency>
                            {
                                new PackageDependency("PackageB", VersionRange.Parse("1.0.0")),
                            }),
                    });
            }
        }

        [Fact]
        public void CanAddMultipleDependenciesToSameGroup()
        {
            using (PackageRepository packageRepository = PackageRepository.Create(TestRootPath)
                .Package("PackageA", "1.0.0", out Package package)
                .Dependency("PackageB", "1.0.0", "net45")
                .Dependency("PackageC", "1.1.0", "net45")
                .Dependency("PackageD", "1.2.0", "net45"))
            {
                ValidatePackageDependencies(
                    packageRepository,
                    package,
                    new List<PackageDependencyGroup>
                    {
                        new PackageDependencyGroup(
                            FrameworkConstants.CommonFrameworks.Net45,
                            new List<PackageDependency>
                            {
                                new PackageDependency("PackageB", VersionRange.Parse("1.0.0")),
                                new PackageDependency("PackageC", VersionRange.Parse("1.1.0")),
                                new PackageDependency("PackageD", VersionRange.Parse("1.2.0")),
                            }),
                    });
            }
        }

        private void ValidatePackageDependencies(PackageRepository packageRepository, Package package, IEnumerable<PackageDependencyGroup> expectedDependencyGroups)
        {
            FileInfo nuspecFile = new FileInfo(packageRepository.GetManifestFilePath(package.Id, package.Version));

            nuspecFile.ShouldExist();

            using (FileStream stream = File.OpenRead(nuspecFile.FullName))
            {
                Manifest manifest = Manifest.ReadFrom(stream, validateSchema: false);

                List<PackageDependencyGroup> dependencyGroups = manifest.Metadata.DependencyGroups.ToList();

                dependencyGroups.ShouldBe(expectedDependencyGroups);
            }
        }
    }
}