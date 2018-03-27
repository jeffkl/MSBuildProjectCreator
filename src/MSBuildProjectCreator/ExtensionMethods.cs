// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

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
        /// <param name="item">The item to make into an <see cref="IEnumerable{T}"/></param>
        /// <returns>The current object as an <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<T> AsEnumerable<T>(this T item)
            where T : class
        {
            // Return the item in an array for now unless we can find a cheaper way to do this (ie custom Enumerator?)
            return new[]
            {
                item
            };
        }

        /// <summary>
        /// Merge's two dictionaries by combining all values and overriding with the first with the second.
        /// </summary>
        /// <param name="first">The first dictionary and all of its values to start with.</param>
        /// <param name="second">The second dictionary to merge with the first and override its values.</param>
        /// <returns>A merged <see cref="IDictionary{String,String}"/> with the values of the first dictionary overridden by the second.</returns>
        public static IDictionary<string, string> Merge(this IDictionary<string, string> first, IDictionary<string, string> second)
        {
            return Merge(first, second, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Merge's two dictionaries by combining all values and overriding with the first with the second.
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
    }
}