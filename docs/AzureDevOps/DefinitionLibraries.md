# Definition libraries

For YAML pipelines, the code re-use happens via [pipeline templates](https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#template-references).
Once you start using C#, you can also re-use code more efficiently since templates can be quite cumbersome to use in some areas such as parameter contracts which are not strong typed.

It can also be useful to define a set of re-usable items such as build steps rather than defining one step at a time.
For this purpose, Sharpliner has **libraries**.
Libraries are collections of items that can be referenced in a pipeline.

Same as for template, you can define libraries of stages, jobs, steps and variables.
For a full list of classes you can override, see [PublicDefinitions.cs](https://github.com/sharpliner/sharpliner/blob/main/src/Sharpliner/AzureDevOps/PublicDefinitions.cs).

The definition looks like this:

```csharp
class ProjectBuildSteps : StepLibrary
{
    public override List<Conditioned<Step>> Steps =>
    [
        DotNet.Install.Sdk("6.0.100"),

        If.IsBranch("main")
            .Step(DotNet.Restore.Projects("src/MyProject.sln")),

        DotNet.Build("src/MyProject.sln"),
    ];
}
```

You can then reference this library in between build steps and it will get expanded into the pipeline's YAML:

```csharp
new Job("Build")
{
    Steps =
    [
        Script.Inline("echo 'Hello World'"),

        StepLibrary<ProjectBuildSteps>(),

        Script.Inline("echo 'Goodbye World'"),
    ]
}
```

The example above expects the library to have a parameterless constructor.
However, you can also control the library creation and have something a bit more modular:

```csharp
class ProjectBuildSteps(string project) : StepLibrary
{
    public override List<Conditioned<Step>> Steps =>
    [
        DotNet.Install.Sdk("6.0.100"),

        If.IsBranch("main")
            .Step(DotNet.Restore.Projects(project)),

        DotNet.Build(project),
    ];
}
```

Then you can use this library for different projects easily:

```csharp
new Job("Build")
{
    Steps =
    [
        Script.Inline("echo 'Hello World'"),

        StepLibrary(new ProjectBuildSteps("src/MyProject.sln")),

        Script.Inline("echo 'Goodbye World'"),
    ]
}
```

Furthermore, you can also pass a list of items to the methods that reference libraries and get those expanded without the need to declare a new library class.
This gives you quite a bit flexibility when dealing with repetitive patterns:

```csharp
new Job("Build")
{
    Steps =
    [
        Script.Inline("echo 'Hello World'"),

        ExpandSteps([ "5.0.403", "6.0.100" ].Select(version => DotNet.Install.Sdk(version))),

        Script.Inline("echo 'Goodbye World'"),
    ]
}
```