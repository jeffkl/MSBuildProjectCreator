// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public abstract class TestBase : MSBuildTestBase, IDisposable
    {
        protected TestBase()
        {
            TestRootPath = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())).FullName;
            WriteGlobalJson();
        }

        public string DotNetSdkVersion
        {
            get =>
#if NETCOREAPP3_1
            "3.1.100";
#elif NET6_0
            "6.0.100";
#elif NET7_0 || NETFRAMEWORK
            "7.0.100";
#else
            Unknown target framework!
#endif
        }

        public string TargetFramework
        {
            get =>
#if NETCOREAPP3_1
            "netcoreapp3.1";
#elif NET6_0
            "net6.0";
#elif NET7_0
            "net7.0";
#elif NETFRAMEWORK
            "net472";
#else
            Unknown target framework!
#endif
        }

        public string TestRootPath { get; }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool isDisposing)
        {
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

        protected string GetTempFileName(string? extension = null)
        {
            return Path.Combine(TestRootPath, $"{Path.GetRandomFileName()}{extension ?? string.Empty}");
        }

        protected string GetTempProjectPath(string? extension = null)
        {
            DirectoryInfo tempDirectoryInfo = Directory.CreateDirectory(Path.Combine(TestRootPath, Path.GetRandomFileName()));

            return Path.Combine(tempDirectoryInfo.FullName, $"{Path.GetRandomFileName()}{extension ?? string.Empty}");
        }

        private void WriteGlobalJson()
        {
            File.WriteAllText(
                Path.Combine(TestRootPath, "global.json"),
                $@"{{
  ""sdk"": {{
    ""version"": ""{DotNetSdkVersion}"",
    ""rollForward"": ""latestMinor""
  }}
}}");
        }
    }
}