// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class ProjectCreator
    {
        /// <summary>
        /// Adds a &lt;Compile /&gt; item to the current item group.
        /// </summary>
        /// <param name="include">The file or wildcard to include in the list of items.</param>
        /// <param name="exclude">An optional file or wildcard to exclude from the list of items.</param>
        /// <param name="dependentUpon">An optional path to a file that this file depends on to compile correctly.</param>
        /// <param name="link">An optional notational path to be displayed when the file is physically located outside the influence of the project file.</param>
        /// <param name="isVisible">An optional value indicating whether to display the file in Solution Explorer in Visual Studio.</param>
        /// <param name="metadata">An optional <see cref="IDictionary{String,String}"/> containing metadata for the item.</param>
        /// <param name="condition">An optional condition to add to the item.</param>
        /// <param name="label">An optional label to add to the item.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemCompile(string include, string exclude = null, string dependentUpon = null, string link = null, bool? isVisible = null, IDictionary<string, string> metadata = null, string condition = null, string label = null)
        {
            return ItemInclude(
                itemType: "Compile",
                include: include,
                metadata: metadata.Merge(new Dictionary<string, string>
                {
                    { "DependentUpon", dependentUpon },
                    { "Link", link },
                    { "Visible", isVisible?.ToString() },
                }),
                condition: condition,
                label: label);
        }

        /// <summary>
        /// Adds a &lt;Content /&gt; item to the current item group.
        /// </summary>
        /// <param name="include">The file or wildcard to include in the list of items.</param>
        /// <param name="exclude">An optional file or wildcard to exclude from the list of items.</param>
        /// <param name="dependentUpon">An optional path to a file that this file depends on to compile correctly.</param>
        /// <param name="link">An optional notational path to be displayed when the file is physically located outside the influence of the project file.</param>
        /// <param name="isVisible">An optional value indicating whether to display the file in Solution Explorer in Visual Studio.</param>
        /// <param name="copyToOutputDirectory">An optional value specifying whether to copy the file to the output directory. Values are:
        /// <list type="bullet">
        ///   <item>
        ///     <description>Never - (Default) Do not copy the item to the output directory.</description>
        ///   </item>
        ///   <item>
        ///     <description>Always - Always copy the item to the output directory.</description>
        ///   </item>
        ///   <item>
        ///     <description>PreserveNewest - Only top the item to the output directory if it has changed.</description>
        ///   </item>
        /// </list>
        /// </param>
        /// <param name="metadata">An optional <see cref="IDictionary{String,String}"/> containing metadata for the item.</param>
        /// <param name="condition">An optional condition to add to the item.</param>
        /// <param name="label">An optional label to add to the item.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemContent(string include, string exclude = null, string dependentUpon = null, string link = null, bool? isVisible = null, string copyToOutputDirectory = null, IDictionary<string, string> metadata = null, string condition = null, string label = null)
        {
            return ItemInclude(
                itemType: "Content",
                include: include,
                exclude: exclude,
                metadata: metadata.Merge(new Dictionary<string, string>
                {
                    { "CopyToOutputDirectory", copyToOutputDirectory },
                    { "DependentUpon", dependentUpon },
                    { "Link", link },
                    { "Visible", isVisible?.ToString() },
                }),
                condition: condition,
                label: label);
        }

        /// <summary>
        /// Adds an item to the current item group.  If <paramref name="include"/> is null, the item is not added.
        /// </summary>
        /// <param name="itemType">The type of the item to add.</param>
        /// <param name="include">The file or wildcard to include in the list of items.</param>
        /// <param name="exclude">An optional file or wildcard to exclude from the list of items.</param>
        /// <param name="metadata">An optional <see cref="IDictionary{String,String}"/> containing metadata for the item.</param>
        /// <param name="condition">An optional condition to add to the item.</param>
        /// <param name="label">An optional label to add to the item.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemInclude(
            string itemType,
            string include,
            string exclude = null,
            IDictionary<string, string> metadata = null,
            string condition = null,
            string label = null)
        {
            return
                include == null
                    ? this
                    : Item(LastItemGroup, itemType, include, exclude, metadata, null, null, condition, label: label);
        }

        /// <summary>
        /// Adds a &lt;None /&gt; item to the current item group.
        /// </summary>
        /// <param name="include">The file or wildcard to include in the list of items.</param>
        /// <param name="exclude">An optional file or wildcard to exclude from the list of items.</param>
        /// <param name="dependentUpon">An optional path to a file that this file depends on to compile correctly.</param>
        /// <param name="link">An optional notational path to be displayed when the file is physically located outside the influence of the project file.</param>
        /// <param name="isVisible">An optional value indicating whether to display the file in Solution Explorer in Visual Studio.</param>
        /// <param name="copyToOutputDirectory">An optional value specifying whether to copy the file to the output directory. Values are:
        /// <list type="bullet">
        ///   <item>
        ///     <description>Never - (Default) Do not copy the item to the output directory.</description>
        ///   </item>
        ///   <item>
        ///     <description>Always - Always copy the item to the output directory.</description>
        ///   </item>
        ///   <item>
        ///     <description>PreserveNewest - Only top the item to the output directory if it has changed.</description>
        ///   </item>
        /// </list>
        /// </param>
        /// <param name="metadata">An optional <see cref="IDictionary{String,String}"/> containing metadata for the item.</param>
        /// <param name="condition">An optional condition to add to the item.</param>
        /// <param name="label">An optional label to add to the item.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemNone(string include, string exclude = null, string dependentUpon = null, string link = null, bool? isVisible = null, string copyToOutputDirectory = null, IDictionary<string, string> metadata = null, string condition = null, string label = null)
        {
            return ItemInclude(
                itemType: "None",
                include: include,
                exclude: exclude,
                metadata: metadata.Merge(new Dictionary<string, string>
                {
                    { "CopyToOutputDirectory", copyToOutputDirectory },
                    { "DependentUpon", dependentUpon },
                    { "Link", link },
                    { "Visible", isVisible?.ToString() },
                }),
                condition: condition,
                label: label);
        }

        /// <summary>
        /// Adds a &lt;PackageReference /&gt; item to the current item group.
        /// </summary>
        /// <param name="include">The ID of the package to reference.</param>
        /// <param name="version">An optional version of the package to reference.</param>
        /// <param name="includeAssets">An optional value specifying which assets belonging to the package should be consumed.</param>
        /// <param name="excludeAssets">An optional value specifying which assets belonging to the package should be not consumed.</param>
        /// <param name="privateAssets">An optional value specifying which assets belonging to the package should not flow to dependent projects.</param>
        /// <param name="metadata">An optional <see cref="IDictionary{String,String}"/> containing metadata for the item.</param>
        /// <param name="condition">An optional condition to add to the item.</param>
        /// <param name="label">An optional label to add to the item.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemPackageReference(string include, string version = null, string includeAssets = null, string excludeAssets = null, string privateAssets = null, IDictionary<string, string> metadata = null, string condition = null, string label = null)
        {
            return ItemInclude(
                itemType: "PackageReference",
                include: include,
                metadata: metadata.Merge(new Dictionary<string, string>
                {
                    { "Version", version },
                    { "IncludeAssets", includeAssets },
                    { "ExcludeAssets", excludeAssets },
                    { "PrivateAssets", privateAssets },
                }),
                condition: condition,
                label: label);
        }

        /// <summary>
        /// Adds a &lt;ProjectReference /&gt; item to the current item group.
        /// </summary>
        /// <param name="project">A <see cref="Project"/> to get the full path from.</param>
        /// <param name="name">An optional name for the project reference.</param>
        /// <param name="projectGuid">An optional project GUID for the project reference.</param>
        /// <param name="referenceOutputAssembly">An optional value indicating if the output of the project reference should be referenced by the current project.</param>
        /// <param name="metadata">An optional <see cref="IDictionary{String,String}"/> containing metadata for the item.</param>
        /// <param name="condition">An optional condition to add to the item.</param>
        /// <param name="label">An optional label to add to the item.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemProjectReference(Project project, string name = null, string projectGuid = null, bool? referenceOutputAssembly = null, IDictionary<string, string> metadata = null, string condition = null, string label = null)
        {
            return ItemProjectReference(project.Xml, name, projectGuid, referenceOutputAssembly, metadata, condition, label);
        }

        /// <summary>
        /// Adds a &lt;ProjectReference /&gt; item to the current item group.
        /// </summary>
        /// <param name="rootElement">A <see cref="ProjectRootElement"/> to get the full path from.</param>
        /// <param name="name">An optional name for the project reference.</param>
        /// <param name="projectGuid">An optional project GUID for the project reference.</param>
        /// <param name="referenceOutputAssembly">An optional value indicating if the output of the project reference should be referenced by the current project.</param>
        /// <param name="metadata">An optional <see cref="IDictionary{String,String}"/> containing metadata for the item.</param>
        /// <param name="condition">An optional condition to add to the item.</param>
        /// <param name="label">An optional label to add to the item.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemProjectReference(ProjectRootElement rootElement, string name = null, string projectGuid = null, bool? referenceOutputAssembly = null, IDictionary<string, string> metadata = null, string condition = null, string label = null)
        {
            return ItemProjectReference(rootElement.FullPath, name, projectGuid, referenceOutputAssembly, metadata, condition, label);
        }

        /// <summary>
        /// Adds a &lt;ProjectReference /&gt; item to the current item group.
        /// </summary>
        /// <param name="projectCreator">A <see cref="ProjectCreator"/> to get the full path from.</param>
        /// <param name="name">An optional name for the project reference.</param>
        /// <param name="projectGuid">An optional project GUID for the project reference.</param>
        /// <param name="referenceOutputAssembly">An optional value indicating if the output of the project reference should be referenced by the current project.</param>
        /// <param name="metadata">An optional <see cref="IDictionary{String,String}"/> containing metadata for the item.</param>
        /// <param name="condition">An optional condition to add to the item.</param>
        /// <param name="label">An optional label to add to the item.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemProjectReference(ProjectCreator projectCreator, string name = null, string projectGuid = null, bool? referenceOutputAssembly = null, IDictionary<string, string> metadata = null, string condition = null, string label = null)
        {
            return ItemProjectReference(projectCreator.RootElement, name, projectGuid, referenceOutputAssembly, metadata, condition, label);
        }

        /// <summary>
        /// Adds a &lt;ProjectReference /&gt; item to the current item group.
        /// </summary>
        /// <param name="include">The path to the project for the reference.</param>
        /// <param name="name">An optional name for the project reference.</param>
        /// <param name="projectGuid">An optional project GUID for the project reference.</param>
        /// <param name="referenceOutputAssembly">An optional value indicating if the output of the project reference should be referenced by the current project.</param>
        /// <param name="metadata">An optional <see cref="IDictionary{String,String}"/> containing metadata for the item.</param>
        /// <param name="condition">An optional condition to add to the item.</param>
        /// <param name="label">An optional label to add to the item.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemProjectReference(string include, string name = null, string projectGuid = null, bool? referenceOutputAssembly = null, IDictionary<string, string> metadata = null, string condition = null, string label = null)
        {
            return ItemInclude(
                itemType: "ProjectReference",
                include: include,
                metadata: metadata.Merge(new Dictionary<string, string>
                {
                    { "Name", name },
                    { "Project", projectGuid },
                    { "ReferenceOutputAssembly", referenceOutputAssembly?.ToString() },
                }),
                condition: condition,
                label: label);
        }

        /// <summary>
        /// Adds a &lt;Reference /&gt; item to the current item group.
        /// </summary>
        /// <param name="include">The item or path for the reference.</param>
        /// <param name="name">An optional name for the reference.</param>
        /// <param name="hintPath">An optional hint path for the reference.</param>
        /// <param name="isSpecificVersion">An optional value indicating whether or not to use a specific version for the reference.</param>
        /// <param name="aliases">An optional list of aliases for the reference.</param>
        /// <param name="isPrivate">An optional value indicating whether or not the reference should be copied to the output folder.</param>
        /// <param name="metadata">An optional <see cref="IDictionary{String,String}"/> containing metadata for the item.</param>
        /// <param name="condition">An optional condition to add to the item.</param>
        /// <param name="label">An optional label to add to the item.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemReference(string include, string name = null, string hintPath = null, bool? isSpecificVersion = null, string aliases = null, bool? isPrivate = null, IDictionary<string, string> metadata = null, string condition = null, string label = null)
        {
            return ItemInclude(
                itemType: "Reference",
                include: include,
                metadata: metadata.Merge(new Dictionary<string, string>
                {
                    { "Aliases", aliases },
                    { "HintPath", hintPath },
                    { "Name", name },
                    { "Private", isPrivate?.ToString() },
                    { "SpecificVersion", isSpecificVersion?.ToString() },
                }),
                condition: condition,
                label: label);
        }

        /// <summary>
        /// Remove items in the current item group.  If <paramref name="remove"/> is <code>null</code>, the item is not added.
        /// </summary>
        /// <param name="itemType">The type of the item to remove items from.</param>
        /// <param name="remove">The file or wildcard to remove in the list of items.</param>
        /// <param name="metadata">An optional <see cref="IDictionary{String,String}"/> containing metadata for the item.</param>
        /// <param name="condition">An optional condition to add to the item.</param>
        /// <param name="label">An optional label to add to the item.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemRemove(
            string itemType,
            string remove,
            IDictionary<string, string> metadata = null,
            string condition = null,
            string label = null)
        {
            return remove == null
                    ? this
                    : Item(
                        itemGroup: LastItemGroup,
                        itemType: itemType,
                        include: null,
                        exclude: null,
                        metadata: metadata,
                        remove: remove,
                        update: null,
                        condition: condition,
                        label: label);
        }

        /// <summary>
        /// Updates items in the current item group.  If <paramref name="update"/> is <code>null</code>, the item is not added.
        /// </summary>
        /// <param name="itemType">The type of the item to update items in.</param>
        /// <param name="update">The file or wildcard to update in the list of items.</param>
        /// <param name="metadata">An optional <see cref="IDictionary{String,String}"/> containing metadata for the item.</param>
        /// <param name="condition">An optional condition to add to the item.</param>
        /// <param name="label">An optional label to add to the item.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemUpdate(
            string itemType,
            string update,
            IDictionary<string, string> metadata = null,
            string condition = null,
            string label = null)
        {
            return update == null
                    ? this
                    : Item(
                        itemGroup: LastItemGroup,
                        itemType: itemType,
                        include: null,
                        exclude: null,
                        metadata: metadata,
                        remove: null,
                        update: update,
                        condition: condition,
                        label: label);
        }

        /// <summary>
        /// Gets a list of items in the project.
        /// </summary>
        /// <param name="itemType">The item type.</param>
        /// <param name="items">A <see cref="IReadOnlyCollection{ProjectItem}"/> containing the items.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryGetItems(string itemType, out IReadOnlyCollection<ProjectItem> items)
        {
            items = Project.GetItems(itemType).ToList();

            return this;
        }

        /// <summary>
        /// Gets a list of items in the project.
        /// </summary>
        /// <typeparam name="T">The type of the object that the selector will return.</typeparam>
        /// <param name="itemType">The item type.</param>
        /// <param name="selector">A <see cref="Func{ProjectItem, T}"/> that gets the desired return object.</param>
        /// <param name="items">A <see cref="IReadOnlyCollection{T}"/> containing the items.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryGetItems<T>(string itemType, Func<ProjectItem, T> selector, out IReadOnlyCollection<T> items)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            items = Project.GetItems(itemType).Select(selector).ToList();

            return this;
        }

        /// <summary>
        /// Gets a list of items in the project.
        /// </summary>
        /// <param name="itemType">The item type.</param>
        /// <param name="metadataName">The name of a metadata item to get.</param>
        /// <param name="items">A <see cref="IReadOnlyDictionary{String,String}"/> containing the ItemSpec and metadata value.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator TryGetItems(string itemType, string metadataName, out IReadOnlyDictionary<string, string> items)
        {
            if (string.IsNullOrWhiteSpace(metadataName))
            {
                throw new ArgumentNullException(nameof(metadataName));
            }

            items = Project.GetItems(itemType).ToDictionary(i => i.EvaluatedInclude, i => i.GetMetadataValue(metadataName));

            return this;
        }

        /// <summary>
        /// Adds an item to the current item group.
        /// </summary>
        /// <param name="itemGroup">The parent item group to add the item to.</param>
        /// <param name="itemType">The type of the item to add.</param>
        /// <param name="include">The file or wildcard to include in the list of items.</param>
        /// <param name="exclude">An optional file or wildcard to exclude from the list of items.</param>
        /// <param name="metadata">An optional <see cref="IDictionary{String,String}"/> containing metadata for the item.</param>
        /// <param name="remove">The file or wildcard to remove from the list of items.</param>
        /// <param name="update">The file or wildcard to update in the list of items.</param>
        /// <param name="condition">An optional condition to add to the item.</param>
        /// <param name="keepDuplicates">Specifies whether an item should be added to the target group if it's an exact duplicate of an existing item.</param>
        /// <param name="keepMetadata">The metadata for the source items to add to the target items.</param>
        /// <param name="label">An optional label to add to the item.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        private ProjectCreator Item(
            ProjectItemGroupElement itemGroup,
            string itemType,
            string include,
            string exclude = null,
            IDictionary<string, string> metadata = null,
            string remove = null,
            string update = null,
            string condition = null,
            string keepDuplicates = null,
            string keepMetadata = null,
            string label = null)
        {
            ProjectItemElement item = include == null
                ? RootElement.CreateItemElement(itemType)
                : RootElement.CreateItemElement(itemType, include);

            item.Remove = remove;
            item.Update = update;

            // Item must be added after Include, Update, or Remove is set but before metadata is added
            itemGroup.AppendChild(item);

            if (metadata != null)
            {
                foreach (KeyValuePair<string, string> metadatum in metadata.Where(i => i.Value != null))
                {
                    item.AddMetadata(metadatum.Key, metadatum.Value);
                }
            }

            item.Condition = condition;
            item.Label = label;
            item.Exclude = exclude;

            if (keepDuplicates != null)
            {
                item.KeepDuplicates = keepDuplicates;
            }

            if (keepMetadata != null)
            {
                item.KeepMetadata = keepMetadata;
            }

            return this;
        }
    }
}