using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class NpmAuthenticateTaskTests
{
    [Fact]
    public Task Serialize_Task_With_Required_WorkingFile_Test()
    {
        var task = new NpmAuthenticateTask(".npmrc");

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_Custom_Endpoints_Test()
    {
        var task = new NpmAuthenticateTask("packages/mypackage/.npmrc")
        {
            CustomEndpoints = ["ExternalNpmRegistry", "AnotherExternalNpmRegistry"]
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_Azure_DevOps_Service_Connection_Test()
    {
        var task = new NpmAuthenticateTask(".npmrc")
        {
            AzureDevOpsServiceConnection = "MyAzureDevOpsServiceConnection",
            FeedUrl = "https://pkgs.dev.azure.com/my-org/my-project/_packaging/my-feed/npm/registry/"
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public void Custom_Endpoints_Are_Emitted_As_Official_Comma_Separated_Input()
    {
        var task = new NpmAuthenticateTask(".npmrc")
        {
            CustomEndpoints = [" ExternalNpmRegistry ", "", " AnotherExternalNpmRegistry "]
        };

        Assert.Equal("npmAuthenticate@0", task.Task);
        Assert.Equal(".npmrc", task.Inputs["workingFile"]);
        Assert.Equal("ExternalNpmRegistry,AnotherExternalNpmRegistry", task.Inputs["customEndpoint"]);
        Assert.Equal(["ExternalNpmRegistry", "AnotherExternalNpmRegistry"], task.CustomEndpoints);
    }
}
