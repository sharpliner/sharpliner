using FluentAssertions;
using Sharpliner.AzureDevOps.Model.Tasks;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps.Model.Tasks.Marketplace;

public class PublishTestResultsTaskTests
{
    [Fact]
    public void Serialize_Task_Test()
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

        var yaml = SharplinerSerializer.Serialize(task);
        yaml.Trim().Should().Be("""
            task: PublishTestResults@2

            inputs:
              testResultsFormat: JUnit
              testResultsFiles: '**/TEST-*.xml'
              searchFolder: $(System.DefaultWorkingDirectory)
              mergeTestResults: true
              failTaskOnFailedTests: true
              failTaskOnFailureToPublishResults: true
              failTaskOnMissingResultsFile: true
              testRunTitle: Test run title
              buildPlatform: x64
              buildConfiguration: Release
              publishRunAttachments: true
            """);
    }

    [Fact]
    public void Serialize_Task_With_Defaults_Test()
    {
        var task = new PublishTestResultsTask(TestResultsFormat.JUnit, "**/TEST-*.xml");

        var yaml = SharplinerSerializer.Serialize(task);
        yaml.Trim().Should().Be("""
            task: PublishTestResults@2

            inputs:
              testResultsFormat: JUnit
              testResultsFiles: '**/TEST-*.xml'
            """);
    }
}
