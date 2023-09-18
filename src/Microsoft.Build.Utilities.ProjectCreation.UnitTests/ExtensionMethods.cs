// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Shouldly;
using System.IO;
using System.Xml.Linq;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    internal static class ExtensionMethods
    {
        public static FileInfo ShouldExist(this FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                throw new ShouldAssertException($"The file \"{fileInfo.FullName}\" should exist but does not");
            }

            return fileInfo;
        }

        public static DirectoryInfo ShouldExist(this DirectoryInfo directoryInfo)
        {
            if (!directoryInfo.Exists)
            {
                throw new ShouldAssertException($"The directory \"{directoryInfo.FullName}\" should exist but does not");
            }

            return directoryInfo;
        }
    }
}