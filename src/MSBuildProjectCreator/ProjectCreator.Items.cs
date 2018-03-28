// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using System.Collections.Generic;

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
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemCompile(string include, string exclude = null, string dependentUpon = null, string link = null, bool? isVisible = null, IDictionary<string, string> metadata = null, string condition = null)
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
                condition: condition);
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
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemContent(string include, string exclude = null, string dependentUpon = null, string link = null, bool? isVisible = null, string copyToOutputDirectory = null, IDictionary<string, string> metadata = null, string condition = null)
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
                condition: condition);
        }

        /// <summary>
        /// Adds an item to the current item group.
        /// </summary>
        /// <param name="itemType">The type of the item to add.</param>
        /// <param name="include">The file or wildcard to include in the list of items.</param>
        /// <param name="exclude">An optional file or wildcard to exclude from the list of items.</param>
        /// <param name="metadata">An optional <see cref="IDictionary{String,String}"/> containing metadata for the item.</param>
        /// <param name="condition">An optional condition to add to the item.</param>
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemInclude(
            string itemType,
            string include,
            string exclude = null,
            IDictionary<string, string> metadata = null,
            string condition = null)
        {
            return Item(LastItemGroup, itemType, include, exclude, metadata, null, null, condition);
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
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemNone(string include, string exclude = null, string dependentUpon = null, string link = null, bool? isVisible = null, string copyToOutputDirectory = null, IDictionary<string, string> metadata = null, string condition = null)
        {
            return ItemInclude(
                itemType: "None",
                include: include,
                metadata: metadata.Merge(new Dictionary<string, string>
                {
                    { "CopyToOutputDirectory", copyToOutputDirectory },
                    { "DependentUpon", dependentUpon },
                    { "Link", link },
                    { "Visible", isVisible?.ToString() },
                }),
                condition: condition);
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
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemPackageReference(string include, string version = null, string includeAssets = null, string excludeAssets = null, string privateAssets = null, IDictionary<string, string> metadata = null, string condition = null)
        {
            return ItemInclude(
                itemType: "PackageReference",
                include: include,
                metadata: metadata.Merge(new Dictionary<string, string>
                {
                    { "Version", version },
                    { "IncludeAssets", includeAssets },
                    { "ExcludeAssets", excludeAssets },
                    { "PrivateAssets", privateAssets }
                }),
                condition: condition);
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
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemProjectReference(Project project, string name = null, string projectGuid = null, bool? referenceOutputAssembly = null, IDictionary<string, string> metadata = null, string condition = null)
        {
            return ItemProjectReference(project.Xml, name, projectGuid, referenceOutputAssembly, metadata, condition);
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
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemProjectReference(ProjectRootElement rootElement, string name = null, string projectGuid = null, bool? referenceOutputAssembly = null, IDictionary<string, string> metadata = null, string condition = null)
        {
            return ItemProjectReference(rootElement.FullPath, name, projectGuid, referenceOutputAssembly, metadata, condition);
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
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemProjectReference(ProjectCreator projectCreator, string name = null, string projectGuid = null, bool? referenceOutputAssembly = null, IDictionary<string, string> metadata = null, string condition = null)
        {
            return ItemProjectReference(projectCreator.RootElement, name, projectGuid, referenceOutputAssembly, metadata, condition);
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
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemProjectReference(string include, string name = null, string projectGuid = null, bool? referenceOutputAssembly = null, IDictionary<string, string> metadata = null, string condition = null)
        {
            return ItemInclude(
                itemType: "ProjectReference",
                include: include,
                metadata: metadata.Merge(new Dictionary<string, string>
                {
                    { "Name", name },
                    { "Project", projectGuid },
                    { "ReferenceOutputAssembly", referenceOutputAssembly?.ToString() }
                }),
                condition: condition);
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
        /// <returns>The current <see cref="ProjectCreator"/>.</returns>
        public ProjectCreator ItemReference(string include, string name = null, string hintPath = null, bool? isSpecificVersion = null, string aliases = null, bool? isPrivate = null, IDictionary<string, string> metadata = null, string condition = null)
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
                    { "SpecificVersion", isSpecificVersion?.ToString() }
                }),
                condition: condition);
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
            string keepMetadata = null)
        {
            ProjectItemElement item = RootElement.CreateItemElement(itemType, include);

            itemGroup.AppendChild(item);

            if (metadata != null)
            {
                foreach (KeyValuePair<string, string> metadatum in metadata)
                {
                    item.AddMetadata(metadatum.Key, metadatum.Value);
                }
            } // ProjectItemElement item = itemGroup.AddItem(itemType, include, metadata);

            item.Condition = condition;
            item.Exclude = exclude;

            if (keepDuplicates != null)
            {
                item.KeepDuplicates = keepDuplicates;
            }

            if (keepMetadata != null)
            {
                item.KeepMetadata = keepMetadata;
            }

            item.Remove = remove;
            item.Update = update;

            return this;
        }
    }
}