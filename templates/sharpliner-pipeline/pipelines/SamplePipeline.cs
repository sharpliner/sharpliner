using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace SharplinerPipelineProject.Pipelines;

/// <summary>
/// Sample pipeline definition showing basic Sharpliner usage.
/// This pipeline will be published to 'sample-pipeline.yml' in your repository.
/// </summary>
class SamplePipeline : SingleStagePipelineDefinition
{
    public override string TargetFile => "sample-pipeline.yml";

    public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

    public override SingleStagePipeline Pipeline => new()
    {
        // Trigger the pipeline on changes to main branch
        Trigger = new Trigger("main"),

        // Also trigger on pull requests
        Pr = new PrTrigger("main"),

        Jobs =
        [
            new Job("Build", "Build and test")
            {
                Pool = new HostedPool("Azure Pipelines", "ubuntu-latest"),
                Steps =
                [
                    // Install .NET SDK
                    DotNet.Install.Sdk("8.0.x"),

                    // Restore dependencies
                    DotNet.Restore.Projects("**/*.csproj"),

                    // Build the project
                    DotNet.Build("**/*.csproj") with
                    {
                        DisplayName = "Build project"
                    },

                    // Run tests
                    DotNet.Test("**/*Tests.csproj") with
                    {
                        DisplayName = "Run tests"
                    },
                ]
            }
        ],
    };
}