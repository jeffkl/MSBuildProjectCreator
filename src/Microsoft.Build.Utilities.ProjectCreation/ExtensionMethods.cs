// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using NuGet.Packaging.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Provides extension methods.
    /// </summary>
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Gets the current object as an <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="item">The item to make into an <see cref="IEnumerable{T}"/>.</param>
        /// <returns>The current object as an <see cref="IEnumerable{T}"/>.</returns>
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
        /// <returns>A merged <see cref="IDictionary{String,String}"/> with the values of the first dictionary overridden by the second.</returns>
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
        /// <param name="comparer">The <see cref="IEqualityComparer{String}"/> implementation to use when comparing keys, or null to use the default <see cref="IEqualityComparer{String}"/> for the type of the key. </param>
        /// <returns>A merged <see cref="IDictionary{String,String}"/> with the values of the first dictionary overridden by the second.</returns>
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
        /// Gets the current list of strings as <see cref="PackageType" /> objects instead.
        /// </summary>
        /// <param name="packageTypes">An <see cref="IEnumerable{String}" /> containing package types.</param>
        /// <returns>An <see cref="IEnumerable{PackageType}" /> containing the package types.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Any package types are invalid.</exception>
        public static IEnumerable<PackageType> ToPackageTypes(this IEnumerable<string>? packageTypes)
        {
            if (packageTypes == null)
            {
                yield break;
            }

            foreach (string packageType in packageTypes)
            {
                if (string.Equals(PackageType.Dependency.Name, packageType, StringComparison.OrdinalIgnoreCase))
                {
                    yield return PackageType.Dependency;
                }
                else if (string.Equals(PackageType.DotnetCliTool.Name, packageType, StringComparison.OrdinalIgnoreCase))
                {
                    yield return PackageType.DotnetCliTool;
                }
                else if (string.Equals(PackageType.DotnetPlatform.Name, packageType, StringComparison.OrdinalIgnoreCase))
                {
                    yield return PackageType.DotnetPlatform;
                }
                else if (string.Equals(PackageType.DotnetTool.Name, packageType, StringComparison.OrdinalIgnoreCase))
                {
                    yield return PackageType.DotnetTool;
                }
                else if (string.Equals(PackageType.Legacy.Name, packageType, StringComparison.OrdinalIgnoreCase))
                {
                    yield return PackageType.Legacy;
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"Unknown package type '{packageType}'");
                }
            }
        }
    }
}