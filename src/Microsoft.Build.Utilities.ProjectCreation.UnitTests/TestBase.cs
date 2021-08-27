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
#elif NET5_0
            "net5.0";
#elif NET6_0
            "net6.0";
#else
            "net472";
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
    }
}