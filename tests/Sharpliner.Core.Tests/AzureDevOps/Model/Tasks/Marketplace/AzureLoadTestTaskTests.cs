using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class AzureLoadTestTaskTests
{
    [Fact]
    public Task Serialize_Task_With_Required_Inputs_Test()
    {
        var task = new AzureLoadTestTask("my-azure-subscription", "loadtest.yaml", "my-resource-group", "my-load-test-resource");

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_All_Inputs_Test()
    {
        var task = new AzureLoadTestTask("my-azure-subscription", "config/loadtest.yaml", "my-resource-group", "my-load-test-resource")
        {
            DisplayName = "Run load test",
            LoadTestRunName = "run-$(Build.BuildId)",
            LoadTestRunDescription = "Load test run from $(Build.SourceBranchName)",
            EnvironmentVariables =
            [
                new("MYAPP_URL", "$(myAppUrl)"),
                new("duration_in_sec", "120"),
            ],
            Secrets =
            [
                new("API_KEY", "$(apiKeySecret)"),
            ],
            OverrideParameters = """{"engineInstances":2}""",
            OutputVariableName = "loadTestRunId",
            WaitForCompletion = false,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public void Environment_Variables_And_Secrets_Are_Serialized_As_Json_Test()
    {
        var task = new AzureLoadTestTask("my-azure-subscription", "loadtest.yaml", "my-resource-group", "my-load-test-resource")
        {
            EnvironmentVariables = [new("env1", "value1")],
            Secrets = [new("key1", "$(secret1)")],
        };

        Assert.Equal(
            """
            [
              {
                "name": "env1",
                "value": "value1"
              }
            ]
            """,
            task.Inputs["env"].ToString());

        Assert.Equal(
            """
            [
              {
                "name": "key1",
                "value": "$(secret1)"
              }
            ]
            """,
            task.Inputs["secrets"].ToString());

        Assert.Equal([new AzureLoadTestVariable("env1", "value1")], task.EnvironmentVariables);
        Assert.Equal([new AzureLoadTestVariable("key1", "$(secret1)")], task.Secrets);
    }

    [Fact]
    public void Environment_Variables_Can_Be_Removed_Test()
    {
        var task = new AzureLoadTestTask("my-azure-subscription", "loadtest.yaml", "my-resource-group", "my-load-test-resource")
        {
            EnvironmentVariables = [new("env1", "value1")],
        };

        task = task with { EnvironmentVariables = null };

        Assert.Null(task.EnvironmentVariables);
        Assert.False(task.Inputs.ContainsKey("env"));
    }
}
