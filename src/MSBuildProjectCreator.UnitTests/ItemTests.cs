// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class ItemTests : MSBuildTestBase
    {
        [Fact]
        public void CompileItem()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .ItemCompile(
                    "1C18F6564FFE4850A3D2710899454FE1",
                    dependentUpon: "BA2012419E7943F996968C103B0DE557",
                    link: "ADC2E3D8861743639BBF3E9313FEE962",
                    isVisible: false,
                    metadata: new Dictionary<string, string>
                    {
                        { "Custom", "37F6FB078E9C462FAF984DB5AFF38297" }
                    },
                    condition: "22484FB0B4364AE8BB9D43D44D40D46D")
                .Xml
                .ShouldBe(
                    @"<Project>
  <ItemGroup>
    <Compile Include=""1C18F6564FFE4850A3D2710899454FE1"" Condition=""22484FB0B4364AE8BB9D43D44D40D46D"">
      <Custom>37F6FB078E9C462FAF984DB5AFF38297</Custom>
      <DependentUpon>BA2012419E7943F996968C103B0DE557</DependentUpon>
      <Link>ADC2E3D8861743639BBF3E9313FEE962</Link>
      <Visible>False</Visible>
    </Compile>
  </ItemGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void ContentItem()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .ItemContent("2B63E02B870E4F2FBE812995C081B0C5")
                .Xml
                .ShouldBe(
                    @"<Project>
  <ItemGroup>
    <Content Include=""2B63E02B870E4F2FBE812995C081B0C5"" />
  </ItemGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void ContentItemMetadata()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .ItemContent(
                    "693A78F97EAB427EBDFD98ECE35581F7",
                    dependentUpon: "42AB9F17D7C04CB591FB1ACD065F279B",
                    link: "566C8C835EA34902B5B7D82DAF959617",
                    isVisible: false,
                    copyToOutputDirectory: "1C57073B00964567A77666363AE52845",
                    metadata: new Dictionary<string, string>
                    {
                        { "Custom", "C791E7D345444C87B541211E0C60E344" }
                    },
                    condition: "946D6DF1B32C47DBB0F34CA2E46FA6B8")
                .Xml
                .ShouldBe(
                    @"<Project>
  <ItemGroup>
    <Content Include=""693A78F97EAB427EBDFD98ECE35581F7"" Condition=""946D6DF1B32C47DBB0F34CA2E46FA6B8"">
      <Custom>C791E7D345444C87B541211E0C60E344</Custom>
      <CopyToOutputDirectory>1C57073B00964567A77666363AE52845</CopyToOutputDirectory>
      <DependentUpon>42AB9F17D7C04CB591FB1ACD065F279B</DependentUpon>
      <Link>566C8C835EA34902B5B7D82DAF959617</Link>
      <Visible>False</Visible>
    </Content>
  </ItemGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void CustomItemInclude()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .ItemInclude(
                    itemType: "F5392360029A428CA7F3E0AEA48DB695",
                    include: "E4E5CD271D7849639D2EB47018CE596F",
                    metadata: new Dictionary<string, string>
                    {
                        { "C2656BF5ABE24763B5D6789D20BE1086", "131E00C18D9F4704A5FC2865D7BF4DBA" },
                        { "D6B4D9F865B041AE9EEB7FBD0F09C058", "D02633A1BC4D4515A119D334CF87EAB9" },
                        { "E9E4F1CB10C24CA88172FE5AD54E158B", "51F1085F4ED5462282B10195D6B55747" }
                    },
                    exclude: "0950E170EE594396BCD2830147479BB4",
                    condition: "8C3F4A9DF1F44455A1CCE80C17B0246C")
                .Xml
                .ShouldBe(
                    @"<Project>
  <ItemGroup>
    <F5392360029A428CA7F3E0AEA48DB695 Include=""E4E5CD271D7849639D2EB47018CE596F"" Condition=""8C3F4A9DF1F44455A1CCE80C17B0246C"" Exclude=""0950E170EE594396BCD2830147479BB4"">
      <C2656BF5ABE24763B5D6789D20BE1086>131E00C18D9F4704A5FC2865D7BF4DBA</C2656BF5ABE24763B5D6789D20BE1086>
      <D6B4D9F865B041AE9EEB7FBD0F09C058>D02633A1BC4D4515A119D334CF87EAB9</D6B4D9F865B041AE9EEB7FBD0F09C058>
      <E9E4F1CB10C24CA88172FE5AD54E158B>51F1085F4ED5462282B10195D6B55747</E9E4F1CB10C24CA88172FE5AD54E158B>
    </F5392360029A428CA7F3E0AEA48DB695>
  </ItemGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void ItemIncludeNotAddedIfNull()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .ItemInclude("D7DAD38333D04A5999F9791575DBB57D", null)
                .ItemInclude("FDA04096ED074663997F13D37E81E87A", "CBCFA7D42A3B4A1B93127FAFA13CD3DB")
                .Xml
                .ShouldBe(
                    @"<Project>
  <ItemGroup>
    <FDA04096ED074663997F13D37E81E87A Include=""CBCFA7D42A3B4A1B93127FAFA13CD3DB"" />
  </ItemGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void MetadataNotAddedIfNull()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .ItemInclude(
                    itemType: "DB5003A476FA461EB0452DDDCCE7F802",
                    include: "303F33834A6843EDB44DB8D3186E97E0",
                    metadata: new Dictionary<string, string>
                    {
                        { "CDBA5A760C9C45CFB2E9532D4B4AE2B7", "D6A68EA723C848E19D2E17C09F7F2532" },
                        { "FDD9C6C5582B404188CD8C938DB2CDD9", null }
                    })
                .Xml.ShouldBe(
                    @"<Project>
  <ItemGroup>
    <DB5003A476FA461EB0452DDDCCE7F802 Include=""303F33834A6843EDB44DB8D3186E97E0"">
      <CDBA5A760C9C45CFB2E9532D4B4AE2B7>D6A68EA723C848E19D2E17C09F7F2532</CDBA5A760C9C45CFB2E9532D4B4AE2B7>
    </DB5003A476FA461EB0452DDDCCE7F802>
  </ItemGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void NoneItem()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .ItemNone(
                    "1F65516B40BE4EC099C5DB8AF87999E1",
                    dependentUpon: "87E0338EB2E14F278DBC5486AFB6C51D",
                    link: "C7631E32CE214C28BEAD5AB1D5477D6C",
                    isVisible: false,
                    copyToOutputDirectory: "C19852875EC14889A2B66A237AAB674A",
                    metadata: new Dictionary<string, string>
                    {
                        { "A9106C7FC44641B0A6636D3559A3A8F4", "024B55145A22481BA5EFBADD4CA633A7" }
                    },
                    condition: "C88DEB23F8444037835F4E63AE236F7F")
                .Xml
                .ShouldBe(
                    @"<Project>
  <ItemGroup>
    <None Include=""1F65516B40BE4EC099C5DB8AF87999E1"" Condition=""C88DEB23F8444037835F4E63AE236F7F"">
      <A9106C7FC44641B0A6636D3559A3A8F4>024B55145A22481BA5EFBADD4CA633A7</A9106C7FC44641B0A6636D3559A3A8F4>
      <CopyToOutputDirectory>C19852875EC14889A2B66A237AAB674A</CopyToOutputDirectory>
      <DependentUpon>87E0338EB2E14F278DBC5486AFB6C51D</DependentUpon>
      <Link>C7631E32CE214C28BEAD5AB1D5477D6C</Link>
      <Visible>False</Visible>
    </None>
  </ItemGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void PackageReference()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .ItemPackageReference(
                    "30FD03495EF5456D98DC09F5886DB230",
                    version: "4C896A13564E4138B1BFD81304BCE0C3",
                    includeAssets: "F647EECC49A349CFB0C161E8D38E8C71",
                    excludeAssets: "86D60FA951834BC4816C498549A6F236",
                    privateAssets: "4E5C92D014734A8B9CE23198715B6C63",
                    metadata: new Dictionary<string, string>
                    {
                        { "E5E3AEA9BFB547BABCEEEAFEDEB70BDA", "2ECE306CA8C540FBABD7893948504F26" }
                    },
                    condition: "66AF7AA732084E35ACC81361B21ADA3E")
                .Xml.ShouldBe(
                    @"<Project>
  <ItemGroup>
    <PackageReference Include=""30FD03495EF5456D98DC09F5886DB230"" Condition=""66AF7AA732084E35ACC81361B21ADA3E"">
      <E5E3AEA9BFB547BABCEEEAFEDEB70BDA>2ECE306CA8C540FBABD7893948504F26</E5E3AEA9BFB547BABCEEEAFEDEB70BDA>
      <Version>4C896A13564E4138B1BFD81304BCE0C3</Version>
      <IncludeAssets>F647EECC49A349CFB0C161E8D38E8C71</IncludeAssets>
      <ExcludeAssets>86D60FA951834BC4816C498549A6F236</ExcludeAssets>
      <PrivateAssets>4E5C92D014734A8B9CE23198715B6C63</PrivateAssets>
    </PackageReference>
  </ItemGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void ProjectReferenceAnotherProject()
        {
            ProjectCreator project1 = ProjectCreator.Create("D2E2064AB0914029BA04470269D4253B");

            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .ItemProjectReference(
                    project1,
                    name: "E39F90168BA547D7A3D3ABB365689C61",
                    projectGuid: "EDF72CB2AD114C3CB03B2C79178BF09C",
                    referenceOutputAssembly: false,
                    condition: "F0E7173CB4714887A28EB6929227427A")
                .Xml.ShouldBe(
                    $@"<Project>
  <ItemGroup>
    <ProjectReference Include=""{project1.FullPath}"" Condition=""F0E7173CB4714887A28EB6929227427A"">
      <Name>E39F90168BA547D7A3D3ABB365689C61</Name>
      <Project>EDF72CB2AD114C3CB03B2C79178BF09C</Project>
      <ReferenceOutputAssembly>False</ReferenceOutputAssembly>
    </ProjectReference>
  </ItemGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void ProjectReferenceWithPath()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .ItemProjectReference(
                    "CB53CFCFC8C44B169B5910F9C18A9C97",
                    name: "48E21478110940879DA354EA18C9D7A1",
                    projectGuid: "17BC6030262A402D911188CAF0968F51",
                    referenceOutputAssembly: true,
                    metadata: new Dictionary<string, string>
                    {
                        { "C533633B322444B8B41A8D964DF6013B", "2997CF6ABB7544A880F8D14BC3B26D5C" }
                    },
                    condition: "EEA7E2E6077A4B56918D03CAA17411E5")
                .Xml.ShouldBe(
                    @"<Project>
  <ItemGroup>
    <ProjectReference Include=""CB53CFCFC8C44B169B5910F9C18A9C97"" Condition=""EEA7E2E6077A4B56918D03CAA17411E5"">
      <C533633B322444B8B41A8D964DF6013B>2997CF6ABB7544A880F8D14BC3B26D5C</C533633B322444B8B41A8D964DF6013B>
      <Name>48E21478110940879DA354EA18C9D7A1</Name>
      <Project>17BC6030262A402D911188CAF0968F51</Project>
      <ReferenceOutputAssembly>True</ReferenceOutputAssembly>
    </ProjectReference>
  </ItemGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void RemoveItem()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .ItemRemove(
                    itemType: "F10988936B3B421BB4FD7E33B9C0DEC9",
                    remove: "52CE3EA8F90F44779EC543225D6F182A",
                    metadata: new Dictionary<string, string>
                    {
                        { "CCBE2A083AF44A1A8DD0AB38F174D983", "7D228EF67CC648E98D7D46BFFEF935FE" }
                    },
                    condition: "5E1B774BD945407CA45227EE8BF5737A")
                .Xml
                .ShouldBe(
                    @"<Project>
  <ItemGroup>
    <F10988936B3B421BB4FD7E33B9C0DEC9 Remove=""52CE3EA8F90F44779EC543225D6F182A"" Condition=""5E1B774BD945407CA45227EE8BF5737A"">
      <CCBE2A083AF44A1A8DD0AB38F174D983>7D228EF67CC648E98D7D46BFFEF935FE</CCBE2A083AF44A1A8DD0AB38F174D983>
    </F10988936B3B421BB4FD7E33B9C0DEC9>
  </ItemGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void UpdateItem()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .ItemUpdate(
                    itemType: "E8517D605F70454BA7095F64B0EEB526",
                    update: "F58734B746BF4E76AB71E4151BB15A6F",
                    metadata: new Dictionary<string, string>
                    {
                        { "EDD4A94F3C1241B4A6543E60C96FF51D", "10A8CF193EE54D679F25A8E1F83E0097" }
                    },
                    condition: "8DAE9F4AB6264A9CB2435C482C8B3DBB")
                .Xml
                .ShouldBe(
                    @"<Project>
  <ItemGroup>
    <E8517D605F70454BA7095F64B0EEB526 Update=""F58734B746BF4E76AB71E4151BB15A6F"" Condition=""8DAE9F4AB6264A9CB2435C482C8B3DBB"">
      <EDD4A94F3C1241B4A6543E60C96FF51D>10A8CF193EE54D679F25A8E1F83E0097</EDD4A94F3C1241B4A6543E60C96FF51D>
    </E8517D605F70454BA7095F64B0EEB526>
  </ItemGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }
    }
}