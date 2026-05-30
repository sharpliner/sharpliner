using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class CopyFilesTaskTests
{
    [Fact]
    public Task Serialize_Task_Test()
    {
        var task = new CopyFilesTask("**", "$(Build.ArtifactStagingDirectory)")
        {
            SourceFolder = new ParameterReference("sourceDir"),
            CleanTargetFolder = true,
            Overwrite = true,
            FlattenFolders = true,
            PreserveTimestamp = true,
            RetryCount = 3,
            DelayBetweenRetries = 100,
            IgnoreMakeDirErrors = true,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_Defaults_Test()
    {
        var task = new CopyFilesTask("**", "$(Build.ArtifactStagingDirectory)")
        {
            SourceFolder = "foo",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
