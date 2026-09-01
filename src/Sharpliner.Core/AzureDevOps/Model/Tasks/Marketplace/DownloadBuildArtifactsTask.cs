using System.Collections.Generic;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Defines shared inputs for the <c>DownloadBuildArtifacts@1</c> Azure Pipelines task.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/download-build-artifacts-v1">official Azure DevOps pipelines documentation</see>
/// and the
/// <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/DownloadBuildArtifactsV1/task.json">official task specification</see>.
/// </summary>
public abstract record DownloadBuildArtifactsTask : AzureDevOpsTask
{
    /// <summary>
    /// Downloads a specific artifact or specific files from the build.
    /// Default value: <see cref="BuildArtifactDownloadType.Single"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<BuildArtifactDownloadType>? DownloadType
    {
        get => GetExpression<BuildArtifactDownloadType>("downloadType");
        init => SetProperty("downloadType", value);
    }

    /// <summary>
    /// The name of the artifact to download.
    /// Required when <see cref="DownloadType"/> is <see cref="BuildArtifactDownloadType.Single"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ArtifactName
    {
        get => GetExpression<string>("artifactName");
        init => SetProperty("artifactName", value);
    }

    /// <summary>
    /// File matching patterns to download, one pattern per line.
    /// Default value: <c>**</c>.
    /// More details can be found in the
    /// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/file-matching-patterns">file matching patterns documentation</see>.
    /// </summary>
    [YamlIgnore]
    public List<string> ItemPatterns
    {
        get
        {
            var patterns = GetString("itemPattern");
            return string.IsNullOrEmpty(patterns) ? [] : [.. patterns.Split(System.Environment.NewLine)];
        }
        init => SetProperty("itemPattern", value is null || value.Count == 0 ? null : string.Join(System.Environment.NewLine, value));
    }

    /// <summary>
    /// Path on the agent machine where the artifacts are downloaded.
    /// Default value: <c>$(System.ArtifactsDirectory)</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? DownloadPath
    {
        get => GetExpression<string>("downloadPath");
        init => SetProperty("downloadPath", value);
    }

    /// <summary>
    /// Deletes all existing files in the destination folder before downloading artifacts.
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? CleanDestinationFolder
    {
        get => GetExpression<bool>("cleanDestinationFolder");
        init => SetProperty("cleanDestinationFolder", value);
    }

    /// <summary>
    /// Number of files to download simultaneously.
    /// Default value: <c>8</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<int>? ParallelizationLimit
    {
        get => GetExpression<int>("parallelizationLimit");
        init => SetProperty("parallelizationLimit", value);
    }

    /// <summary>
    /// Checks that all files are fully downloaded.
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? CheckDownloadedFiles
    {
        get => GetExpression<bool>("checkDownloadedFiles");
        init => SetProperty("checkDownloadedFiles", value);
    }

    /// <summary>
    /// Number of times to retry downloading a build artifact when the download fails.
    /// Default value: <c>4</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<int>? RetryDownloadCount
    {
        get => GetExpression<int>("retryDownloadCount");
        init => SetProperty("retryDownloadCount", value);
    }

    /// <summary>
    /// Extracts all downloaded files that have a <c>.tar</c> extension.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? ExtractTars
    {
        get => GetExpression<bool>("extractTars");
        init => SetProperty("extractTars", value);
    }

    private protected DownloadBuildArtifactsTask(
        BuildArtifactSource source,
        AdoExpression<BuildArtifactDownloadType> downloadType,
        AdoExpression<string> downloadPath)
        : base("DownloadBuildArtifacts@1")
    {
        SetProperty("buildType", source);
        DownloadType = downloadType;
        DownloadPath = downloadPath;
        DisplayName = "Download build artifacts";
    }
}

/// <summary>
/// Downloads artifacts produced by the current build using <c>DownloadBuildArtifacts@1</c>.
/// </summary>
public record DownloadCurrentBuildArtifactsTask : DownloadBuildArtifactsTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadCurrentBuildArtifactsTask"/> class.
    /// </summary>
    /// <param name="downloadType">Whether to download a single artifact or specific files.</param>
    /// <param name="downloadPath">Path on the agent machine where the artifacts are downloaded.</param>
    public DownloadCurrentBuildArtifactsTask(
        AdoExpression<BuildArtifactDownloadType> downloadType,
        AdoExpression<string> downloadPath)
        : base(BuildArtifactSource.Current, downloadType, downloadPath)
    {
    }
}

/// <summary>
/// Downloads artifacts produced by a specific build using <c>DownloadBuildArtifacts@1</c>.
/// </summary>
public record DownloadSpecificBuildArtifactsTask : DownloadBuildArtifactsTask
{
    /// <summary>
    /// The project from which to download the build artifacts.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Project
    {
        get => GetExpression<string>("project");
        init => SetProperty("project", value);
    }

