// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Shouldly;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests.PackageFeedTests
{
    public class PackageFeedTemplatesTest : PackageFeedTestBase
    {
        [Fact]
        public void SinglePackageTemplate()
        {
            PackageFeed.Templates.SinglePackage(FeedRootPath, out Package package);

            package.Id.ShouldBe("SomePackage");
            package.Version.ShouldBe("1.0.0");
        }
    }
}