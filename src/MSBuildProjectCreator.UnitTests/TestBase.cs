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

            File.WriteAllText(
                Path.Combine(TestRootPath, "global.json"),
                @"{
   ""sdk"": {
    ""version"": ""5.0.100"",
    ""rollForward"": ""latestMinor""
  }
}");
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