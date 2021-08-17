using Sharpliner.AzureDevOps;

namespace Sharpliner.Tests.AzureDevOps
{
    internal abstract class TestPipeline : AzureDevOpsPipelineDefinition
    {
        public override string TargetFile => "azure-pipelines.yml";

        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;
    }
}
