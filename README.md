This repository contains tooling for Azure DevOps pipelines YAML definition using C#.
Use Sharpliner to define pipelines in a type-safe comfortable environment of the C# languages to avoid syntax errors and bugs.

## Demo time

> TODO

## Something missing?

This project is still under development and we probably don't cover 100% of the cases. If you find a missing feature / API / property, file an issue in this repository, or even better, file a PR and we will have a look!

## Roadmap

### Implement model

Goal is to make all of the pipeline pieces available in C#: https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema

### Fluent definition

There should be a fluent factory model that will enable readable definition on-par or better than YAML:

Something in the lines of:
```
Pipelines.Create("pipeline-name")
    .TriggeredBy(...)
    .WithVariableGroup("some-variables")
    .WithVariable("IsPublic", true)
    .AddStage("stage-name", "Stage 1")
        .DependsOn("stage-foo")
        .AddStep(...)
            .OnlyWhenSucceeded()
        .AddStep(...)
    .SaveTo("azure-pipelines.yml")
```

Additionally, there should be implementations for known steps such as `AddNuGetRestoreStep` etc.

### Guardian build task

Every time we change the C# definition of the pipeline, it needs to be exported into the `.yaml` file in the repo
which the pipeline is defined by. I currently don't see a way to have the C# definition only.

There should be a build step defined that will run in PRs and verify that the C# definition was exported into the YAML file in the repo.
It will export the definition and compare with the YAML file and fail if it doesn't match.
This way you will have a validation that you did the export before opening a PR.

### Onboarding

**Nice to have: **There should be an onboarding process that will help you get started. There should be some dotnet tool that will take your YAML and create the C# counter-part.

## Contribute

> TODO
