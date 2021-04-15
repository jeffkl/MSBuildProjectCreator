// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Packaging;
using System;
using System.IO;
using System.Reflection;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public abstract class TestBase : MSBuildTestBase, IDisposable
    {
        private static readonly string ThisAssemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        private readonly Lazy<object> _pathResolverLazy;

        private readonly Lazy<string> _testRootPathLazy = new Lazy<string>(() => Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())).FullName);

        protected TestBase()
        {
            File.WriteAllText(
                Path.Combine(TestRootPath, "global.json"),
                @"{
   ""sdk"": {
    ""version"": ""5.0.100"",
    ""rollForward"": ""latestMinor""
  }
}");
            File.WriteAllText(
                Path.Combine(TestRootPath, "NuGet.config"),
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <packageSources>
    <clear />
    <add key=""NuGet.org"" value=""https://api.nuget.org/v3/index.json"" />
  </packageSources>
</configuration>");

            Environment.CurrentDirectory = TestRootPath;

            _pathResolverLazy = new Lazy<object>(() => new VersionFolderPathResolver(Path.Combine(TestRootPath, ".nuget", "packages")));
        }

        public string TestRootPath => _testRootPathLazy.Value;

        public object VersionFolderPathResolver => _pathResolverLazy.Value;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Directory.Exists(ThisAssemblyDirectory))
                {
                    Environment.CurrentDirectory = ThisAssemblyDirectory;
                }

                if (Directory.Exists(TestRootPath))
                {
                    try
                    {
                        Directory.Delete(TestRootPath, recursive: true);
                    }
                    catch (Exception)
                    {
                        // Ignored
                    }
                }
            }
        }

        protected string GetTempFileName(string extension = null)
        {
            Directory.CreateDirectory(TestRootPath);

            return Path.Combine(TestRootPath, $"{Path.GetRandomFileName()}{extension ?? string.Empty}");
        }
    }
}