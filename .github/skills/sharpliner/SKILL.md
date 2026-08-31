---
name: sharpliner
description: Create, convert, modify, and troubleshoot Azure DevOps pipeline definitions written in C# with the Sharpliner .NET library. Use when working with Sharpliner projects, PipelineDefinition classes, generated Azure Pipelines YAML, or when converting azure-pipelines.yml to Sharpliner.
license: MIT
---

# Sharpliner

Use Sharpliner's public C# API to define Azure DevOps pipelines and generate YAML. Work from the
project's installed Sharpliner version and compile frequently; do not guess API signatures or start
by disassembling NuGet assemblies.

## Gather context

1. Find the pipeline project, its target framework, and its `Sharpliner` or `Sharpliner.Core`
   `PackageReference`. Do not change package versions unless asked.
2. Read existing definition classes and generated YAML to preserve local naming, paths, pools,
   templates, and style.
3. If converting YAML, inventory its triggers, resources, variables, stages, jobs, steps,
   conditions, templates, and parameters before writing C#.
4. Determine the correct definition type:
   - `SingleStagePipelineDefinition` for a pipeline containing jobs directly.
   - `PipelineDefinition` for a pipeline containing stages.
   - `ExtendsPipelineDefinition` for a pipeline that extends a template.
   - `StageTemplateDefinition`, `JobTemplateDefinition`, or `StepTemplateDefinition` for a
     reusable YAML template.
   - The corresponding collection type when generating multiple similar files.

`Sharpliner` includes the MSBuild integration that writes YAML during build. `Sharpliner.Core`
contains the same definition API without automatic publishing and is intended for custom
serialization through `SharplinerSerializer`.

## Discover APIs without reverse engineering

Use these sources in order:

1. Existing definitions in the user's project.
2. [Getting started](https://github.com/sharpliner/sharpliner/blob/main/docs/AzureDevOps/GettingStarted.md)
   and the [definition reference](https://github.com/sharpliner/sharpliner/blob/main/docs/AzureDevOps/DefinitionReference.md).
3. The public source for the installed version. Select its matching repository tag, then inspect
   `src/Sharpliner.Core/AzureDevOps/PublicDefinitions.cs`, model types, and task builders.
4. IntelliSense, the XML documentation next to `Sharpliner.Core.dll` in the installed package,
   and compiler diagnostics.

Do not infer an API from Azure Pipelines YAML alone. If the installed version differs from the
latest documentation, the version-matched public source and compiler are authoritative. Do not use
internal APIs.

## Start from a valid definition

```csharp
using Sharpliner;
using Sharpliner.AzureDevOps;

namespace MyProject.Pipelines;

class PullRequestPipeline : SingleStagePipelineDefinition
{
    public override string TargetFile => "azure-pipelines.yml";
    public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

    public override SingleStagePipeline Pipeline => new()
    {
        Pr = new PrTrigger("main"),
        Jobs =
        [
            new Job("Build")
            {
                Pool = new HostedPool("Azure Pipelines", "ubuntu-latest"),
                Steps =
                [
                    Checkout.Self,
                    DotNet.Build("MyProject.sln"),
                    DotNet.Test("MyProject.sln"),
                ]
            }
        ],
    };
}
```

Prefer collection expressions when the project language version supports them; otherwise follow
the syntax already used by the project.

## Translate Azure Pipelines concepts

- Keep the YAML hierarchy: `Pipeline` → `Stages` → `Jobs` → `Steps`, or use
  `SingleStagePipeline` → `Jobs` → `Steps`.
- Use strongly typed builders such as `Checkout`, `DotNet`, `NuGet`, `Bash`, `Powershell`,
  `Pwsh`, `Script`, `Publish`, and `Download` when the installed version provides the operation.
- Any Azure Pipelines task remains available through `Task("TaskName@Major", "display name")`
  with its `Inputs` dictionary. Use this fallback instead of inventing a strongly typed builder.
- Use Sharpliner expressions and references for compile-time conditions, runtime conditions,
  parameters, variables, and dependency outputs. Preserve Azure DevOps strings such as
  `$(VariableName)` when no typed reference is appropriate.
- Preserve template paths and repository aliases exactly. Prefer typed template definitions when
  the template is owned by the project; use `StageTemplate`, `JobTemplate`, or `StepTemplate` for
  external or existing YAML templates.
- Use C# `with` expressions to customize task records returned by builders.

Consult the
[marketplace task models](https://github.com/sharpliner/sharpliner/tree/main/src/Sharpliner.Core/AzureDevOps/Model/Tasks/Marketplace)
before assuming a built-in Azure Pipelines task has a strongly typed API.

## Validate

1. Build the pipeline project with `dotnet build <pipeline-project>`. The `Sharpliner` package
   generates all discovered definitions as part of the build.
2. Fix compiler or Sharpliner validation errors by checking the installed-version API; do not work
   around them with guessed casts or internal types.
3. Inspect the generated YAML and compare its semantics with the requested pipeline. Check
   triggers, dependencies, conditions, variable syntax, task versions and inputs, template paths,
   and output location.
4. Run the repository's existing focused tests or validation command when present.
5. Commit the C# definition and generated YAML when that repository tracks generated pipelines.
   Do not hand-edit generated YAML; change the definition and rebuild.
