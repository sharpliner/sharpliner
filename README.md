Sharpliner is a .NET library that lets you use C# for Azure DevOps pipeline definition.
Exchange YAML indentation problems for the type-safe environment of C# and let the intellisense speed up your work!

## Getting started

All you have to do is reference our NuGet package in your project, override a class and build the project! Dead simple!

For more detailed steps, check our [documentation](https://github.com/sharpliner/sharpliner/blob/main/docs/AzureDevOps/GettingStarted.md).

## Example

```csharp
class PullRequestPipeline : SingleStageAzureDevOpsPipelineDefinition
{
    public override string TargetFile => "azure-pipelines.yml";

    public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

    public override AzureDevOpsPipeline Pipeline => new()
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
                    // You can also specify the full UseDotNet@0 task
                    DotNet.Install(DotNetPackageType.Sdk, "$(DotnetVersion)").DisplayAs("Install .NET SDK"),

                    // Can also be shortened using macros:
                    //   Dotnet.Command("test", projects: "src/MyProject.sln").DisplayAs("Build and test")
                    Task("DotNetCoreCLI@2", "Build and test") with
                    {
                        Inputs = new()
                        {
                            { "command", "test" },
                            { "projects", "src/MyProject.sln" },
                        }
                    },

                    // If statements supported everywhere
                    If.IsBranch("main")
                        // Loads the contents from a file and inlines the script into the YAML
                        .Step(PowerShell.FromResourceFile("New-Report.ps1", "Create build report")),
                }
            }
        },
    };
}
```

## Sharpliner features

Apart from the obvious benefits of using static type language with IDE support, not having to have to deal with indentation problems ever again or the ability to generate YAML programatically, there are several other upsides to Sharpliner.

### Pipeline validation
Your pipeline definition can be validated during publishing and you can uncover issues, such as invalid properties or just syntax errors, you would only find by trying to run the pipeline in CI. This gives you a faster dev loop and greater productivity.

### Sourcing scripts from files
When you need to add cmd, PowerShell or bash steps into your pipeline, mainatining these bits inside YAML can be error prone. With Sharpliner you can keep scripts in their own files (`.ps1`, `.sh`..) where you get the natural environment you're used to such as syntax highlighting. Sharpliner gives you APIs to load these on build time and include them inline:

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

This project is still under development and we probably don't cover 100% of the cases. If you find a missing feature / API / property, file an issue in project's repository, or even better - file a PR and we will work with you to get you going.
