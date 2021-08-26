// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.LibraryModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Provides extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the current object as an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="item">The item to make into an <see cref="IEnumerable{T}"/>.</param>
        /// <returns>The current object as an <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<T> AsEnumerable<T>(this T item)
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
        /// Enumerates <see cref="LibraryIncludeFlags" /> for exclude.
        /// </summary>
        /// <param name="libraryIncludeFlags">The <see cref="LibraryIncludeFlags" /> to enumerate.</param>
        /// <returns>Nothing if <paramref name="libraryIncludeFlags" /> is <see cref="LibraryIncludeFlags.None" />, <see cref="LibraryIncludeFlags.All" />, or all flags.</returns>
        public static IEnumerable<LibraryIncludeFlags> EnumerateExcludeFlags(this LibraryIncludeFlags libraryIncludeFlags)
        {
            if (libraryIncludeFlags == LibraryIncludeFlags.All)
            {
                yield return LibraryIncludeFlags.All;
            }
            else if (libraryIncludeFlags != LibraryIncludeFlags.None)
            {
                foreach (LibraryIncludeFlags flags in Enum
                    .GetValues(typeof(LibraryIncludeFlags))
                    .Cast<LibraryIncludeFlags>()
                    .Where(i => i != LibraryIncludeFlags.All && i != LibraryIncludeFlags.None && libraryIncludeFlags.HasFlag(i)))
                {
                    yield return flags;
                }
            }
        }

        /// <summary>
        /// Enumerates <see cref="LibraryIncludeFlags" /> for include.
        /// </summary>
        /// <param name="libraryIncludeFlags">The <see cref="LibraryIncludeFlags" /> to enumerate.</param>
        /// <returns>Nothing if <paramref name="libraryIncludeFlags" /> is <see cref="LibraryIncludeFlags.All" />, <see cref="LibraryIncludeFlags.None" />, or all flags.</returns>
        public static IEnumerable<LibraryIncludeFlags> EnumerateIncludeFlags(this LibraryIncludeFlags libraryIncludeFlags)
        {
            if (libraryIncludeFlags == LibraryIncludeFlags.None)
            {
                yield return LibraryIncludeFlags.None;
            }
            else if (libraryIncludeFlags != LibraryIncludeFlags.All)
            {
                foreach (LibraryIncludeFlags flags in Enum
                    .GetValues(typeof(LibraryIncludeFlags))
                    .Cast<LibraryIncludeFlags>()
                    .Where(i => i != LibraryIncludeFlags.All && i != LibraryIncludeFlags.None && libraryIncludeFlags.HasFlag(i)))
                {
                    yield return flags;
                }
            }
        }

        /// <summary>
        /// Merges two dictionaries by combining all values and overriding with the first with the second.
        /// </summary>
        /// <param name="first">The first dictionary and all of its values to start with.</param>
        /// <param name="second">The second dictionary to merge with the first and override its values.</param>
        /// <returns>A merged <see cref="IDictionary{String,String}"/> with the values of the first dictionary overridden by the second.</returns>
        public static IDictionary<string, string> Merge(this IDictionary<string, string> first, IDictionary<string, string> second)
        {
            return Merge(first, second, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Merges two dictionaries by combining all values and overriding with the first with the second.
        /// </summary>
        /// <param name="first">The first dictionary and all of its values to start with.</param>
        /// <param name="second">The second dictionary to merge with the first and override its values.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{String}"/> implementation to use when comparing keys, or null to use the default <see cref="IEqualityComparer{String}"/> for the type of the key. </param>
        /// <returns>A merged <see cref="IDictionary{String,String}"/> with the values of the first dictionary overridden by the second.</returns>
        public static IDictionary<string, string> Merge(this IDictionary<string, string> first, IDictionary<string, string> second, IEqualityComparer<string> comparer)
        {
            Dictionary<string, string> result = first == null
                ? new Dictionary<string, string>(comparer)
                : new Dictionary<string, string>(first, comparer);

            foreach (KeyValuePair<string, string> item in second.Where(i => i.Value != null))
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
        public static T[] ToArrayWithSingleElement<T>(this T item)
            where T : class
        {
            return new[]
            {
                item,
            };
        }
    }
}