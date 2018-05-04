// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using System;
using System.IO;

// ReSharper disable once CheckNamespace
namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Represents project templates.
    /// </summary>
    public partial class ProjectCreatorTemplates
    {
        /// <summary>
        /// Creates a legacy C# project.
        /// </summary>
        /// <param name="path">An optional relative or full path for the project.</param>
        /// <param name="outputType">An optional output type for the project.</param>
        /// <param name="targetFrameworkVersion">An optional target framework version for the project.</param>
        /// <param name="rootNamespace">An optional root namespace for the project.</param>
        /// <param name="assemblyName">An optional assembly name for the project.</param>
        /// <param name="defaultConfiguration">An optional default value for the Configuration property.</param>
        /// <param name="defaultPlatform">An optional default value for the Platform property.</param>
        /// <param name="projectGuid">An optional GUID for the project.  If none is specified, one is generated.</param>
        /// <param name="fileAlignment">An optional file alignment for the project.</param>
        /// <param name="projectCreator">An optional <see cref="Action{ProjectCreator}"/> delegate to call in the body of the project.</param>
        /// <param name="defaultTargets">An optional list of default targets for the project.</param>
        /// <param name="initialTargets">An optional list of initial targets for the project.</param>
        /// <param name="toolsVersion">An optional tools version for the project.</param>
        /// <param name="treatAsLocalProperty">An optional list of properties to treat as local properties.</param>
        /// <param name="projectCollection">An optional <see cref="ProjectCollection"/> to use when loading the project.</param>
        /// <param name="projectFileOptions">An optional <see cref="NewProjectFileOptions"/> specifying options when creating a new file.</param>
        /// <returns>A <see cref="ProjectCreator"/> object that is used to construct an MSBuild project.</returns>
        public ProjectCreator LegacyCsproj(
            string path = null,
            string outputType = "Library",
            string targetFrameworkVersion = "v4.6",
            string rootNamespace = "ClassLibrary",
            string assemblyName = "ClassLibrary",
            string defaultConfiguration = "Debug",
            string defaultPlatform = "AnyCPU",
            string projectGuid = null,
            string fileAlignment = "512",
            Action<ProjectCreator> projectCreator = null,
            string defaultTargets = "Build",
            string initialTargets = null,
            string toolsVersion = null,
            string treatAsLocalProperty = null,
            ProjectCollection projectCollection = null,
            NewProjectFileOptions? projectFileOptions = NewProjectFileOptions.IncludeAllOptions)
        {
            if (path != null)
            {
                rootNamespace = assemblyName = Path.GetFileNameWithoutExtension(path);
            }

            return ProjectCreator.Create(
                    path: path,
                    defaultTargets: defaultTargets,
                    initialTargets: initialTargets,
                    sdk: null,
                    toolsVersion: toolsVersion,
                    treatAsLocalProperty: treatAsLocalProperty,
                    projectCollection: projectCollection,
                    projectFileOptions: projectFileOptions)
                .Import(@"$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props", conditionOnExistence: true)
                .PropertyGroup()
                    .Property("Configuration", defaultConfiguration, setIfEmpty: true)
                    .Property("Platform", defaultPlatform, setIfEmpty: true)
                    .Property("ProjectGuid", projectGuid ?? Guid.NewGuid().ToString("B").ToUpperInvariant())
                    .Property("OutputType", outputType)
                    .Property("RootNamespace", rootNamespace)
                    .Property("AssemblyName", assemblyName)
                    .Property("TargetFrameworkVersion", targetFrameworkVersion)
                    .Property("FileAlignment", fileAlignment)
                .PropertyGroup(" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ")
                    .Property("DebugSymbols", "true")
                    .Property("DebugType", "full")
                    .Property("Optimize", "false")
                    .Property("OutputPath", @"bin\Debug")
                    .Property("DefineConstants", "DEBUG;TRACE")
                    .Property("ErrorReport", "prompt")
                    .Property("WarningLevel", "4")
                .PropertyGroup(" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ")
                    .Property("DebugType", "pdbonly")
                    .Property("Optimize", "true")
                    .Property("OutputPath", @"bin\Release")
                    .Property("DefineConstants", "TRACE")
                    .Property("ErrorReport", "prompt")
                    .Property("WarningLevel", "4")
                .ItemGroup()
                    .ItemReference("System")
                    .ItemReference("System.Core")
                    .ItemReference("System.Xml.Linq")
                    .ItemReference("System.Data.DataSetExtensions")
                    .ItemReference("Microsoft.CSharp")
                    .ItemReference("System.Data")
                    .ItemReference("System.Net.Http")
                    .ItemReference("System.Xml")
                .CustomAction(projectCreator)
                .Import(@"$(MSBuildToolsPath)\Microsoft.CSharp.targets");
        }
    }
}