// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Provides a base class for unit test classes that use MSBuild. This class resolves MSBuild related assemblies automatically.
    /// </summary>
    /// <remarks>
    /// Prefer calling <see cref="MSBuildAssemblyResolver.Register"/> from a <c>ModuleInitalizerAttribute</c> if your target framework
    /// supports one. This base class is provided for backwards compatibility with older versions of .NET.
    /// </remarks>
    public abstract class MSBuildTestBase
    {
        static MSBuildTestBase()
        {
            MSBuildAssemblyResolver.Register();
        }
    }
}