using Sharpliner.AzureDevOps;

namespace Sharpliner.CI;

class PullRequestPipeline : SingleStagePipelineDefinition
{
    public override string TargetFile => Pipelines.Location + "pr.yml";

    public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

    public override SingleStagePipeline Pipeline => new()
    {
        Trigger = new Trigger("main")
        {
            Batch = true,
        },

        Pr = new PrTrigger("main"),

        Jobs =
        {
            new Job("Build", "Build and test")
            {
                Pool = new HostedPool("Azure Pipelines", "windows-latest"),
                Steps =
                {
                    StepTemplate(Pipelines.TemplateLocation + "install-dotnet-sdk.yml", new()
                    {
                        { "version", "6.0.100" }
                    }),

                    Powershell
                        .Inline("New-Item -Path 'artifacts' -Name 'packages' -ItemType 'directory'")
                        .DisplayAs("Create artifacts/packages"),

                    DotNet
                        .Build("src/**/*.csproj", includeNuGetOrg: true)
                        .DisplayAs("Build"),

                    ValidateYamlsArePublished("eng/Sharpliner.CI/Sharpliner.CI.csproj"),

                    DotNet
                        .Test("tests/**/*.csproj")
                        .DisplayAs("Test"),
                }
            }
        },
    };
}
