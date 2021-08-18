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
    /// Represents the <see cref="BuildMessageEventArgs"/> that were logged.
    /// </summary>
    public sealed class BuildMessageEventArgsCollection : IReadOnlyCollection<BuildMessageEventArgs>
    {
        private readonly IReadOnlyCollection<BuildMessageEventArgs> _messageEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildMessageEventArgsCollection"/> class.
        /// </summary>
        /// <param name="messageEvents">A <see cref="IReadOnlyCollection{BuildMessageEventArgs}"/> containing the logged message events.</param>
        internal BuildMessageEventArgsCollection(IReadOnlyCollection<BuildMessageEventArgs> messageEvents)
        {
            _messageEvents = messageEvents ?? throw new ArgumentNullException(nameof(messageEvents));
        }

        /// <inheritdoc cref="IReadOnlyCollection{T}.Count"/>
        public int Count => _messageEvents.Count;

        /// <summary>
        /// Gets the <see cref="BuildMessageEventArgs"/> that were logged with <see cref="MessageImportance.High"/>.
        /// </summary>
        public IReadOnlyCollection<BuildMessageEventArgs> High => _messageEvents.Where(i => i.Importance == MessageImportance.High).ToList();

        /// <summary>
        /// Gets the <see cref="BuildMessageEventArgs"/> that were logged with <see cref="MessageImportance.Low"/>.
        /// </summary>
        public IReadOnlyCollection<BuildMessageEventArgs> Low => _messageEvents.Where(i => i.Importance == MessageImportance.Low).ToList();

        /// <summary>
        /// Gets the <see cref="BuildMessageEventArgs"/> that were logged with <see cref="MessageImportance.Normal"/>.
        /// </summary>
        public IReadOnlyCollection<BuildMessageEventArgs> Normal => _messageEvents.Where(i => i.Importance == MessageImportance.Normal).ToList();

        /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
        public IEnumerator<BuildMessageEventArgs> GetEnumerator()
        {
            return _messageEvents.GetEnumerator();
        }

        /// <inheritdoc cref="IEnumerable.GetEnumerator"/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}