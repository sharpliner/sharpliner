using FluentAssertions;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class ArchiveFilesTaskTests
{
    [Fact]
    public Task Serialize_Task_Test()
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

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_Defaults_Test()
    {
        var task = new ArchiveFilesTask("$(Build.SourcesDirectory)", ArchiveType.Tar, "$(Build.ArtifactStagingDirectory)/Archive.tar");

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
