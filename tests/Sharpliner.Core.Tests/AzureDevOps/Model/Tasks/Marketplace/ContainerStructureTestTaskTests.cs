using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class ContainerStructureTestTaskTests
{
    [Fact]
    public Task Serialize_Task_Test()
    {
        var task = new ContainerStructureTestTask(
            "my-docker-service-connection",
            "my-org/my-image",
            "tests/container-structure.yaml")
        {
            Tag = "1.2.3",
            TestRunTitle = "Container structure test run",
            FailTaskOnFailedTests = true,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_Defaults_Test()
    {
        var task = new ContainerStructureTestTask(
            "my-docker-service-connection",
            "my-org/my-image",
            "tests/container-structure.yaml");

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public void Default_Input_Values_Match_Task_Spec()
    {
        var task = new ContainerStructureTestTask(
            "my-docker-service-connection",
            "my-org/my-image",
            "tests/container-structure.yaml");

        Assert.Equal("$(Build.BuildId)", Assert.Single(task.Tag!.FlattenDefinitions()));
        Assert.False(Assert.Single(task.FailTaskOnFailedTests!.FlattenDefinitions()));
    }
}
