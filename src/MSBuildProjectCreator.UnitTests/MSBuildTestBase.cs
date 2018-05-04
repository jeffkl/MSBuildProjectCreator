// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Locator;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public abstract class MSBuildTestBase
    {
        public static readonly VisualStudioInstance CurrentVisualStudioInstance = MSBuildLocator.RegisterDefaults();

        protected MSBuildTestBase()
        {
            MSBuildPath = CurrentVisualStudioInstance.MSBuildPath;
        }

        protected string MSBuildPath { get; }
    }
}
