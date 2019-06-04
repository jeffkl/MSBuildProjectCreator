// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public abstract class TestBase : MSBuildTestBase
    {
        private readonly string _testRootPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        public string TestRootPath
        {
            get
            {
                Directory.CreateDirectory(_testRootPath);
                return _testRootPath;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Directory.Exists(TestRootPath))
                {
                    Directory.Delete(TestRootPath, recursive: true);
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