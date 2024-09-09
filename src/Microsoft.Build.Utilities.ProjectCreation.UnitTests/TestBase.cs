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
#if NET6_0
            "6.0.100";
#elif  NET8_0 || NETFRAMEWORK
            "8.0.100";
#elif  NET9_0 || NETFRAMEWORK
            "9.0.0";
#else
    #error Unknown target framework!
#endif
        }

        public string TargetFramework
        {
            get =>
#if NET6_0
            "net6.0";
#elif NET8_0
            "net8.0";
#elif NET9_0
            "net9.0";
#elif NETFRAMEWORK
            "net472";
#else
    #error Unknown target framework!
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