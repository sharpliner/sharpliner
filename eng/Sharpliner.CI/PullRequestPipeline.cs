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
                    StepLibrary(new ProjectBuildSteps("src/**/*.csproj")),

                    ValidateYamlsArePublished("eng/Sharpliner.CI/Sharpliner.CI.csproj"),

                    DotNet
                        .Test("tests/**/*.csproj")
                        .DisplayAs("Test"),
                }
            }
        },
    };
}
