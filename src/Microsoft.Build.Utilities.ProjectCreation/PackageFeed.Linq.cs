// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class PackageFeed
    {
        /// <summary>
        /// Executes the specified action for each item in the list.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}" /> containing items to iterate through.</param>
        /// <param name="action">An <see cref="Action{T, PackageFeed}" /> to execute against the current <see cref="PackageFeed" />.</param>
        /// <returns>The current <see cref="PackageFeed" />.</returns>
        public PackageFeed ForEach<T>(IEnumerable<T> source, Action<T, PackageFeed> action)
        {
            if (source != null)
            {
                foreach (T item in source)
                {
                    action(item, this);
                }
            }

            return this;
        }
    }
}