using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Creates <c>DownloadBuildArtifacts@1</c> tasks for valid build source modes.
/// </summary>
public class DownloadBuildArtifactsTaskBuilder
{
    /// <summary>
    /// Downloads artifacts produced by the current build.
    /// </summary>
    /// <param name="downloadType">Whether to download a single artifact or specific files.</param>
    /// <param name="downloadPath">Path on the agent machine where the artifacts are downloaded.</param>
    public DownloadCurrentBuildArtifactsTask Current(
        AdoExpression<BuildArtifactDownloadType> downloadType,
        AdoExpression<string> downloadPath)
        => new(downloadType, downloadPath);

    /// <summary>
    /// Downloads artifacts from the latest build of a pipeline.
    /// </summary>
    /// <param name="project">The project from which to download the build artifacts.</param>
    /// <param name="definition">The build pipeline from which to download artifacts.</param>
    /// <param name="downloadType">Whether to download a single artifact or specific files.</param>
    /// <param name="downloadPath">Path on the agent machine where the artifacts are downloaded.</param>
    public DownloadSpecificBuildArtifactsTask Latest(
        AdoExpression<string> project,
        AdoExpression<string> definition,
        AdoExpression<BuildArtifactDownloadType> downloadType,
        AdoExpression<string> downloadPath)
        => new(project, definition, BuildArtifactVersion.Latest, downloadType, downloadPath);

    /// <summary>
    /// Downloads artifacts from the latest build of a pipeline on a branch.
    /// </summary>
    /// <param name="project">The project from which to download the build artifacts.</param>
    /// <param name="definition">The build pipeline from which to download artifacts.</param>
    /// <param name="branchName">Branch or ref name used to filter builds.</param>
    /// <param name="downloadType">Whether to download a single artifact or specific files.</param>
    /// <param name="downloadPath">Path on the agent machine where the artifacts are downloaded.</param>
    public DownloadSpecificBuildArtifactsTask LatestFromBranch(
        AdoExpression<string> project,
        AdoExpression<string> definition,
        AdoExpression<string> branchName,
        AdoExpression<BuildArtifactDownloadType> downloadType,
        AdoExpression<string> downloadPath)
        => new(project, definition, BuildArtifactVersion.LatestFromBranch, downloadType, downloadPath)
        {
            BranchName = branchName,
        };

    /// <summary>
    /// Downloads artifacts from a specific build of a pipeline.
    /// </summary>
    /// <param name="project">The project from which to download the build artifacts.</param>
    /// <param name="definition">The build pipeline from which to download artifacts.</param>
    /// <param name="buildId">The build from which to download artifacts.</param>
    /// <param name="downloadType">Whether to download a single artifact or specific files.</param>
    /// <param name="downloadPath">Path on the agent machine where the artifacts are downloaded.</param>
    public DownloadSpecificBuildArtifactsTask Specific(
        AdoExpression<string> project,
        AdoExpression<string> definition,
        AdoExpression<string> buildId,
        AdoExpression<BuildArtifactDownloadType> downloadType,
        AdoExpression<string> downloadPath)
        => new(project, definition, BuildArtifactVersion.Specific, downloadType, downloadPath)
        {
            BuildId = buildId,
        };

    internal DownloadBuildArtifactsTaskBuilder()
    {
    }
}
