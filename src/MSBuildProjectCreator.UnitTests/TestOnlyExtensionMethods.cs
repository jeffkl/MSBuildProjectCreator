// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public static class TestOnlyExtensionMethods
    {
        public static ProjectCreator ForTestingOnly(this ProjectCreator creator, string param1, string param2)
        {
            return creator
                .Import(param1, param2);
        }

        public static ProjectCreator TestingOnlyTemplate(this ProjectCreatorTemplates template, NewProjectFileOptions projectFileOptions, string param1 = null, string param2 = null)
        {
            return ProjectCreator.Create(projectFileOptions: projectFileOptions)
                .Import(param1, param2);
        }
    }
}