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

        protected string GetTempFileName(string extension = null)
        {
            return Path.Combine(TestRootPath, $"{Path.GetRandomFileName()}{extension ?? string.Empty}");
        }

        protected string GetTempProjectPath(string extension = null)
        {
            DirectoryInfo tempDirectoryInfo = Directory.CreateDirectory(Path.Combine(TestRootPath, Path.GetRandomFileName()));

            return Path.Combine(tempDirectoryInfo.FullName, $"{Path.GetRandomFileName()}{extension ?? string.Empty}");
        }
    }
}