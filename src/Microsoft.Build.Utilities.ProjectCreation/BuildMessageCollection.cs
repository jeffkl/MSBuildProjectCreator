// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Represents the messages that were logged.
    /// </summary>
    public sealed class BuildMessageCollection : IReadOnlyCollection<string>
    {
        private readonly BuildEventArgsCollection _buildOutput;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildMessageCollection" /> class.
        /// </summary>
        /// <param name="buildOutput">The <see cref="BuildEventArgsCollection" /> object that has message events.</param>
        internal BuildMessageCollection(BuildEventArgsCollection buildOutput)
        {
            _buildOutput = buildOutput ?? throw new ArgumentNullException(nameof(buildOutput));
        }

        /// <inheritdoc cref="IReadOnlyCollection{T}.Count" />
        public int Count => _buildOutput.MessageEvents.Count;

        /// <summary>
        /// Gets the messages that were logged with <see cref="MessageImportance.High" />.
        /// </summary>
        public IReadOnlyCollection<string> High => _buildOutput.MessageEvents.High.Select(i => i.Message).ToList()!;

        /// <summary>
        /// Gets the messages that were logged with <see cref="MessageImportance.Low" />.
        /// </summary>
        public IReadOnlyCollection<string> Low => _buildOutput.MessageEvents.Low.Select(i => i.Message).ToList()!;

        /// <summary>
        /// Gets the messages that were logged with <see cref="MessageImportance.Normal" />.
        /// </summary>
        public IReadOnlyCollection<string> Normal => _buildOutput.MessageEvents.Normal.Select(i => i.Message).ToList()!;

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator" />
        public IEnumerator<string> GetEnumerator()
        {
            return _buildOutput.MessageEvents.Select(i => i.Message).ToList().GetEnumerator();
        }

        /// <inheritdoc cref="IEnumerable.GetEnumerator" />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}