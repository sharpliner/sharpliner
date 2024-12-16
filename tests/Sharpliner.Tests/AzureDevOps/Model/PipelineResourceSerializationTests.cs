using FluentAssertions;
using Sharpliner.AzureDevOps;

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
    public Task RepositoryResource_Serialization_Test()
    {
        var pipeline = new ResourcePipeline();

        return Verify(pipeline.Serialize());
    }
}
