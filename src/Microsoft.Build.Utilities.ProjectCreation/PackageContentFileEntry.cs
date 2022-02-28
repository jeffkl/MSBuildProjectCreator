// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Represents a content file entry for a package.
    /// </summary>
    public class PackageContentFileEntry
    {
        internal PackageContentFileEntry(string? buildAction = null, string? copyToOutput = null, string? include = null, string? exclude = null, string? flatten = null)
        {
            BuildAction = buildAction;
            Include = include;
            Exclude = exclude;

            if (bool.TryParse(copyToOutput, out bool shouldCopyToOutput))
            {
                CopyToOutput = shouldCopyToOutput;
            }

            if (bool.TryParse(flatten, out bool shouldFlatten))
            {
                Flatten = shouldFlatten;
            }
        }

        /// <summary>
        /// Gets or sets the build action of the content file.
        /// </summary>
        public string? BuildAction { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not a content file should be copied to the output directory.
        /// </summary>
        public bool? CopyToOutput { get; set; }

        /// <summary>
        /// Gets or sets the exclude pattern action of the content file.
        /// </summary>
        public string? Exclude { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not content file hierarchy should be flattend when copying to the output directory.
        /// </summary>
        public bool? Flatten { get; set; }

        /// <summary>
        /// Gets or sets the include pattern of the content file.
        /// </summary>
        public string? Include { get; set; }
    }
}