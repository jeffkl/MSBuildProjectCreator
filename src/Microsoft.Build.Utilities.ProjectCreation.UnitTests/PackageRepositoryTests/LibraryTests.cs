// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Shouldly;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests.PackageRepositoryTests
{
    public class LibraryTests : TestBase
    {
        [Fact]
        public void BasicLibrary()
        {
            using (PackageRepository packageRepository = PackageRepository.Create(TestRootPath)
                .Package("PackageA", "1.0.0", out Package packageA)
                .Library("net45"))
            {
                VerifyAssembly(packageRepository, packageA, "net45");
            }
        }

        [Fact]
        public void LibraryWithVersion()
        {
            const string assemblyVersion = "2.3.4.5";

            using (PackageRepository packageRepository = PackageRepository.Create(TestRootPath)
                .Package("PackageA", "1.0.0", out Package packageA)
                .Library("net45", assemblyVersion: assemblyVersion))
            {
                VerifyAssembly(packageRepository, packageA, "net45", version: "2.3.4.5");
            }
        }

        [Fact]
        public void MultipleLibrariesMultipleTargetFrameworks()
        {
            using (PackageRepository packageRepository = PackageRepository.Create(TestRootPath)
                .Package("PackageA", "1.0.0", out Package packageA)
                .Library("net45")
                .Library("netstandard2.0"))
            {
                VerifyAssembly(packageRepository, packageA, "net45");
                VerifyAssembly(packageRepository, packageA, "netstandard2.0");
            }
        }

        [Fact]
        public void MultipleLibrariesSameTargetFramework()
        {
            using (PackageRepository packageRepository = PackageRepository.Create(TestRootPath)
                .Package("PackageA", "1.0.0", out Package packageA)
                .Library("net45", filename: null)
                .Library("net45", filename: "CustomAssembly.dll"))
            {
                VerifyAssembly(packageRepository, packageA, "net45");
                VerifyAssembly(packageRepository, packageA, "net45", assemblyFileName: "CustomAssembly.dll");
            }
        }

        private void VerifyAssembly(PackageRepository packageRepository, Package package, string targetFramework, string assemblyFileName = null, string version = null)
        {
            DirectoryInfo packageDirectory = new DirectoryInfo(packageRepository.GetInstallPath(package.Id, package.Version))
                .ShouldExist();

            DirectoryInfo libDirectory = new DirectoryInfo(Path.Combine(packageDirectory.FullName, "lib", targetFramework))
                .ShouldExist();

            FileInfo classLibrary = new FileInfo(Path.Combine(libDirectory.FullName, assemblyFileName ?? $"{package.Id}.dll"))
                .ShouldExist();

            AssemblyName assemblyName = AssemblyName.GetAssemblyName(classLibrary.FullName);

            assemblyName.Version.ShouldBe(version == null ? new Version(1, 0, 0, 0) : Version.Parse(version));
        }
    }
}