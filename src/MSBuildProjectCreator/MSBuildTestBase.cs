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
        static MSBuildTestBase()
        {
            AppDomain.CurrentDomain.AssemblyResolve += MSBuildAssemblyResolver.AssemblyResolve;
        }
    }
}
