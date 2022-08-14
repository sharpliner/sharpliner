[![Build Status](https://dev.azure.com/premun/Sharpliner/_apis/build/status/sharpliner-pr?branchName=main)](https://dev.azure.com/premun/Sharpliner/_build/latest?definitionId=4&branchName=main) [![Nuget](https://img.shields.io/nuget/v/Sharpliner)](https://www.nuget.org/packages/Sharpliner/)

Sharpliner is a .NET library that lets you use C# for Azure DevOps pipeline definition.
Exchange YAML indentation problems for the type-safe environment of C# and let the intellisense speed up your work!

## Table of contents
- [Getting started](#getting-started)
- [Example](#example)
- [Sharpliner features](#sharpliner-features)
  - [Intellisense](#intellisense)
  - [Useful macros](#useful-macros)
  - [Sourcing scripts from files](#sourcing-scripts-from-files)
  - [Pipeline validation](#pipeline-validation)
- [Something missing?](#something-missing)
- [Developing Sharpliner](#developing-sharpliner)

## Getting started

All you have to do is reference our [NuGet package](https://www.nuget.org/packages/Sharpliner/)  in your project, override a class with your definition and `dotnet build` the project! Dead simple!

For more detailed steps, check our [documentation](https://github.com/sharpliner/sharpliner/blob/main/docs/AzureDevOps/GettingStarted.md).

## Example

```csharp
// Just override prepared abstract classes and `dotnet build` the project, nothing else is needed!
// You can also generate collections of definitions dynamically
//    see https://github.com/sharpliner/sharpliner/blob/main/docs/AzureDevOps/DefinitionCollections.md
// For a full list of classes you can override
//    see https://github.com/sharpliner/sharpliner/blob/main/src/Sharpliner/AzureDevOps/PublicDefinitions.cs
class PullRequestPipeline : SingleStagePipelineDefinition
{
    // Say where to publish the YAML to
    public override string TargetFile => "azure-pipelines.yml";
    public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

    public override SingleStagePipeline Pipeline => new()
    {
        Pr = new PrTrigger("main"),

        Variables =
        {
            // YAML ${{ if }} conditions are available with handy macros that expand into the
            // expressions such as comparing branch names. We also have "else"
            If.IsBranch("net-6.0")
                .Variable("DotnetVersion", "6.0.100")
                .Group("net6-kv")
            .Else
                .Variable("DotnetVersion", "5.0.202"),
        },

        Jobs =
        {
            new Job("Build")
            {
                Pool = new HostedPool("Azure Pipelines", "windows-latest"),
                Steps =
                {
                    // Many tasks have helper methods for shorter notation
                    DotNet.Install.Sdk("$(DotnetVersion)").DisplayAs("Install .NET SDK"),

                    // You can also specify any pipeline task in full
                    Task("DotNetCoreCLI@2", "Build and test") with
                    {
                        Inputs = new()
                        {
                            { "command", "test" },
                            { "projects", "src/MyProject.sln" },
                        }
                    },

                    // If statements supported (almost) everywhere
                    If.IsPullRequest
                        // You can load script contents from a .ps1 file and inline them into YAML
                        // This way you can write scripts with syntax highlighting separately
                        .Step(Powershell.FromResourceFile("New-Report.ps1", "Create build report")),
                }
            }
        },
    };
}
```

## Sharpliner features

Apart from the obvious benefits of using static type language with IDE support, not having to have to deal with indentation problems ever again, being able to split the code easily or the ability to generate YAML programatically, there are several other benefits of using Sharpliner.

### Intellisense
One of the best things when using Sharpliner is that you won't have to go the YAML reference every time you're adding a new piece of your pipeline.
Having everything strongly typed will make your IDE give you hints all the way!

![Example intellisense for pipeline variables](https://raw.githubusercontent.com/sharpliner/sharpliner/main/docs/images/variables-intellisense.png)

### Nice APIs

Imagine you want to install the .NET SDK. For that, Azure Pipelines have the `DotNetCoreCLI@2` task.
However, this task's specification is quite long since the task does many things:

```yaml
# .NET Core
# Build, test, package, or publish a dotnet application, or run a custom dotnet command
# https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/build/dotnet-core-cli?view=azure-devops
- task: DotNetCoreCLI@2
  inputs:
    command: 'build' # Options: build, push, pack, publish, restore, run, test, custom
    publishWebProjects: true # Required when command == Publish
    projects: # Optional
    custom: # Required when command == Custom
    arguments: # Optional
    publishTestResults: true # Optional
    testRunTitle: # Optional
    zipAfterPublish: true # Optional
    modifyOutputPath: true # Optional
    feedsToUse: 'select' # Options: select, config
    vstsFeed: # Required when feedsToUse == Select
    feedRestore: # Required when command == restore. projectName/feedName for project-scoped feed. FeedName only for organization-scoped feed.
    includeNuGetOrg: true # Required when feedsToUse == Select
    nugetConfigPath: # Required when feedsToUse == Config
    externalFeedCredentials: # Optional
    noCache: false
    restoreDirectory:
    restoreArguments: # Optional
    verbosityRestore: 'Detailed' # Options: -, quiet, minimal, normal, detailed, diagnostic
    packagesToPush: '$(Build.ArtifactStagingDirectory)/*.nupkg' # Required when command == Push
    nuGetFeedType: 'internal' # Required when command == Push# Options: internal, external
    publishVstsFeed: # Required when command == Push && NuGetFeedType == Internal
    publishPackageMetadata: true # Optional
    publishFeedCredentials: # Required when command == Push && NuGetFeedType == External
    packagesToPack: '**/*.csproj' # Required when command == Pack
    packDirectory: '$(Build.ArtifactStagingDirectory)' # Optional
    nobuild: false # Optional
    includesymbols: false # Optional
    includesource: false # Optional
    versioningScheme: 'off' # Options: off, byPrereleaseNumber, byEnvVar, byBuildNumber
    versionEnvVar: # Required when versioningScheme == byEnvVar
    majorVersion: '1' # Required when versioningScheme == ByPrereleaseNumber
    minorVersion: '0' # Required when versioningScheme == ByPrereleaseNumber
    patchVersion: '0' # Required when versioningScheme == ByPrereleaseNumber
    buildProperties: # Optional
    verbosityPack: 'Detailed' # Options: -, quiet, minimal, normal, detailed, diagnostic
    workingDirectory:
```

Notice how some of the properties are only valid in a specific combination with other.
With Sharpliner, we remove some of this complexity using nice fluent APIs:

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
},
```

### Useful macros
Some very common pipeline patterns such as comparing the current branch name or detecting pull requests are very cumbersome to do in YAML (long conditions full of complicated `${{ if }}` syntax).
For many of these, we have handy macros so that you get more readable and shorter code.

For example this YAML

```yaml
${{ if eq(variables['Build.SourceBranch'], 'refs/heads/production') }}:
    name: rg-suffix
    value: -pr
${{ if ne(variables['Build.SourceBranch'], 'refs/heads/production') }}:
    name: rg-suffix
    value: -prod
```

can become this C#

```csharp
If.IsBranch("production")
    .Variable("rg-suffix", "-pr")
.Else
    .Variable("rg-suffix", "-prod")
```

### Re-usable pipeline blocks
Sharpliner lets you re-use code more easily than YAML templates do.
Apart from obvious C# code re-use, you can also define sets of C# building blocks and re-use them in your pipelines:

```csharp
class ProjectBuildSteps : StepLibrary
{
    public override List<Conditioned<Step>> Steps => new()
    {
        DotNet.Install.Sdk("6.0.100"),

        If.IsBranch("main")
            .Step(DotNet.Restore.Projects("src/MyProject.sln")),

        DotNet.Build("src/MyProject.sln"),
    };
}
```

You can then reference this library in between build steps and it will get expanded into the pipeline's YAML:

```csharp
...
    new Job("Build")
    {
        Steps =
        {
            Script.Inline("echo 'Hello World'"),

            StepLibrary<ProjectBuildSteps>(),

            Script.Inline("echo 'Goodbye World'"),
        }
    }
...
```

More about this feature can be found [here (DefinitionLibraries.md)](https://github.com/sharpliner/sharpliner/blob/main/docs/AzureDevOps/DefinitionLibraries.md).


### Sourcing scripts from files
When you need to add cmd, PowerShell or bash steps into your pipeline, mainatining these bits inside YAML can be error prone.
With Sharpliner you can keep scripts in their own files (`.ps1`, `.sh`..) where you get the natural environment you're used to such as syntax highlighting.
Sharpliner gives you APIs to load these on build time and include them inline:

```csharp
Steps =
{
    Bash.FromResourceFile("embedded-script.sh") with
    {
        DisplayName = "Run post-build clean-up",
        Timeout = TimeSpan.FromMinutes(5),
    }
}
```

### Pipeline validation
Your pipeline definition can be validated during publishing and you can uncover issues, such as typos inside `dependsOn`, you would only find by trying to run the pipeline in CI.
This gives you a faster dev loop and greater productivity.

We are continuosly adding new validations as we find new error-prone spots.
Each validation can be individually configured/silenced in case you don't wish to take advantage of these:

```csharp
class YourCustomConfiguration : SharplinerConfiguration
{
    public override void Configure()
    {
        // You can set severity for various validations
        Validations.DependsOn = ValidationSeverity.Off;
        Validations.Name = ValidationSeverity.Warning;

        // You can also further customize serialization
        Serialization.PrettifyYaml = false;
        Serialization.UseElseExpression = true;
        Serialization.IncludeHeaders = false;

        // You can add hooks that execute during the publish process
        Hooks.BeforePublish = (definition, path) => {};
        Hooks.AfterPublish = (definition, path) => {};
    }
}
```

## Something missing?

If you find a missing feature / API / property / use case, file an issue in project's repository.
We try to be very responsive and for small asks can deliver you a new version very fast.

If you want to start contributing, either you already know about something missing or you can choose from some of the open issues.
We will help you review your first change so that you can continue with something advanced!

Another way to start is to try out Sharpliner to define your own, already existing pipeline.
This way you can uncover missing features or you can introduce shortcuts for definitions of build tasks or similar that you use frequently.
Contributions like these are also very welcome!
In these cases, it is worth starting with describing your intent in an issue first.

## Developing Sharpliner

Contributions are very welcome and if you find yourself opening the codebase there are couple of things you should know.
The repository layout is quite simple:

```bash
.
├── artifacts            # All build outputs go here. Nuke it to clean
├── docs                 # Documentation
├── eng                  # CI/CD for the repo
│   ├── Sharpliner.CI    # C# definitions for pipelines of this repo
│   └── pipelines        # YAML pipelines of the repository
├── src
│   └── Sharpliner       # Main Sharpliner project
│       └── build        # Targets/props for the Sharpliner .nupkg
├── tests
│   ├── NuGet.Tests      # E2E tests using the Sharpliner  .nupkg
│   └── Sharpliner.Tests # Unit tests for the main Sharpliner project
└── Sharpliner.sln       # Main solution of the project
```

Developing is quite easy - open the `Sharpliner.sln` solution in VS. However, the solution won't build 100% the first time.
This is because of the `Sharpliner.CI` project.
This projects uses Sharpliner and defines pipelines for the Sharpliner repository - the YAML is published to `eng/pipelines`.
This way we test quite many Sharpliner features right in the PR build.
The `Sharpliner.CI` project expects that a package `Sharpliner.43.43.43.nupkg` is built locally which it then references it simulating the real usage of Sharpliner from `nuget.org`.

To build all of the solution 100%, **you have to build `Sharpliner.CI` from console** as building inside VS won't work on cold checkout.
This will package `Sharpliner.csproj` first and produce the `43.43.43` package:
```
> dotnet build eng/Sharpliner.CI/Sharpliner.CI.csproj
```

If you make changes to the main library and want to test it using `Sharpliner.CI`, clean and then build the CI project from console:
```
> dotnet clean eng/Sharpliner.CI/Sharpliner.CI.csproj
> dotnet build eng/Sharpliner.CI/Sharpliner.CI.csproj
```
