// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Shouldly;
using Xunit;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class NuspecReaderTests
    {
        [Fact]
        public void LicenseExpressionIsNullForFileLicenses()
        {
            string contents =
@"<package xmlns=""http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd"">
  <metadata>
    <id>PackageL</id>
    <version>16.4.60</version>
    <authors>UserA</authors>
    <requireLicenseAcceptance>false</requireLicenseAcceptance>
    <license type=""file"">LICENSE</license>
    <packageTypes>
      <packageType name=""Dependency"" />
    </packageTypes>
  </metadata>
</package>";

            NuspecReader nuspec = new NuspecReader(contents);

            nuspec.License.ShouldBe("LICENSE");
            nuspec.LicenseExpression.ShouldBeNull();
            nuspec.LicenseType.ShouldBe("file");
            nuspec.LicenseVersion.ShouldBeNull();
            nuspec.LicenseUrl.ShouldBeNull();
        }
    }
}