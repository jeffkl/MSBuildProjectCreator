# Overview
[![Build status](https://ci.appveyor.com/api/projects/status/39naoww2fy6xslt7/branch/master?svg=true)](https://ci.appveyor.com/project/CBT/msbuildprojectcreator/branch/master)
[![NuGet](https://img.shields.io/nuget/v/MSBuild.ProjectCreation.svg)](https://www.nuget.org/packages/MSBuild.ProjectCreation)
[![NuGet](https://img.shields.io/nuget/dt/MSBuild.ProjectCreation.svg)](https://www.nuget.org/packages/MSBuild.ProjectCreation)

This class library is a [fluent interface](https://en.wikipedia.org/wiki/Fluent_interface) for generating MSBuild projects.  Its primarily for unit tests that need MSBuild projects to do their testing.

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
    public ProjectCreator ItemMyCustomType(this ProjectCreator creator, string include, string param1, string param2, string condition = null)
    {
        return creator.ItemInclude(
            "MyCustomType",
            include,
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
