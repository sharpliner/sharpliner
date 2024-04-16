using FluentAssertions;
using Sharpliner.AzureDevOps.Tasks;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class DeleteFilesTaskTests
{
    [Fact]
    public void Serialize_Task_Test()
    {
        var task = new DeleteFilesTask("*")
        {
            SourceFolder = "$(Build.ArtifactStagingDirectory)",
            RemoveSourceFolder = true,
            RemoveDotFiles = true,
        };

        var yaml = SharplinerSerializer.Serialize(task);
        yaml.Trim().Should().Be("""
            task: DeleteFiles@1

            inputs:
              Contents: '*'
              SourceFolder: $(Build.ArtifactStagingDirectory)
              RemoveSourceFolder: true
              RemoveDotFiles: true
            """);
    }

    [Fact]
    public void Serialize_Task_With_Defaults_Test()
    {
        var task = new DeleteFilesTask("*");

        var yaml = SharplinerSerializer.Serialize(task);
        yaml.Trim().Should().Be("""
            task: DeleteFiles@1

            inputs:
              Contents: '*'
            """);
    }
}
