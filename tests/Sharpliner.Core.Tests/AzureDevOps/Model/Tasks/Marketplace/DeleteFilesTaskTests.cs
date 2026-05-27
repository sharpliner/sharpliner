using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class DeleteFilesTaskTests
{
    [Fact]
    public Task Serialize_Task_Test()
    {
        var task = new DeleteFilesTask("*")
        {
            SourceFolder = "$(Build.ArtifactStagingDirectory)",
            RemoveSourceFolder = true,
            RemoveDotFiles = true,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_Defaults_Test()
    {
        var task = new DeleteFilesTask("*");

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
