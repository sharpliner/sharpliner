using Sharpliner.AzureDevOps;

namespace Sharpliner.Tests.AzureDevOps;

public class StageTests
{
    private class StagePipeline : PipelineDefinition
    {
        public override string TargetFile => "stage-pipeline";
        public override Pipeline Pipeline => new()
        {
            Stages =
            [
                Stage("Stage1").DisplayAs("DisplayName") with
                {
                    Pool = new HostedPool(vmImage: "windows-2022"),
                }
            ]
        };
    }

    [Fact]
    public Task ResourcePipeline_Serialization_Test()
    {
        var pipeline = new StagePipeline();

        return Verify(pipeline.Serialize());
    }
}
