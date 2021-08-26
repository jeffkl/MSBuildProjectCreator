// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Packaging;
using Shouldly;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests.PackageFeedTests
{
    public abstract class PackageFeedTestBase : TestBase
    {
        protected PackageFeedTestBase()
        {
            FeedRootPath = Directory.CreateDirectory(Path.Combine(TestRootPath, "Feed"));
        }

        /// <summary>
        /// Gets a <see cref="DirectoryInfo" /> of the root of the package feed.
        /// </summary>
        protected DirectoryInfo FeedRootPath { get; }

        protected byte[] GetFileBytes(PackageArchiveReader packageArchiveReader, string filePath)
        {
            return ReadFile(packageArchiveReader, filePath, (stream, archiveEntry) =>
            {
                using BinaryReader binaryReader = new BinaryReader(stream);

                return binaryReader.ReadBytes((int)archiveEntry.Length);
            });
        }

        protected string GetFileContents(PackageArchiveReader packageArchiveReader, string filePath)
        {
            return ReadFile(packageArchiveReader, filePath, (stream, _) =>
            {
                using TextReader reader = new StreamReader(stream);

                return reader.ReadToEnd();
            });
        }

        protected PackageArchiveReader GetPackageArchiveReader(Package package)
        {
            return new PackageArchiveReader(
                new FileStream(package.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096),
                leaveStreamOpen: false);
        }

        protected Assembly LoadAssembly(PackageArchiveReader packageArchiveReader, string filePath)
        {
            byte[] bytes = GetFileBytes(packageArchiveReader, filePath);

            return Assembly.Load(bytes);
        }

        protected void ShouldThrowExceptionIfNoPackageAdded(Action action)
        {
            InvalidOperationException exception = Should.Throw<InvalidOperationException>(action);

            exception.Message.ShouldBe(Strings.ErrorWhenAddingAnythingBeforePackage);
        }

        private T ReadFile<T>(PackageArchiveReader packageArchiveReader, string filePath, Func<Stream, ZipArchiveEntry, T> streamFunc)
        {
            packageArchiveReader.GetFiles().ShouldContain(filePath);

            ZipArchiveEntry archiveEntry = packageArchiveReader.GetEntry(filePath);

            using Stream fileStream = archiveEntry.Open();

            return streamFunc(fileStream, archiveEntry);
        }
    }
}