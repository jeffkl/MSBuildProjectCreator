# Overview
[![Build Status](https://jeffkl.visualstudio.com/Public/_apis/build/status/MSBuildProjectCreator?branchName=main)](https://jeffkl.visualstudio.com/Public/_build/latest?definitionId=18&branchName=main)
[![NuGet](https://img.shields.io/nuget/v/MSBuild.ProjectCreation.svg)](https://www.nuget.org/packages/MSBuild.ProjectCreation)
[![NuGet](https://img.shields.io/nuget/dt/MSBuild.ProjectCreation.svg)](https://www.nuget.org/packages/MSBuild.ProjectCreation)

This class library is a [fluent interface](https://en.wikipedia.org/wiki/Fluent_interface) for generating MSBuild projects and NuGet package repositories.  Its primarily for unit tests that need MSBuild projects to do their testing.

## Example
You want to test a custom MSBuild task that you are building so your unit tests need to generate a project that you can build with MSBuild.  The following code would generate the necessary project:

```C#
ProjectCreator creator = ProjectCreator.Create("test.proj")
    .UsingTaskAssemblyFile("MyTask", pathToMyTaskAssembly)
    .ItemInclude("CustomItem", "abc")
    .Property("CustomProperty", "value")
    .Target("Build")
    .Task(
        name: "MyTask",
        parameters: new Dictionary<string, string>
        {
            { "MyProperty", "$(CustomProperty)" },
            { "Items", "@(CustomItem)" }
        });
```

The resulting project would look like this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <UsingTask TaskName="MyTask" AssemblyFile="..." />
  <ItemGroup>
    <CustomItem Include="abc" />
  </ItemGroup>
  <PropertyGroup>
    <CustomProperty>value</CustomProperty>
  </PropertyGroup>
  <Target Name="Build">
    <MyTask MyProperty="$(CustomProperty)" Items="@(CustomItem)" />
  </Target>
</Project>
```

## Building Projects
Use the `TryBuild` methods to build your projects.  `TryBuild` returns a `BuildOutput` object which captures the build output for you.

This example creates a project that logs a message, executes `TryBuild`, and asserts some conditions against the build output.
```C#
ProjectCreator.Create()
    .TaskMessage("Hello, World!")
    .TryBuild(out bool success, out BuildOutput log);

Assert.True(success);
Assert.Equal("Hello, World!", log.Messages.Single().Message);
```

## Extensibility
You can extend the `ProjectCreator` class by adding [extension methods](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods).  

### Custom Item Type
You can implement a method that adds a known item type with metadata.  It is recommended that you prefix the method with `Item` so it is included alphabetically with other methods that add items.  Your method should call the `ProjectCreator.ItemInclude` method with the custom metadata.
```C#
public static class ExtensionsMethods
{
    public static ProjectCreator ItemMyCustomType(this ProjectCreator creator, string include, string param1, string param2, string condition = null)
    {
        return creator.ItemInclude(
            "MyCustomType",
            include,
            null,
            new Dictionary<string, string>
            {
                { "Metadata1", param1 },
                { "Metadata2", param2 }
            },
            condition);
    }
}
```

The above extension method would add the following item:

```xml
<ItemGroup>
  <MyCustomType Include="X">
    <Metadata1>Y</Metadata1>
    <Metadata2>Y</Metadata2>
  </MyCustomType>
</ItemGroup>
```

## Templates
Several project templates are included for convenience purposes.  One template is the SDK-style C# project:

```C#
using Microsoft.Build;
using Microsoft.Build.Utilities.ProjectCreation;

namespace MyApplication
{
    public static void Main(string[] args)
    {
        ProjectRootElement project = ProjectCreator.Templates.SdkCsproj("project1.csproj");
    }
}
```

In the above example, the generated project looks like this:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
  </PropertyGroup>
</Project>

```

### Custom Templates

You can create your own templates by adding [extension methods](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods).

Your extension methods should extend the `ProjectCreatorTemplates` class so they are available as methods to the `ProjectCreator.Templates` property.

```C#
public static class ExtensionMethods
{
    public ProjectCreator LogsMessage(this ProjectCreatorTemplates template, string text, string path = null, MessageImportance ? importance = null, string condition = null)
    {
        return ProjectCreator.Create(path)
            .TaskMessage(text, importance, condition);
    }
}
```

The above extension method can be called like this:

```C#
using Microsoft.Build;
using Microsoft.Build.Utilities.ProjectCreation;

namespace MyApplication
{
    public static void Main(string[] args)
    {
        ProjectRootElement project = ProjectCreator.Templates.LogsMessage("Hello, World!");
    }
}
```

And the resulting project would look like this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Target Name="Build">
    <Message Text="Hello, World!" />
  </Target>
</Project>
```

# Package Repositories and Feeds
NuGet and MSBuild are very tightly coupled and a lot of times you need packages available when building projects.  This API offers two solutions:

1. Package repository - This allows you to create a repository of restored packages as if NuGet has already installed them.
2. Package feed - This allows you to create a file-based package feed of actual `.nupkg` files.

## Package Repository
Create a package repository if you want to generate packages as if they've already been installed.  If you want to create actual `.nupkg` packages, see [Package Feed]

### Example

Create a package repository with a package that supports two target frameworks:

```C#
using(PackageRepository.Create(rootPath)
    .Package("MyPackage", "1.2.3", out PackageIdentity package)
        .Library("net472")
        .Library("netstandard2.0"))
{
    // Create projects that reference packages
}
```

The resulting package would have a `lib\net472\MyPackage.dll` and `lib\netstandard2.0\MyPackage.dll` class library.  This allows you to restore and build projects that consume the packages

```C#
using(PackageRepository.Create(rootPath)
    .Package("MyPackage", "1.0.0", out PackageIdentity package)
        .Library("netstandard2.0"))
{
    ProjectCreator.Templates.SdkCsproj()
        .ItemPackageReference(package)
        .Save(Path.Combine(rootPath, "ClassLibraryA", "ClassLibraryA.csproj"))
        .TryBuild(restore: true, out bool result, out BuildOutput buildOutput);
}
```

The result would be a project that references the `MyPackage` package and would restore and build accordingly.

## Package Feed
Create a package feed if you want to generate `.nupkg` packages that can be installed by NuGet.  If you want to create a repository of packages as if they've already been installed, see [Package Repository].

### Example

Create a package feed with a package that supports two target frameworks:

```C#
PackageFeed.Create(rootPath)
    .Package("MyPackage", "1.2.3", out Package package)
        .Library("net472")
        .Library("netstandard2.0"))
    .Save();

ProjectCreator projectCreator = ProjectCreator.Create()
    .ItemPackageReference(package)

```

The resulting package would have a `lib\net472\MyPackage.dll` and `lib\netstandard2.0\MyPackage.dll` class library.  This allows you to restore and build projects that consume the packages
