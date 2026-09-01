using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class DownloadBuildArtifactsTaskTests
{
    [Fact]
    public Task Serialize_Current_Build_Task_Test()
    {
        var task = new DownloadCurrentBuildArtifactsTask(
            BuildArtifactDownloadType.Single,
            new VariableReference("artifactDirectory"))
        {
            ArtifactName = "drop",
            CleanDestinationFolder = true,
            ParallelizationLimit = 16,
            CheckDownloadedFiles = true,
            RetryDownloadCount = 6,
            ExtractTars = true,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Specific_Build_Task_Test()
    {
        var task = new DownloadSpecificBuildArtifactsTask(
            "project",
            "42",
            BuildArtifactVersion.LatestFromBranch,
            BuildArtifactDownloadType.SpecificFiles,
            "$(Pipeline.Workspace)")
        {
            SpecificBuildWithTriggering = true,
            AllowPartiallySucceededBuilds = true,
            BranchName = "refs/heads/main",
            Tags = ["release", "signed"],
            ItemPatterns = ["drop/**/*.zip", "!drop/**/*.symbols.zip"],
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    private class BuilderPipeline : SingleStagePipelineDefinition
    {
        public override string TargetFile => "azure-pipelines.yml";

        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("download")
                {
                    Steps =
                    {
                        Download.BuildArtifacts.Current(BuildArtifactDownloadType.Single, "$(System.ArtifactsDirectory)") with
                        {
                            ArtifactName = "current",
                        },
                        Download.BuildArtifacts.Latest("project", "12", BuildArtifactDownloadType.Single, "latest") with
                        {
                            ArtifactName = "drop",
                        },
                        Download.BuildArtifacts.LatestFromBranch(
                            "project",
                            "12",
                            "refs/heads/main",
                            BuildArtifactDownloadType.SpecificFiles,
                            "branch"),
                        Download.BuildArtifacts.Specific(
                            "project",
                            "12",
                            "1234",
                            BuildArtifactDownloadType.Single,
                            "specific") with
                        {
                            ArtifactName = "drop",
                        },
                    }
                }
            }
        };
    }

    [Fact]
    public Task Serialize_Builder_Source_Modes_Test()
    {
        BuilderPipeline pipeline = new();

        return Verify(pipeline.Serialize());
    }
}
