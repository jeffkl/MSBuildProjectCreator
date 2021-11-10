// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    /// <summary>
    /// Represents an exception that is thrown by the <see cref="ProjectCreator" />.
    /// </summary>
    public class ProjectCreatorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectCreatorException" /> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ProjectCreatorException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectCreatorException" /> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner <see cref="Exception" />.</param>
        public ProjectCreatorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}