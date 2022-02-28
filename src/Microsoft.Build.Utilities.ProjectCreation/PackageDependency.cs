// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Represents a package dependency.
    /// </summary>
    public class PackageDependency : IEqualityComparer<PackageDependency>, IEquatable<PackageDependency>
    {
        /// <summary>
        /// Represents the default assets to prevent from flowing to down stream dependencies.
        /// </summary>
        public const string DefaultPrivateAssets = "Build,ContentFiles,Analyzers";

        private static readonly IncludeFlags NoContent = IncludeFlags.All & ~IncludeFlags.ContentFiles;

        private static readonly char[] SplitChars = new[] { ';', ',', ' ' };

        internal PackageDependency(string id, string version, string? includeAssets = "All", string? excludeAssets = "None", string privateAssets = DefaultPrivateAssets)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Version = version ?? throw new ArgumentNullException(nameof(version));

            IncludeFlags include = GetIncludeFlags(includeAssets);
            IncludeFlags exclude = GetIncludeFlags(excludeAssets);
            IncludeFlags @private = GetIncludeFlags(privateAssets);

            IncludeFlags effectiveInclude = include & ~exclude & ~@private;

            ExcludeAssets = exclude.ToString();

            if ((NoContent & ~effectiveInclude) != IncludeFlags.None)
            {
                ExcludeAssets = (NoContent & ~effectiveInclude).ToString();
            }
        }

        internal PackageDependency(string id, string version, string excludeAssets)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Version = version ?? throw new ArgumentNullException(nameof(version));
            ExcludeAssets = excludeAssets ?? throw new ArgumentNullException(nameof(excludeAssets));
        }

        private PackageDependency()
        {
        }

        [Flags]
        private enum IncludeFlags
        {
            None = 0,

            Runtime = 1 << 0,

            Compile = 1 << 1,

            Build = 1 << 2,

            Native = 1 << 3,

            ContentFiles = 1 << 4,

            Analyzers = 1 << 5,

            BuildTransitive = 1 << 6,

            All = Analyzers | Build | Compile | ContentFiles | Native | Runtime | BuildTransitive,
        }

        /// <summary>
        /// Gets the assets to exclude.
        /// </summary>
        public string ExcludeAssets { get; } = string.Empty;

        /// <summary>
        /// Gets the ID of the dependency.
        /// </summary>
        public string? Id { get; }

        /// <summary>
        /// Gets the version of the dependency.
        /// </summary>
        public string? Version { get; }

        /// <inheritdoc />
        public bool Equals(PackageDependency? x, PackageDependency? y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return x.Id == y.Id && x.Version == y.Version && x.ExcludeAssets == y.ExcludeAssets;
        }

        /// <inheritdoc />
        public bool Equals(PackageDependency? other)
        {
            return other != null && Id == other.Id && Version == other.Version && ExcludeAssets == other.ExcludeAssets;
        }

        /// <inheritdoc />
        public int GetHashCode(PackageDependency obj)
        {
            return (obj.Id, obj.Version, obj.ExcludeAssets).GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Id} / {Version} / {ExcludeAssets}";
        }

        private IncludeFlags GetIncludeFlags(string? assets)
        {
            if (assets == null)
            {
                return IncludeFlags.None;
            }

            IncludeFlags includeFlags = IncludeFlags.None;

            foreach (string? item in assets.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries))
            {
                if (Enum.TryParse(item, out IncludeFlags value))
                {
                    includeFlags |= value;
                }
            }

            return includeFlags;
        }
    }
}