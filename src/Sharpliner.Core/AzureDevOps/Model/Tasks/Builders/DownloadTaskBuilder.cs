using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating a download task using the <c>download</c> keyword or the <c>DownloadPipelineArtifact</c> task.
/// </summary>
public class DownloadTaskBuilder
{
    /// <summary>
    /// Creates a <c>DownloadSecureFile@1</c> task that downloads a secure file to the agent machine.
    /// <para>
    /// The secure file is downloaded at the beginning of the stage regardless of the task's position,
    /// and is deleted when the job finishes.
    /// </para>
    /// <para>For example:</para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Download.SecureFile("ca.pem", retryCount: 5) with
    ///     {
    ///         Name = "caFile",
    ///     },
    ///     Bash.Inline($"cat {DownloadSecureFileTask.OutputSecureFilePath("caFile")}")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: DownloadSecureFile@1
    ///   name: caFile
    ///   inputs:
    ///     secureFile: ca.pem
    ///     retryCount: 5
    /// </code>
    /// </summary>
    /// <param name="secureFile">The file name or GUID of the secure file to download.</param>
    /// <param name="retryCount">Optional number of retries when download fails. Default value: <c>8</c>.</param>
    /// <param name="socketTimeout">Optional timeout in milliseconds for the download socket.</param>
    public DownloadSecureFileTask SecureFile(
        AdoExpression<string> secureFile,
        AdoExpression<int>? retryCount = null,
        AdoExpression<int>? socketTimeout = null)
        => new(secureFile, retryCount, socketTimeout);

    /// <summary>
    /// <para>
    /// Creates a <c>UniversalPackages@1</c> task that downloads a Universal Package.
    /// </para>
    /// <para>
    /// For example:
    /// </para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Download.UniversalPackage("MyProject/my-feed", "tooling-assets", "1.2.*") with
    ///     {
    ///         Directory = "$(Pipeline.Workspace)/packages"
    ///     }
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: UniversalPackages@1
    ///   inputs:
    ///     command: download
    ///     feed: MyProject/my-feed
    ///     packageName: tooling-assets
    ///     packageVersion: 1.2.*
    ///     directory: $(Pipeline.Workspace)/packages
    /// </code>
    /// More details can be found in the
    /// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/universal-packages-v1">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    /// <param name="feed">Feed name, or <c>project/feed</c> for project-scoped feeds.</param>
    /// <param name="packageName">Universal Package name.</param>
    /// <param name="packageVersion">Package version to download. Wildcards are supported, such as <c>*</c>, <c>1.*</c>, or <c>1.2.*</c>.</param>
    public UniversalPackagesV1DownloadTask UniversalPackage(
        string feed,
        string packageName,
        string packageVersion) => new(feed, packageName, packageVersion);

    /// <summary>
    /// Creates a <c>DownloadBuildArtifacts@1</c> task.
    /// </summary>
    public DownloadBuildArtifactsTaskBuilder BuildArtifacts { get; } = new();

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
    /// Creates a task-backed download step that downloads pipeline artifacts from the current run using <c>DownloadPipelineArtifact@2</c>.
    /// </para>
    /// This differs from <see cref="Current"/> by emitting the full task form with <c>source: current</c>.
    /// <para>
    /// For example:
    /// </para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Download.CurrentBuild("WebApp", variables.Pipeline.Workspace, ["**/*.zip"])
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: DownloadPipelineArtifact@2
    ///   inputs:
    ///     source: current
    ///     artifact: WebApp
    ///     path: $(Pipeline.Workspace)
    ///     patterns: '**/*.zip'
    /// </code>
    /// </summary>
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
    /// </param>
    /// <param name="patterns">
    /// One or more file matching patterns that limit which files get downloaded.
    /// <para>
    /// Default value: <c>**</c>
    /// </para>
    /// </param>
    public SpecificDownloadTask CurrentBuild(
        AdoExpression<string>? artifact = null,
        AdoExpression<string>? path = null,
        IEnumerable<string>? patterns = null)
        =>
        new(DownloadPipelineArtifactSource.Current)
        {
            Artifact = artifact,
            Path = path,
            Patterns = patterns?.ToList(),
        };

    /// <summary>
    /// <para>
    /// Creates a download task that downloads an artifact from a given pipeline run.
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
        AdoExpression<string> project,
        AdoExpression<int> definition,
        AdoExpression<int> buildId,
        AdoExpression<string>? artifact = null,
        AdoExpression<string>? path = null,
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
    /// Creates a task-backed download step that downloads artifacts from the latest run of a specific pipeline.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Download.Latest("public", 56, "MyProject.CLI") with
    ///     {
    ///         AllowPartiallySucceededBuilds = true,
    ///         Tags = ["release"],
    ///     }
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: DownloadPipelineArtifact@2
    ///   inputs:
    ///     source: specific
    ///     runVersion: latest
    ///     project: public
    ///     pipeline: 56
    ///     artifact: MyProject.CLI
    ///     allowPartiallySucceededBuilds: true
    ///     tags: release
    /// </code>
    /// </summary>
    /// <param name="project">The project GUID from which to download the pipeline artifacts.</param>
    /// <param name="definition">The definition ID of the build pipeline.</param>
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
    /// </param>
    /// <param name="patterns">
    /// One or more file matching patterns that limit which files get downloaded.
    /// <para>
    /// Default value: <c>**</c>
    /// </para>
    /// </param>
    public SpecificDownloadTask Latest(
        AdoExpression<string> project,
        AdoExpression<int> definition,
        AdoExpression<string>? artifact = null,
        AdoExpression<string>? path = null,
        IEnumerable<string>? patterns = null)
        =>
        new(RunVersion.Latest, project, definition)
        {
            Artifact = artifact,
            Path = path,
            Patterns = patterns?.ToList(),
        };

    /// <summary>
    /// <para>
    /// Creates a download task that downloads an artifact from a given pipeline run.
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
        AdoExpression<string> project,
        AdoExpression<int> definition,
        AdoExpression<string>? branchName = null,
        AdoExpression<string>? artifact = null,
        AdoExpression<string>? path = null,
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
    /// Creates a download task that downloads an artifact from a pipeline resource.
    /// <para>For example:</para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Download.FromPipelineResource("myPipelineResource", "MyArtifact", ["**/*.dll"])
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - download: myPipelineResource
    ///   artifact: MyArtifact
    ///   patterns: '**/*.dll'
    /// </code>
    /// </summary>
    /// <param name="resourceName">Already defined pipeline resource</param>
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
