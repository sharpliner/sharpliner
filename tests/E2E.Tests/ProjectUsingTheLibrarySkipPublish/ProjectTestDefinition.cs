using Sharpliner.AzureDevOps;

namespace E2E.Tests.ProjectUsingTheLibrarySkipPublish;

// These NuGet.Tests are E2E testing following scenario:
// 1. Skip publishing of YAMLs

public class ProjectTestDefinition() : SingleStagePipelineDefinition
{
    public override string TargetFile => throw new NotImplementedException("This will not be called by sharpliner");
    public override SingleStagePipeline Pipeline => throw new NotImplementedException("This will not be called by sharpliner");
}
