using System.Collections.Generic;
using System.Linq;

namespace Sharpliner.AzureDevOps.Tasks;

public class DownloadTaskBuilder
{
    /// <summary>
    /// Creates a download task that downloads artifacts from the current build.
    /// </summary>
    public CurrentDownloadTask Current => new();

    /// <summary>
    /// Creates a download task that skips downloading artifacts for the current job.
    /// </summary>
    public NoneDownloadTask None => new();

    /// <summary>
    /// Creates a download task that an artifact from a given pipeline run.
    /// </summary>
    /// <param name="project">The project GUID from which to download the pipeline artifacts.</param>
    /// <param name="definition">The definition ID of the build pipeline.</param>
    /// <param name="buildId">The build from which to download the artifacts. For example: 1764</param>
    /// <param name="artifact">The name of the artifact to download. If left empty, all artifacts associated to the pipeline run will be downloaded.</param>
    /// <param name="path">
    /// Directory to download the artifact files. Can be relative to the pipeline workspace directory or absolute.
    /// If multi-download option is applied (by leaving an empty artifact name), a sub-directory will be created for each.
    /// Default value: $(Pipeline.Workspace)
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/artifacts/pipeline-artifacts?view=azure-devops">official Azure DevOps pipelines documentation</see>.
    /// </param>
    /// <param name="patterns">
    /// One or more file matching patterns that limit which files get downloaded.
    /// Default value: **
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/file-matching-patterns?view=azure-devops">official Azure DevOps pipelines documentation</see>.
    /// </param>
    public SpecificDownloadTask SpecificBuild(
        string project,
        int definition,
        int buildId,
        string? artifact = null,
        string? path = null,
        IEnumerable<string>? patterns = null)
        =>
        new(RunVersion.Specific, project, definition)
        {
            BuildId = buildId,
            Artifact = artifact,
            Path = path,
            Patterns = patterns?.ToList()
        };

    /// <summary>
    /// Creates a download task that an artifact from a given pipeline run.
    /// </summary>
    /// <param name="project">The project GUID from which to download the pipeline artifacts.</param>
    /// <param name="definition">The definition ID of the build pipeline.</param>
    /// <param name="branchName">Specify to filter on branch/ref name. Default value: refs/heads/master</param>
    /// <param name="artifact">The name of the artifact to download. If left empty, all artifacts associated to the pipeline run will be downloaded.</param>
    /// <param name="path">
    /// Directory to download the artifact files. Can be relative to the pipeline workspace directory or absolute.
    /// If multi-download option is applied (by leaving an empty artifact name), a sub-directory will be created for each.
    /// Default value: $(Pipeline.Workspace)
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/artifacts/pipeline-artifacts?view=azure-devops">official Azure DevOps pipelines documentation</see>.
    /// </param>
    /// <param name="patterns">
    /// One or more file matching patterns that limit which files get downloaded.
    /// Default value: **
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/file-matching-patterns?view=azure-devops">official Azure DevOps pipelines documentation</see>.
    /// </param>
    public SpecificDownloadTask LatestFromBranch(
        string project,
        int definition,
        string? branchName = null,
        string? artifact = null,
        string? path = null,
        IEnumerable<string>? patterns = null)
        =>
        new(RunVersion.LatestFromBranch, project, definition)
        {
            BranchName = branchName,
            Artifact = artifact,
            Path = path,
            Patterns = patterns?.ToList()
        };

    internal DownloadTaskBuilder()
    {
    }
}
