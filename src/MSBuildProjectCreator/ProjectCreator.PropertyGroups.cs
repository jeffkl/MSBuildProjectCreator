// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Construction;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class ProjectCreator
    {
        /// <summary>
        /// Stores the last property group that was added.
        /// </summary>
        private ProjectPropertyGroupElement _lastPropertyGroup;

        /// <summary>
        /// Gets the last property group that was added if there is one, otherwise one is added.
        /// </summary>
        protected ProjectPropertyGroupElement LastPropertyGroup
        {
            get
            {
                if (_lastPropertyGroup == null)
                {
                    PropertyGroup();
                }

                return _lastPropertyGroup;
            }
        }

        /// <summary>
        /// Adds a &lt;PropertyGroup /&gt; element to the current project.
        /// </summary>
        /// <param name="condition">An optional condition to add to the property group.</param>
        /// <param name="label">An optional label to add to the property group.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator PropertyGroup(string condition = null, string label = null)
        {
            _lastPropertyGroup = AddTopLevelElement(RootElement.CreatePropertyGroupElement());

            _lastPropertyGroup.Condition = condition;

            _lastPropertyGroup.Label = label;

            return this;
        }

        /// <summary>
        /// Adds a &lt;PropertyGroup /&gt; element to the specified <see cref="ProjectElementContainer"/>.
        /// </summary>
        /// <param name="parent">The parent <see cref="ProjectElementContainer"/> to add the property group to.</param>
        /// <param name="condition">An optional condition to add to the property group.</param>
        /// <param name="label">An optional label to add to the property group.</param>
        /// <returns>The <see cref="ProjectElementContainer"/> that was added.</returns>
        protected ProjectPropertyGroupElement PropertyGroup(ProjectElementContainer parent, string condition = null, string label = null)
        {
            ProjectPropertyGroupElement propertyGroup = RootElement.CreatePropertyGroupElement();

            parent.AppendChild(propertyGroup);

            propertyGroup.Condition = condition;

            propertyGroup.Label = label;

            return propertyGroup;
        }
    }
}