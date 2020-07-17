// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Construction;
using Microsoft.Build.Utilities.ProjectCreation.Resources;
using System.Collections.Generic;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class ProjectCreator
    {
        private ProjectChooseElement _lastChoose;
        private ProjectItemGroupElement _lastOtherwiseItemGroup;
        private ProjectPropertyGroupElement _lastOtherwisePropertyGroup;
        private ProjectWhenElement _lastWhen;
        private ProjectItemGroupElement _lastWhenItemGroup;
        private ProjectPropertyGroupElement _lastWhenPropertyGroup;

        /// <summary>
        /// Gets the last &lt;Choose /&gt; element that was added.
        /// </summary>
        protected ProjectChooseElement LastChoose
        {
            get
            {
                if (_lastChoose == null)
                {
                    Choose();
                }

                return _lastChoose;
            }
        }

        /// <summary>
        /// Adds a &lt;Choose /&gt; element to the current project.
        /// </summary>
        /// <param name="label">An optional label to add to the Choose.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator Choose(string label = null)
        {
            _lastChoose = AddTopLevelElement(RootElement.CreateChooseElement());
            _lastChoose.Label = label;

            _lastOtherwiseItemGroup = null;
            _lastOtherwisePropertyGroup = null;
            _lastWhen = null;
            _lastWhenItemGroup = null;
            _lastWhenPropertyGroup = null;

            return this;
        }

        /// <summary>
        /// Adds an &lt;Otherwise /&gt; element to the current project.
        /// </summary>
        /// <param name="label">An optional label to add to the Otherwise.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        /// <exception cref="ProjectCreatorException">A &lt;When /&gt; element has not been added
        /// -or-
        /// An &lt;Otherwise /&gt; has already been added to the current &lt;Choose /&gt; element.</exception>
        public ProjectCreator Otherwise(string label = null)
        {
            if (_lastWhen == null)
            {
                throw new ProjectCreatorException(Strings.ErrorOtherwiseRequiresWhen);
            }

            if (_lastChoose.OtherwiseElement != null)
            {
                throw new ProjectCreatorException(Strings.ErrorOtherwiseCanOnlyBeSetOnce);
            }

            ProjectOtherwiseElement projectOtherwiseElement = RootElement.CreateOtherwiseElement();
            projectOtherwiseElement.Label = label;
            LastChoose.AppendChild(projectOtherwiseElement);

            return this;
        }

        /// <summary>
        /// Adds an &lt;ItemGroup /&gt; element to the current &lt;Otherwise /&gt; element.
        /// </summary>
        /// <param name="condition">An optional condition to add to the item group.</param>
        /// <param name="label">An optional label to add to the item group.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        /// <exception cref="ProjectCreatorException">A &lt;When /&gt; element has not been added.</exception>
        public ProjectCreator OtherwiseItemGroup(string condition = null, string label = null)
        {
            if (_lastWhen == null)
            {
                throw new ProjectCreatorException(Strings.ErrorOtherwiseRequiresWhen);
            }

            if (_lastChoose.OtherwiseElement == null)
            {
                Otherwise();
            }

            _lastOtherwiseItemGroup = ItemGroup(_lastChoose.OtherwiseElement, condition, label);

            return this;
        }

        /// <summary>
        /// Adds an item to the current item group within the current &lt;Otherwise /&gt; element.
        /// </summary>
        /// <param name="itemType">The type of the item to add.</param>
        /// <param name="include">The file or wildcard to include in the list of items.</param>
        /// <param name="exclude">An optional file or wildcard to exclude from the list of items.</param>
        /// <param name="metadata">An optional <see cref="IDictionary{String,String}"/> containing metadata for the item.</param>
        /// <param name="condition">An optional condition to add to the item.</param>
        /// <param name="label">An option label to add to the item.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        /// <exception cref="ProjectCreatorException">A &lt;When /&gt; element has not been added.</exception>
        public ProjectCreator OtherwiseItemInclude(
            string itemType,
            string include,
            string exclude = null,
            IDictionary<string, string> metadata = null,
            string condition = null,
            string label = null)
        {
            if (_lastWhen == null)
            {
                throw new ProjectCreatorException(Strings.ErrorOtherwiseRequiresWhen);
            }

            if (_lastOtherwiseItemGroup == null)
            {
                OtherwiseItemGroup();
            }

            return Item(
                itemGroup: _lastOtherwiseItemGroup,
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
        /// Adds a property to the current &lt;PropertyGroup /&gt; element within the current &lt;Otherwise /&gt; element.
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
        public ProjectCreator OtherwiseProperty(string name, string unevaluatedValue, string condition = null, bool setIfEmpty = false, string label = null)
        {
            if (_lastOtherwisePropertyGroup == null)
            {
                OtherwisePropertyGroup();
            }

            Property(_lastOtherwisePropertyGroup, name, unevaluatedValue, condition, setIfEmpty, label);

            return this;
        }

        /// <summary>
        /// Adds a &lt;PropertyGroup /&gt; element to the current &lt;Otherwise /&gt; element.
        /// </summary>
        /// <param name="condition">An optional condition to add to the property group.</param>
        /// <param name="label">An optional label to add to the property group.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator OtherwisePropertyGroup(string condition = null, string label = null)
        {
            if (LastChoose.OtherwiseElement == null)
            {
                Otherwise();
            }

            _lastOtherwisePropertyGroup = PropertyGroup(LastChoose.OtherwiseElement, condition, label);

            return this;
        }

        /// <summary>
        /// Adds a &lt;When /&gt; element to the current &lt;Choose /&gt; element.
        /// </summary>
        /// <param name="condition">An optional condition to add to the &lt;When /&gt; element.</param>
        /// <param name="label">An optional label to add to the &lt;When /&gt; element.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator When(string condition = null, string label = null)
        {
            ProjectChooseElement lastChoose = LastChoose;

            _lastWhen = RootElement.CreateWhenElement(condition);
            _lastWhen.Label = label;
            lastChoose.AppendChild(_lastWhen);

            _lastWhenPropertyGroup = null;
            _lastWhenItemGroup = null;

            return this;
        }

        /// <summary>
        /// Adds an &lt;ItemGroup /&gt; element to the current &lt;When /&gt; element.
        /// </summary>
        /// <param name="condition">An optional condition to add to the item group.</param>
        /// <param name="label">An optional label to add to the item group.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        /// <exception cref="ProjectCreatorException">A &lt;When /&gt; element has not been added.</exception>
        public ProjectCreator WhenItemGroup(string condition = null, string label = null)
        {
            if (_lastWhen == null)
            {
                throw new ProjectCreatorException(Strings.ErrorWhenItemGroupRequiresWhen);
            }

            _lastWhenItemGroup = ItemGroup(_lastWhen, condition, label);

            return this;
        }

        /// <summary>
        /// Adds an item to the current item group within the current &lt;When /&gt; element.
        /// </summary>
        /// <param name="itemType">The type of the item to add.</param>
        /// <param name="include">The file or wildcard to include in the list of items.</param>
        /// <param name="exclude">An optional file or wildcard to exclude from the list of items.</param>
        /// <param name="metadata">An optional <see cref="IDictionary{String,String}"/> containing metadata for the item.</param>
        /// <param name="condition">An optional condition to add to the item.</param>
        /// <param name="label">An option label to add to the item.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator WhenItemInclude(
            string itemType,
            string include,
            string exclude = null,
            IDictionary<string, string> metadata = null,
            string condition = null,
            string label = null)
        {
            if (_lastWhenItemGroup == null)
            {
                WhenItemGroup();
            }

            return Item(
                itemGroup: _lastWhenItemGroup,
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
        /// Adds a property to the current &lt;PropertyGroup /&gt; element within the current &lt;When /&gt; element.
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
        public ProjectCreator WhenProperty(string name, string unevaluatedValue, string condition = null, bool setIfEmpty = false, string label = null)
        {
            if (_lastWhenPropertyGroup == null)
            {
                WhenPropertyGroup();
            }

            Property(_lastWhenPropertyGroup, name, unevaluatedValue, condition, setIfEmpty, label);

            return this;
        }

        /// <summary>
        /// Adds a &lt;PropertyGroup /&gt; element to the current &lt;When /&gt; element.
        /// </summary>
        /// <param name="condition">An optional condition to add to the property group.</param>
        /// <param name="label">An optional label to add to the property group.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        /// <exception cref="ProjectCreatorException">A &lt;When /&gt; element has not been added.</exception>
        public ProjectCreator WhenPropertyGroup(string condition = null, string label = null)
        {
            if (_lastWhen == null)
            {
                throw new ProjectCreatorException(Strings.ErrorWhenPropertyGroupRequiresWhen);
            }

            _lastWhenPropertyGroup = PropertyGroup(_lastWhen, condition, label);

            return this;
        }
    }
}