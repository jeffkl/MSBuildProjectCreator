// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Evaluation;
using Shouldly;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class LinqTests : MSBuildTestBase
    {
        [Fact]
        public void ForEachItems()
        {
            ProjectCreator.Create(projectFileOptions: NewProjectFileOptions.None)
                .ItemGroup()
                .ForEach(
                    new[]
                    {
                        "DD5FA432D4024EDE9AAD31C830C0777B",
                        "5F4B1101B5624C7A9664E2608CCD7F9A",
                        "C17A88666FF843789F3CCA1F28EA950B",
                    },
                    (item, i) => i.ItemInclude("MyItem", item))
                .Xml
                .ShouldBe(
                    @"<Project>
  <ItemGroup>
    <MyItem Include=""DD5FA432D4024EDE9AAD31C830C0777B"" />
    <MyItem Include=""5F4B1101B5624C7A9664E2608CCD7F9A"" />
    <MyItem Include=""C17A88666FF843789F3CCA1F28EA950B"" />
  </ItemGroup>
</Project>",
                    StringCompareShould.IgnoreLineEndings);
        }
    }
}