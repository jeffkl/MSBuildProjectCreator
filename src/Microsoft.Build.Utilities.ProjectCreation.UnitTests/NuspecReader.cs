// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Microsoft.Build.Utilities.ProjectCreation.UnitTests
{
    public class NuspecReader
    {
        private static readonly XNamespace NuspecNamespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd";

        private readonly XDocument _document;

        private readonly XmlNamespaceManager _namespaceManager = new XmlNamespaceManager(new NameTable());

        public NuspecReader(string contents)
        {
            _document = XDocument.Parse(contents);

            _namespaceManager.AddNamespace("ns", NuspecNamespace.NamespaceName);
        }

        public NuspecReader(FileInfo fileInfo)
            : this(File.ReadAllText(fileInfo.FullName))
        {
        }

        public string? Authors => GetElement("authors");

        public IEnumerable<PackageContentFileEntry> ContentFiles => GetContentFiles();

        public string? Copyright => GetElement("copyright");

        public IEnumerable<(string? TargetFramework, IEnumerable<PackageDependency>? Dependencies)> DependencyGroups
        {
            get
            {
                foreach (XElement group in _document.XPathSelectElements("//ns:package/ns:metadata/ns:dependencies/ns:group", _namespaceManager))
                {
                    yield return (group.Attribute("targetFramework")?.Value, GetDependencies(group.Elements()));
                }
            }
        }

        public string? Description => GetElement("description");

        public bool DevelopmentDependency => string.Equals(bool.TrueString, GetElement("developmentDependency"), StringComparison.OrdinalIgnoreCase);

        public string? Icon => GetElement("icon");

        public string? IconUrl => GetElement("iconUrl");

        public string? Id => GetElement("id");

        public string? Language => GetElement("language");

        public string? License => GetElement("license");

        public string? LicenseType => GetAttribute("license", "type");

        public string? LicenseVersion => GetAttribute("license", "version");

        public string? Owners => GetElement("owners");

        public IEnumerable<string?> PackageTypes
        {
            get
            {
                foreach (XElement group in _document.XPathSelectElements("//ns:package/ns:metadata/ns:packageTypes/ns:packageType", _namespaceManager))
                {
                    yield return group.Attribute("name")?.Value;
                }
            }
        }

        public string? ProjectUrl => GetElement("projectUrl");

        public string? ReleaseNotes => GetElement("releaseNotes");

        public string? RepositoryBranch => GetAttribute("repository", "branch");

        public string? RepositoryCommit => GetAttribute("repository", "commit");

        public string? RepositoryType => GetAttribute("repository", "type");

        public string? RepositoryUrl => GetAttribute("repository", "url");

        public bool RequireLicenseAcceptance => string.Equals(bool.TrueString, GetElement("requireLicenseAcceptance"), StringComparison.OrdinalIgnoreCase);

        public bool Serviceable => string.Equals(bool.TrueString, GetElement("serviceable"), StringComparison.OrdinalIgnoreCase);

        public string? Summary => GetElement("summary");

        public string? Tags => GetElement("tags");

        public string? Title => GetElement("title");

        public string? Version => GetElement("version");

        private IEnumerable<PackageContentFileEntry> GetContentFiles()
        {
            foreach (XElement file in _document.XPathSelectElements("//ns:package/ns:metadata/ns:contentFiles/ns:files", _namespaceManager))
            {
                yield return new PackageContentFileEntry(
                    file.Attribute("buildAction")?.Value,
                    file.Attribute("copyToOutput")?.Value,
                    file.Attribute("include")?.Value,
                    file.Attribute("exclude")?.Value,
                    file.Attribute("flatten")?.Value);
            }
        }

        private IEnumerable<PackageDependency> GetDependencies(IEnumerable<XElement> dependencies)
        {
            foreach (XElement item in dependencies)
            {
                string? id = (string?)item.Attribute("id");
                string? version = (string?)item.Attribute("version");
                string? exclude = (string?)item.Attribute("exclude");

                if (id != null && version != null && exclude != null)
                {
                    yield return new PackageDependency(id, version, exclude);
                }
            }
        }

        private string? GetElement(string name)
        {
            return _document.XPathSelectElement($"//ns:package/ns:metadata/ns:{name}", _namespaceManager)?.Value;
        }

        private string? GetAttribute(string elementName, string attributeName)
        {
            return _document.XPathSelectElement($"//ns:package/ns:metadata/ns:{elementName}", _namespaceManager)?.Attribute(attributeName)?.Value;
        }
    }
}