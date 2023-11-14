# Getting started with Sharpliner for AzureDevOps

This documentation shows how to define and publish YAML for [Azure DevOps pipelines](https://docs.microsoft.com/en-us/azure/devops/pipelines/?view=azure-devops).
For more detailed reference of various parts of the pipeline, see [Azure DevOps definition reference](https://github.com/sharpliner/sharpliner/blob/main/docs/AzureDevOps/DefinitionReference.md).

## Table of Contents
- [1. Create a new project](#1-create-a-new-project)
- [2. Create a pipeline definition](#2-create-a-pipeline-definition)
- [3. Define the pipeline](#3-define-the-pipeline)
- [4. Export the pipeline](#4-export-the-pipeline)
- [5. Customize serialization or configure validations](#5-customize-serialization-or-configure-validations)
- [6. Make sure you commit your changes](#6-make-sure-you-commit-your-changes)

## 1. Create a new project

For best experience, we recommend creating a dedicated project that will contain your pipeline definitions.
It is possible to add Sharpliner into any other kind of project and have pipelines there but it is better to separate concerns this way.
You can have as many pipeline definitions in one project as you want.

In this example, we create a `MyProject.Pipelines.csproj` and add the **Sharpliner** NuGet package reference (grab the newest from `nuget.org`).
Your file should look something like this:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <!-- .NET 5.0/6.0/7.0 are supported at the moment -->
    <TargetFramework>net6.0</TargetFramework>
    <!-- Use C# 10+ for best experience with some of the definition syntax -->
    <LangVersion>10.0</LangVersion>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Sharpliner" Version="x.y.z" />
  </ItemGroup>
</Project>
```

## 2. Create a pipeline definition

Inside of the `MyProject.Pipelines` pipeline project, create a new class.
Let's call it `PullRequestPipeline.cs` and make it inherit from `Sharpliner.AzureDevops.PipelineDefinition`.
Once you do this, you will see that you need to implement some of the abstract members of the pipeline:

```csharp
using Sharpliner.AzureDevOps;

namespace MyProject.Pipelines;

class PullRequestPipeline : PipelineDefinition
{
    // Name and where to serialize the YAML of this pipeline into
    public override string TargetFile => "azure-pipelines.yml";
    public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

    public override Pipeline Pipeline => new()
    {
        // This is where your pipeline will go
    }
}
```

You can also decide to override `SingleStagePipelineDefinition` in case you have a simpler pipeline with only a single stage.

> **Note:** Sometimes you need to generate a large number of similar YAML files dynamically. In this case, please see [DefinitionCollections.md](https://github.com/sharpliner/sharpliner/blob/main/docs/AzureDevOps/DefinitionCollections.md).

For a full list of classes you can override, see [PublicDefinitions.cs](https://github.com/sharpliner/sharpliner/blob/main/src/Sharpliner/AzureDevOps/PublicDefinitions.cs).

## 3. Define the pipeline

To define the pipeline, just start setting properties of the `Pipeline` member and Intellisense should do the rest.
There are several helper methods you will get from the parent class that will help you write cleaner definitions.
Other than that, everything is usually named exactly like you would expect it to in the [AzDO pipelines YAML schema](https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema).

Check out the [full reference with tips](./DefinitionReference.md) so that you can write clean definitions using some of the syntax we provide.

An example of a pipeline that builds and tests your PR can look like this:

```csharp
...

public override SingleStagePipelineDefinition Pipeline => new()
{
    Pr = new PrTrigger("main"),

    Variables =
    {
        If.IsBranch("net-6.0")
            .Variable("DotnetVersion", "6.0.100")
            .Group("net6-kv")
        .Else
            .Variable("DotnetVersion", "5.0.202"),
    },

    Jobs =
    {
        new Job("Build", "Build and test")
        {
            Pool = new HostedPool("Azure Pipelines", "windows-latest"),
            Steps =
            {
                If.IsPullRequest
                    .Step(Powershell.Inline("Write-Host 'Hello-World'").DisplayAs("Hello world")),

                DotNet.Install
                    .Sdk("$(DotnetVersion)")
                    .DisplayAs("Install .NET SDK"),

                DotNet
                    .Build("src/MyProject.sln", includeNuGetOrg: true)
                    .DisplayAs("Build"),

                DotNet
                    .Test("src/MyProject.sln")
                    .DisplayAs("Test"),
            }
        }
    },
}

...
```

## 4. Export the pipeline

Save the changes and build the project.
Based on the settings provided in the overriden members, Sharpliner will determine where you want your YAML file created.
The output should look something like this:

```cmd
λ dotnet build src/MyProject.Pipelines.csproj

Microsoft (R) Build Engine version 16.11.0 for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  Restored MyProject.Pipelines.csproj (in 135 ms).
  MyProject.Pipelines -> bin/MyProject.Pipelines/net8.0/MyProject.Pipelines.dll
  Publishing all pipeline definitions inside bin/MyProject.Pipelines/net8.0/MyProject.Pipelines.dll
  PullRequestPipeline / azure-pipelines.yml:
    Validating pipeline..
    Publishing changes to azure-pipelines.yml..

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:01.23
```

Your `azure-pipelines.yml` should look something like this:

```yaml
pr:
  branches:
    include:
    - main

variables:
- ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/net-6.0') }}:
  - name: DotnetVersion
    value: 6.0.100

  - group: net6-kv

- ${{ else }}:
  - name: DotnetVersion
    value: 5.0.202

jobs:
- job: Build
  displayName: Build and test
  pool:
    vmImage: windows-latest
    name: Azure Pipelines
  steps:
  - ${{ if eq(variables['Build.Reason'], 'PullRequest') }}:
    - powershell: |-
        Write-Host 'Hello-World'
      displayName: Hello world

  - task: UseDotNet@2
    displayName: Install .NET SDK
    inputs:
      packageType: sdk
      version: $(DotnetVersion)

  - task: DotNetCoreCLI@2
    displayName: Build
    inputs:
      command: build
      projects: src/MyProject.sln
      includeNuGetOrg: true

  - task: DotNetCoreCLI@2
    displayName: Test
    inputs:
      command: test
      projects: src/MyProject.sln
```

## 5. Customize serialization or configure validations

The YAML publishing process can be configured in a few ways. It is possible to customize how YAML is serialized and you can also configure the level at which are validation errors shown.

The validations are additional checks that Sharpliner does around your model to catch some errors before you try to run your pipeline. An example of such a check can be validation that the `dependsOn` fields have valid counterparts or that `name` fields do not contain invalid characters.

To configure serialization/validations, add a class into your project that inherits from a pre-prepared `SharplinerConfiguration` class:

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

## 6. Make sure you commit your changes

It can happen that you change your C# definition and you forget to publish or commit the exported YAML file with it.
To safeguard against these cases, we have created an AzureDevOps task called `SharplinerValidateTask` which will compare what is in your definition and what is in the YAML and fail the build in case they don't match.

You can add this step to your pipeline:

```csharp
...
    ValidateYamlsArePublished("src/MyProject.Pipelines.csproj"),
...
```

Please note that this step builds the given project using .NET so the SDK has to be available on the build agent.
