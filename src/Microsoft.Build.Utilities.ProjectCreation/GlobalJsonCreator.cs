// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// A fluent API for generating global.json files.
    /// </summary>
    public partial class GlobalJsonCreator
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private FileInfo? _fullPath = null;
        private GlobalJson _globalJson = new();

        private GlobalJsonCreator()
        {
        }

        /// <summary>
        /// Gets the full path to the global.json file.
        /// </summary>
        public string? FullPath => _fullPath?.FullName;

        /// <summary>
        /// Implicitly converts a <see cref="GlobalJsonCreator" /> to a string.
        /// </summary>
        /// <param name="creator">The <see cref="GlobalJsonCreator" /> to convert.</param>
        public static implicit operator string(GlobalJsonCreator creator) => creator.ToJson();

        /// <summary>
        /// Creates a new <see cref="GlobalJsonCreator" /> instance.
        /// </summary>
        /// <returns>A <see cref="GlobalJsonCreator" /> object used to construct a global.json file.</returns>
        public static GlobalJsonCreator Create()
        {
            return new GlobalJsonCreator();
        }

        /// <summary>
        /// Creates a new <see cref="GlobalJsonCreator" /> instance.
        /// </summary>
        /// <param name="directory">The directory where the global.json file will be saved.</param>
        /// <param name="sdkVersion">The SDK version to use.</param>
        /// <param name="rollForward">The optional SDK roll-forward policy.</param>
        /// <param name="allowPrerelease">Whether to allow prerelease SDK versions.</param>
        /// <returns>A <see cref="GlobalJsonCreator" /> object used to construct a global.json file.</returns>
        public static GlobalJsonCreator Create(
            DirectoryInfo? directory = null,
            string? sdkVersion = null,
            GlobalJsonSdkRollForward? rollForward = null,
            bool? allowPrerelease = null)
        {
            GlobalJsonCreator globalJsonCreator = new GlobalJsonCreator()
                .SdkVersion(sdkVersion)
                .SdkRollForward(rollForward)
                .SdkAllowPrerelease(allowPrerelease);

            if (directory != null)
            {
                FileInfo fullPath = new(Path.Combine(directory.FullName, "global.json"));

                globalJsonCreator._fullPath = fullPath;
            }

            return globalJsonCreator;
        }

        /// <summary>
        /// Creates a new <see cref="GlobalJsonCreator" /> instance.
        /// </summary>
        /// <param name="directory">The directory where the global.json file will be saved.</param>
        /// <param name="sdkVersion">The SDK version to use.</param>
        /// <param name="rollForward">The optional SDK roll-forward policy.</param>
        /// <param name="allowPrerelease">Whether to allow prerelease SDK versions.</param>
        /// <returns>A <see cref="GlobalJsonCreator" /> object used to construct a global.json file.</returns>
        public static GlobalJsonCreator Create(
            string? directory,
            string? sdkVersion,
            GlobalJsonSdkRollForward? rollForward = null,
            bool? allowPrerelease = null)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentNullException(nameof(directory));
            }

            return Create(new DirectoryInfo(directory), sdkVersion, rollForward, allowPrerelease);
        }

        /// <summary>
        /// Adds an MSBuild SDK to the msbuild-sdks section of the global.json file.
        /// </summary>
        /// <param name="name">The name of the MSBuild SDK.</param>
        /// <param name="version">The version of the MSBuild SDK.</param>
        /// <returns>The current <see cref="GlobalJsonCreator" />.</returns>
        public GlobalJsonCreator MSBuildSdk(string name, string version)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentNullException(nameof(version));
            }

            _globalJson.MSBuildSdks ??= [];

            _globalJson.MSBuildSdks[name] = version;

            return this;
        }

        /// <summary>
        /// Saves the global.json file.
        /// </summary>
        /// <returns>The current <see cref="GlobalJsonCreator" />.</returns>
        public GlobalJsonCreator Save()
        {
            if (_fullPath is null)
            {
                throw new InvalidOperationException(Strings.ErrorGlobalJsonDirectoryNotSpecified);
            }

            _fullPath.Directory!.Create();

            string json = ToJson();

            File.WriteAllText(_fullPath.FullName, json, Encoding.UTF8);

            return this;
        }

        /// <summary>
        /// Saves the global.json file to a specified directory.
        /// </summary>
        /// <param name="directory">The directory to save the global.json file to.</param>
        /// <returns>The current <see cref="GlobalJsonCreator" />.</returns>
        /// <exception cref="ArgumentNullException">If no directory was specified when creating the <see cref="GlobalJsonCreator" /> instance.</exception>
        public GlobalJsonCreator Save(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
            {
                throw new ArgumentNullException(nameof(directory));
            }

            Save(new DirectoryInfo(directory));

            return this;
        }

        /// <summary>
        /// Saves the global.json file to a specified directory.
        /// </summary>
        /// <param name="directory">A <see cref="DirectoryInfo" /> representing the directory to save the global.json file to.</param>
        /// <returns>The current <see cref="GlobalJsonCreator" />.</returns>
        public GlobalJsonCreator Save(DirectoryInfo directory)
        {
            if (directory is null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            _fullPath = new FileInfo(Path.Combine(directory.FullName, "global.json"));

            Save();

            return this;
        }

        /// <summary>
        /// Sets whether to allow prerelease SDK versions.
        /// </summary>
        /// <param name="allowPrerelease">A value indicating whether to allow prerelease SDK versions.</param>
        /// <returns>The current <see cref="GlobalJsonCreator" />.</returns>
        public GlobalJsonCreator SdkAllowPrerelease(bool? allowPrerelease)
        {
            _globalJson.Sdk ??= new GlobalJsonSdk();

            _globalJson.Sdk.AllowPrerelease = allowPrerelease;

            return this;
        }

        /// <summary>
        /// Sets a custom error message displayed when the SDK resolver can't find a compatible .NET SDK.
        /// </summary>
        /// <param name="message">A custom error message displayed when the SDK resolver can't find a compatible .NET SDK.</param>
        /// <returns>The current <see cref="GlobalJsonCreator" />.</returns>
        public GlobalJsonCreator SdkErrorMessage(string? message)
        {
            _globalJson.Sdk ??= new GlobalJsonSdk();

            _globalJson.Sdk.ErrorMessage = message;

            return this;
        }

        /// <summary>
        /// Adds a location that should be considered when searching for a compatible .NET SDK.
        /// </summary>
        /// <param name="path">The location that should be considered when searching for a compatible .NET SDK. Paths can be absolute or relative to the location of the global.json file. The special value $host$ represents the location corresponding to the running dotnet executable.
        /// These paths are searched in the order they're defined and the first matching SDK is used.
        /// This feature enables using local SDK installations (such as SDKs relative to a repository root or placed in a custom folder) that aren't installed globally on the system.
        /// </param>
        /// <remarks>
        /// The "paths" feature only works when using commands that engage the.NET SDK, such as dotnet run.It does NOT affect scenarios such as running the native apphost launcher (app.exe), running with dotnet app.dll, or running with dotnet exec app.dll.To use the "paths" feature, you must use SDK commands like dotnet run.
        /// </remarks>
        /// <returns>The current <see cref="GlobalJsonCreator" />.</returns>
        public GlobalJsonCreator SdkPath(string path)
        {
            _globalJson.Sdk ??= new GlobalJsonSdk();

            _globalJson.Sdk.Paths ??= [];

            _globalJson.Sdk.Paths.Add(path);

            return this;
        }

        /// <summary>
        /// Sets the roll-forward policy to use.
        /// </summary>
        /// <param name="rollForward">The roll-forward policy to use when selecting an SDK version, either as a fallback when a specific SDK version is missing or as a directive to use a later version. A version must be specified with a rollForward value, unless you're setting it to latestMajor. The default roll forward behavior is determined by the matching rules.</param>
        /// <returns>The current <see cref="GlobalJsonCreator" />.</returns>
        public GlobalJsonCreator SdkRollForward(GlobalJsonSdkRollForward? rollForward)
        {
            _globalJson.Sdk ??= new GlobalJsonSdk();

            _globalJson.Sdk.RollForward = rollForward;

            return this;
        }

        /// <summary>
        /// Sets the version of the .NET SDK to use.
        /// </summary>
        /// <param name="sdkVersion">
        /// The version of the .NET SDK to use. This field:
        /// <list type="bullet">
        /// <item>Requires the full version number, such as 10.0.100.</item>
        /// <item>Doesn't support version numbers like 10, 10.0, or 10.0.x.</item>
        /// <item>Doesn't have wildcard support.</item>
        /// <item>Doesn't support version ranges.</item>
        /// </list>
        /// </param>
        /// <returns>The current <see cref="GlobalJsonCreator" />.</returns>
        public GlobalJsonCreator SdkVersion(string? sdkVersion)
        {
            _globalJson.Sdk ??= new GlobalJsonSdk();

            _globalJson.Sdk.Version = sdkVersion;

            return this;
        }

        /// <summary>
        /// Adds a test runner to discover/run tests with.
        /// </summary>
        /// <param name="name">The name of the test runner.</param>
        /// <returns>The current <see cref="GlobalJsonCreator" />.</returns>
        public GlobalJsonCreator TestRunner(string name)
        {
            _globalJson.Test ??= new GlobalJsonTest();

            _globalJson.Test.Runner = name;

            return this;
        }

        /// <summary>
        /// Gets the JSON representation of the global.json file.
        /// </summary>
        /// <returns>A JSON string representing the global.json file.</returns>
        public string ToJson() => JsonSerializer.Serialize(_globalJson, JsonSerializerOptions);

        internal record GlobalJson
        {
            public GlobalJsonSdk? Sdk { get; set; }

            [JsonPropertyName("msbuild-sdks")]
            public Dictionary<string, string>? MSBuildSdks { get; set; }

            public GlobalJsonTest? Test { get; set; }
        }

        internal record GlobalJsonSdk
        {
            public string? Version { get; set; }

            public bool? AllowPrerelease { get; set; }

            public GlobalJsonSdkRollForward? RollForward { get; set; }

            public List<string>? Paths { get; set; }

            public string? ErrorMessage { get; set; }
        }

        internal record GlobalJsonTest
        {
            public string? Runner { get; set; }
        }
    }
}
