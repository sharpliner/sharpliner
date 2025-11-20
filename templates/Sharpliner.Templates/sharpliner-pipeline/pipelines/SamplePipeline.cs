using Sharpliner;
using Sharpliner.AzureDevOps;

namespace SharplinerPipelineProject.Pipelines;

/// <summary>
/// Sample pipeline definition showing basic Sharpliner usage.
/// Upon building your project, it will be published to 'sample-pipeline.yml' at the root of your repository.
/// This example demonstrates using a step library to create reusable step collections.
/// </summary>
class SamplePipeline : SingleStagePipelineDefinition
{
    public override string TargetFile => "sample-pipeline.yml";
    public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

    public override SingleStagePipeline Pipeline => new()
    {
        Trigger = new Trigger("main"),
        Pr = new PrTrigger("main"),
        Jobs =
        [
            new Job("Build", "Build and test")
            {
                Pool = new HostedPool("Azure Pipelines", "ubuntu-latest"),
                Steps =
                [
                    // Use a step library to group common build steps
                    StepLibrary(new BuildSteps()),

                    DotNet.Test("**/*Tests.csproj") with
                    {
                        DisplayName = "Run tests"
                    },
                ]
            }
        ],
    };
}
