using Sharpliner.AzureDevOps;

namespace Sharpliner.Tests.AzureDevOps;

public class WorkspaceTests
{
    private class WorkspacePipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("job1")
                {
                    Workspace = JobWorkspace.Resources,
                }
            }
        };
    }

    [Fact]
    public Task Serialize_Pipeline_Test()
    {
        WorkspacePipeline pipeline = new();
        
        return Verify(pipeline.Serialize());
    }
}
