// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Construction;
using System.Collections.Generic;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class ProjectCreator
    {
        /// <summary>
        /// Stores the last target that was added.
        /// </summary>
        private ProjectTargetElement _lastTarget;

        /// <summary>
        /// Stores the last item group that was added to the current target.
        /// </summary>
        private ProjectItemGroupElement _lastTargetItemGroup;

        /// <summary>
        /// Stores the last property group that was added to the current target.
        /// </summary>
        private ProjectPropertyGroupElement _lastTargetPropertyGroup;

        /// <summary>
        /// Gets the last target that was added if there is one, otherwise one is added with a default name from <see cref="ProjectCreatorConstants.DefaultTargetName"/>.
        /// </summary>
        protected ProjectTargetElement LastTarget
        {
            get
            {
                if (_lastTarget == null)
                {
                    Target(ProjectCreatorConstants.DefaultTargetName);
                }

                return _lastTarget;
            }
        }

        /// <summary>
        /// Adds a &lt;Target /&gt; element to the current project.
        /// </summary>
        /// <param name="name">The name of the target.</param>
        /// <param name="condition">An optional condition to add to the target.</param>
        /// <param name="afterTargets">A semicolon-separated list of target names. When specified, indicates that this target should run after the specified target or targets.</param>
        /// <param name="beforeTargets">A semicolon-separated list of target names. When specified, indicates that this target should run before the specified target or targets.</param>
        /// <param name="dependsOnTargets">The targets that must be executed before this target can be executed or top-level dependency analysis can occur. Multiple targets are separated by semicolons.</param>
        /// <param name="inputs">The files that form inputs into this target. Multiple files are separated by semicolons.</param>
        /// <param name="outputs">The files that form outputs into this target. Multiple files are separated by semicolons.</param>
        /// <param name="returns">The set of items that will be made available to tasks that invoke this target, for example, MSBuild tasks. Multiple targets are separated by semicolons.</param>
        /// <param name="keepDuplicateOutputs">If true, multiple references to the same item in the target's Returns are recorded. By default, this attribute is false.</param>
        /// <param name="label">An optional label to add to the target.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator Target(
            string name,
            string condition = null,
            string afterTargets = null,
            string beforeTargets = null,
            string dependsOnTargets = null,
            string inputs = null,
            string outputs = null,
            string returns = null,
            bool? keepDuplicateOutputs = null,
            string label = null)
        {
            _lastTarget = AddTopLevelElement(RootElement.CreateTargetElement(name));

            _lastTarget.AfterTargets = afterTargets ?? string.Empty;
            _lastTarget.BeforeTargets = beforeTargets ?? string.Empty;
            _lastTarget.Condition = condition;
            _lastTarget.DependsOnTargets = dependsOnTargets ?? string.Empty;
            _lastTarget.Inputs = inputs ?? string.Empty;
            _lastTarget.Outputs = outputs ?? string.Empty;
            _lastTarget.Returns = returns;
            _lastTarget.Label = label;

            if (keepDuplicateOutputs != null)
            {
                _lastTarget.KeepDuplicateOutputs = keepDuplicateOutputs.ToString();
            }

            return this;
        }

        /// <summary>
        /// Adds an &lt;ItemGroup /&gt; element to the current target.
        /// </summary>
        /// <param name="condition">An optional condition to add to the item group.</param>
        /// <param name="label">An optional label to add to the item group.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TargetItemGroup(string condition = null, string label = null)
        {
            _lastTargetItemGroup = ItemGroup(LastTarget, condition, label);

            return this;
        }

        /// <summary>
        /// Adds an item to the current item group within the current target.
        /// </summary>
        /// <param name="itemType">The type of the item to add.</param>
        /// <param name="include">The file or wildcard to include in the list of items.</param>
        /// <param name="exclude">An optional file or wildcard to exclude from the list of items.</param>
        /// <param name="metadata">An optional <see cref="IDictionary{String,String}"/> containing metadata for the item.</param>
        /// <param name="condition">An optional condition to add to the item.</param>
        /// <param name="label">An optional label to add to the item.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TargetItemInclude(
            string itemType,
            string include,
            string exclude = null,
            IDictionary<string, string> metadata = null,
            string condition = null,
            string label = null)
        {
            if (_lastTargetItemGroup == null)
            {
                TargetItemGroup();
            }

            return Item(
                itemGroup: _lastTargetItemGroup,
                itemType: itemType,
                include: include,
                exclude: exclude,
                metadata: metadata,
                remove: null,
                update: null,
                condition: condition,
                label: label);
        }

        /// <summary>
        /// Adds an &lt;OnError /&gt; element to the current target.
        /// </summary>
        /// <param name="executeTargets">The targets to execute if a task fails. Separate multiple targets with semicolons. Multiple targets are executed in the order specified.</param>
        /// <param name="condition">Condition to be evaluated.</param>
        /// <param name="label">An optional label to add.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TargetOnError(string executeTargets, string condition = null, string label = null)
        {
            ProjectOnErrorElement onErrorElement = RootElement.CreateOnErrorElement(executeTargets);

            LastTarget.AppendChild(onErrorElement);

            onErrorElement.Condition = condition;
            onErrorElement.Label = label;

            return this;
        }

        /// <summary>
        /// Adds a property element to the current &lt;PropertyGroup /&gt; of the current target.  A property group is automatically added if necessary.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="unevaluatedValue">The unevaluated value of the property.</param>
        /// <param name="condition">An optional condition to add to the property.</param>
        /// <param name="setIfEmpty">An optional value indicating whether or not a condition should be added that checks if the property has already been set.</param>
        /// <param name="label">An optional label to add to the property.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        /// <remarks>
        /// The <paramref name="setIfEmpty"/> parameter will add a condition such as " '$(Property)' == '' " which will only set the property if it has not already been set.
        /// </remarks>
        public ProjectCreator TargetProperty(string name, string unevaluatedValue, string condition = null, bool setIfEmpty = false, string label = null)
        {
            if (_lastTargetPropertyGroup == null)
            {
                TargetPropertyGroup();
            }

            return Property(_lastTargetPropertyGroup, name, unevaluatedValue, condition, setIfEmpty, label);
        }

        /// <summary>
        /// Adds a &lt;PropertyGroup /&gt; to the current target.
        /// </summary>
        /// <param name="condition">An optional condition to add to the property group.</param>
        /// <param name="label">An optional label to add to the property group.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TargetPropertyGroup(string condition = null, string label = null)
        {
            _lastTargetPropertyGroup = PropertyGroup(LastTarget, condition, label);

            return this;
        }
    }
}