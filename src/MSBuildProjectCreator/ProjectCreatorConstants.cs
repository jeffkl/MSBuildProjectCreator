// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Represents constants used by the <see cref="ProjectCreator"/> class.
    /// </summary>
    public static class ProjectCreatorConstants
    {
        /// <summary>
        /// The default target target name to use when adding a target.
        /// </summary>
        public const string DefaultTargetName = "Build";

        /// <summary>
        /// The default SDK to use for SDK-style projects.
        /// </summary>
        public const string SdkCsprojDefaultSdk = "Microsoft.NET.Sdk";

        /// <summary>
        /// The default target framework for SDK-style projects.
        /// </summary>
        public const string SdkCsprojDefaultTargetFramework = "netstandard2.0";
    }
}