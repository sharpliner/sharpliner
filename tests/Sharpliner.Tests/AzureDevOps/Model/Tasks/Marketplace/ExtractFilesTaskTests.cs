using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class ExtractFilesTaskTests
{
    [Fact]
    public Task Serialize_Task_Test()
    {
        var task = new ExtractFilesTask("$(Build.ArtifactStagingDirectory)/ExtractedFiles")
        {
            ArchiveFilePatterns = "$(Build.ArtifactStagingDirectory)/Archive.zip",
            CleanDestinationFolder = false,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_Defaults_Test()
    {
        var task = new ExtractFilesTask("$(Build.ArtifactStagingDirectory)/ExtractedFiles")
        {
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
