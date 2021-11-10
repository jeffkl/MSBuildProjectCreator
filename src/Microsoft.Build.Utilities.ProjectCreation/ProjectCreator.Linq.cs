// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class ProjectCreator
    {
        /// <summary>
        /// Executes the specified action for each item in a list.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}" /> containing items to iterate through.</param>
        /// <param name="action">An <see cref="Action{T, ProjectCreator}" /> to execute against the current <see cref="ProjectCreator" />.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator ForEach<T>(IEnumerable<T> source, Action<T, ProjectCreator> action)
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
