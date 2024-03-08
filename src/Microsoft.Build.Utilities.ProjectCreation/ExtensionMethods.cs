// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Provides extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the current object as an <see cref="IEnumerable{T}" />.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="item">The item to make into an <see cref="IEnumerable{T}" />.</param>
        /// <returns>The current object as an <see cref="IEnumerable{T}" />.</returns>
        [DebuggerStepThrough]
        public static IEnumerable<T> AsEnumerable<T>(this T? item)
            where T : class
        {
            if (item == null)
            {
                return Enumerable.Empty<T>();
            }

            // Return the item in an array for now unless we can find a cheaper way to do this (ie custom Enumerator?)
            return new[]
            {
                item,
            };
        }

        /// <summary>
        /// Merges two dictionaries by combining all values and overriding with the first with the second.
        /// </summary>
        /// <param name="first">The first dictionary and all of its values to start with.</param>
        /// <param name="second">The second dictionary to merge with the first and override its values.</param>
        /// <returns>A merged <see cref="IDictionary{String,String}" /> with the values of the first dictionary overridden by the second.</returns>
        [DebuggerStepThrough]
        public static IDictionary<string, string?> Merge(this IDictionary<string, string?>? first, IDictionary<string, string?> second)
        {
            return Merge(first, second, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Merges two dictionaries by combining all values and overriding with the first with the second.
        /// </summary>
        /// <param name="first">The first dictionary and all of its values to start with.</param>
        /// <param name="second">The second dictionary to merge with the first and override its values.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{String}" /> implementation to use when comparing keys, or null to use the default <see cref="IEqualityComparer{String}" /> for the type of the key. </param>
        /// <returns>A merged <see cref="IDictionary{String,String}" /> with the values of the first dictionary overridden by the second.</returns>
        [DebuggerStepThrough]
        public static IDictionary<string, string?> Merge(this IDictionary<string, string?>? first, IDictionary<string, string?> second, IEqualityComparer<string> comparer)
        {
            Dictionary<string, string?> result = first == null
                ? new Dictionary<string, string?>(comparer)
                : new Dictionary<string, string?>(first, comparer);

            foreach (KeyValuePair<string, string?> item in second.Where(i => i.Value != null))
            {
                result[item.Key] = item.Value;
            }

            return result;
        }

        /// <summary>
        /// Gets the current object as an array of objects.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="item">The item to make into an array.</param>
        /// <returns>An array of T objects.</returns>
        [DebuggerStepThrough]
        public static T[] ToArrayWithSingleElement<T>(this T item)
            where T : class
        {
            return new[]
            {
                item,
            };
        }

        /// <summary>
        /// Creates an entry in the current <see cref="ZipArchive" /> based on the specified <see cref="Stream" />.
        /// </summary>
        /// <param name="archive">The <see cref="ZipArchive" /> to create the entry in.</param>
        /// <param name="relativePath">The relative path to the entry inside the archive.</param>
        /// <param name="stream">The <see cref="Stream" /> to use when creating the entry.</param>
        /// <returns>The <see cref="ZipArchiveEntry" /> that was created.</returns>
        internal static ZipArchiveEntry CreateEntryFromStream(this ZipArchive archive, string relativePath, Stream stream)
        {
            ZipArchiveEntry? entry = archive.CreateEntry(relativePath);

            using (Stream? destination = entry.Open())
            {
                stream.Position = 0;

                stream.CopyTo(destination);
            }

            return entry;
        }

        internal static void WriteAttributeStringIfNotNull(this XmlWriter writer, string localName, string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                writer.WriteAttributeString(localName, value);
            }
        }

        internal static void WriteAttributeStringIfNotNull(this XmlWriter writer, string localName, bool? value)
        {
            if (value.HasValue)
            {
                writer.WriteAttributeString(localName, value.Value ? "true" : "false");
            }
        }

        internal static void WriteElementStringIfNotNull(this XmlWriter writer, string localName, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                writer.WriteElementString(localName, value);
            }
        }

        internal static void WriteElementStringIfNotNull(this XmlWriter writer, string localName, bool? value)
        {
            if (value.HasValue)
            {
                writer.WriteElementString(localName, value.Value ? "true" : "false");
            }
        }

        internal static string ReadAsText(this FileInfo file)
        {
            using StreamReader reader = file.OpenText();
            return reader.ReadToEnd();
        }
    }
}