Sharpliner is a .NET library that lets you use C# for Azure DevOps pipeline definition.
Exchange YAML indentation problems for the type-safe environment of C# and let the intellisense speed up your work!

## Getting started

All you have to do is reference our NuGet package in your project, override a class with your definition and build the project! Dead simple!

For more detailed steps, check our [documentation](https://github.com/sharpliner/sharpliner/blob/main/docs/AzureDevOps/GettingStarted.md).

## Example

```csharp
// Just override prepared abstract classes and build the project, nothing else is needed!
class PullRequestPipeline : SingleStageAzureDevOpsPipelineDefinition
{
    // Say where to publish the YAML to
    public override string TargetFile => "azure-pipelines.yml";
    public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

    public override AzureDevOpsPipeline Pipeline => new()
    {
        Pr = new PrTrigger("main"),

        Variables =
        {
            // YAML ${{ if }} conditions are available with handy macros
            // that expand into the lengthy expressions such as comparing branch names.
            // We even have "else" :)
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

                    // If statements supported almost everywhere
                    If.IsPullRequest
                        // You can load the script contents from a .ps1 file and inline them into the YAML
                        // This way you can keep writing scripts with syntax highlighting and IDE support
                        .Step(PowerShell.FromResourceFile("New-Report.ps1", "Create build report")),
                }
            }
        },
    };
}
```

## Sharpliner features

Apart from the obvious benefits of using static type language with IDE support, not having to have to deal with indentation problems ever again, being able to split the code easily or the ability to generate YAML programatically, there are several other benefits of using Sharpliner.

### Pipeline validation
Your pipeline definition can be validated during publishing and you can uncover issues, such as typos inside `dependsOn`, you would only find by trying to run the pipeline in CI.
This gives you a faster dev loop and greater productivity.

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

## Something missing?

This project is still under development and we probably don't cover 100% of the cases, properties and tasks. If you find a missing feature / API / property / use case, file an issue in project's repository - or even better - file a PR and we will work with you to get you going!

If you want to start contributing, either you already know about something missing or you can choose from some of the open issues.
Ideal change to start with, if you want to get the feel for the project, can be for example a missing part of the model (missing property or some definition).
We are very responsive and will help you review your first change so that you can continue with something advanced!

Another way to start is to use Sharpliner to define your own, already existing pipeline.
This way you can uncover missing features or you can introduce shortcuts for definitions of build tasks or similar that you use frequently.
Contributions like these are also very welcome!
In these cases, it is worth starting with describing your intent in an issue first.
