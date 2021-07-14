// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Provides a base class for unit test classes that use MSBuild.  This class resolves MSBuild related assemblies automatically.
    /// </summary>
    public abstract class MSBuildTestBase
    {
        static MSBuildTestBase()
        {
            Environment.SetEnvironmentVariable("MSBUILD_EXE_PATH", string.IsNullOrWhiteSpace(MSBuildAssemblyResolver.DotNetSdksPath) ? null : Path.Combine(MSBuildAssemblyResolver.DotNetSdksPath, "MSBuild.dll"));
            Environment.SetEnvironmentVariable("MSBuildExtensionsPath", MSBuildAssemblyResolver.DotNetSdksPath);
            Environment.SetEnvironmentVariable("MSBuildSDKsPath", string.IsNullOrWhiteSpace(MSBuildAssemblyResolver.DotNetSdksPath) ? null : Path.Combine(MSBuildAssemblyResolver.DotNetSdksPath, "Sdks"));

            AppDomain.CurrentDomain.AssemblyResolve += MSBuildAssemblyResolver.AssemblyResolve;
        }
    }
}