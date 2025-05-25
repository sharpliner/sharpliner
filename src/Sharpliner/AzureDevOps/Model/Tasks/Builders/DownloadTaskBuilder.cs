using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating a download task using the <c>download</c> keyword or the <c>DownloadPipelineArtifact</c> task.
/// </summary>
public class DownloadTaskBuilder
{
    /// <summary>
    /// <para>
    /// Creates a download task that downloads artifacts from the current build.
    /// </para>
    /// This uses the <c>download: current</c> keyword.
    /// <para>
    /// For example:
    /// </para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Download.Current with
    ///     {
    ///         Tags =
    ///         [
    ///             "release",
    ///             "nightly",
    ///         ],
    ///         Artifact = "Frontend",
    ///         Patterns =
    ///         [
    ///             "frontend/**/*",
    ///             "frontend.config",
    ///         ]
    ///     },
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - download: current
    ///   artifact: Frontend
    ///   patterns: |-
    ///     frontend/**/*
    ///     frontend.config
    ///   tags: release,nightly
    /// </code>
    /// </summary>
    public CurrentDownloadTask Current => new();

    /// <summary>
    /// <para>
    /// Creates a download task that skips downloading artifacts for the current job.
    /// </para>
    /// This uses the <c>download: none</c> keyword.
    /// <para>
    /// For example:
    /// </para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Download.None
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - download: none
    /// </code>
    /// </summary>
    public NoneDownloadTask None => new();

    /// <summary>
    /// <para>
    /// Creates a download task that an artifact from a given pipeline run.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Download.SpecificBuild("public", 56, 1745, "MyProject.CLI", patterns: [ "**/*.dll", "**/*.exe" ]) with
    ///     {
    ///         AllowPartiallySucceededBuilds = true,
    ///         RetryDownloadCount = 3,
    ///         Tags = ["non-release", "preview"],
    ///     }
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: DownloadPipelineArtifact@2
    ///   inputs:
    ///     runVersion: specific
    ///     project: public
    ///     pipeline: 56
    ///     runId: 1745
    ///     artifact: MyProject.CLI
    ///     patterns: |-
    ///       **/*.dll
    ///       **/*.exe
    ///     allowPartiallySucceededBuilds: true
    ///     retryDownloadCount: 3
    ///     tags: non-release,preview
    /// </code>
    /// </summary>
    /// <param name="project">The project GUID from which to download the pipeline artifacts.</param>
    /// <param name="definition">The definition ID of the build pipeline.</param>
    /// <param name="buildId">The build from which to download the artifacts. For example: 1764</param>
    /// <param name="artifact">The name of the artifact to download. If left empty, all artifacts associated to the pipeline run will be downloaded.</param>
    /// <param name="path">
    /// <para>
    /// Directory to download the artifact files. Can be relative to the pipeline workspace directory or absolute.
    /// </para>
    /// <para>
    /// If multi-download option is applied (by leaving an empty artifact name), a sub-directory will be created for each.
    /// </para>
    /// <para>
    /// Default value: <c>$(Pipeline.Workspace)</c>
    /// </para>
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/artifacts/pipeline-artifacts?view=azure-devops">Artifacts in Azure Pipelines</see>.
    /// </param>
    /// <param name="patterns">
    /// One or more file matching patterns that limit which files get downloaded.
    /// <para>
    /// Default value: <c>**</c>
    /// </para>
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/file-matching-patterns?view=azure-devops">file matching patterns</see>.
    /// </param>
    public SpecificDownloadTask SpecificBuild(
        Conditioned<string> project,
        Conditioned<int> definition,
        Conditioned<int> buildId,
        Conditioned<string>? artifact = null,
        Conditioned<string>? path = null,
        IEnumerable<string>? patterns = null)
        =>
        new(RunVersion.Specific, project, definition)
        {
            BuildId = buildId,
            Artifact = artifact,
            Path = path,
            Patterns = patterns?.ToList(),
        };

    /// <summary>
    /// <para>
    /// Creates a download task that an artifact from a given pipeline run.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Download.LatestFromBranch("internal", 23, "refs/heads/develop", path: variables.Build.ArtifactStagingDirectory) with
    ///     {
    ///         AllowFailedBuilds = true,
    ///         CheckDownloadedFiles = true,
    ///         PreferTriggeringPipeline = true,
    ///         Artifact = "Another.CLI",
    ///         PipelineId = 23,
    ///     }
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: DownloadPipelineArtifact@2
    ///   inputs:
    ///     runVersion: latestFromBranch
    ///     project: internal
    ///     pipeline: 23
    ///     runBranch: refs/heads/develop
    ///     path: $(Build.ArtifactStagingDirectory)
    ///     allowFailedBuilds: true
    ///     checkDownloadedFiles: true
    ///     preferTriggeringPipeline: true
    ///     artifact: Another.CLI
    /// </code>
    /// </summary>
    /// <param name="project">The project GUID from which to download the pipeline artifacts.</param>
    /// <param name="definition">The definition ID of the build pipeline.</param>
    /// <param name="branchName">Specify to filter on branch/ref name. Default value: refs/heads/master</param>
    /// <param name="artifact">The name of the artifact to download. If left empty, all artifacts associated to the pipeline run will be downloaded.</param>
    /// <param name="path">
    /// <para>
    /// Directory to download the artifact files. Can be relative to the pipeline workspace directory or absolute.
    /// </para>
    /// <para>
    /// If multi-download option is applied (by leaving an empty artifact name), a sub-directory will be created for each.
    /// </para>
    /// <para>
    /// Default value: <c>$(Pipeline.Workspace)</c>
    /// </para>
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/artifacts/pipeline-artifacts?view=azure-devops">Artifacts in Azure Pipelines</see>.
    /// </param>
    /// <param name="patterns">
    /// One or more file matching patterns that limit which files get downloaded.
    /// <para>
    /// Default value: <c>**</c>
    /// </para>
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/file-matching-patterns?view=azure-devops">file matching patterns</see>.
    /// </param>
    public SpecificDownloadTask LatestFromBranch(
        Conditioned<string> project,
        Conditioned<int> definition,
        Conditioned<string>? branchName = null,
        Conditioned<string>? artifact = null,
        Conditioned<string>? path = null,
        IEnumerable<string>? patterns = null)
        =>
        new(RunVersion.LatestFromBranch, project, definition)
        {
            BranchName = branchName,
            Artifact = artifact,
            Path = path,
            Patterns = patterns?.ToList(),
        };

    /// <summary>
    /// Creates a download task that an artifact from a pipeline resource.
    /// </summary>
    /// <param name="resourceName">Alredy defined pipeline resource</param>
    /// <param name="artifact">The name of the artifact to download. If left empty, all artifacts associated to the pipeline run will be downloaded.</param>
    /// <param name="patterns">
    /// One or more file matching patterns that limit which files get downloaded.
    /// Default value: **
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/file-matching-patterns?view=azure-devops">official Azure DevOps pipelines documentation</see>.
    /// </param>
    public DownloadFromPipelineResourceTask FromPipelineResource(
        string resourceName,
        string? artifact = null,
        IEnumerable<string>? patterns = null)
    {
        DownloadFromPipelineResourceTask task = new(resourceName);

        if (artifact != null)
        {
            task = task with { Artifact = artifact };
        }

        if (patterns != null)
        {
            task = task with { Patterns = [.. patterns] };
        }

        return task;
    }

    internal DownloadTaskBuilder()
    {
    }
}
