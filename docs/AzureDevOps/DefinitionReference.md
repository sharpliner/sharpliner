# Azure DevOps pipeline definition reference

Here you can find detailed reference on how to define various parts of the pipeline.

## Build steps

Build steps are basic building blocks of the build.
Azure DevOps pipelines [define several basic tasks](https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#steps) that can be used as the pipeline's build steps:
- `script`
- `bash`
- `pwsh`
- `powerShell`
- `publish`
- `download`
- `checkout`
- `task`

For each of these, there are two ways how you can define these.

Either you new them the regular way though this requires a longer syntax
```csharp
...
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
            Condition = "eq(variables['Build.Reason'], \"PullRequest\")",
        },
    }
...
```

or you can use the shorthand style. For each of the basic commands, a method is defined on the parent class with the same name as the original task
```csharp
...
    Steps =
    {
        // Tasks are represented as C# records so you can use the `with` keyword to override the properties
        Task("DotNetCoreCLI@2", "Build solution") with
        {
            Inputs = new()
            {
                { "command", "build" },
                { "includeNuGetOrg", true },
                { "projects", "src/MyProject.sln" },
            },
            Timeout = TimeSpan.FromMinutes(20)
        },

        // Some of the shorthand styles define more options and a cleaner way of defining them
        // E.g. Bash gives you several ways where to get the script from such as Bash.FromResourceFile or Bash.FromFile
        Bash.Inline("./.dotnet/dotnet test src/MySolution.sln") with
        {
            DisplayName = "Run unit tests",
            ContinueOnError = true,
            Condition = "eq(variables['Build.Reason'], \"PullRequest\")",
        },
    }
...
```
.


## Pipeline variables

Similarly to Build steps, there's a shorthand style of definition of variables too:
```csharp
    Variables =
    {
        new Variable("Configuration", "Release"), // We can create the objects and then resue them for definition too
        Variable("Configuration", "Release"),     // Or we have shorthand style like we do for build steps
        Group("PR keyvault variables"),           // Also shorthand style for variable groups
    }
```

## Conditioned expressions

The Azure DevOps pipeline YAML allows you to specify conditioned expressions which are evaulated when pipeline is started.
Sharpliner allows to defined conditioned blocks as well in almost any part of the definition.
This feature was a little bit problematic to mimic in C# but we've found a nice way to express these:

```csharp
Variables =
{
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
        .Else
            .Group("azure-int"),
},
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

    - ${{ if ne(variables['Build.SourceBranch'], refs/heads/main) }}:
      - group: azure-int
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
