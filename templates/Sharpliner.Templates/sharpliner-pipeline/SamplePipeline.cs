using Sharpliner;
using Sharpliner.AzureDevOps;

namespace SharplinerPipelineProject;

/// <summary>
/// Sample pipeline definition showing basic Sharpliner usage.
/// Upon building your project, it will be published to 'pipelines/sample-pipeline.yml'.
/// This example demonstrates using a step template for reusable YAML components.
/// </summary>
class SamplePipeline : SingleStagePipelineDefinition
{
    public override string TargetFile => "pipelines/sample-pipeline.yml";
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
                    // Reference an example step template with strong-typed parameters
                    new BuildStepsTemplate(new()
                    {
                        SdkVersion = "8.0.x"
                    }),

                    DotNet.Test("**/*Tests.csproj") with
                    {
                        DisplayName = "Run tests"
                    },
                ]
            }
        ],
    };
}