    /// <summary>
    /// The build pipeline from which to download artifacts.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Definition
    {
        get => GetExpression<string>("definition");
        init => SetProperty("definition", value);
    }

    /// <summary>
    /// Attempts to download artifacts from the triggering build before using the selected build.
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? SpecificBuildWithTriggering
    {
        get => GetExpression<bool>("specificBuildWithTriggering");
        init => SetProperty("specificBuildWithTriggering", value);
    }

    /// <summary>
    /// Selects which build version to download.
    /// Default value: <see cref="BuildArtifactVersion.Latest"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<BuildArtifactVersion>? BuildVersionToDownload
    {
        get => GetExpression<BuildArtifactVersion>("buildVersionToDownload");
        init => SetProperty("buildVersionToDownload", value);
    }

    /// <summary>
    /// Allows artifacts to be downloaded from partially succeeded builds.
    /// Applies when <see cref="BuildVersionToDownload"/> is not <see cref="BuildArtifactVersion.Specific"/>.
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? AllowPartiallySucceededBuilds
    {
        get => GetExpression<bool>("allowPartiallySucceededBuilds");
        init => SetProperty("allowPartiallySucceededBuilds", value);
    }

    /// <summary>
    /// Branch or ref name used to filter builds.
    /// Required when <see cref="BuildVersionToDownload"/> is <see cref="BuildArtifactVersion.LatestFromBranch"/>.
    /// Default value: <c>refs/heads/master</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? BranchName
    {
        get => GetExpression<string>("branchName");
        init => SetProperty("branchName", value);
    }

    /// <summary>
    /// The build from which to download artifacts.
    /// Required when <see cref="BuildVersionToDownload"/> is <see cref="BuildArtifactVersion.Specific"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? BuildId
    {
        get => GetExpression<string>("buildId");
        init => SetProperty("buildId", value);
    }

    /// <summary>
    /// Build tags used to filter builds.
    /// Applies when <see cref="BuildVersionToDownload"/> is not <see cref="BuildArtifactVersion.Specific"/>.
    /// </summary>
    [YamlIgnore]
    public List<string> Tags
    {
        get
        {
            var tags = GetString("tags");
            return string.IsNullOrEmpty(tags) ? [] : [.. tags.Split(",")];
        }
        init => SetProperty("tags", value is null || value.Count == 0 ? null : string.Join(",", value));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadSpecificBuildArtifactsTask"/> class.
    /// </summary>
    /// <param name="project">The project from which to download the build artifacts.</param>
    /// <param name="definition">The build pipeline from which to download artifacts.</param>
    /// <param name="buildVersion">The build version to download.</param>
    /// <param name="downloadType">Whether to download a single artifact or specific files.</param>
    /// <param name="downloadPath">Path on the agent machine where the artifacts are downloaded.</param>
    public DownloadSpecificBuildArtifactsTask(
        AdoExpression<string> project,
        AdoExpression<string> definition,
        AdoExpression<BuildArtifactVersion> buildVersion,
        AdoExpression<BuildArtifactDownloadType> downloadType,
        AdoExpression<string> downloadPath)
        : base(BuildArtifactSource.Specific, downloadType, downloadPath)
    {
        Project = project;
        Definition = definition;
        BuildVersionToDownload = buildVersion;
    }
}

internal enum BuildArtifactSource
{
    [YamlMember(Alias = "current")]
    Current,

    [YamlMember(Alias = "specific")]
    Specific,
}

/// <summary>
/// Artifact selection modes supported by <c>DownloadBuildArtifacts@1</c>.
/// </summary>
public enum BuildArtifactDownloadType
{
    /// <summary>
    /// Download one named artifact.
    /// </summary>
    [YamlMember(Alias = "single")]
    Single,

    /// <summary>
    /// Download files selected by matching patterns.
    /// </summary>
    [YamlMember(Alias = "specific")]
    SpecificFiles,
}

/// <summary>
/// Build versions supported by <c>DownloadBuildArtifacts@1</c>.
/// </summary>
public enum BuildArtifactVersion
{
    /// <summary>
    /// The latest build.
    /// </summary>
    [YamlMember(Alias = "latest")]
    Latest,

    /// <summary>
    /// The latest build from a specific branch and specified build tags.
    /// </summary>
    [YamlMember(Alias = "latestFromBranch")]
    LatestFromBranch,

    /// <summary>
    /// A specific build.
    /// </summary>
    [YamlMember(Alias = "specific")]
    Specific,
}
