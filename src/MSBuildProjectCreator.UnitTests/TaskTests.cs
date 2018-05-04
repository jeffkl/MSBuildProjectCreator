// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class TaskTests : MSBuildTestBase
    {
        [Fact]
        public void TaskComplex()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Target("961A58B685144822BA9E4EF2DD130D32")
                .Task(
                    name: "B027CE82A5DD4CB4A9CBEE7E95ABE0B4",
                    condition: "4676E6EA62224812BA52836372BD1284",
                    parameters: new Dictionary<string, string>
                    {
                        { "AE9DD9DCD68A457DBA58999FDC498FC4", "6BB6F118F11544D3BD14AF1546FD2EFA" },
                        { "F3D2E68EF8974F7DA6DA52A95A630F63", "94EC352D0BC343B29D605238AD1C926E" }
                    },
                    continueOnError: "45D134C0BBBF4CD4AFE522F024564C40",
                    architecture: "B262E0B911BB4602BCC8C359AFDCE49A",
                    runtime: "8D142986AD3849D29BDA8B14EC1E1693")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Target Name=""961A58B685144822BA9E4EF2DD130D32"">
    <B027CE82A5DD4CB4A9CBEE7E95ABE0B4 ContinueOnError=""45D134C0BBBF4CD4AFE522F024564C40"" Condition=""4676E6EA62224812BA52836372BD1284"" MSBuildArchitecture=""B262E0B911BB4602BCC8C359AFDCE49A"" MSBuildRuntime=""8D142986AD3849D29BDA8B14EC1E1693"" AE9DD9DCD68A457DBA58999FDC498FC4=""6BB6F118F11544D3BD14AF1546FD2EFA"" F3D2E68EF8974F7DA6DA52A95A630F63=""94EC352D0BC343B29D605238AD1C926E"" />
  </Target>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void TaskErrorComplex()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Target("BDB28B57F57C45EE875A0014A04B0066")
                .TaskError(
                    text: "4946953C34DD4D99950CEF02CAD62A9B",
                    code: "041C5C792200468AA202E1857E5CE859",
                    file: "332D350318784FE6884EC45EE018BC56",
                    helpKeyword: "135A42EC8E84460B872E35B0CF42A143",
                    condition: "EFF78CE53810465989A9BDD7A51A8319")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Target Name=""BDB28B57F57C45EE875A0014A04B0066"">
    <Error Condition=""EFF78CE53810465989A9BDD7A51A8319"" Text=""4946953C34DD4D99950CEF02CAD62A9B"" Code=""041C5C792200468AA202E1857E5CE859"" File=""332D350318784FE6884EC45EE018BC56"" HelpKeyword=""135A42EC8E84460B872E35B0CF42A143"" />
  </Target>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void TaskErrorSimple()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Target("2014762990AD4443876DAA34D04DD840")
                .TaskError("06EEEC82451645E38FE1C49F554CD398")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Target Name=""2014762990AD4443876DAA34D04DD840"">
    <Error Text=""06EEEC82451645E38FE1C49F554CD398"" />
  </Target>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void TaskMessageComplex()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Target("4FA5705643E74D8ABF4CB728FB44C113")
                .TaskMessage(
                    text: "41432B9D8D1A4CB9A0C0AA49AC9B87FB",
                    importance: MessageImportance.High,
                    condition: "7B8F0FCEFEC84718AB49B12DC225A699")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Target Name=""4FA5705643E74D8ABF4CB728FB44C113"">
    <Message Condition=""7B8F0FCEFEC84718AB49B12DC225A699"" Text=""41432B9D8D1A4CB9A0C0AA49AC9B87FB"" Importance=""High"" />
  </Target>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void TaskMessageSimple()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Target("89436F50B25E4BF5B2DFA93BD2B084B0")
                .TaskMessage("0F4E52432EE04A29A19A392B76B4F7B4")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Target Name=""89436F50B25E4BF5B2DFA93BD2B084B0"">
    <Message Text=""0F4E52432EE04A29A19A392B76B4F7B4"" />
  </Target>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void TaskSimple()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Task("CAD7EC3FFA634946996F92FA823E3A9D")
                .Xml
                .ShouldBe(
                    $@"<Project>
  <Target Name=""{ProjectCreatorConstants.DefaultTargetName}"">
    <CAD7EC3FFA634946996F92FA823E3A9D />
  </Target>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void TaskWarningComplex()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Target("87EDEB694A6B412C93E8563339CEDB88")
                .TaskWarning(
                    text: "D45047D07E6B44AFAD2E22834AE576A2",
                    code: "D4254C975C5347E3AC184E4935A60A50",
                    file: "0AA3071BC82E4B65836C14726B70977E",
                    helpKeyword: "6CEB0695E6904237BEAEB278530906B4",
                    condition: "C13B210213B34C5AB5BC95577026CABF")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Target Name=""87EDEB694A6B412C93E8563339CEDB88"">
    <Warning Condition=""C13B210213B34C5AB5BC95577026CABF"" Text=""D45047D07E6B44AFAD2E22834AE576A2"" Code=""D4254C975C5347E3AC184E4935A60A50"" File=""0AA3071BC82E4B65836C14726B70977E"" HelpKeyword=""6CEB0695E6904237BEAEB278530906B4"" />
  </Target>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void TaskWarningSimple()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .Target("8728EC428B884F18A35925F55636AAC1")
                .TaskWarning("B43E325C047340279ADB343A17A2D973")
                .Xml
                .ShouldBe(
                    @"<Project>
  <Target Name=""8728EC428B884F18A35925F55636AAC1"">
    <Warning Text=""B43E325C047340279ADB343A17A2D973"" />
  </Target>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }
    }
}