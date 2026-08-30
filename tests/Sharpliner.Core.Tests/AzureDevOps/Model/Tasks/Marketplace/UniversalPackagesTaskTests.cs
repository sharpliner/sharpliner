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

    [Fact]
    public Task Serialize_Download_Task_Internal_Feed_Test()
    {
        var task = new UniversalPackagesDownloadTask()
        {
            DownloadDirectory = "$(System.DefaultWorkingDirectory)/packages",
            FeedsToUse = "internal",
            VstsFeed = "my-feed",
            VstsFeedPackage = "my-package",
            VstsPackageVersion = "1.*"
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Download_Task_External_Feed_Test()
    {
        var task = new UniversalPackagesDownloadTask()
        {
            DownloadDirectory = "$(System.DefaultWorkingDirectory)/packages",
            FeedsToUse = "external",
            ExternalFeedCredentials = "my-service-connection",
            FeedDownloadExternal = "OtherProject/other-feed",
            PackageDownloadExternal = "other-package",
            VersionDownloadExternal = "2.*"
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Publish_Task_Internal_Feed_Test()
    {
        var task = new UniversalPackagesPublishTask()
        {
            PublishDirectory = "$(Build.ArtifactStagingDirectory)",
            FeedsToUsePublish = "internal",
            VstsFeedPublish = "my-feed",
            PublishPackageMetadata = false,
            VstsFeedPackagePublish = "my-package",
            VersionOption = "patch",
            PackagePublishDescription = "Description of my package",
            PublishedPackageVar = "PublishedPackageVariable"
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Publish_Task_External_Feed_Test()
    {
        var task = new UniversalPackagesPublishTask()
        {
            PublishDirectory = "$(Build.ArtifactStagingDirectory)",
            FeedsToUsePublish = "external",
            PublishFeedCredentials = "my-service-connection",
            FeedPublishExternal = "OtherProject/other-feed",
            PackagePublishExternal = "other-package",
            VersionOption = "custom",
            VersionPublish = "1.2.3"
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
