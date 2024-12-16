using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class PublishTestResultsTaskTests
{
    [Fact]
    public Task Serialize_Task_Test()
    {
        var task = new PublishTestResultsTask(TestResultsFormat.JUnit, "**/TEST-*.xml")
        {
            SearchFolder = "$(System.DefaultWorkingDirectory)",
            MergeTestResults = true,
            FailTaskOnFailedTests = true,
            FailTaskOnFailureToPublishResults = true,
            FailTaskOnMissingResultsFile = true,
            TestRunTitle = "Test run title",
            BuildPlatform = "x64",
            BuildConfiguration = "Release",
            PublishRunAttachments = true,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_Defaults_Test()
    {
        var task = new PublishTestResultsTask(TestResultsFormat.JUnit, "**/TEST-*.xml");

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
