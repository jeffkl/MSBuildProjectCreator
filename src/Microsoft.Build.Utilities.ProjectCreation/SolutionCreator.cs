// Copyright (c) Jeff Kluge. All rights reserved.
//
// Licensed under the MIT license.

using Microsoft.VisualStudio.SolutionPersistence;
using Microsoft.VisualStudio.SolutionPersistence.Model;
using Microsoft.VisualStudio.SolutionPersistence.Serializer;
using System;
using System.Threading;

namespace Microsoft.Build.Utilities.ProjectCreation
{
    public partial class SolutionCreator
    {
        private SolutionFolderModel? _lastSolutionFolder = default;

        private SolutionProjectModel? _lastProject = default;

        private string _path;

        private SolutionModel _solutionModel;

        private SolutionCreator(string path)
        {
            _path = path;

            _solutionModel = new SolutionModel();
        }

        public static SolutionCreator Create(string path)
        {
            return new SolutionCreator(path);
        }

        public SolutionCreator Folder(string path)
        {
            _lastSolutionFolder = _solutionModel.AddFolder(path);

            return this;
        }

        public SolutionCreator Project(ProjectCreator project, string? projectTypeName = null)
        {
            _lastProject = _solutionModel.AddProject(project.FullPath, projectTypeName, folder: null);

            return this;
        }

        public SolutionCreator Save()
        {
            ISolutionSerializer? serializer = SolutionSerializers.GetSerializerByMoniker(_path);

            if (serializer == null)
            {
                throw new InvalidOperationException($"No solution serializer found for path '{_path}'.");
            }

            serializer.SaveAsync(_path, _solutionModel, CancellationToken.None).GetAwaiter().GetResult();

            return this;
        }
    }
}
