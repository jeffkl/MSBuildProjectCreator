using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Utilities.ProjectCreation;

namespace TestApp
{
    class Program
    {
        static Program()
        {
            AppDomain.CurrentDomain.AssemblyResolve += MSBuildAssemblyResolver.AssemblyResolve;
        }
        static void Main(string[] args)
        {
            ProjectCreator projectCreator = ProjectCreator.Templates.SdkCsproj(path: @"foo.csproj").Save();

            Project project = projectCreator.Project;

            Console.WriteLine($"Assembly Name: {project.GetPropertyValue("AssemblyName")}");
        }
    }
}
