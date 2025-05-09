using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class UniversalPackagesTaskTests
{
    [Fact]
    public Task Serialize_Publish_Task_Test()
    {
        var task = new UniversalPackagesPublishTask()
        {
            PublishDirectory = "$(Build.ArtifactStagingDirectory)"
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Publish_Task_With_Defaults_Test()
    {
        var task = new UniversalPackagesPublishTask()
        {

        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Download_Task_Test()
    {
        var task = new UniversalPackagesDownloadTask()
        {
            DownloadDirectory = "$(Build.ArtifactStagingDirectory)"
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Download_Task_With_Defaults_Test()
    {
        var task = new UniversalPackagesDownloadTask()
        {
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
