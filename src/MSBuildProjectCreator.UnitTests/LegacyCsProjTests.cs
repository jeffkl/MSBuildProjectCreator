// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class LegacyCsProjTests
    {
        [Fact]
        public void TemplateWithCustomContents()
        {
            ProjectCreator.Templates.LegacyCsproj(
                projectGuid: "{C990AF6A-8FE6-4285-A018-7045AC189DC0}",
                projectCreator: projectCreator =>
                {
                    projectCreator
                        .ItemGroup()
                        .ItemCompile("Class1.cs")
                        .ItemCompile(@"Properties\AssemblyInfo.cs")
                        .ItemGroup()
                        .ItemNone(
                            "App.config",
                            metadata: new Dictionary<string, string>
                            {
                                { "SubType", "Designer" }
                            });
                })
                .Xml
                .ShouldBe(
                    @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" DefaultTargets=""Build"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProjectGuid>{C990AF6A-8FE6-4285-A018-7045AC189DC0}</ProjectGuid>
    <OutputType>Library</OutputType>
    <RootNamespace>ClassLibrary</RootNamespace>
    <AssemblyName>ClassLibrary</AssemblyName>
    <TargetFrameworkVersion>v4.6</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""Microsoft.CSharp"" />
    <Reference Include=""System"" />
    <Reference Include=""System.Core"" />
    <Reference Include=""System.Data"" />
    <Reference Include=""System.Data.DataSetExtensions"" />
    <Reference Include=""System.Net.Http"" />
    <Reference Include=""System.Xml"" />
    <Reference Include=""System.Xml.Linq"" />
  </ItemGroup>
  <ItemGroup>
    <Compile Include=""Class1.cs"" />
    <Compile Include=""Properties\AssemblyInfo.cs"" />
  </ItemGroup>
  <ItemGroup>
    <None Include=""App.config"">
      <SubType>Designer</SubType>
    </None>
  </ItemGroup>
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }
    }
}
