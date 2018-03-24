// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Construction;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class ProjectCreator
    {
        /// <summary>
        /// Adds a property element to the current &lt;PropertyGroup /&gt;.  A property group is automatically added if necessary.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="unevaluatedValue">The unevaluated value of the property.</param>
        /// <param name="condition">An optional condition to add to the property.</param>
        /// <param name="setIfEmpty">An optional value indicating whether or not a condition should be added that checks if the property has already been set.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        /// <remarks>
        /// The <paramref name="setIfEmpty"/> parameter will add a condition such as " '$(Property)' == '' " which will only set the property if it has not already been set.
        /// </remarks>
        public ProjectCreator Property(string name, string unevaluatedValue, string condition = null, bool setIfEmpty = false)
        {
            return Property(LastPropertyGroup, name, unevaluatedValue, condition, setIfEmpty);
        }

        /// <summary>
        /// Adds a property element to the current &lt;PropertyGroup /&gt;.  A property group is automatically added if necessary.
        /// </summary>
        /// <param name="propertyGroup">The <see cref="ProjectPropertyGroupElement"/> to add the property to.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="unevaluatedValue">The unevaluated value of the property.</param>
        /// <param name="condition">An optional condition to add to the property.</param>
        /// <param name="setIfEmpty">An optional value indicating whether or not a condition should be added that checks if the property has already been set.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        /// <remarks>
        /// The <paramref name="setIfEmpty"/> parameter will add a condition such as " '$(Property)' == '' " which will only set the property if it has not already been set.
        /// </remarks>
        private ProjectCreator Property(ProjectPropertyGroupElement propertyGroup, string name, string unevaluatedValue, string condition = null, bool setIfEmpty = false)
        {
            ProjectPropertyElement propertyElement = propertyGroup.AddProperty(name, unevaluatedValue);

            if (setIfEmpty && condition != null)
            {
                propertyElement.Condition = $" '$({propertyElement.Name})' == '' And {condition} ";
            }
            else if (setIfEmpty)
            {
                propertyElement.Condition = $" '$({propertyElement.Name})' == '' ";
            }
            else
            {
                propertyElement.Condition = condition;
            }

            return this;
        }
    }
}