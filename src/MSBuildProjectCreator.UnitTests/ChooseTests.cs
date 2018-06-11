// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Shouldly;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class ChooseTests : MSBuildTestBase
    {
        [Fact]
        public void ChooseComplex()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Choose()
                .When("86578122B49842F891B9E4364611C4B5")
                    .WhenProperty("B7AF35A6A56D42EFACF4DFE1D7B0BC22", "0CA39A3C5E5348C4B5F1F50A33D8E6D8")
                .WhenItemInclude("B5A8719D78D94961AFFA5134869C0DA5", "451053866BBB40B4A384EF5621994A65")
                .WhenPropertyGroup("F192F2869950424BBE8FF979B12D98F8")
                        .WhenProperty("EB4919AA53C5457BB43F2E655469CF87", "1169C171C8F545D98691AB1DA5022763")
                .When("ED64B6236EB94B639CF64DA27F4DAA21")
                    .WhenProperty("EA807B50DDD64BFAB67D74F34B2D091F", "8DB5478CDBC74BD1A222FE8FFA3F02A7")
                .WhenItemInclude("F5624A5EE6B54AF6BBF6A03A163290A6", "4B45B6646D704CD8B06ED8401853C6F2")
                .Otherwise()
                    .OtherwisePropertyGroup()
                        .OtherwiseProperty("EEE980CF390149DFB7B54B944B1F81C3", "7A886840832A4AACB0190A13E97D3502")
                .OtherwiseItemGroup("84B3B0E939CE4F0CAABE555F3DD5940F")
                    .OtherwiseItemInclude("BDD653F0C8504129845B4CBBAD0732B1", "0887CC7FB1B842BD9B69AACD0ACBC6D3")
                .OtherwiseItemGroup()
                    .OtherwiseItemInclude("C93C964CC1D6475BA622D37155E27398", "C005C57892AD48F8994C1003D91F8A01")
                .Property("F5EA50D160D84FEA9D19049E41BD75EB", "2B50B6EF9CB148D583726948D7565E01")
                .Choose()
                .When("BFB1AB9E0D224C42AEB2A19A8247FD37")
                    .WhenProperty("DF4F395C964342C98430D7F9DD047D0F", "DCF0536D69BA4904BE25A69B1C834E92")
                    .WhenItemInclude("A9CA9BB3DFDE41808AA7646F44D628F3", "16405519E583482D8BAF5D7F23F13C2F")
                .OtherwiseProperty("EF325D1CDDA249D396CEA0F6B6C52BFB", "8732DB2BD30E4334A9B0913DA6DA9E0F")
                .OtherwiseItemInclude("A604962F0D14471C961BA98592A78ADC", "3224EC5ED0A743E2B4C32BC4E6AC4BAC")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Choose>
    <When Condition=""86578122B49842F891B9E4364611C4B5"">
      <PropertyGroup>
        <B7AF35A6A56D42EFACF4DFE1D7B0BC22>0CA39A3C5E5348C4B5F1F50A33D8E6D8</B7AF35A6A56D42EFACF4DFE1D7B0BC22>
      </PropertyGroup>
      <ItemGroup>
        <B5A8719D78D94961AFFA5134869C0DA5 Include=""451053866BBB40B4A384EF5621994A65"" />
      </ItemGroup>
      <PropertyGroup Condition=""F192F2869950424BBE8FF979B12D98F8"">
        <EB4919AA53C5457BB43F2E655469CF87>1169C171C8F545D98691AB1DA5022763</EB4919AA53C5457BB43F2E655469CF87>
      </PropertyGroup>
    </When>
    <When Condition=""ED64B6236EB94B639CF64DA27F4DAA21"">
      <PropertyGroup>
        <EA807B50DDD64BFAB67D74F34B2D091F>8DB5478CDBC74BD1A222FE8FFA3F02A7</EA807B50DDD64BFAB67D74F34B2D091F>
      </PropertyGroup>
      <ItemGroup>
        <F5624A5EE6B54AF6BBF6A03A163290A6 Include=""4B45B6646D704CD8B06ED8401853C6F2"" />
      </ItemGroup>
    </When>
    <Otherwise>
      <PropertyGroup>
        <EEE980CF390149DFB7B54B944B1F81C3>7A886840832A4AACB0190A13E97D3502</EEE980CF390149DFB7B54B944B1F81C3>
      </PropertyGroup>
      <ItemGroup Condition=""84B3B0E939CE4F0CAABE555F3DD5940F"">
        <BDD653F0C8504129845B4CBBAD0732B1 Include=""0887CC7FB1B842BD9B69AACD0ACBC6D3"" />
      </ItemGroup>
      <ItemGroup>
        <C93C964CC1D6475BA622D37155E27398 Include=""C005C57892AD48F8994C1003D91F8A01"" />
      </ItemGroup>
    </Otherwise>
  </Choose>
  <PropertyGroup>
    <F5EA50D160D84FEA9D19049E41BD75EB>2B50B6EF9CB148D583726948D7565E01</F5EA50D160D84FEA9D19049E41BD75EB>
  </PropertyGroup>
  <Choose>
    <When Condition=""BFB1AB9E0D224C42AEB2A19A8247FD37"">
      <PropertyGroup>
        <DF4F395C964342C98430D7F9DD047D0F>DCF0536D69BA4904BE25A69B1C834E92</DF4F395C964342C98430D7F9DD047D0F>
      </PropertyGroup>
      <ItemGroup>
        <A9CA9BB3DFDE41808AA7646F44D628F3 Include=""16405519E583482D8BAF5D7F23F13C2F"" />
      </ItemGroup>
    </When>
    <Otherwise>
      <PropertyGroup>
        <EF325D1CDDA249D396CEA0F6B6C52BFB>8732DB2BD30E4334A9B0913DA6DA9E0F</EF325D1CDDA249D396CEA0F6B6C52BFB>
      </PropertyGroup>
      <ItemGroup>
        <A604962F0D14471C961BA98592A78ADC Include=""3224EC5ED0A743E2B4C32BC4E6AC4BAC"" />
      </ItemGroup>
    </Otherwise>
  </Choose>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void ChooseDuplicateOtherwiseThrows()
        {
            Assert.Throws<ProjectCreatorException>(() =>
                ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                    .When("775FF5802F144DEDAACE58B9490C33BA")
                    .Otherwise()
                    .Otherwise())
                .Message
                .ShouldBe("You can only add one Otherwise to a Choose.");
        }

        [Fact]
        public void ChooseSimple()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .When("9E523BB0891C418C8F65DFF9AC8DAE4D")
                .WhenProperty("A3E6CF45FEC246018B4EF3DBA9194874", "782A17D1FABD4109A7AF3FBF81C2BEE4")
                .OtherwiseProperty("CED127BDCF6E4CD68320A7F74DFB753E", "636471FE65DE453489594A635A64C958")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Choose>
    <When Condition=""9E523BB0891C418C8F65DFF9AC8DAE4D"">
      <PropertyGroup>
        <A3E6CF45FEC246018B4EF3DBA9194874>782A17D1FABD4109A7AF3FBF81C2BEE4</A3E6CF45FEC246018B4EF3DBA9194874>
      </PropertyGroup>
    </When>
    <Otherwise>
      <PropertyGroup>
        <CED127BDCF6E4CD68320A7F74DFB753E>636471FE65DE453489594A635A64C958</CED127BDCF6E4CD68320A7F74DFB753E>
      </PropertyGroup>
    </Otherwise>
  </Choose>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void OtherwiseItemGroupIncludeSimple()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .When("CD76E65E1E824325A4925A194AD4F483")
                .OtherwiseItemInclude("CDC2199A17E246ABA6F265152F787089", "92FE0C66858E4D7DAA74D8E0EA2DB048")
                .OtherwiseItemGroup("B7C857786BF3465AB66FBFFAC1685282")
                .OtherwiseItemInclude("B8F32E43B164439E9BC571B44DAAC1C6", "C49C78223E25441FBC7564E29C7DDC4F")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Choose>
    <When Condition=""CD76E65E1E824325A4925A194AD4F483"" />
    <Otherwise>
      <ItemGroup>
        <CDC2199A17E246ABA6F265152F787089 Include=""92FE0C66858E4D7DAA74D8E0EA2DB048"" />
      </ItemGroup>
      <ItemGroup Condition=""B7C857786BF3465AB66FBFFAC1685282"">
        <B8F32E43B164439E9BC571B44DAAC1C6 Include=""C49C78223E25441FBC7564E29C7DDC4F"" />
      </ItemGroup>
    </Otherwise>
  </Choose>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void OtherwiseItemGroupThowsIfNoWhen()
        {
            Assert.Throws<ProjectCreatorException>(() =>
                    ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                        .OtherwiseItemGroup())
                .Message
                .ShouldBe("You must add a When before adding an Otherwise.");
        }

        [Fact]
        public void OtherwiseItemIncludeSimple()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .When("FC8B0F3FC5AF46A0ACC25560933CF785")
                .OtherwiseItemInclude("B799C10460D94BE5984EC263CFE3D137", "35E7FB0DAF0545A0B536C356D767FA16")
                .OtherwiseItemInclude("C0BC9946435E4DDDB2EA6A2D2B646887", "9903791A0A484F628894BD9CDEFFCECC")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Choose>
    <When Condition=""FC8B0F3FC5AF46A0ACC25560933CF785"" />
    <Otherwise>
      <ItemGroup>
        <B799C10460D94BE5984EC263CFE3D137 Include=""35E7FB0DAF0545A0B536C356D767FA16"" />
        <C0BC9946435E4DDDB2EA6A2D2B646887 Include=""9903791A0A484F628894BD9CDEFFCECC"" />
      </ItemGroup>
    </Otherwise>
  </Choose>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void OtherwiseItemThowsIfNoWhen()
        {
            Assert.Throws<ProjectCreatorException>(() =>
                    ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                        .OtherwiseItemInclude("DF56AA18486E493E843BF397B1524E32", "6936461B06F8470EAB28C980E50F86E2"))
                .Message
                .ShouldBe("You must add a When before adding an Otherwise.");
        }

        [Fact]
        public void OtherwisePropertyGroupThowsIfNoWhen()
        {
            Assert.Throws<ProjectCreatorException>(() =>
                    ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                        .OtherwisePropertyGroup())
                .Message
                .ShouldBe("You must add a When before adding an Otherwise.");
        }

        [Fact]
        public void OtherwisePropertyThowsIfNoWhen()
        {
            Assert.Throws<ProjectCreatorException>(() =>
                    ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                        .OtherwiseProperty("C59717A7F40F40AE984A2B0DFDA165FC", "F070DA5E5F8A46619653DDE31818A23A"))
                .Message
                .ShouldBe("You must add a When before adding an Otherwise.");
        }

        [Fact]
        public void OtherwiseThowsIfNoWhen()
        {
            Assert.Throws<ProjectCreatorException>(() =>
                    ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                        .Otherwise())
                .Message
                .ShouldBe("You must add a When before adding an Otherwise.");
        }

        [Fact]
        public void WhenOtherwiseSimple()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .When("7466B77C62134D72A9B37A7B4377EA1A")
                .WhenProperty("BAEA343538474A71A3E9D9C01DABA1EE", "798AFBC1D5894E95BC7CD61D27B41CB6")
                .OtherwiseProperty("BAEA343538474A71A3E9D9C01DABA1EE", "F08CB25B5C7C40AE88ADD98CC052DBD6")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Choose>
    <When Condition=""7466B77C62134D72A9B37A7B4377EA1A"">
      <PropertyGroup>
        <BAEA343538474A71A3E9D9C01DABA1EE>798AFBC1D5894E95BC7CD61D27B41CB6</BAEA343538474A71A3E9D9C01DABA1EE>
      </PropertyGroup>
    </When>
    <Otherwise>
      <PropertyGroup>
        <BAEA343538474A71A3E9D9C01DABA1EE>F08CB25B5C7C40AE88ADD98CC052DBD6</BAEA343538474A71A3E9D9C01DABA1EE>
      </PropertyGroup>
    </Otherwise>
  </Choose>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void WhenPropertyGroupThowsIfNoWhen()
        {
            Assert.Throws<ProjectCreatorException>(() =>
                    ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                        .WhenPropertyGroup())
                .Message
                .ShouldBe("You must add a When before adding a When PropertyGroup.");
        }

        [Fact]
        public void WhenPropertyThowsIfNoWhen()
        {
            Assert.Throws<ProjectCreatorException>(() =>
                    ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                        .WhenProperty("A32ADB0D9F5D4BFAA927A39E1749A2A8", "A00D15F65FD34166BE03764D642ADD2A"))
                .Message
                .ShouldBe("You must add a When before adding a When PropertyGroup.");
        }
    }
}