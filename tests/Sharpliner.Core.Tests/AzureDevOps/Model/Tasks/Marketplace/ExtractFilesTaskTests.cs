using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class ExtractFilesTaskTests
{
    [Fact]
    public Task Serialize_Task_Test()
    {
        var task = new ExtractFilesTask("$(Build.ArtifactStagingDirectory)/Archive.zip", "$(Build.ArtifactStagingDirectory)/ExtractedFiles")
        {
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

    [Fact]
    public void Default_Input_Values_Match_Task_Spec()
    {
        var task = new ExtractFilesTask("$(Build.ArtifactStagingDirectory)/ExtractedFiles");

        Assert.Equal("**/*.zip", Assert.Single(task.ArchiveFilePatterns!.FlattenDefinitions()));
        Assert.True(Assert.Single(task.CleanDestinationFolder!.FlattenDefinitions()));
        Assert.False(Assert.Single(task.OverwriteExistingFiles!.FlattenDefinitions()));
    }

    [Fact]
    public Task Serialize_Archive_Patterns_And_Condition_Test()
    {
        var task = new ExtractFilesTask(
            """
            **/*.zip
            **/*.7z
            **/*.rar
            **/*.tar.gz
            """,
            "$(Pipeline.Workspace)/extracted")
        {
            OverwriteExistingFiles = true,
            PathToSevenZipTool = "/usr/local/bin/7z",
        }.When("and(succeeded(), ne(variables['SkipExtract'], 'true'))");

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
