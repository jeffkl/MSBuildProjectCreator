// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly GlobalJson _globalJson;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        private GlobalJsonCreator(FileInfo fullPath, string sdkVersion, GlobalJsonSdkRollForward? rollForward, bool? allowPrerelease)
        {
            _globalJson = new GlobalJson(
                fullPath,
                new GlobalJsonSdk
                {
                    Version = sdkVersion,
                    RollForward = rollForward,
                    AllowPrerelease = allowPrerelease,
                });
        }

        /// <summary>
        /// Gets the full path to the global.json file.
        /// </summary>
        public string FullPath => _globalJson.FullPath.FullName;

        /// <summary>
        /// Implicitly converts a <see cref="GlobalJsonCreator" /> to a string.
        /// </summary>
        /// <param name="creator">The <see cref="GlobalJsonCreator" /> to convert.</param>
        public static implicit operator string(GlobalJsonCreator creator) => creator.ToJson();

        /// <summary>
        /// Creates a new <see cref="GlobalJsonCreator" /> instance.
        /// </summary>
        /// <param name="directory">The directory where the global.json file will be saved.</param>
        /// <param name="sdkVersion">The SDK version to use.</param>
        /// <param name="rollForward">The optional SDK roll-forward policy.</param>
        /// <param name="allowPrerelease">Whether to allow prerelease SDK versions.</param>
        /// <returns>A <see cref="GlobalJsonCreator" /> object used to construct a global.json file.</returns>
        public static GlobalJsonCreator Create(
            DirectoryInfo directory,
            string sdkVersion,
            GlobalJsonSdkRollForward? rollForward = null,
            bool? allowPrerelease = null)
        {
            if (string.IsNullOrWhiteSpace(sdkVersion))
            {
                throw new ArgumentNullException(nameof(sdkVersion));
            }

            FileInfo fullPath = new(Path.GetFullPath(Path.Combine(directory.FullName, "global.json")));

            return new GlobalJsonCreator(fullPath, sdkVersion, rollForward, allowPrerelease);
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
            string sdkVersion,
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
            _globalJson.FullPath.Directory!.Create();

            string json = ToJson();

            File.WriteAllText(_globalJson.FullPath.FullName, json, Encoding.UTF8);

            return this;
        }

        /// <summary>
        /// Gets the JSON representation of the global.json file.
        /// </summary>
        /// <returns>A JSON string representing the global.json file.</returns>
        public string ToJson() => JsonSerializer.Serialize(_globalJson, _jsonSerializerOptions);

        internal record GlobalJson
        {
            public GlobalJson(FileInfo fullPath, GlobalJsonSdk sdk)
            {
                FullPath = fullPath;
                Sdk = sdk;
            }

            [JsonIgnore]
            public FileInfo FullPath { get; init; }

            public GlobalJsonSdk Sdk { get; init; }

            [JsonPropertyName("msbuild-sdks")]
            public Dictionary<string, string>? MSBuildSdks { get; set; }
        }

        internal record GlobalJsonSdk
        {
            public required string Version { get; init; }

            public GlobalJsonSdkRollForward? RollForward { get; init; }

            public bool? AllowPrerelease { get; init; }
        }
    }
}
