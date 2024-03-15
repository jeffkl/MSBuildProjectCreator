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

        // List of namespaces from https://github.com/NuGet/NuGet.Client/blob/6917e6c883d45f45672e20d4b1c45758dad2fa84/src/NuGet.Core/NuGet.Packaging/PackageCreation/Authoring/ManifestSchemaUtility.cs#L23-L50
        [Theory]
        [InlineData("http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd")]
        [InlineData("http://schemas.microsoft.com/packaging/2011/08/nuspec.xsd")]
        [InlineData("http://schemas.microsoft.com/packaging/2011/10/nuspec.xsd")]
        [InlineData("http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd")]
        [InlineData("http://schemas.microsoft.com/packaging/2013/01/nuspec.xsd")]
        [InlineData("http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd")]
        public void ReaderCanParseKnownXmlNamespaces(string xmlns)
        {
            string contents =
$@"<package xmlns=""{xmlns}"">
  <metadata>
    <id>PackageL</id>
    <version>16.4.60</version>
  </metadata>
</package>";

            NuspecReader nuspec = new NuspecReader(contents);

            nuspec.Id.ShouldBe("PackageL");
            nuspec.Version.ShouldBe("16.4.60");
        }
    }
}