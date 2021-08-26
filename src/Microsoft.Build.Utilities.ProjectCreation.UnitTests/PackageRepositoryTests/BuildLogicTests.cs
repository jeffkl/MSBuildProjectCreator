// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Shouldly;
using System;
using System.IO;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests.PackageRepositoryTests
{
    public class BuildLogicTests : TestBase
    {
        [Fact]
        public void BuildLogicRequiresPackage()
        {
            InvalidOperationException exception;

            exception = Should.Throw<InvalidOperationException>(() =>
            {
                PackageRepository.Create(TestRootPath)
                    .BuildMultiTargetingProps();
            });

            exception.Message.ShouldBe(Strings.ErrorWhenAddingBuildLogicRequiresPackage);

            exception = Should.Throw<InvalidOperationException>(() =>
            {
                PackageRepository.Create(TestRootPath)
                    .BuildMultiTargetingTargets();
            });

            exception.Message.ShouldBe(Strings.ErrorWhenAddingBuildLogicRequiresPackage);

            exception = Should.Throw<InvalidOperationException>(() =>
            {
                PackageRepository.Create(TestRootPath)
                    .BuildTransitiveProps();
            });

            exception.Message.ShouldBe(Strings.ErrorWhenAddingBuildLogicRequiresPackage);

            exception = Should.Throw<InvalidOperationException>(() =>
            {
                PackageRepository.Create(TestRootPath)
                    .BuildTransitiveTargets();
            });

            exception.Message.ShouldBe(Strings.ErrorWhenAddingBuildLogicRequiresPackage);

            exception = Should.Throw<InvalidOperationException>(() =>
            {
                PackageRepository.Create(TestRootPath)
                    .BuildProps();
            });

            exception.Message.ShouldBe(Strings.ErrorWhenAddingBuildLogicRequiresPackage);

            exception = Should.Throw<InvalidOperationException>(() =>
            {
                PackageRepository.Create(TestRootPath)
                    .BuildTargets();
            });

            exception.Message.ShouldBe(Strings.ErrorWhenAddingBuildLogicRequiresPackage);
        }

        [Fact]
        public void BuildMultiTargetingTest()
        {
            PackageRepository.Create(TestRootPath)
                .Package("PackageA", "2.0")
                .BuildMultiTargetingProps(out ProjectCreator buildMultiTargetingPropsProject)
                .BuildMultiTargetingTargets(out ProjectCreator buildMultiTargetingTargetsProject);

            buildMultiTargetingPropsProject.FullPath.ShouldBe(Path.Combine(TestRootPath, ".nuget", "packages", "packagea", "2.0.0", "buildMultiTargeting", "PackageA.props"));
            buildMultiTargetingTargetsProject.FullPath.ShouldBe(Path.Combine(TestRootPath, ".nuget", "packages", "packagea", "2.0.0", "buildMultiTargeting", "PackageA.targets"));

            File.Exists(buildMultiTargetingPropsProject.FullPath).ShouldBeTrue();
            File.Exists(buildMultiTargetingTargetsProject.FullPath).ShouldBeTrue();
        }

        [Fact]
        public void BuildPropsTest()
        {
            PackageRepository.Create(TestRootPath)
                .Package("PackageA", "2.0")
                .BuildProps(out ProjectCreator buildPropsProject)
                .BuildTargets(out ProjectCreator buildTargetsProject);

            buildPropsProject.FullPath.ShouldBe(Path.Combine(TestRootPath, ".nuget", "packages", "packagea", "2.0.0", "build", "PackageA.props"));
            buildTargetsProject.FullPath.ShouldBe(Path.Combine(TestRootPath, ".nuget", "packages", "packagea", "2.0.0", "build", "PackageA.targets"));

            File.Exists(buildPropsProject.FullPath).ShouldBeTrue();
            File.Exists(buildTargetsProject.FullPath).ShouldBeTrue();
        }

        [Fact]
        public void BuildTransitiveTest()
        {
            PackageRepository.Create(TestRootPath)
                .Package("PackageA", "2.0")
                .BuildTransitiveProps(out ProjectCreator buildTransitivePropsProject)
                .BuildTransitiveTargets(out ProjectCreator buildTransitiveTargetsProject);

            buildTransitivePropsProject.FullPath.ShouldBe(Path.Combine(TestRootPath, ".nuget", "packages", "packagea", "2.0.0", "buildTransitive", "PackageA.props"));
            buildTransitiveTargetsProject.FullPath.ShouldBe(Path.Combine(TestRootPath, ".nuget", "packages", "packagea", "2.0.0", "buildTransitive", "PackageA.targets"));

            File.Exists(buildTransitivePropsProject.FullPath).ShouldBeTrue();
            File.Exists(buildTransitiveTargetsProject.FullPath).ShouldBeTrue();
        }
    }
}