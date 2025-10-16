// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Framework;
using Shouldly;
using System;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class BuildEngineTests : MSBuildTestBase
    {
        [Fact]
        public void ConsoleLog()
        {
            BuildEngine buildEngine = GetBuildEngineWithEvents(i =>
            {
                i.LogErrorEvent(
                    new BuildErrorEventArgs(null, "A6DAB901460D483FBDF3A0980B14C46F", "48F7F352E2914991827100BCEB69331F", 3, 4, 0, 0, "D988473FF8634A16A0CD8FE94FF20D53", null, null)
                    {
                        BuildEventContext = BuildEventContext.Invalid,
                    });
                i.LogWarningEvent(
                    new BuildWarningEventArgs(null, "AE1B25881A694A70B2EA299C04625596", "07006F38A63E420AAB4124EBE58081BC", 1, 2, 0, 0, "3A3DD4A40DA44BA5BBB123E105EE1F71", null, null)
                    {
                        BuildEventContext = BuildEventContext.Invalid,
                    });
                i.LogMessageEvent(
                    new BuildMessageEventArgs("61BD637C7D704D4B98C25805E3111152", null, null, MessageImportance.High)
                    {
                        BuildEventContext = BuildEventContext.Invalid,
                    });
                i.LogMessageEvent(
                    new BuildMessageEventArgs("B02496FA4D3348A6997DC918EBF7455B", null, null, MessageImportance.Normal)
                    {
                        BuildEventContext = BuildEventContext.Invalid,
                    });
                i.LogMessageEvent(
                    new BuildMessageEventArgs("2C254C4346A347AE94AE5E7FB6C03B0C", null, null, MessageImportance.Low)
                    {
                        BuildEventContext = BuildEventContext.Invalid,
                    });
            });

            buildEngine.GetConsoleLog()
                .ShouldBe(
                    @"48F7F352E2914991827100BCEB69331F(3,4): error A6DAB901460D483FBDF3A0980B14C46F: D988473FF8634A16A0CD8FE94FF20D53
07006F38A63E420AAB4124EBE58081BC(1,2): warning AE1B25881A694A70B2EA299C04625596: 3A3DD4A40DA44BA5BBB123E105EE1F71
61BD637C7D704D4B98C25805E3111152
B02496FA4D3348A6997DC918EBF7455B
",
                    StringCompareShould.IgnoreLineEndings);
        }

        [Theory]
        [InlineData("6E1BF8E271E345BC892FA348132C02A9", "28C465226FCF47498C3C728FA8DEC0AB")]
        public void Errors(string expectedMessage, string expectedCode)
        {
            BuildErrorEventArgs args = GetBuildEngineWithEvents(buildEngine => buildEngine.LogErrorEvent(new BuildErrorEventArgs(null, expectedCode, null, 0, 0, 0, 0, expectedMessage, null, null)))
                .ErrorEvents
                .ShouldHaveSingleItem();

            args.Message.ShouldBe(expectedMessage);

            args.Code.ShouldBe(expectedCode);
        }

        [Theory]
        [InlineData("High")]
        [InlineData("Normal")]
        [InlineData("Low")]
        public void MessagesByImportance(string value)
        {
            const string expectedMessage = "D2C36BFAE11847CE95B4135775E0156F";

            MessageImportance importance = (MessageImportance)Enum.Parse(typeof(MessageImportance), value);

            BuildEngine buildEngine = GetBuildEngineWithEvents(i => i.LogMessageEvent(new BuildMessageEventArgs(expectedMessage, null, null, importance)));

            string actualItem = buildEngine.Messages.ShouldHaveSingleItem();

            actualItem.ShouldBe(expectedMessage);

            switch (importance)
            {
                case MessageImportance.High:
                    buildEngine.Messages.High.ShouldHaveSingleItem().ShouldBe(actualItem);
                    break;

                case MessageImportance.Normal:
                    buildEngine.Messages.Normal.ShouldHaveSingleItem().ShouldBe(actualItem);
                    break;

                case MessageImportance.Low:
                    buildEngine.Messages.Low.ShouldHaveSingleItem().ShouldBe(actualItem);
                    break;
            }
        }

        [Theory]
        [InlineData("87647890B61644A1B8CCE28EA7F2CD67", "1684AC30BB494C86AAFE67CD081547F9")]
        public void Warnings(string expectedMessage, string expectedCode)
        {
            BuildWarningEventArgs args = GetBuildEngineWithEvents(buildEngine => buildEngine.LogWarningEvent(new BuildWarningEventArgs(null, expectedCode, null, 0, 0, 0, 0, expectedMessage, null, null)))
                .WarningEvents
                .ShouldHaveSingleItem();

            args.Message.ShouldBe(expectedMessage);

            args.Code.ShouldBe(expectedCode);
        }

        private BuildEngine GetBuildEngineWithEvents(Action<IBuildEngine> action)
        {
            BuildEngine buildEngine = BuildEngine.Create();

            action(buildEngine);

            return buildEngine;
        }
    }
}