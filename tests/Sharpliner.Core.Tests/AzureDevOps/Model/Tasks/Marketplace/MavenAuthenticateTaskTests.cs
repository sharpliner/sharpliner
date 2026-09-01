using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class MavenAuthenticateTaskTests
{
    [Fact]
    public Task Serialize_Task_With_Feeds_And_Service_Connections_Test()
    {
        var task = new MavenAuthenticateTask
        {
            ArtifactsFeeds = ["MyFeedInOrg1", "MyFeedInOrg2"],
            MavenServiceConnections = ["central", "MavenOrg"]
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_Azure_DevOps_Service_Connection_Test()
    {
        var task = new MavenAuthenticateTask
        {
            AzureDevOpsServiceConnection = "MyAzureDevOpsServiceConnection",
            ArtifactsFeeds = ["MyFeedInOrg1", "CrossOrgFeed"]
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public void List_Inputs_Are_Emitted_As_Official_Comma_Separated_Inputs()
    {
        var task = new MavenAuthenticateTask
        {
            ArtifactsFeeds = [" MyFeedInOrg1 ", "", " MyFeedInOrg2 "],
            MavenServiceConnections = [" central ", "", " MavenOrg "]
        };

        Assert.Equal("MavenAuthenticate@0", task.Task);
        Assert.Equal("MyFeedInOrg1,MyFeedInOrg2", task.Inputs["artifactsFeeds"]);
        Assert.Equal("central,MavenOrg", task.Inputs["mavenServiceConnections"]);
        Assert.Equal(["MyFeedInOrg1", "MyFeedInOrg2"], task.ArtifactsFeeds);
        Assert.Equal(["central", "MavenOrg"], task.MavenServiceConnections);
    }

    [Fact]
    public void Optional_Azure_DevOps_Service_Connection_Is_Not_Emitted_When_Blank()
    {
        var task = new MavenAuthenticateTask
        {
            AzureDevOpsServiceConnection = " "
        };

        Assert.DoesNotContain("azureDevOpsServiceConnection", task.Inputs.Keys);
    }

    [Fact]
    public void Azure_DevOps_Service_Connection_Builder_Requires_Service_Connection()
    {
        var builder = new MavenTaskBuilder();

        Assert.Throws<System.ArgumentException>(() => builder.Authenticate(""));
        Assert.Throws<System.ArgumentException>(() => builder.Authenticate(" "));
    }
}
