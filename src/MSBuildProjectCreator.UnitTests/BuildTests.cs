// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Execution;
using Shouldly;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class BuildTests : TestBase
    {
        [Fact]
        public void BuildTargetOutputsTest()
        {
            ProjectCreator
                .Create(Path.Combine(TestRootPath, "project1.proj"))
                .Target("Build", returns: "@(MyItems)")
                .TargetItemInclude("MyItems", "E32099C7AF4E481885B624E5600C718A")
                .TargetItemInclude("MyItems", "7F38E64414104C6182F492B535926187")
                .Save()
                .TryBuild("Build", out bool result, out BuildOutput _, out IDictionary<string, TargetResult> targetOutputs);

            result.ShouldBeTrue();

            KeyValuePair<string, TargetResult> item = targetOutputs.ShouldHaveSingleItem();

            item.Key.ShouldBe("Build");

            item.Value.Items.Select(i => i.ItemSpec).ShouldBe(new[] { "E32099C7AF4E481885B624E5600C718A", "7F38E64414104C6182F492B535926187" });
        }

        [Fact]
        public void RestoreTargetCanBeRun()
        {
            ProjectCreator
                .Create(Path.Combine(TestRootPath, "project1.proj"))
                .Target("Restore")
                    .TaskMessage("312D2E6ABDDC4735B437A016CED1A68E", Framework.MessageImportance.High, condition: "'$(MSBuildRestoreSessionId)' != ''")
                    .TaskError("MSBuildRestoreSessionId was not defined", condition: "'$(MSBuildRestoreSessionId)' == ''")
                .TryRestore(out bool result, out BuildOutput buildOutput);

            result.ShouldBeTrue(buildOutput.GetConsoleLog());

            buildOutput.MessageEvents.High.ShouldContain(i => i.Message == "312D2E6ABDDC4735B437A016CED1A68E" && i.Importance == Framework.MessageImportance.High, buildOutput.GetConsoleLog());
        }

        [Fact]
        public void CanRestoreAndBuild()
        {
            ProjectCreator.Create(
                    path: GetTempFileName(".csproj"))
                .Target("Restore")
                    .TaskMessage("Restoring...", Framework.MessageImportance.High)
                .Target("Build")
                    .TaskMessage("Building...", Framework.MessageImportance.High)
                .Save()
                .TryBuild(restore: true, "Build", out bool result, out BuildOutput buildOutput);

            result.ShouldBeTrue(buildOutput.GetConsoleLog());

            buildOutput.MessageEvents.High.ShouldContain(i => i.Message == "Restoring...", buildOutput.GetConsoleLog());

            buildOutput.MessageEvents.High.ShouldContain(i => i.Message == "Building...", buildOutput.GetConsoleLog());
        }
    }
}