# Azure DevOps pipeline definition reference

Here you can find detailed reference on how to define various parts of the pipeline.

For a full list of classes you can override to create a YAML file, see [PublicDefinitions.cs](https://github.com/sharpliner/sharpliner/blob/main/src/Sharpliner/AzureDevOps/PublicDefinitions.cs).

## Table of Contents
- [Build steps](#build-steps)
- [Azure Pipelines tasks](#azure-pipelines-tasks)
  - [Contributions welcome](#contributions-welcome)
- [Pipeline variables](#pipeline-variables)
- [Pipeline parameters](#pipeline-parameters)
- [Conditioned expressions](#conditioned-expressions)
  - [Conditions](#conditions)
- [Templates](#templates)
- [Definition libraries](#definition-libraries)

## Build steps

Build steps are basic building blocks of the build.
Azure DevOps pipelines [define several basic tasks](https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#steps) that can be used as the pipeline's build steps:
- `script`
- `bash`
- `pwsh`
- `powerShell`
- `publish`
- `download`
- `checkout`
- `task`

For each of these, there are two ways how you can define these.

Either you "new" them the regular way though this requires a longer syntax similar to what you would do in YAML:

```csharp
Steps =
{
    new AzureDevOpsTask("DotNetCoreCLI@2", "Build solution")
    {
        Inputs = new()
        {
            { "command", "build" },
            { "includeNuGetOrg", true },
            { "projects", "src/MyProject.sln" },
        },
        Timeout = TimeSpan.FromMinutes(20)
    },

    new InlineBashTask("./.dotnet/dotnet test src/MySolution.sln")
    {
        DisplayName = "Run unit tests",
        ContinueOnError = true,
    },
}
```

or you can use the shorthand style. For each of the basic commands, a method/property is defined on the parent class with the same name as the original task:

```csharp
Steps =
[
    Checkout.Self,

    Download.LatestFromBranch("internal", 23, "refs/heads/develop", artifact: "CLI.Package") with
    {
        AllowPartiallySucceededBuilds = true,
        CheckDownloadedFiles = true,
        PreferTriggeringPipeline = true,
    }

    // Tasks are represented as C# records so you can use the `with` keyword to override the properties
    DotNet.Build("src/MyProject.sln", includeNuGetOrg: true) with
    {
        Timeout = TimeSpan.FromMinutes(20)
    },

    // Some of the shorthand styles define more options and a cleaner way of defining them
    // E.g. Bash gives you several ways where to get the script from such as Bash.FromResourceFile or Bash.FromFile
    Bash.Inline("./.dotnet/dotnet test src/MySolution.sln") with
    {
        DisplayName = "Run unit tests",
        ContinueOnError = true,
    },

    Publish("ArtifactName", "bin/**/*.dll", "Publish build artifacts"),
]
```

Please notice the `with` keyword which is a [new feature in C#](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record#nondestructive-mutation) that allows modifying of records.

## Azure Pipelines tasks

Even though it is possible to use any of the non-default [Azure Pipelines tasks](https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/?view=azure-devops) by specifying its name + inputs:

```csharp
Task("DotNetCoreCLI@2", "Run unit tests") with
{
    Inputs = new()
    {
        { "command", "test" },
        { "projects", "src/MyProject.sln" },
    }
}
```

some of the tasks are quite hard to comprehend such as the [.NET Core CLI task](https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/build/dotnet-core-cli?view=azure-devops) whose specification is quite long since the task can do many different things.
Notice how some of the properties are only valid in a specific combination with other.
With Sharpliner, we remove some of this complexity by restricting which properties are available at a time and by using nice fluent APIs:

```csharp
DotNet.Install.Sdk(parameters["version"]),

DotNet.Restore.FromFeed("dotnet-7-preview-feed", includeNuGetOrg: false) with
{
    ExternalFeedCredentials = "feeds/dotnet-7",
    NoCache = true,
    RestoreDirectory = ".packages",
},

DotNet.Build("src/MyProject.csproj") with
{
    Timeout = TimeSpan.FromMinutes(20)
}
```

Note how we use the `with` keyword to extend the `record` object with new properties.

### Contributions welcome

Currently, we don't support many marketplace tasks in C# as the project is still growing.
If you find one useful, hit us up with a request, or better, with a pull request and we can add it to our library.

## Pipeline variables

Similarly to Build steps, there's a shorthand style of definition of variables too:
```csharp
Variables =
[
    Variable("Configuration", "Release"),     // We have shorthand style like we do for build steps
    Group("PR keyvault variables"),
    new Variable("Configuration", "Release"), // We can also create the objects and resue them too
    Variables(                                // You can also save some keystrokes and define multiple variables at once
        ("variable1", "value1"),
        ("variable2", true))
]
```

## Pipeline parameters
To define [pipeline runtime parameters](https://docs.microsoft.com/en-us/azure/devops/pipelines/process/runtime-parameters?view=azure-devops&tabs=script), utilize the `*Parameter` shorthands:

```csharp
Parameters =
[
    StringParameter("project", "AzureDevops project"),
    StringParameter("version", ".NET version", allowedValues: new[] { "5.0.100", "5.0.102" }),
    BooleanParameter("restore", "Restore NuGets", defaultValue: true),
    StepParameter("afterBuild", "After steps", Bash.Inline("cp -R logs $(Build.ArtifactStagingDirectory)")),
]
```

## Conditioned expressions

The Azure DevOps pipeline YAML allows you to specify conditioned expressions which are evaulated when pipeline is started.
Sharpliner allows to define conditioned blocks as well in almost any part of the definition.
If you find a place where If cannot be used, raise an issue and we can add it easily.

This feature was a little bit problematic to mimic in C# but we've found a nice way to express these:

```csharp
Variables =
[
    // You can create one if statement and chain multiple definitions beneath it
    If.Equal("$(Environment.Target)", "Cloud")
        .Variable("target", "Azure")
        .Variable("isCloud", true)

        // You can nest another if statement beneath
        .If.NotEqual(variables["Build.Reason"], "PullRequest")
            .Group("azure-int")
        .EndIf // You can jump out of the nested section too

        // You can use many macros such as IsBranch or IsPullRequest
        .If.IsBranch("main")
            .Group("azure-prod")

        // You can also swap the previous condition with an "else"
        // Azure Pipelines now support ${{ else }} but you can also revert to using an
        // inverted if condition using SharplinerSerializer.UseElseExpression setting
        .Else
            .Group("azure-pr"),
]
```

The resulting YAML will look like this:

```yaml
- ${{ if eq($(Environment.Target), Cloud) }}:
  - name: target
    value: Azure

  - name: isCloud
    value: true

  - ${{ if ne(variables['Build.Reason'], PullRequest) }}:
    - group: azure-int

    - ${{ if eq(variables['Build.SourceBranch'], refs/heads/main) }}:
      - group: azure-prod

    - ${{ else }}:
      - group: azure-pr
```

You can also specify conditions in places like template parameters (which are improved dictionaries really):

```csharp
StepTemplate("template1.yaml",
[
    { "some", "value" },
    {
        If.IsPullRequest,
        new TemplateParameters()
        {
            { "pr", true }
        }
    },
    { "other", 123 },
]),
```

This will produce following YAML:

```yaml
template: template1.yaml
parameters:
  some: value
  ${{ if eq(variables['Build.Reason'], 'PullRequest') }}:
    pr: true
  other: 123
```

### Conditions

The conditions themselves are defined similarly to what Azure DevOps requires, so this example YAML condition:
```yaml
${{ if or(and(ne(true, true), eq(variables['Build.SourceBranch'], 'refs/heads/production')), ne(variables['Configuration'], 'Debug')) }}
```

would have this C# definition:
```csharp
If.Or(
    And(NotEqual("true", "true"), Equal(variables["Build.SourceBranch"], "'refs/heads/production'")),
    NotEqual(variables["Configuration"], "'Debug'"))
```

The logic operators such as `Equal` or `Or` expect either a string or a nested condition.
Additionally, you can also use `variables["name"]` instead of `"variables[\"name\"]"` as shorthand notation but it has the same effect.

Finally, many of the commonly used conditions have macros prepared for a shorter syntax.
These are:
```csharp
// eq(variables['Build.SourceBranch'], 'refs/heads/production')
If.IsBranch("production")
If.IsNotBranch("production")

// eq(variables['Build.Reason'], 'PullRequest')
If.IsPullRequest
If.IsNotPullRequest

// You can mix these too
If.And(IsNotPullRequest, IsBranch("production"))

// You can specify any custom condition in case we missed an API :)
If.Condition("containsValue(...)")
```

## Each expression

The `${{ each var in collection }}` expression is also supported. Similarly to `If` and `EndIf`, you can use `Each` and `EndEach`:

```csharp
Stages =
{
    If.IsBranch("main")
        .Each("env", "parameters.stages")
            .Stage(new Stage("stage-${{ env.name }}"))
            .Stage(new Stage("stage2-${{ env.name }}")
            {
                Jobs =
                {
                    Each("foo", "bar")
                        .Job(new Job("job-${{ foo }}"))
                    .EndEach
                    .If.Equal("foo", "bar")
                        .Job(new Job("job2-${{ foo }}"))
                }
            })
}
```

generates the following YAML:

```yaml
stages:
- ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
  - ${{ each env in parameters.stages }}:
    - stage: stage-${{ env.name }}

    - stage: stage2-${{ env.name }}
      jobs:
      - ${{ each foo in bar }}:
        - job: job-${{ foo }}

      - ${{ if eq('foo', 'bar') }}:
        - job: job2-${{ foo }}
```

## Templates

Azure pipelines allow you to [define a parametrized template](https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#template-references) for a **stage**, **job**, **step** or **variables** and then insert those templates in your pipeline.
Sharpliner also supports definition and refercing of templates, however with C# it's easier to define these entities directly in code.
Anyway, the functionality is useful when for example migrating from large YAML code base step by step.
We also found it useful to create both YAML templates + C# representations for calling them and bind their parameters this way.

To define a template, you do it similarly as when you define the pipeline - you inherit from one of these classes:
- `StageTemplateDefinition`
- `JobTemplateDefinition`
- `StepTemplateDefinition`
- `VariableTemplateDefinition`

Example definition:

``` csharp
class InstallDotNetTemplate : StepTemplateDefinition
{
    // Where to publish the YAML to
    public override string TargetFile => "templates/build-csproj.yml";

    public override List<TemplateParameter> Parameters =>
    [
        StringParameter("project"),
        StringParameter("version", allowedValues: [ "5.0.100", "5.0.102" ]),
        BooleanParameter("restore", defaultValue: true),
        StepParameter("afterBuild", Bash.Inline("cp -R logs $(Build.ArtifactStagingDirectory)")),
    ];

    public override ConditionedList<Step> Definition =>
    [
        DotNet.Install.Sdk(parameters["version"]),

        If.Equal(parameters["restore"], "true")
            .Step(DotNet.Restore.Projects(parameters["project"])),

        DotNet.Build(parameters["project"]),

        StepParameterReference("afterBuild"),
    ];
}
```

This produces following YAML template:

```yaml
parameters:
- name: project
  type: string

- name: version
  type: string
  values:
  - 5.0.100
  - 5.0.102

- name: restore
  type: boolean
  default: true

- name: afterBuild
  type: step
  default:
    bash: |-
      cp -R logs $(Build.ArtifactStagingDirectory)

steps:
- task: UseDotNet@2
  inputs:
    packageType: sdk
    version: ${{ parameters.version }}

- ${{ if eq(${{ parameters.restore }}, true) }}:
  - task: DotNetCoreCLI@2
    inputs:
      command: restore
      projects: ${{ parameters.project }}

- task: DotNetCoreCLI@2
  inputs:
    command: build
    projects: ${{ parameters.project }}

- ${{ parameters.afterBuild }}
```

To use the template, reference it in the following way:

```csharp
Steps =
[
    StepTemplate("pipelines/install-dotnet.yml", new()
    {
        { "project", "src/MyProject.csproj" },
        { "version", "5.0.100" },
    })
]
```

## Definition libraries

Sharpliner lets you re-use code more easily than YAML templates do.
Apart from obvious C# code re-use, you can also define sets of C# building blocks and re-use them in your pipelines:

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

More about this feature can be found [here (DefinitionLibraries.md)](https://github.com/sharpliner/sharpliner/blob/main/docs/AzureDevOps/DefinitionLibraries.md).
