using FluentAssertions;
using Sharpliner.AzureDevOps.Tasks;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class ArchiveFilesTaskTests
{
    [Fact]
    public void Serialize_Task_Test()
    {
        var task = new ArchiveFilesTask("$(Build.SourcesDirectory)", ArchiveType.Tar, "$(Build.ArtifactStagingDirectory)/Archive.tar")
        {
            IncludeRootFolder = true,
            SevenZipCompression = SevenZipCompressionLevel.Ultra,
            TarCompression = TarCompressionType.Xz,
            ReplaceExistingArchive = true,
            Verbose = true,
            Quiet = false,
        };

        var yaml = SharplinerSerializer.Serialize(task);
        yaml.Trim().Should().Be("""
            task: ArchiveFiles@2

            inputs:
              rootFolderOrFile: $(Build.SourcesDirectory)
              archiveType: tar
              archiveFile: $(Build.ArtifactStagingDirectory)/Archive.tar
              includeRootFolder: true
              sevenZipCompression: ultra
              tarCompression: xz
              replaceExistingArchive: true
              verbose: true
              quiet: false
            """);
    }

    [Fact]
    public void Serialize_Task_With_Defaults_Test()
    {
        var task = new ArchiveFilesTask("$(Build.SourcesDirectory)", ArchiveType.Tar, "$(Build.ArtifactStagingDirectory)/Archive.tar");

        var yaml = SharplinerSerializer.Serialize(task);
        yaml.Trim().Should().Be("""
            task: ArchiveFiles@2

            inputs:
              rootFolderOrFile: $(Build.SourcesDirectory)
              archiveType: tar
              archiveFile: $(Build.ArtifactStagingDirectory)/Archive.tar
            """);
    }
}
