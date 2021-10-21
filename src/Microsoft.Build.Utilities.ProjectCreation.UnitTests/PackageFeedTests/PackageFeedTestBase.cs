// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Packaging;
using Shouldly;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

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

        protected byte[] GetFileBytes(string packageFullPath, string filePath)
        {
            using PackageArchiveReader packageArchiveReader = GetPackageArchiveReader(packageFullPath);

            return ReadFile(packageArchiveReader, filePath, (stream, archiveEntry) =>
            {
                using BinaryReader binaryReader = new BinaryReader(stream);

                return binaryReader.ReadBytes((int)archiveEntry.Length);
            });
        }

        protected string GetFileContents(string packageFullPath, string filePath)
        {
            using PackageArchiveReader packageArchiveReader = GetPackageArchiveReader(packageFullPath);

            return ReadFile(packageArchiveReader, filePath, (stream, _) =>
            {
                using TextReader reader = new StreamReader(stream);

                return reader.ReadToEnd();
            });
        }

        protected string GetFileContents(string packageFullPath, Func<string, bool> fileFunc)
        {
            using PackageArchiveReader packageArchiveReader = GetPackageArchiveReader(packageFullPath);

            return ReadFile(packageArchiveReader, fileFunc, (stream, _) =>
            {
                using TextReader reader = new StreamReader(stream);

                return reader.ReadToEnd();
            });
        }

        protected NuspecReader GetNuspecReader(Package package)
        {
            return new NuspecReader(XDocument.Parse(GetFileContents(package.FullPath, filePath => filePath.EndsWith($"{package.Id}.nuspec", StringComparison.OrdinalIgnoreCase))));
        }

        protected Assembly LoadAssembly(string packageFullPath, string filePath)
        {
            byte[] bytes = GetFileBytes(packageFullPath, filePath);

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

        private T ReadFile<T>(PackageArchiveReader packageArchiveReader, Func<string, bool> fileFunc, Func<Stream, ZipArchiveEntry, T> streamFunc)
        {
            string filePath = packageArchiveReader.GetFiles().FirstOrDefault(fileFunc);

            filePath.ShouldNotBeNull();

            ZipArchiveEntry archiveEntry = packageArchiveReader.GetEntry(filePath);

            using Stream fileStream = archiveEntry.Open();

            return streamFunc(fileStream, archiveEntry);
        }

        private PackageArchiveReader GetPackageArchiveReader(string fullPath)
        {
            return new PackageArchiveReader(
                new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096),
                leaveStreamOpen: false);
        }
    }
}