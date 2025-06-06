# Azure DevOps pipeline definition reference

Here you can find a detailed reference on how to define various parts of the pipeline.

For a full list of classes you can override to create a YAML file, see [PublicDefinitions.cs](https://github.com/sharpliner/sharpliner/blob/main/src/Sharpliner/AzureDevOps/PublicDefinitions.cs).

## Table of Contents

- [Build steps](#build-steps)
- [Azure Pipelines tasks](#azure-pipelines-tasks)
  - [Dotnet](#dotnet)
  - [NuGet](#nuget)
  - [Contributions welcome](#contributions-welcome)
  - [Marketplace tasks](#marketplace-tasks)
- [Pipeline variables](#pipeline-variables)
  - [Dependency variables](#dependency-variables)
- [Pipeline parameters](#pipeline-parameters)
- [Conditional expressions](#conditional-expressions)
  - [Conditions](#conditions)
- [Templates](#templates)
- [Definition libraries](#definition-libraries)

## Build steps

Build steps are basic building blocks of the build.
Azure DevOps pipelines [define several basic tasks](https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/steps) that can be used as the pipeline's build steps:

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

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#classic-pipeline-steps)]

or you can use the shorthand style. For each of the basic commands, a method/property is defined on the parent class with the same name as the original task:

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#shorthand-pipeline-steps)]

Please notice the `with` keyword which is a [new feature in C#](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record#nondestructive-mutation) that allows modifying of records.

## Azure Pipelines tasks

Even though it is possible to use any of the non-default [Azure Pipelines tasks](https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/?view=azure-devops) by specifying its name + inputs:

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#azure-pipeline-task)]

some of the tasks are quite hard to comprehend such as the [.NET Core CLI task](https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/build/dotnet-core-cli?view=azure-devops) whose specification is quite long since the task can do many different things.
Notice how some of the properties are only valid in a specific combination with another.
With Sharpliner, we remove some of this complexity by restricting which properties are available at a time and by using nice fluent APIs for dotnet and similar complex tasks.

*Note how we use the `with` keyword to extend the `record` object with new properties.*

### Dotnet

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#dotnet-tasks)]

### NuGet

The [NuGet v2 task](https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/nuget-command-v2?view=azure-pipelines) also has multiple combinations based on the command.

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#nuget-tasks-code)]

Generated YAML:

[!code-yaml[](tests/Sharpliner.Tests/Verified/AzureDevOps.Docs/DefinitionReferenceTests.NuGet_Test.verified.txt)]

### Contributions welcome

Currently, we don't support many marketplace tasks in C# as the project is still growing.
If you find one useful, hit us up with a request, or better, with a pull request and we can add it to our library.

### Marketplace tasks

Marketplace tasks can be supported and can be added to the organization if requested.

Supported marketplace tasks:
| Azure DevOps Extension | Repository | NuGet | Author |
|------------------------|------------|---------|--------|
| [![JFrog Azure DevOps Extension](https://img.shields.io/badge/JFrog%20Azure%20DevOps%20Extension-0078D7?style=flat&logo=visualstudiocode&logoColor=white)](https://marketplace.visualstudio.com/items?itemName=JFrog.jfrog-azure-devops-extension) | [![Repo](https://img.shields.io/badge/sharpliner/jfrog--extensions-181717?style=flat&logo=github&logoColor=white)](https://github.com/sharpliner/jfrog-extensions) | [![NuGet version](https://img.shields.io/nuget/v/Sharpliner.Extensions.JFrog.svg?style=flat-square)](https://www.nuget.org/packages/Sharpliner.Extensions.JFrog/) | [![GitHub followers](https://img.shields.io/github/followers/flcdrg?label=flcdrg&style=social)](https://github.com/flcdrg) |


## Pipeline variables

Similarly to Build steps, there's a shorthand style of definition of variables too:

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#pipeline-variables)]

You can define variables and pass them to methods to make the code more readable:

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#pipeline-variables-readable)]

### Dependency Variables

If your variable is defined in another job, you can use the dependencies shorthand to refrence them.  Azure DevOps has [many different possible yaml outputs](https://learn.microsoft.com/en-us/azure/devops/pipelines/process/expressions?view=azure-devops#dependency-syntax-overview) for referencing dependency variables, which is simplified by our shorthand syntax: `dependencies.<job|stage>.<deploy?>[]`.  Simply specify whether your reference is meant to live inside a `job` entry or a `stage` entry, then if the reference was instantiated within a deploy job, you simply add `.deploy` to the chain.

For example, if you wanted to reference a variable created in a deployment job inside another stage's job entry, you'd do so like this.

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/DependencyVariableReferenceTests.cs#dependency-variables)]

This chart explains what shorthand to use to get the expected yaml output, as well as when you'd want to use that specific syntax.

| Variable Transfer Type                                   | YAML Output                                                                                                            | Sharpliner Syntax                                                           | Required Variables                                            | Set In         | Referenced By |
| -------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------- | ------------------------------------------------------------- | -------------- | ------------- |
| Stage to stage dependency (different stages)             | `stageDependencies.<stage-name>.outputs['<job-name>.<step-name>.<variable-name>']`                                     | `dependencies.stage["stage"]["job"]["step"]["variable"]`                    | StageName<br>JobName<br>StepName<br>VariableName              | job            | stage         |
| Job to job dependency (same stage)                       | `dependencies.<job-name>.outputs['<step-name>.<variable-name>']`                                                       | `dependencies.job["job"]["step"]["variable"]`                               | JobName<br>StepName<br>VariableName                           | job            | job           |
| Job to job dependency (different stages)                 | `stageDependencies.<stage-name>.<job-name>.outputs['<step-name>.<variable-name>']`                                     | `dependencies.job["stage"]["job"]["step"]["variable"]`                      | StageName<br>JobName<br>StepName<br>VariableName              | job            | job           |
| Stage to stage dependency (deployment job)               | `dependencies.<stage-name>.outputs['<deployment-job-name>.<deployment-job-name>.<step-name>.<variable-name>']`         | `dependencies.stage.deploy["stage"]["job"]["step"]["variable"]`             | StageName<br>JobName<br>StepName<br>VariableName              | deployment job | stage         |
| Stage to stage dependency (deployment job with resource) | `dependencies.<stage-name>.outputs['<deployment-job-name>.Deploy_<Deploy_resource-name>.<step-name>.<variable-name>']` | `dependencies.stage.deploy["stage"]["job"]["step"]["variable"]["resource"]` | StageName<br>JobName<br>StepName<br>VariableName ResourceName | deployment job | stage         |
| Job to job dependency (different stages, deployment)     | `stageDependencies.<stage-name>.<job-name>.outputs['<job-name>.<step-name>.<variable-name>]`                           | `dependencies.job.deploy["stage"]["job"]["step"]["variable"]`               | StageName<br>JobName<br>StepName<br>VariableName              | deployment job | job           |

## Pipeline parameters

To define [pipeline runtime parameters](https://docs.microsoft.com/en-us/azure/devops/pipelines/process/runtime-parameters?view=azure-devops&tabs=script), utilize the `*Parameter` shorthands:

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#pipeline-parameters)]

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#enum-definition)]

When referencing these parameters, you can just refer to the parameter and it will be replaced with a parameter reference expression:

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#pipeline-parameters-readable)]

See more examples in the [test class](../../tests/Sharpliner.Tests/AzureDevOps/TemplateTests.cs#49)

## Conditional expressions

The Azure DevOps pipeline YAML allows you to specify conditional expressions which are evaluated when the pipeline is started.
Sharpliner allows you to define conditional blocks as well in almost any part of the definition.
If you find a place where `If` cannot be used, raise an issue and we can add it easily.

This feature was a little bit problematic to mimic in C# but we've found a nice way to express these:

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#conditioned-expressions-code)]

The resulting YAML will look like this:

[!code-yaml[](tests/Sharpliner.Tests/Verified/AzureDevOps.Docs/DefinitionReferenceTests.Serialize_ConditionedExpressionsPipeline_Test.verified.txt)]

You can also specify conditions in places like template parameters (which are improved dictionaries really):

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#template-conditioned-expressions-code)]

This will produce following YAML:

[!code-yaml[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#template-conditioned-expressions-yaml)]

### Conditions

The conditions themselves are defined similarly to what Azure DevOps requires, so this example YAML condition:

[!code-yaml[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#conditions-yaml)]

would have this C# definition:

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#conditions-code)]

The logic operators such as `Equal` or `Or` expect either a string or a nested condition.
Additionally, you can also use `variables["name"]` instead of `"variables[\"name\"]"` as shorthand notation but it has the same effect.

Finally, many of the commonly used conditions have macros available for a shorter syntax.
These are:

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#conditions-macros)]

## Each expression

The `${{ each var in collection }}` expression is also supported. Similarly to `If` and `EndIf`, you can use `Each` and `EndEach`:

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#each-expression-code)]

generates the following YAML:

[!code-yaml[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#each-expression-yaml)]

## Templates

Azure pipelines allow you to [define a parameterized template](https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#template-references) for a **stage**, **job**, **step** or **variables** and then insert those templates in your pipeline.
Sharpliner also supports the definition and referencing of templates, however with C# it's easier to define these entities directly in code.
Anyway, the functionality is useful when for example migrating from large YAML code base step by step.
We also found it useful to create both YAML templates + C# representations for calling them and bind their parameters this way.

To define a template, you do it similarly as when you define the pipeline - you inherit from one of these classes:

- `StageTemplateDefinition`
- `JobTemplateDefinition`
- `StepTemplateDefinition`
- `VariableTemplateDefinition`

Additionally, Sharpliner allows you to define a type for the template parameters so that usage of your own template is compile-time type-safe.
In such case, you inherit from the generic versions of these classes:

- `StageTemplateDefinition<TParameters>`
- `JobTemplateDefinition<TParameters>`
- `StepTemplateDefinition<TParameters>`
- `VariableTemplateDefinition<TParameters>`

Example usage:

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#strongly-typed-parameters-code)]

Which produces following YAML template:

[!code-yaml[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#strongly-typed-parameters-yaml)]

You can also define a template without strong-typing the parameters:

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#untyped-parameters-template)]

To use the template, reference it in the following way:

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#use-typed-template)]

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#use-untyped-template)]

## Definition libraries

Sharpliner lets you re-use code more easily than YAML templates do.
Apart from obvious C# code re-use, you can also define sets of C# building blocks and re-use them in your pipelines:

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#definition-library)]

You can then reference this library in between build steps and it will get expanded into the pipeline's YAML:

[!code-csharp[](tests/Sharpliner.Tests/AzureDevOps/Docs/DefinitionReferenceTests.cs#definition-library-usage)]

More about this feature can be found [here (DefinitionLibraries.md)](https://github.com/sharpliner/sharpliner/blob/main/docs/AzureDevOps/DefinitionLibraries.md).
