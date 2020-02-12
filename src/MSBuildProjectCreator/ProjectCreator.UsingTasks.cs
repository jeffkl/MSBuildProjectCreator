// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Construction;
using Microsoft.Build.Utilities.ProjectCreation.Resources;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class ProjectCreator
    {
        /// <summary>
        /// Stores the last UsingTask that was added.
        /// </summary>
        private ProjectUsingTaskElement _lastUsingTask;

        /// <summary>
        /// Adds a &lt;UsingTask /&gt; that refers to an assembly by a file path.
        /// </summary>
        /// <param name="taskName">The name of the task.</param>
        /// <param name="assemblyFile">The full or relative file path of the assembly to load.</param>
        /// <param name="taskFactory">An optional class in the assembly that is responsible for generating instances of the specified Task name.</param>
        /// <param name="runtime">An optional runtime for the task.</param>
        /// <param name="architecture">An optional architecture for the task.</param>
        /// <param name="condition">An optional condition to add to the task.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator UsingTaskAssemblyFile(string taskName, string assemblyFile, string taskFactory = null, string runtime = null, string architecture = null, string condition = null)
        {
            _lastUsingTask = AddTopLevelElement(RootElement.CreateUsingTaskElement(taskName, assemblyFile, null, runtime, architecture));

            _lastUsingTask.TaskFactory = taskFactory;
            _lastUsingTask.Condition = condition;

            return this;
        }

        /// <summary>
        /// Adds a &lt;UsingTask /&gt; that refers to an assembly by name.
        /// </summary>
        /// <param name="taskName">The name of the task.</param>
        /// <param name="assemblyName">The name of the assembly to load.</param>
        /// <param name="taskFactory">An optional class in the assembly that is responsible for generating instances of the specified Task name.</param>
        /// <param name="runtime">An optional runtime for the task.</param>
        /// <param name="architecture">An optional architecture for the task.</param>
        /// <param name="condition">An optional condition to add to the task.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator UsingTaskAssemblyName(string taskName, string assemblyName, string taskFactory, string runtime = null, string architecture = null, string condition = null)
        {
            _lastUsingTask = AddTopLevelElement(RootElement.CreateUsingTaskElement(taskName, null, assemblyName, runtime, architecture));

            _lastUsingTask.TaskFactory = taskFactory;
            _lastUsingTask.Condition = condition;

            return this;
        }

        /// <summary>
        /// Adds a &lt;Task /&gt; element to the current &lt;UsingTask /&gt;.
        /// </summary>
        /// <param name="body">The data that is passed to the TaskFactory to generate an instance of the task.</param>
        /// <param name="evaluate">An optional value indicating if the body should be evaluated.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator UsingTaskBody(string body, bool? evaluate = null)
        {
            if (_lastUsingTask == null)
            {
                throw new ProjectCreatorException(Strings.ErrorUsingTaskBodyRequiresUsingTask);
            }

            if (_lastUsingTask.TaskBody != null)
            {
                throw new ProjectCreatorException(Strings.ErrorUsingTaskBodyCanOnlyBeSetOnce);
            }

            _lastUsingTask.AddUsingTaskBody(evaluate?.ToString(), body);

            return this;
        }

        /// <summary>
        /// Adds a parameter to the current &lt;UsingTask /&gt;
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="parameterType">An optional .NET type of the parameter, for example, "System.String".</param>
        /// <param name="output">An optional value indicating whether or not the property is an output.</param>
        /// <param name="required">An optional value indicating whether or not the property is required.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator UsingTaskParameter(string name, string parameterType = null, bool? output = null, bool? required = null)
        {
            if (_lastUsingTask == null)
            {
                throw new ProjectCreatorException(Strings.ErrorUsingTaskParameterRequiresUsingTask);
            }

            if (_lastUsingTask.ParameterGroup == null)
            {
                _lastUsingTask.AddParameterGroup();
            }

            // ReSharper disable once PossibleNullReferenceException
            _lastUsingTask.ParameterGroup.AddParameter(
                name,
                output?.ToString() ?? string.Empty,
                required?.ToString() ?? string.Empty,
                parameterType ?? string.Empty);

            return this;
        }
    }
}