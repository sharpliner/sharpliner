using FluentAssertions;
using Sharpliner.AzureDevOps.Model.Tasks;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class CopyFilesTaskTests
{
    [Fact]
    public void Serialize_Task_Test()
    {
        var task = new CopyFilesTask("**", "$(Build.ArtifactStagingDirectory)")
        {
            SourceFolder = "$(Build.SourcesDirectory)",
            CleanTargetFolder = true,
            Overwrite = true,
            FlattenFolders = true,
            PreserveTimestamp = true,
            RetryCount = 3,
            DelayBetweenRetries = 100,
            IgnoreMakeDirErrors = true,
        };

        var yaml = SharplinerSerializer.Serialize(task);
        yaml.Trim().Should().Be("""
            task: CopyFiles@2

            inputs:
              Contents: '**'
              TargetFolder: $(Build.ArtifactStagingDirectory)
              SourceFolder: $(Build.SourcesDirectory)
              CleanTargetFolder: true
              Overwrite: true
              flattenFolders: true
              preserveTimestamp: true
              retryCount: 3
              delayBetweenRetries: 100
              ignoreMakeDirErrors: true
            """);
    }

    [Fact]
    public void Serialize_Task_With_Defaults_Test()
    {
        var task = new CopyFilesTask("**", "$(Build.ArtifactStagingDirectory)");

        var yaml = SharplinerSerializer.Serialize(task);
        yaml.Trim().Should().Be("""
            task: CopyFiles@2

            inputs:
              Contents: '**'
              TargetFolder: $(Build.ArtifactStagingDirectory)
            """);
    }
}
