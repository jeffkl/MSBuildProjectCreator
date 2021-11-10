// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Construction;
using Microsoft.Build.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class ProjectCreator
    {
        /// <summary>
        /// Stores the last task that was added to the project.
        /// </summary>
        private ProjectTaskElement? _lastTask;

        /// <summary>
        /// Adds a task element to the current target.
        /// </summary>
        /// <param name="name">The name of the task.</param>
        /// <param name="condition">An optional condition to add to the task.</param>
        /// <param name="parameters">An optional <see cref="IDictionary{String,String}" /> that contains the parameters to pass to the task.</param>
        /// <param name="continueOnError">An optional value indicating if the build should continue in the case of an error.  The valid values are:
        /// <list type="Bullet">
        ///   <item>
        ///     <description><code>WarnAndContinue</code> or <code>true</code> - When a task fails, subsequent tasks in the Target element and the build continue to execute, and all errors from the task are treated as warnings.</description>
        ///   </item>
        ///   <item>
        ///     <description><code>ErrorAndContinue</code> - When a task fails, subsequent tasks in the Target element and the build continue to execute, and all errors from the task are treated as errors.</description>
        ///   </item>
        ///   <item>
        ///     <description><code>ErrorAndStop</code> or <code>false</code> - (Default) When a task fails, the remaining tasks in the Target element and the build aren't executed, and the entire Target element and the build is considered to have failed.</description>
        ///   </item>
        /// </list>
        /// </param>
        /// <param name="architecture">an optional architecture for the task.</param>
        /// <param name="runtime">An optional runtime for the task.</param>
        /// <param name="label">An optional label to add to the task.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator Task(string name, string? condition = null, IDictionary<string, string?>? parameters = null, string? continueOnError = null, string? architecture = null, string? runtime = null, string? label = null)
        {
            _lastTask = LastTarget.AddTask(name);

            _lastTask.ContinueOnError = continueOnError;
            _lastTask.Condition = condition;
            _lastTask.Label = label;
            _lastTask.MSBuildArchitecture = architecture;
            _lastTask.MSBuildRuntime = runtime;

            if (parameters != null)
            {
                foreach (KeyValuePair<string, string?> parameter in parameters.Where(i => i.Value != null))
                {
                    _lastTask.SetParameter(parameter.Key, parameter.Value);
                }
            }

            return this;
        }

        /// <summary>
        /// Adds an &lt;Error /&gt; task to the current target.
        /// </summary>
        /// <param name="text">The message to display.</param>
        /// <param name="code">An optional code to display.</param>
        /// <param name="file">An optional file to display.</param>
        /// <param name="helpKeyword">An optional help keyword.</param>
        /// <param name="condition">An optional condition to add to the task.</param>
        /// <param name="label">An optional label to add to the task.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TaskError(string text, string? code = null, string? file = null, string? helpKeyword = null, string? condition = null, string? label = null)
        {
            return Task(
                "Error",
                condition,
                new Dictionary<string, string?>
                {
                    { "Text", text },
                    { "Code", code },
                    { "File", file },
                    { "HelpKeyword", helpKeyword },
                },
                label: label);
        }

        /// <summary>
        /// Adds a &lt;Message /&gt; task to the current target.
        /// </summary>
        /// <param name="text">The message to display.</param>
        /// <param name="importance">An optional <see cref="MessageImportance" /> to use.</param>
        /// <param name="condition">An optional condition to add to the task.</param>
        /// <param name="label">An optional label to add to the task.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TaskMessage(string text, MessageImportance? importance = null, string? condition = null, string? label = null)
        {
            return Task(
                "Message",
                condition,
                new Dictionary<string, string?>
                {
                    { "Text", text },
                    { "Importance", importance?.ToString() },
                },
                label: label);
        }

        /// <summary>
        /// Adds an &lt;Output /&gt; element to the current task that receives an item from task.  To add an output property, use <see cref="TaskOutputProperty" />.
        /// </summary>
        /// <param name="taskParameter">The name of the task's output parameter.</param>
        /// <param name="itemType">The item that receives the task's output parameter value.</param>
        /// <param name="condition">An optional condition to add to the output element.</param>
        /// <param name="label">An optional label to add to the output element.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TaskOutputItem(string taskParameter, string itemType, string? condition = null, string? label = null)
        {
            if (_lastTask == null)
            {
                throw new ProjectCreatorException(Strings.ErrorTaskOutputItemRequiresTask);
            }

            _lastTask.AddOutputItem(taskParameter, itemType, condition);
            _lastTask.Label = label;

            return this;
        }

        /// <summary>
        /// Adds an &lt;Output /&gt; element to the current task that receives a property from task.  To add an output Item, use <see cref="TaskOutputItem" />.
        /// </summary>
        /// <param name="taskParameter">The name of the task's output parameter.</param>
        /// <param name="propertyName">The property that receives the task's output parameter value.</param>
        /// <param name="condition">An optional condition to add to the output element.</param>
        /// <param name="label">An optional label to add to the output element.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TaskOutputProperty(string taskParameter, string propertyName, string? condition = null, string? label = null)
        {
            if (_lastTask == null)
            {
                throw new ProjectCreatorException(Strings.ErrorTaskOutputPropertyRequiresTask);
            }

            _lastTask.AddOutputProperty(taskParameter, propertyName, condition);
            _lastTask.Label = label;

            return this;
        }

        /// <summary>
        /// Adds a &lt;Warning /&gt; task to the current target.
        /// </summary>
        /// <param name="text">The message to display.</param>
        /// <param name="code">An optional code to display.</param>
        /// <param name="file">An optional file to display.</param>
        /// <param name="helpKeyword">An optional help keyword.</param>
        /// <param name="condition">An optional condition to add to the task.</param>
        /// <param name="label">An optional label to add to the task.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator TaskWarning(string text, string? code = null, string? file = null, string? helpKeyword = null, string? condition = null, string? label = null)
        {
            return Task(
                "Warning",
                condition,
                new Dictionary<string, string?>
                {
                    { "Text", text },
                    { "Code", code },
                    { "File", file },
                    { "HelpKeyword", helpKeyword },
                },
                label: label);
        }
    }
}