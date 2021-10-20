using Sharpliner.AzureDevOps;

namespace Sharpliner.Tests.AzureDevOps;

internal abstract class TestPipeline : PipelineDefinition
{
    public override string TargetFile => "azure-pipelines.yml";

    public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;
}

internal abstract class SimpleTestPipeline : SingleStagePipelineDefinition
{
    public override string TargetFile => "azure-pipelines.yml";

    public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;
}
