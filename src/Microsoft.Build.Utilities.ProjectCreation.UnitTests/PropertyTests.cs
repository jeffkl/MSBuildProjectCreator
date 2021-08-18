// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Shouldly;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class PropertyTests : MSBuildTestBase
    {
        [Fact]
        public void ProjectPropertySetIfEmpty()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Property(
                    "EA952E8415904C0692B8808B0E6C6601",
                    "4082DB4D80DE4501A15E66800971A026",
                    setIfEmpty: true)
                .Xml
                .ShouldBe(
                    @"<Project>
  <PropertyGroup>
    <EA952E8415904C0692B8808B0E6C6601 Condition="" '$(EA952E8415904C0692B8808B0E6C6601)' == '' "">4082DB4D80DE4501A15E66800971A026</EA952E8415904C0692B8808B0E6C6601>
  </PropertyGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void ProjectPropertySetIfEmptyPrependsCondition()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Property(
                    name: "E1A695D73E91481A9D1FAEE1C4C8407C",
                    unevaluatedValue: "5C50C035A20E4372B6A64C08D111F9F7",
                    condition: "9894AD0320B64027AA92732D436A5F0A",
                    setIfEmpty: true,
                    label: "label")
                .Xml
                .ShouldBe(
                    @"<Project>
  <PropertyGroup>
    <E1A695D73E91481A9D1FAEE1C4C8407C Condition="" '$(E1A695D73E91481A9D1FAEE1C4C8407C)' == '' And 9894AD0320B64027AA92732D436A5F0A "" Label=""label"">5C50C035A20E4372B6A64C08D111F9F7</E1A695D73E91481A9D1FAEE1C4C8407C>
  </PropertyGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void PropertyNotSetIfNull()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Property("BFB895A7AD3F4CAFBE514DCFE5D30354", null)
                .Property("B712A0D4A328439E8862D6715E044AB7", "2C9E4222FEBF43F7B841099EC3DDB14B")
                .Xml
                .ShouldBe(
                    @"<Project>
  <PropertyGroup>
    <B712A0D4A328439E8862D6715E044AB7>2C9E4222FEBF43F7B841099EC3DDB14B</B712A0D4A328439E8862D6715E044AB7>
  </PropertyGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void PropertySetIfEmpty()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Property("A47F78111F084710B139CD5AEEB5395E", string.Empty)
                .Xml
                .ShouldBe(
                    @"<Project>
  <PropertyGroup>
    <A47F78111F084710B139CD5AEEB5395E />
  </PropertyGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void PropertySimple()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Property("FF38992245B549C5B353B1662A7A330D", "60B667098861411CA81AD8A8A355A649")
                .Xml
                .ShouldBe(
                    @"<Project>
  <PropertyGroup>
    <FF38992245B549C5B353B1662A7A330D>60B667098861411CA81AD8A8A355A649</FF38992245B549C5B353B1662A7A330D>
  </PropertyGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void TryGetPropertyValueSimple()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Property("E7A39154F5AB476A928067251F88FFCE", "E8F579A7E3374F389120CF6D888E74B9")
                .Property("FAB58E5B32D14990ACE2490D7593FDF6", "60F55FB14D2E44B2BA4EC91488D9FF8F")
                .TryGetPropertyValue("E7A39154F5AB476A928067251F88FFCE", out string property1)
                .TryGetPropertyValue("FAB58E5B32D14990ACE2490D7593FDF6", out string property2);

            property1.ShouldBe("E8F579A7E3374F389120CF6D888E74B9");
            property2.ShouldBe("60F55FB14D2E44B2BA4EC91488D9FF8F");
        }
    }
}