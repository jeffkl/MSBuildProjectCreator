// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Packaging;
using System;
using System.IO;
using Xunit.Abstractions;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public abstract class TestBase : MSBuildTestBase, IDisposable
    {
        private readonly string _currentDirectoryBackup;

        private readonly Lazy<object> _pathResolverLazy;

        private readonly string _testRootPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        protected TestBase()
        {
            string globalJson = Path.Combine(TestRootPath, "global.json");
#if NETCOREAPP3_1
            File.WriteAllText(
                globalJson,
                @"{
   ""sdk"": {
    ""version"": ""3.1.100"",
    ""rollForward"": ""latestFeature""
  }
}");
#else
            File.WriteAllText(
                globalJson,
                @"{
   ""sdk"": {
    ""version"": ""5.0.100"",
    ""rollForward"": ""latestMinor""
  }
}");
#endif

            string nugetConfig = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <packageSources>
    <clear />
    <add key=""NuGet.org"" value=""https://api.nuget.org/v3/index.json"" />
  </packageSources>
</configuration>";

            File.WriteAllText("NuGet.config", nugetConfig);

            // Save the current directory to restore it later
            _currentDirectoryBackup = Environment.CurrentDirectory;

            Environment.CurrentDirectory = TestRootPath;

            _pathResolverLazy = new Lazy<object>(() => new VersionFolderPathResolver(Path.Combine(TestRootPath, ".nuget", "packages")));
        }

        public string TestRootPath
        {
            get
            {
                Directory.CreateDirectory(_testRootPath);
                return _testRootPath;
            }
        }

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
                if (Directory.Exists(_currentDirectoryBackup))
                {
                    Environment.CurrentDirectory = _currentDirectoryBackup;
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