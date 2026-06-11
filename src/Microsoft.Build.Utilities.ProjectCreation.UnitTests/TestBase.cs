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

        public string TargetFramework
        {
            get =>
#if NET8_0
            "net8.0";
#elif NET10_0
            "net10.0";
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
    ""version"": ""{GetDotNetSdkVersionString()}"",
    ""rollForward"": ""latestMinor""
  }}
}}");
            string GetDotNetSdkVersionString()
            {
#if NET8_0
            return "8.0.100";
#elif NET10_0
            return "10.0.100";
#elif NETFRAMEWORK
                System.Diagnostics.FileVersionInfo fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(MSBuildAssemblyResolver.MSBuildExePath);

                return fileVersionInfo.FileMajorPart switch
                {
                    17 => "8.0.100",
                    18 => "10.0.100",
                    _ => throw new InvalidOperationException($"Unexpected MSBuild version: {fileVersionInfo.FileMajorPart}"),
                };
#else
#error Unknown target framework!
#endif
            }
        }
    }
}
