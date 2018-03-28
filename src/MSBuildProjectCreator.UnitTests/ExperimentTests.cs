// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Framework;
using System.Collections.Generic;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class ExperimentTests
    {
#if DEBUG
        [Fact]
#else
        [Fact(Skip = "Used for experimenting")]
#endif
        public void Experiment1()
        {
            ProjectCreator projectA = ProjectCreator.Templates.LegacyCsproj(
                path: "ProjectA",
                projectCreator: projectCreator =>
                {
                    projectCreator
                        .ItemGroup()
                        .ItemCompile("Class1.cs")
                        .ItemCompile(@"Properties\AssemblyInfo.cs")
                        .ItemGroup()
                        .ItemNone("App.config", metadata: new Dictionary<string, string> { { "SubType", "Designer" } });
                });

            ProjectCreator.Create("dirs.proj")
                .PropertyGroup()
                .Property("Foo", "bar")
                .ItemProjectReference(projectA)
                .ItemPackageReference("Newtonsoft.Json", "10.0.3")
                .ItemGroup()
                .ItemReference(@"C:\foo.dll", "Foo")
                .Target("Build")
                .TaskMessage("Hello World", MessageImportance.High)
                .Task("Message", parameters: new Dictionary<string, string>
                {
                    { "Text", "Hello World" },
                    { "Importance", "High" }
                })
                .TryBuild(out bool _, out BuildOutput _);

            ProjectCreator
                .Templates
                .LogsMessage("Hello World", path: "abc.proj")
                .TryBuild(out bool _, out BuildOutput _);
        }
    }
}