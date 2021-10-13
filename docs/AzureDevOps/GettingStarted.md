# Getting started with Sharpliner for AzureDevOps

This documentation shows how to define and publish YAML for [AzureDevOps pipelines](https://docs.microsoft.com/en-us/azure/devops/pipelines/?view=azure-devops).

For best experience, we recommend creating a dedicated project that will contain your pipeline definitions.
It is possible to add Sharpliner into any other kind of project and have pipelines there but it is better to separate concerns this way.
You can have as many pipeline definitions in one project as you want.

## 1. Create a new project

In this example, we create a `MyProject.Pipelines.csproj` and add the **Sharpliner** NuGet package reference (grab the newest from `nuget.org`).
Your file should look something like this:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Sharpliner" Version="x.y.z" />
  </ItemGroup>
</Project>
```

## 2. Create a pipeline definition

Inside of the `MyProject.Pipelines` pipeline project, create a new class.
Let's call it `PullRequestPipeline.cs` and make it inherit from `AzureDevopsPipelineDefinition`.
Once you do this, you will see that you need to implement some of the abstract members of the pipeline:

```csharp
using Sharpliner.AzureDevOps;

namespace MyProject.Pipelines
{
    class PullRequestPipeline : AzureDevopsPipelineDefinition
    {
        // Name of the YAML file this pipeline will be serialized into
        public override string TargetFile => "azure-pipelines.yml";

        // How to find this file
        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

        public override AzureDevOpsPipeline Pipeline => new()
        {
            // This is where your pipeline will go
        }
    }
}
```

You can also decide to override `SingleStageAzureDevOpsPipelineDefinition` in case you have a simpler pipeline with only a signle stage.

## 3. Define the pipeline

To define the pipeline, just start setting properties of the `Pipeline` member and Intellisense should do the rest.
There are several helper methods you will get from the parent class that will help you write cleaner definitions.
Other than that, everything is usually named exactly like you would expect it to in the [AzDO pipelines YAML schema](https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema).

Check out the [full reference with tips](./DefinitionReference.md) so that you can write clean definitions using some of the syntax we provide.

An example of a pipeline that builds and tests your PR can look like this:

```csharp
...

public override SingleStageAzureDevOpsPipelineDefinition Pipeline => new()
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

                DotNet.Install.Sdk("$(DotnetVersion)").DisplayAs("Install .NET SDK"),

                DotNet.Build("src/MyProject.sln", includeNuGetOrg: true).DisplayAs("Build"),

                DotNet.Command(DotNetCommand.Test, projects: "src/MyProject.sln").DisplayAs("Test"),
            }
        }
    },
}

...
```

## 4. Export the pipeline

Save the changes and build the project using `dotnet`.
Based on the settings provided in the overriden members, Sharpliner will determine where you want your YAML file created.

The output should look something like this:

```cmd
λ dotnet build src/MyProject.Pipelines.csproj

Microsoft (R) Build Engine version 16.11.0 for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  Restored MyProject.Pipelines.csproj (in 135 ms).
  MyProject.Pipelines -> bin/MyProject.Pipelines/net5.0/MyProject.Pipelines.dll
  Publishing all pipeline definitions inside bin/MyProject.Pipelines/net5.0/MyProject.Pipelines.dll
  Validating pipeline PullRequestPipeline..
  Publishing pipeline PullRequestPipeline to azure-pipelines.yml..
  Published new changes for PullRequestPipeline

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:04.23
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

- ${{ if ne(variables['Build.SourceBranch'], 'refs/heads/net-6.0') }}:
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

## 5. Make sure you commit your changes

It can happen that you change your C# definition and you forget to publish or commit the exported YAML file with it.
To safeguard against these cases, we have created an AzureDevOps task called `SharplinerValidateTask` which will compare what is in your definition and what is in the YAML and fail the build in case they don't match.

You can add this step to your pipeline:

```csharp
...
    ValidateYamlsArePublished("src/MyProject.Pipelines.csproj"),
...
```
