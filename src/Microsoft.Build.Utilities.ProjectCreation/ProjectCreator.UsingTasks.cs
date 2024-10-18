// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class ProjectCreator
    {
        /// <summary>
        /// Stores the last UsingTask that was added.
        /// </summary>
        private ProjectUsingTaskElement? _lastUsingTask;

        /// <summary>
        /// Adds a &lt;UsingTask /&gt; that refers to an assembly by a file path.
        /// </summary>
        /// <param name="taskName">The name of the task.</param>
        /// <param name="assemblyFile">The full or relative file path of the assembly to load.</param>
        /// <param name="taskFactory">An optional class in the assembly that is responsible for generating instances of the specified Task name.</param>
        /// <param name="runtime">An optional runtime for the task.</param>
        /// <param name="architecture">An optional architecture for the task.</param>
        /// <param name="condition">An optional condition to add to the task.</param>
        /// <param name="label">An optional label to add to the task.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator UsingTaskAssemblyFile(string taskName, string assemblyFile, string? taskFactory = null, string? runtime = null, string? architecture = null, string? condition = null, string? label = null)
        {
            _lastUsingTask = AddTopLevelElement(RootElement.CreateUsingTaskElement(taskName, assemblyFile, null, runtime, architecture));

            _lastUsingTask.TaskFactory = taskFactory;
            _lastUsingTask.Condition = condition;
            _lastUsingTask.Label = label;

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
        /// <param name="label">An optional label to add to the task.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator UsingTaskAssemblyName(string taskName, string assemblyName, string taskFactory, string? runtime = null, string? architecture = null, string? condition = null, string? label = null)
        {
            _lastUsingTask = AddTopLevelElement(RootElement.CreateUsingTaskElement(taskName, null, assemblyName, runtime, architecture));

            _lastUsingTask.TaskFactory = taskFactory;
            _lastUsingTask.Condition = condition;
            _lastUsingTask.Label = label;

            return this;
        }

        /// <summary>
        /// Adds a &lt;Task /&gt; element to the current &lt;UsingTask /&gt;.
        /// </summary>
        /// <param name="body">The data that is passed to the TaskFactory to generate an instance of the task.</param>
        /// <param name="evaluate">An optional value indicating if the body should be evaluated.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
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
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator UsingTaskParameter(string name, string? parameterType = null, bool? output = null, bool? required = null)
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
            _lastUsingTask!.ParameterGroup!.AddParameter(
                name,
                output?.ToString() ?? string.Empty,
                required?.ToString() ?? string.Empty,
                parameterType ?? string.Empty);

            return this;
        }

        /// <summary>
        /// Adds a &lt;UsingTask /&gt; with the TaskFactory set to "RoslynCodeTaskFactory" and the provided
        /// code fragment or source file as the task body.
        /// </summary>
        /// <remarks>
        /// See https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-roslyncodetaskfactory for
        /// documentation on using a RoslynCodeTaskFactory.
        /// </remarks>
        /// <param name="taskName">The name of the task.</param>
        /// <param name="sourceCode">C# or VB code to use as the task body. Mutually exclusive with <paramref name="sourcePath"/>.</param>
        /// <param name="sourcePath">Path to a source to use as the task body. Mutually exclusive with <paramref name="sourceCode"/>.</param>
        /// <param name="type">The type of code in the task body. Defaults to "Fragment", can also be "Method" or "Class".</param>
        /// <param name="language">The source language. Defaults to "cs", can also be "vb".</param>
        /// <param name="references">Paths to assemblies that should be added as references during compilation.</param>
        /// <param name="usings">The list of namespaces to include as part of the compilation.</param>
        /// <param name="taskFactory">The TaskFactory to use. Defaults to "RoslynCodeTaskFactory".</param>
        /// <param name="runtime">An optional runtime for the task.</param>
        /// <param name="architecture">An optional architecture for the task.</param>
        /// <param name="condition">An optional condition to add to the task.</param>
        /// <param name="label">An optional label to add to the task.</param>
        /// <param name="evaluate">An optional value indicating if the body should be evaluated.</param>
        /// <returns>The current <see cref="ProjectCreator" />.</returns>
        public ProjectCreator UsingTaskRoslynCodeTaskFactory(
            string taskName,
            string? sourceCode = null,
            string? sourcePath = null,
            string type = "Fragment",
            string language = "cs",
            IEnumerable<string>? references = null,
            IEnumerable<string>? usings = null,
            string taskFactory = "RoslynCodeTaskFactory",
            string? runtime = null,
            string? architecture = null,
            string? condition = null,
            string? label = null,
            bool? evaluate = null)
        {
            if (sourceCode is null && sourcePath is null)
            {
                throw new ProjectCreatorException(Strings.ErrorUsingTaskRoslynCodeTaskFactoryRequiresSourceCodeOrSourcePath);
            }

            if (sourceCode is not null && sourcePath is not null)
            {
                throw new ProjectCreatorException(Strings.ErrorUsingTaskRoslynCodeTaskFactoryRequiresSourceCodeOrSourcePath);
            }

            UsingTaskAssemblyFile(
                taskName,
                assemblyFile: @"$(MSBuildToolsPath)\Microsoft.Build.Tasks.Core.dll",
                taskFactory,
                runtime,
                architecture,
                condition,
                label);

            using StringWriter sw = new();
            XmlWriterSettings settings = new()
            {
                ConformanceLevel = ConformanceLevel.Fragment,
                OmitXmlDeclaration = true,
                Indent = false, // If we don't indent Microsoft.Build.Construction will do it for us
            };
            using (XmlWriter writer = XmlWriter.Create(sw, settings))
            {
                foreach (string r in references ?? [])
                {
                    writer.WriteStartElement("Reference");
                    writer.WriteAttributeString("Include", r);
                    writer.WriteEndElement(); // </Reference>
                }

                foreach (string u in usings ?? [])
                {
                    writer.WriteStartElement("Using");
                    writer.WriteAttributeString("Namespace", u);
                    writer.WriteEndElement(); // </Using>
                }

                writer.WriteStartElement("Code");
                writer.WriteAttributeString("Type", type);
                writer.WriteAttributeString("Language", language);
                writer.WriteAttributeStringIfNotNull("Source", sourcePath);

                if (sourceCode is not null)
                {
                    if (!sourceCode.AsSpan().TrimStart().StartsWith("<![CDATA[".AsSpan(), StringComparison.Ordinal))
                    {
                        writer.WriteCData(sourceCode);
                    }
                    else
                    {
                        writer.WriteRaw(sourceCode);
                    }
                }

                writer.WriteEndElement(); // </Code>
            }

            UsingTaskBody(sw.ToString(), evaluate);

            return this;
        }
    }
}