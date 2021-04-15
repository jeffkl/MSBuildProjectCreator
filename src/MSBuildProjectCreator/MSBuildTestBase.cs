// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Provides a base class for unit test classes that use MSBuild.  This class resolves MSBuild related assemblies automatically.
    /// </summary>
    public abstract class MSBuildTestBase
    {
        private static readonly string[] EnvironmentVariablesToRemove =
        {
            "MSBuildSdksPath",
            "MSBuildExtensionsPath",
        };

        static MSBuildTestBase()
        {
            foreach (string environmentVariableName in EnvironmentVariablesToRemove)
            {
                Environment.SetEnvironmentVariable(environmentVariableName, null);
            }

            AppDomain.CurrentDomain.AssemblyResolve += MSBuildAssemblyResolver.AssemblyResolve;
        }
    }
}