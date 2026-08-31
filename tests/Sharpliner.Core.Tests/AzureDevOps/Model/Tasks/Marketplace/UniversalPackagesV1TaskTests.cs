using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class UniversalPackagesV1TaskTests
{
    [Fact]
    public Task Serialize_Download_Task_Test()
    {
        var task = new UniversalPackagesV1DownloadTask("MyProject/my-feed", "my-package", "1.*")
        {
            Directory = "$(Pipeline.Workspace)/packages",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Publish_Task_With_Explicit_Version_Test()
    {
        var task = new UniversalPackagesV1PublishTask("MyProject/my-feed", "my-package", "1.2.3")
        {
            Directory = "$(Build.ArtifactStagingDirectory)",
            PackageDescription = "Release build package",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Publish_Task_With_Version_Increment_Test()
    {
        var task = new UniversalPackagesV1PublishTask("MyProject/my-feed", "my-package", UniversalPackagesV1VersionIncrement.Patch)
        {
            Directory = "$(Build.ArtifactStagingDirectory)",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
