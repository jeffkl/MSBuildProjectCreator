// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Shouldly;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
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

        protected byte[]? GetFileBytes(string packageFullPath, string filePath)
        {
            using ZipArchive nupkg = GetPackageArchive(packageFullPath);

            return ReadFile(nupkg, filePath, (stream, archiveEntry) =>
            {
                using BinaryReader binaryReader = new BinaryReader(stream);

                return binaryReader.ReadBytes((int)archiveEntry.Length);
            });
        }

        protected string? GetFileContents(string packageFullPath, string filePath)
        {
            using ZipArchive nupkg = GetPackageArchive(packageFullPath);

            return ReadFile(nupkg, filePath, (stream, _) =>
            {
                using TextReader reader = new StreamReader(stream);

                return reader.ReadToEnd();
            });
        }

        protected string GetFileContents(string packageFullPath, Func<string, bool> fileFunc)
        {
            using ZipArchive nupkg = GetPackageArchive(packageFullPath);

            return ReadFile(nupkg, fileFunc, (stream, _) =>
            {
                using TextReader reader = new StreamReader(stream);

                return reader.ReadToEnd();
            });
        }

        protected NuspecReader GetNuspec(Package package)
        {
            package.FullPath.ShouldNotBeNull();

            return new NuspecReader(GetFileContents(package.FullPath, filePath => filePath.EndsWith($"{package.Id}.nuspec", StringComparison.OrdinalIgnoreCase)));
        }

        protected Assembly? LoadAssembly(string packageFullPath, string filePath)
        {
            byte[]? bytes = GetFileBytes(packageFullPath, filePath);

            return bytes == null ? null : Assembly.Load(bytes);
        }

        protected void ShouldThrowExceptionIfNoPackageAdded(Action action)
        {
            InvalidOperationException exception = Should.Throw<InvalidOperationException>(action);

            exception.Message.ShouldBe(Strings.ErrorWhenAddingAnythingBeforePackage);
        }

        private T? ReadFile<T>(ZipArchive nupkg, string filePath, Func<Stream, ZipArchiveEntry, T> streamFunc)
        {
            ZipArchiveEntry? archiveEntry = nupkg.GetEntry(filePath);

            if (archiveEntry == null)
            {
                return default;
            }

            using Stream fileStream = archiveEntry.Open();

            return streamFunc(fileStream, archiveEntry);
        }

        private T ReadFile<T>(ZipArchive nupkg, Func<string, bool> fileFunc, Func<Stream, ZipArchiveEntry, T> streamFunc)
        {
            ZipArchiveEntry? archiveEntry = nupkg.Entries.FirstOrDefault(i => fileFunc(i.FullName));

            archiveEntry.ShouldNotBeNull();

            using Stream fileStream = archiveEntry.Open();

            return streamFunc(fileStream, archiveEntry);
        }

        private ZipArchive GetPackageArchive(string fullPath)
        {
            return new ZipArchive(File.OpenRead(fullPath), ZipArchiveMode.Read, leaveOpen: false);
        }
    }
}