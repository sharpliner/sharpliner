using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class NpmTaskTests
{
    [Fact]
    public Task Serialize_Install_Task_With_Npmrc_Registry_Test()
    {
        var task = new NpmInstallTask
        {
            WorkingDirectory = "src/web",
            Verbose = true,
            CustomRegistry = NpmCustomRegistry.UseNpmrc,
            CustomEndpoint = [" ExternalNpmRegistry ", "", " AnotherExternalNpmRegistry "]
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Ci_Task_With_Azure_Artifacts_Feed_Test()
    {
        var task = new NpmCiTask
        {
            WorkingDirectory = "src/web",
            Verbose = false,
            CustomRegistry = NpmCustomRegistry.UseFeed,
            CustomFeed = "MyProject/MyFeed"
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Custom_Task_Test()
    {
        var task = new NpmCustomTask("dist-tag ls mypackage")
        {
            WorkingDirectory = "src/web",
            CustomRegistry = NpmCustomRegistry.UseFeed,
            CustomFeed = "MyProject/MyFeed"
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Publish_Task_Test()
    {
        var task = new NpmPublishTask
        {
            WorkingDirectory = "src/web",
            Verbose = true,
            PublishRegistry = NpmPublishRegistry.UseFeed,
            PublishFeed = "MyProject/MyFeed",
            PublishPackageMetadata = false
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
