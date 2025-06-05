using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Expressions;

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

internal abstract class SimpleStepTestPipeline : SingleStagePipelineDefinition
{
    public override string TargetFile => "azure-pipelines.yml";

    public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

    public sealed override SingleStagePipeline Pipeline => new()
    {
        Jobs =
        {
            new Job("testJob")
            {
                Steps = Steps
            }
        }
    };

    protected abstract AdoExpressionList<Step> Steps { get; }
}
