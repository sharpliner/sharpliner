using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

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
    public void Serialize_Pipeline_Test()
    {
        WorkspacePipeline pipeline = new();
        string yaml = pipeline.Serialize();
        yaml.Trim().Should().Be(
        """
        jobs:
        - job: job1
          workspace:
            clean: resources
        """);
    }
}
