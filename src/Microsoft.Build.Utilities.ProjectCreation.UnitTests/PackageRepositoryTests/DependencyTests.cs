// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

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
                    new List<(string TargetFramework, IEnumerable<PackageDependency> Dependencies)>
                    {
                        (
                            "net45",
                            new List<PackageDependency>
                            {
                                new PackageDependency("PackageB", "1.0.0", "Build, Analyzers"),
                            }),
                        (
                            "net46",
                            new List<PackageDependency>
                            {
                                new PackageDependency("PackageB", "1.0.0", "Build, Analyzers"),
                            }),
                        (
                            "netstandard2.0",
                            new List<PackageDependency>
                            {
                                new PackageDependency("PackageB", "1.0.0", "Build, Analyzers"),
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
                    new List<(string TargetFramework, IEnumerable<PackageDependency> Dependencies)>
                    {
                        (
                            "net45",
                            new List<PackageDependency>
                            {
                                new PackageDependency("PackageB", "1.0.0", "Build, Analyzers"),
                                new PackageDependency("PackageC", "1.1.0", "Build, Analyzers"),
                                new PackageDependency("PackageD", "1.2.0", "Build, Analyzers"),
                            }),
                    });
            }
        }

        private void ValidatePackageDependencies(PackageRepository packageRepository, Package package, IEnumerable<(string TargetFramework, IEnumerable<PackageDependency> Dependencies)> expectedDependencyGroups)
        {
            FileInfo nuspecFile = new FileInfo(packageRepository.GetManifestFilePath(package.Id, package.Version));

            nuspecFile.ShouldExist();

            NuspecReader nuspec = new NuspecReader(nuspecFile);

            foreach ((string targetFramework, IEnumerable<PackageDependency> dependencies) in expectedDependencyGroups)
            {
                List<PackageDependency> actualDependencies = nuspec.DependencyGroups.First(i => i.TargetFramework == targetFramework).Dependencies.ToList();
                List<PackageDependency> expectedDependencies = dependencies.ToList();

                actualDependencies.Count.ShouldBe(expectedDependencies.Count);

                for (int i = 0; i < actualDependencies.Count; i++)
                {
                    actualDependencies[i].ShouldBe(expectedDependencies[i]);
                }
            }
        }
    }
}