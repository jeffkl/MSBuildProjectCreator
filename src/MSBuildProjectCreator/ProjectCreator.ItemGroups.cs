// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Construction;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class ProjectCreator
    {
        /// <summary>
        /// Stores the last import group that was added.
        /// </summary>
        private ProjectItemGroupElement _lastItemGroup;

        /// <summary>
        /// Gets the last import group that was added if there is one, otherwise one is added.
        /// </summary>
        protected ProjectItemGroupElement LastItemGroup
        {
            get
            {
                if (_lastItemGroup == null)
                {
                    ItemGroup();
                }

                return _lastItemGroup;
            }
        }

        /// <summary>
        /// Adds an &lt;ItemGroup /&gt; element to the current project.
        /// </summary>
        /// <param name="condition">An optional condition to add to the item group.</param>
        /// <param name="label">An optional label to add to the item group.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemGroup(string condition = null, string label = null)
        {
            _lastItemGroup = AddTopLevelElement(RootElement.CreateItemGroupElement());

            _lastItemGroup.Condition = condition;

            _lastItemGroup.Label = label;

            return this;
        }

        /// <summary>
        /// Adds an &lt;ItemGroup /&gt; element to the specifed parent.
        /// </summary>
        /// <param name="parent">A parent <see cref="ProjectElementContainer"/> to add the item group to.</param>
        /// <param name="condition">An optional condition to add to the item group.</param>
        /// <param name="label">An optional label to add to the item group.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        protected ProjectItemGroupElement ItemGroup(ProjectElementContainer parent, string condition = null, string label = null)
        {
            ProjectItemGroupElement itemGroup = RootElement.CreateItemGroupElement();

            parent.AppendChild(itemGroup);

            itemGroup.Condition = condition;

            itemGroup.Label = label;

            return itemGroup;
        }
    }
}