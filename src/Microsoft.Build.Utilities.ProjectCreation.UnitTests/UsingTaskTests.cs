// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Shouldly;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class UsingTaskTests : MSBuildTestBase
    {
        [Fact]
        public void UsingTaskAssemblyFileComplex()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .UsingTaskAssemblyFile(
                    taskName: "BCC53601667C4195A9DCDCEF59C4C0F0",
                    assemblyFile: "125F7786CD46409FA09B4999629BAF4F",
                    taskFactory: "76E271579AB749AE9FE7CBFF73E2B83A",
                    runtime: "9312C4042D974331974FF3706DB0FC48",
                    architecture: "141E4976FAFB4C1B8EE66417EA939743",
                    condition: "C65111E72ADA438AB6DD385F9A1BC887",
                    label: "label")
                .Xml
                .ShouldBe(
                    @"<Project>
  <UsingTask TaskName=""BCC53601667C4195A9DCDCEF59C4C0F0"" Runtime=""9312C4042D974331974FF3706DB0FC48"" Architecture=""141E4976FAFB4C1B8EE66417EA939743"" AssemblyFile=""125F7786CD46409FA09B4999629BAF4F"" TaskFactory=""76E271579AB749AE9FE7CBFF73E2B83A"" Condition=""C65111E72ADA438AB6DD385F9A1BC887"" Label=""label"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void UsingTaskAssemblyFileSimple()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .UsingTaskAssemblyFile("BCC53601667C4195A9DCDCEF59C4C0F0", "125F7786CD46409FA09B4999629BAF4F")
                .Xml
                .ShouldBe(
                    @"<Project>
  <UsingTask TaskName=""BCC53601667C4195A9DCDCEF59C4C0F0"" AssemblyFile=""125F7786CD46409FA09B4999629BAF4F"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(true)]
        [InlineData(false)]
        public void UsingTaskBody(bool? evaluate)
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .UsingTaskAssemblyFile("3F8912C263CA4DAB96C6B72FFCA2564A", "592F50D5289944D78130E4F3B2B2535D", taskFactory: "BA02FBBDEAC84D03AD178D376BEB376F")
                .UsingTaskBody(
                    body: @"<![CDATA[ED112A59822C469D93E9A7E336AE1D96 3986160EE9614D4FB4CA838AC9558E5C 6A170187A26A46D3B3A8CB5D00097F2B]]>",
                    evaluate: evaluate)
                .Xml
                .ShouldBe(
                    $@"<Project>
  <UsingTask TaskName=""3F8912C263CA4DAB96C6B72FFCA2564A"" AssemblyFile=""592F50D5289944D78130E4F3B2B2535D"" TaskFactory=""BA02FBBDEAC84D03AD178D376BEB376F"">
    <Task{(evaluate == null ? string.Empty : $@" Evaluate=""{evaluate.ToString()}""")}><![CDATA[ED112A59822C469D93E9A7E336AE1D96 3986160EE9614D4FB4CA838AC9558E5C 6A170187A26A46D3B3A8CB5D00097F2B]]></Task>
  </UsingTask>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void UsingTaskComplexParameters()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .UsingTaskAssemblyFile("1FD2742ACFA0411F9F76F584D5159D7D", "8F6D3E0F9B4E44BEA35107E418791398", taskFactory: "1391A102F4064BC3B07D909B51057760")
                .UsingTaskParameter(
                    name: "C7796A28546546D98227797F15201385",
                    parameterType: "370804188597444DBF0F389BF3426883",
                    output: false,
                    required: true)
                .UsingTaskParameter(
                    name: "C172F318F7F84B868610677FEC1BC191",
                    parameterType: "5548AA534E444108A9463E6DF83A1B04",
                    output: false,
                    required: true)
                .Xml
                .ShouldBe(
                    @"<Project>
  <UsingTask TaskName=""1FD2742ACFA0411F9F76F584D5159D7D"" AssemblyFile=""8F6D3E0F9B4E44BEA35107E418791398"" TaskFactory=""1391A102F4064BC3B07D909B51057760"">
    <ParameterGroup>
      <C7796A28546546D98227797F15201385 Output=""False"" Required=""True"" ParameterType=""370804188597444DBF0F389BF3426883"" />
      <C172F318F7F84B868610677FEC1BC191 Output=""False"" Required=""True"" ParameterType=""5548AA534E444108A9463E6DF83A1B04"" />
    </ParameterGroup>
  </UsingTask>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Theory]
        [InlineData("""Log.LogMessage(MessageImportance.High, "Hello from an inline task created by Roslyn!");""")]
        [InlineData("""<![CDATA[Log.LogMessage(MessageImportance.High, "Hello from an inline task created by Roslyn!");]]>""")]
        public void UsingTaskInlineFragmentSimple(string code)
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .UsingTaskRoslynCodeTaskFactory("MySample", code)
                .Xml
                .ShouldBe(
                    """
                    <Project>
                      <UsingTask TaskName="MySample" AssemblyFile="$(MSBuildToolsPath)\Microsoft.Build.Tasks.Core.dll" TaskFactory="RoslynCodeTaskFactory">
                        <Task>
                          <Code Type="Fragment" Language="cs"><![CDATA[Log.LogMessage(MessageImportance.High, "Hello from an inline task created by Roslyn!");]]></Code>
                        </Task>
                      </UsingTask>
                    </Project>
                    """,
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void UsingTaskInlineFragmentComplex()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .UsingTaskRoslynCodeTaskFactory(
                    taskName: "MySample",
                    references: ["netstandard"],
                    usings: ["System"],
                    sourceCode: """
                    Log.LogMessage(MessageImportance.High, "Hello from an inline task created by Roslyn!");
                    Log.LogMessageFromText($"Parameter1: '{Parameter1}'", MessageImportance.High);
                    Log.LogMessageFromText($"Parameter2: '{Parameter2}'", MessageImportance.High);
                    Parameter3 = "A value from the Roslyn CodeTaskFactory";
                    """)
                .UsingTaskParameter(
                    name: "Parameter1",
                    parameterType: "System.String",
                    output: false,
                    required: true)
                .UsingTaskParameter(
                    name: "Parameter2",
                    parameterType: "System.String",
                    output: false,
                    required: false)
                .UsingTaskParameter(
                    name: "Parameter3",
                    parameterType: "System.String",
                    output: true,
                    required: false)
                .Xml
                .ShouldBe(
                    """
                    <Project>
                      <UsingTask TaskName="MySample" AssemblyFile="$(MSBuildToolsPath)\Microsoft.Build.Tasks.Core.dll" TaskFactory="RoslynCodeTaskFactory">
                        <ParameterGroup>
                          <Parameter1 Output="False" Required="True" ParameterType="System.String" />
                          <Parameter2 Output="False" Required="False" ParameterType="System.String" />
                          <Parameter3 Output="True" Required="False" ParameterType="System.String" />
                        </ParameterGroup>
                        <Task>
                          <Reference Include="netstandard" />
                          <Using Namespace="System" />
                          <Code Type="Fragment" Language="cs"><![CDATA[Log.LogMessage(MessageImportance.High, "Hello from an inline task created by Roslyn!");
                    Log.LogMessageFromText($"Parameter1: '{Parameter1}'", MessageImportance.High);
                    Log.LogMessageFromText($"Parameter2: '{Parameter2}'", MessageImportance.High);
                    Parameter3 = "A value from the Roslyn CodeTaskFactory";]]></Code>
                        </Task>
                      </UsingTask>
                    </Project>
                    """,
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void UsingTaskInlineSource()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .UsingTaskRoslynCodeTaskFactory(
                    taskName: "MySample",
                    sourcePath: "MySample.vb",
                    type: "Class",
                    language: "vb")
                .Xml
                .ShouldBe(
                    """
                    <Project>
                      <UsingTask TaskName="MySample" AssemblyFile="$(MSBuildToolsPath)\Microsoft.Build.Tasks.Core.dll" TaskFactory="RoslynCodeTaskFactory">
                        <Task>
                          <Code Type="Class" Language="vb" Source="MySample.vb" />
                        </Task>
                      </UsingTask>
                    </Project>
                    """,
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void UsingTaskSimpleParameter()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .UsingTaskAssemblyFile("EED0CD5ACC3B4B9CA8AE6934385F98D7", "759FA929FFC94FBEA0BF2B8D9B2011DE", taskFactory: "614BDFC1A9104F66801575144AC622C0")
                .UsingTaskParameter("E0AF7FD52C1F41AD81525C46A91B006F")
                .Xml
                .ShouldBe(
                    @"<Project>
  <UsingTask TaskName=""EED0CD5ACC3B4B9CA8AE6934385F98D7"" AssemblyFile=""759FA929FFC94FBEA0BF2B8D9B2011DE"" TaskFactory=""614BDFC1A9104F66801575144AC622C0"">
    <ParameterGroup>
      <E0AF7FD52C1F41AD81525C46A91B006F />
    </ParameterGroup>
  </UsingTask>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Fact]
        public void UsingTaskThrowsIfBodySetBeforeUsingTask()
        {
            Should.Throw<ProjectCreatorException>(() =>
                    ProjectCreator.Create()
                        .UsingTaskBody("34EDC44813164B1BB1CBA086703FE33B"))
                .Message.ShouldBe(Strings.ErrorUsingTaskBodyRequiresUsingTask);
        }

        [Fact]
        public void UsingTaskThrowsIfBodySetTwice()
        {
            Should.Throw<ProjectCreatorException>(() =>
                    ProjectCreator.Create()
                        .UsingTaskAssemblyFile("FF22A8CA420D40E0B47F1CE86BEA6FE5", "B67FAA9B113E459E9F63097A4FD28CB4", "3996CE3460F04946893E6B2FDADF2F8B")
                        .UsingTaskBody("28A7D7280BAF4EF3A762C3F6FC0CFEFD")
                        .UsingTaskBody("821B23C0FBB448EEBCE5D445D5E49FA8"))
                .Message.ShouldBe(Strings.ErrorUsingTaskBodyCanOnlyBeSetOnce);
        }

        [Fact]
        public void UsingTaskThrowsIfParameterBeforeUsingTask()
        {
            Should.Throw<ProjectCreatorException>(() =>
                    ProjectCreator.Create()
                        .UsingTaskParameter("FCD109EDDE634D83B7B2D2B5002CAD52"))
                .Message.ShouldBe(Strings.ErrorUsingTaskParameterRequiresUsingTask);
        }
    }
}