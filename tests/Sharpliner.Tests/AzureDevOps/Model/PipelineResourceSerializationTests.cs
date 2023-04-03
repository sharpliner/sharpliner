using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class PipelineResourceSerializationTests
{
    private class ResourcePipeline : SingleStagePipelineDefinition
    {
        public override string TargetFile => "foo.yaml";

        public override SingleStagePipeline Pipeline => new()
        {
            Resources = new Resources()
            {
                Repositories =
                {
                    new RepositoryResource("sharpliner")
                    {
                        Type = RepositoryType.Git,
                        Endpoint = "https://github.com/sharpliner.sharpliner",
                    }
                }
            }
        };
    }

    [Fact]
    public void RepositoryResource_Serialization_Test()
    {
        string yaml = new ResourcePipeline().Serialize();
        yaml.Trim().Should().Be(
            """
            resources:
              repositories:
              - repository: sharpliner
                endpoint: https://github.com/sharpliner.sharpliner
            """);
    }
}
