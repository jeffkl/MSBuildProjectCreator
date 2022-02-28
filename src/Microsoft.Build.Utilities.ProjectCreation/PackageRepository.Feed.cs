// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

/*
using System;
using System.IO;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class PackageRepository
    {
        /// <summary>
        /// Adds a feed to the current <see cref="PackageRepository" />.
        /// </summary>
        /// <param name="uri">The <see cref="Uri" /> of the feed.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Feed(Uri uri)
        {
            return Feed(uri, uri.Host);
        }

        /// <summary>
        /// Adds a feed to the current <see cref="PackageRepository" />.
        /// </summary>
        /// <param name="uri">The URI of the feed.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Feed(string uri)
        {
            return Feed(new Uri(uri));
        }

        /// <summary>
        /// Adds a feed to the current <see cref="PackageRepository" />.
        /// </summary>
        /// <param name="uri">The <see cref="Uri" /> of the feed.</param>
        /// <param name="name">An optional name for the feed.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Feed(Uri uri, string name)
        {
            _packageSourceProvider.AddPackageSource(new PackageSource(uri.ToString(), name ?? uri.Host));

            _settings.SaveToDisk();

            return this;
        }

        /// <summary>
        /// Adds a feed to the current <see cref="PackageRepository" />.
        /// </summary>
        /// <param name="directory">The <see cref="DirectoryInfo" /> of the feed.</param>
        /// <param name="name">An optional name for the feed.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Feed(DirectoryInfo directory, string name)
        {
            return Feed(new Uri(directory.FullName), name);
        }

        /// <summary>
        /// Adds a feed to the current <see cref="PackageRepository" />.
        /// </summary>
        /// <param name="uri">The URI of the feed.</param>
        /// <param name="name">An optional name for the feed.</param>
        /// <returns>The current <see cref="PackageRepository" />.</returns>
        public PackageRepository Feed(string uri, string name)
        {
            return Feed(new Uri(uri), name);
        }
    }
}
*/