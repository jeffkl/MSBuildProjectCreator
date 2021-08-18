// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Shouldly;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class ImportTests : MSBuildTestBase
    {
        [Fact]
        public void ImportComplex()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Import(
                    project: "7DAAB5E7D790429584923E4E1CBCA82A",
                    condition: "D18454A292794A87AF7FABC741228737",
                    sdk: "74DD86391F1448878DAC138A6B0BF706",
                    sdkVersion: "FA38CAE9A09044D0831FB10258F1D433")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Import Project=""7DAAB5E7D790429584923E4E1CBCA82A"" Condition=""D18454A292794A87AF7FABC741228737"" Sdk=""74DD86391F1448878DAC138A6B0BF706"" Version=""FA38CAE9A09044D0831FB10258F1D433"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void ImportOrder()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Import("5D3801D732C14BB2A8A2A8466B2DAD38", label: "label")
                .Property("BCD8381CE5944323B3019379EBE55F5C", "36E81797987E4319A5BCE62F57ACE527", label: "label")
                .Import("5E2A00F750CE4E14B793C51ACCA60F84")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Import Project=""5D3801D732C14BB2A8A2A8466B2DAD38"" Label=""label"" />
  <PropertyGroup>
    <BCD8381CE5944323B3019379EBE55F5C Label=""label"">36E81797987E4319A5BCE62F57ACE527</BCD8381CE5944323B3019379EBE55F5C>
  </PropertyGroup>
  <Import Project=""5E2A00F750CE4E14B793C51ACCA60F84"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void ImportProject()
        {
            ProjectCreator project1 = ProjectCreator.Create("B2EE38CD5D1E4B228655A95B2B7224BA");

            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Import(project1.Project, condition: "8D9C051BE69C4EB99B7C4A53C80A625D")
                .Xml
                .ShouldBe(
                    $@"<Project>
  <Import Project=""{project1.FullPath}"" Condition=""8D9C051BE69C4EB99B7C4A53C80A625D"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void ImportProjectCreator()
        {
            ProjectCreator project1 = ProjectCreator.Create("A4DD67D773834B24AC6AEA317653AD28");

            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Import(project1, condition: "E4E4F3ECECF444D28D65F5A59D4B2E89")
                .Xml
                .ShouldBe(
                    $@"<Project>
  <Import Project=""{project1.FullPath}"" Condition=""E4E4F3ECECF444D28D65F5A59D4B2E89"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void ImportProjectRootElement()
        {
            ProjectCreator project1 = ProjectCreator.Create("24775F0E17A348979DB4DF3D357621F9");

            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Import(project1.RootElement, condition: "5884085731AB4A588AC8337069C3223B")
                .Xml
                .ShouldBe(
                    $@"<Project>
  <Import Project=""{project1.FullPath}"" Condition=""5884085731AB4A588AC8337069C3223B"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void ImportSdk()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Import("6D75CC8EE2FA40AA8A0DC43112A85A0C", sdk: "4E45E3BD92B941338162B69846B462AE")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Import Project=""6D75CC8EE2FA40AA8A0DC43112A85A0C"" Sdk=""4E45E3BD92B941338162B69846B462AE"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void ImportSdkAndVersion()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Import("8F66583869B84CE89D72FF6DAC8A3C66", sdk: "8E35FA0BABDB4488AA096CCF6C82C37A", sdkVersion: "E8835BFC0CF949BFB63AB3917294C41A")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Import Project=""8F66583869B84CE89D72FF6DAC8A3C66"" Sdk=""8E35FA0BABDB4488AA096CCF6C82C37A"" Version=""E8835BFC0CF949BFB63AB3917294C41A"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void ImportSdkTest()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .ImportSdk("B8E5AE4F7DBF4688B0A3F0E07C73FDE2", "BF3306138DB942BDB330230D07A2A8AD", version: "5D31D10637474DE3ADE35AF5236D7D6B", condition: "4A4274FD1456435EB6D059F91A8C279B")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Import Project=""B8E5AE4F7DBF4688B0A3F0E07C73FDE2"" Condition=""4A4274FD1456435EB6D059F91A8C279B"" Sdk=""BF3306138DB942BDB330230D07A2A8AD"" Version=""5D31D10637474DE3ADE35AF5236D7D6B"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void ImportSimple()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Import("78885C0A95004569B281C097FE6A8252")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Import Project=""78885C0A95004569B281C097FE6A8252"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void ImportWithCondition()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Import("01C2CD160C4C4D3A81543D1003C3D750", condition: "541AB4AD8EE747818A54385CED55A50A")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Import Project=""01C2CD160C4C4D3A81543D1003C3D750"" Condition=""541AB4AD8EE747818A54385CED55A50A"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void ImportWithConditionOnExistence()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Import("150D2AEC5CA24ADEBB6A6FDBDE4AA26D", condition: "42649D00AF3644A1A23FACDC111F85D8", conditionOnExistence: true)
                .Xml
                .ShouldBe(
                    @"<Project>
  <Import Project=""150D2AEC5CA24ADEBB6A6FDBDE4AA26D"" Condition=""Exists('150D2AEC5CA24ADEBB6A6FDBDE4AA26D')"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }
    }
}