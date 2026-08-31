using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class AdvancedSecurityPublishTaskTests
{
    [Fact]
    public Task Serialize_Task_Test()
    {
        var task = new AdvancedSecurityPublishTask()
        {
            SarifsInputDirectory = "$(Build.ArtifactStagingDirectory)/sarif-reports",
            EnableRecursiveScanning = true,
            Category = "iac-scan",
            WaitForProcessing = true,
            WaitForProcessingInterval = "10",
            WaitForProcessingTimeout = "300",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Wait_Builder_With_Task_Defaults_Test()
    {
        var task = new AdvancedSecurityTaskBuilder()
            .PublishResultsAndWait("$(Build.SourcesDirectory)");

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
