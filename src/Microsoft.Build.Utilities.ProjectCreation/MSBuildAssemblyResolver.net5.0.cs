// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

#if NET5_0_OR_GREATER
using System.Reflection;
using System.Runtime.Loader;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Resolves MSBuild assemblies.
    /// </summary>
    public static partial class MSBuildAssemblyResolver
    {
        /// <inheritdoc cref="AssemblyLoadContext.Resolving" />
        public static Assembly? AssemblyResolve(AssemblyLoadContext assemblyLoadContext, AssemblyName requestedAssemblyName)
        {
            return AssemblyResolve(requestedAssemblyName, assemblyLoadContext.LoadFromAssemblyPath);
        }
    }
}
#endif