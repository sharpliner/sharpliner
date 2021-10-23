using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class NoopJobTests
{
    private class NoopJobPipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new NoopJob
                {
                    Pool = new HostedPool(vmImage: "windows-2019")
                }
            }
        };
    }

    [Fact]
    public void Serialize_Pipeline_Test()
    {
        NoopJobPipeline pipeline = new();
        string yaml = pipeline.Serialize();
        yaml.Should().Be(
@"jobs:
- job: No_op
  pool:
    vmImage: windows-2019
");
    }
}
