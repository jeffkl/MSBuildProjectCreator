// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Utilities.ProjectCreation.UnitTests;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using Shouldly;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.NuGet.UnitTests
{
    public class LibraryTests : TestBase
    {
        [Fact]
        public void BasicLibrary()
        {
            using (PackageRepository packageRepository = PackageRepository.Create(TestRootPath)
                .Package("PackageA", "1.0.0", out PackageIdentity packageA)
                .Library(FrameworkConstants.CommonFrameworks.Net45))
            {
                VerifyAssembly(packageRepository, packageA, FrameworkConstants.CommonFrameworks.Net45);
            }
        }

        [Fact]
        public void LibraryWithVersion()
        {
            const string assemblyVersion = "2.3.4.5";

            using (PackageRepository packageRepository = PackageRepository.Create(TestRootPath)
                .Package("PackageA", "1.0.0", out PackageIdentity packageA)
                .Library(FrameworkConstants.CommonFrameworks.Net45, assemblyVersion: assemblyVersion))
            {
                VerifyAssembly(packageRepository, packageA, FrameworkConstants.CommonFrameworks.Net45, version: "2.3.4.5");
            }
        }

        [Fact]
        public void MultipleLibrariesMultipleTargetFrameworks()
        {
            using (PackageRepository packageRepository = PackageRepository.Create(TestRootPath)
                .Package("PackageA", "1.0.0", out PackageIdentity packageA)
                .Library(FrameworkConstants.CommonFrameworks.Net45)
                .Library(FrameworkConstants.CommonFrameworks.NetStandard20))
            {
                VerifyAssembly(packageRepository, packageA, FrameworkConstants.CommonFrameworks.Net45);
                VerifyAssembly(packageRepository, packageA, FrameworkConstants.CommonFrameworks.NetStandard20);
            }
        }

        [Fact]
        public void MultipleLibrariesSameTargetFramework()
        {
            using (PackageRepository packageRepository = PackageRepository.Create(TestRootPath)
                .Package("PackageA", "1.0.0", out PackageIdentity packageA)
                .Library(FrameworkConstants.CommonFrameworks.Net45, filename: null)
                .Library(FrameworkConstants.CommonFrameworks.Net45, filename: "CustomAssembly.dll"))
            {
                VerifyAssembly(packageRepository, packageA, FrameworkConstants.CommonFrameworks.Net45);
                VerifyAssembly(packageRepository, packageA, FrameworkConstants.CommonFrameworks.Net45, assemblyFileName: "CustomAssembly.dll");
            }
        }

        private void VerifyAssembly(PackageRepository packageRepository, PackageIdentity packageIdentity, NuGetFramework targetFramework, string assemblyFileName = null, string version = null)
        {
            DirectoryInfo packageDirectory = new DirectoryInfo(packageRepository.GetInstallPath(packageIdentity.Id, packageIdentity.Version))
                .ShouldExist();

            DirectoryInfo libDirectory = new DirectoryInfo(Path.Combine(packageDirectory.FullName, "lib", targetFramework.GetShortFolderName()))
                .ShouldExist();

            FileInfo classLibrary = new FileInfo(Path.Combine(libDirectory.FullName, assemblyFileName ?? $"{packageIdentity.Id}.dll"))
                .ShouldExist();

            AssemblyName assemblyName = AssemblyName.GetAssemblyName(classLibrary.FullName);

            assemblyName.Version.ShouldBe(version == null ? new Version(1, 0, 0, 0) : Version.Parse(version));
        }
    }
}