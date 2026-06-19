// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Specifies the SDK roll-forward policy for global.json.
    /// </summary>
    [JsonConverter(typeof(GlobalJsonSdkRollForwardJsonConverter))]
    public enum GlobalJsonSdkRollForward
    {
        /// <summary>
        /// Uses the specified version. If not found, rolls forward to the latest patch level. If not found, fails. This value is the legacy behavior from the earlier versions of the SDK.
        /// </summary>
        Patch = 5,

        /// <summary>
        /// Uses the latest patch level for the specified major, minor, and feature band. If not found, rolls forward to the next higher feature band within the same major/minor and uses the latest patch level for that feature band. If not found, fails.
        /// </summary>
        Feature = 6,

        /// <summary>
        /// Uses the latest patch level for the specified major, minor, and feature band. If not found, rolls forward to the next higher feature band within the same major/minor version and uses the latest patch level for that feature band. If not found, rolls forward to the next higher minor and feature band within the same major and uses the latest patch level for that feature band. If not found, fails.
        /// </summary>
        Minor = 7,

        /// <summary>
        /// Uses the latest patch level for the specified major, minor, and feature band. If not found, rolls forward to the next higher feature band within the same major/minor version and uses the latest patch level for that feature band. If not found, rolls forward to the next higher minor and feature band within the same major and uses the latest patch level for that feature band. If not found, rolls forward to the next higher major, minor, and feature band and uses the latest patch level for that feature band. If not found, fails.
        /// </summary>
        Major = 8,

        /// <summary>
        /// Uses the latest installed patch level that matches the requested major, minor, and feature band with a patch level that's greater than or equal to the specified value. If not found, fails.
        /// </summary>
        LatestPatch = 0,

        /// <summary>
        /// Uses the highest installed feature band and patch level that matches the requested major and minor with a feature band and patch level that's greater than or equal to the specified value. If not found, fails.
        /// </summary>
        LatestFeature = 1,

        /// <summary>
        /// Uses the highest installed minor, feature band, and patch level that matches the requested major with a minor, feature band, and patch level that's greater than or equal to the specified value. If not found, fails.
        /// </summary>
        LatestMinor = 2,

        /// <summary>
        /// Uses the highest installed .NET SDK with a version that's greater than or equal to the specified value. If not found, fail.
        /// </summary>
        LatestMajor = 3,

        /// <summary>
        /// Doesn't roll forward; an exact match is required.
        /// </summary>
        Disable = 4,
    }

#pragma warning disable SA1649 // File name should match first type name

    internal class GlobalJsonSdkRollForwardJsonConverter : JsonConverter<GlobalJsonSdkRollForward>
#pragma warning restore SA1649 // File name should match first type name
    {
        public override GlobalJsonSdkRollForward Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? value = reader.GetString();

            return value?.Trim() switch
            {
                "patch" => GlobalJsonSdkRollForward.Patch,
                "feature" => GlobalJsonSdkRollForward.Feature,
                "minor" => GlobalJsonSdkRollForward.Minor,
                "major" => GlobalJsonSdkRollForward.Major,
                "latestPatch" => GlobalJsonSdkRollForward.LatestPatch,
                "latestFeature" => GlobalJsonSdkRollForward.LatestFeature,
                "latestMinor" => GlobalJsonSdkRollForward.LatestMinor,
                "latestMajor" => GlobalJsonSdkRollForward.LatestMajor,
                "disable" => GlobalJsonSdkRollForward.Disable,
                _ => throw new JsonException($"Unexpected value '{value}' for {nameof(GlobalJsonSdkRollForward)}."),
            };
        }

        public override void Write(Utf8JsonWriter writer, GlobalJsonSdkRollForward value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case GlobalJsonSdkRollForward.Patch:
                    writer.WriteStringValue("patch");
                    break;

                case GlobalJsonSdkRollForward.Feature:
                    writer.WriteStringValue("feature");
                    break;

                case GlobalJsonSdkRollForward.Minor:
                    writer.WriteStringValue("minor");
                    break;

                case GlobalJsonSdkRollForward.Major:
                    writer.WriteStringValue("major");
                    break;

                case GlobalJsonSdkRollForward.LatestPatch:
                    writer.WriteStringValue("latestPatch");
                    break;

                case GlobalJsonSdkRollForward.LatestFeature:
                    writer.WriteStringValue("latestFeature");
                    break;

                case GlobalJsonSdkRollForward.LatestMinor:
                    writer.WriteStringValue("latestMinor");
                    break;

                case GlobalJsonSdkRollForward.LatestMajor:
                    writer.WriteStringValue("latestMajor");
                    break;

                case GlobalJsonSdkRollForward.Disable:
                    writer.WriteStringValue("disable");
                    break;

                default:
                    throw new JsonException($"Unexpected value '{value}' for {nameof(GlobalJsonSdkRollForward)}.");
            }
        }
    }
}
