using Sharpliner;
using Sharpliner.AzureDevOps;

namespace MyProject.Pipelines;

class PullRequestPipeline : PipelineDefinition
{
    // Name and where to serialize the YAML of this pipeline into
    public override string TargetFile => "azure-pipelines.yml";
    public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

    public override Pipeline Pipeline => new()
    {
        // This is where your pipeline will go
    };
}
