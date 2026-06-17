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
        /// Roll forward to the latest patch version.
        /// </summary>
        LatestPatch,

        /// <summary>
        /// Roll forward to the latest feature version.
        /// </summary>
        LatestFeature,

        /// <summary>
        /// Roll forward to the latest minor version.
        /// </summary>
        LatestMinor,

        /// <summary>
        /// Roll forward to the latest major version.
        /// </summary>
        LatestMajor,

        /// <summary>
        /// Disable roll-forward.
        /// </summary>
        Disable,
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
