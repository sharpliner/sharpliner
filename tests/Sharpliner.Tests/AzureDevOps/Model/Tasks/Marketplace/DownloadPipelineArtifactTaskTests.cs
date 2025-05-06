using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class DownloadPipelineArtifactTaskTests
{
    [Fact]
    public Task Serialize_Task_Test()
    {
        var task = new DownloadPipelineArtifactTask()
        {
            ArtifactName = "docker_image",
            TargetPath = "$(System.ArtifactsDirectory)/docker_build_image"
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_Defaults_Test()
    {
        var task = new DownloadPipelineArtifactTask();

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
