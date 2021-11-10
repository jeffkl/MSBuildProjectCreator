// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

#if NETCOREAPP3_1
using System;
using System.Reflection;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Resolves MSBuild assemblies.
    /// </summary>
    public static partial class MSBuildAssemblyResolver
    {
        /// <summary>
        /// A <see cref="ResolveEventHandler" /> for MSBuild related assemblies.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The event data.</param>
        /// <returns>An MSBuild assembly if one could be located, otherwise <c>null</c>.</returns>
        public static Assembly? AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            return AssemblyResolve(new AssemblyName(args.Name!), Assembly.LoadFrom);
        }
    }
}
#endif