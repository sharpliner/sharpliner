using System;
using System.Collections.Generic;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/utility/download-pipeline-artifact?view=azure-devops">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract record DownloadTask : Step
{
    /// <summary>
    /// Specify current, pipeline resource identifier, or none to disable automatic download.
    /// </summary>
    [YamlMember(Order = 1)]
    public abstract string Download { get; }

    /// <summary>
    /// The name of the artifact to download. If left empty, all artifacts associated to the pipeline run will be downloaded.
    /// </summary>
    [YamlMember(Order = 60)]
    public Conditioned<string>? Artifact { get; init; }

    /// <summary>
    /// One or more file matching patterns (new line delimited) that limit which files get downloaded.
    /// Default value: **
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/file-matching-patterns?view=azure-devops">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    [YamlIgnore]
    public List<string> Patterns { get; init; } = [];

    /// <summary>
    /// Internal property to serialize the patterns as a new line delimited string.
    /// </summary>
    [YamlMember(Alias = "patterns", Order = 61, ScalarStyle = YamlDotNet.Core.ScalarStyle.Literal)]
    public string _Patterns => string.Join("\n", Patterns);

    /// <summary>
    /// Directory to download the artifact files. Can be relative to the pipeline workspace directory or absolute.
    /// If multi-download option is applied (by leaving an empty artifact name), a sub-directory will be created for each.
    /// Default value: $(Pipeline.Workspace)
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/artifacts/pipeline-artifacts?view=azure-devops">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    [YamlMember(Order = 62)]
    public Conditioned<string>? Path { get; init; }

    /// <summary>
    /// A list of tags. Only builds with these tags will be returned.
    /// </summary>
    [YamlIgnore]
    public List<string> Tags { get; init; } = [];

    /// <summary>
    /// Internal property to serialize the tags as a comma-separated string.
    /// </summary>
    [YamlMember(Alias = "tags", Order = 104)]
    public string? _Tags => Tags.Count > 0 ? string.Join(",", Tags) : null;
}

/// <summary>
/// A download task that downloads artifacts for the current job
/// </summary>
public record CurrentDownloadTask : DownloadTask
{
    /// <inheritdoc/>
    [YamlMember(Order = 1)]
    public override string Download => "current";
}

/// <summary>
/// A download task that skips downloading artifacts for the current job
/// </summary>
public record NoneDownloadTask : Step
{
    /// <summary>
    /// Disable automatic download.
    /// </summary>
    [YamlMember(Order = 1)]
    public string Download => "none";
}

/// <summary>
/// A download task that downloads artifacts from a pipeline resource.
/// </summary>
public record DownloadFromPipelineResourceTask : DownloadTask
{
    /// <inheritdoc/>
    [YamlMember(Order = 1)]
    public override string Download { get; }

    /// <summary>
    /// Instantiates a new <see cref="DownloadFromPipelineResourceTask"/> with the specified parameters.
    /// </summary>
    /// <param name="resourceName">The name of the pipeline resource to download.</param>
    public DownloadFromPipelineResourceTask(string resourceName)
    {
        Download = resourceName;
    }
}

/// <summary>
/// A download task that downloads artifacts from a specific pipeline run.
/// </summary>
public record SpecificDownloadTask : AzureDevOpsTask
{
    private const string ArtifactProperty = "artifact";
    private const string PatternsProperty = "patterns";
    private const string PathProperty = "path";
    private const string ProjectProperty = "project";
    private const string PipelineProperty = "pipeline";
    private const string RunVersionProperty = "runVersion";
    private const string RunBranchProperty = "runBranch";
    private const string RunIdProperty = "runId";
    private const string TagsProperty = "tags";
    private const string PreferTriggeringPipelineProperty = "preferTriggeringPipeline";
    private const string AllowPartiallySucceededBuildsProperty = "allowPartiallySucceededBuilds";
    private const string AllowFailedBuildsProperty = "allowFailedBuilds";
    private const string CheckDownloadedFilesProperty = "checkDownloadedFiles";
    private const string RetryDownloadCountProperty = "retryDownloadCount";

    /// <summary>
    /// Instantiates a new <see cref="SpecificDownloadTask"/> with the specified parameters.
    /// </summary>
    /// <param name="runVersion">The build version to download.</param>
    /// <param name="project">The project GUID from which to download the pipeline artifacts.</param>
    /// <param name="pipeline">The definition ID of the build pipeline.</param>
    public SpecificDownloadTask(RunVersion runVersion, string project, int pipeline)
        : base("DownloadPipelineArtifact@2")
    {
        RunVersion = runVersion;
        Project = project;
        Pipeline = pipeline;
    }

    /// <summary>
    /// The name of the artifact to download. If left empty, all artifacts associated to the pipeline run will be downloaded.
    /// </summary>
    [YamlIgnore]
    public string? Artifact
    {
        get => GetString(ArtifactProperty);
        init => SetProperty(ArtifactProperty, value);
    }

    /// <summary>
    /// Specifies either a relative or absolute path on the agent machine where the artifacts will download.
    /// If the multi-download option is applied (by leaving an empty artifact name), a sub-directory will be created for each download
    /// <para>
    /// Default value: <c>$(Pipeline.Workspace)</c>
    /// </para>
    /// <para>
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/artifacts/pipeline-artifacts?view=azure-devops">Artifacts in Azure Pipelines</see>.
    /// </para>
    /// </summary>
    [YamlIgnore]
    public string? Path
    {
        get => GetString(PathProperty);
        init => SetProperty(PathProperty, value);
    }

    /// <summary>
    /// One or more file matching patterns that limit which files get downloaded.
    /// Default value: **
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/file-matching-patterns?view=azure-devops">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    [YamlIgnore]
    public List<string>? Patterns
    {
        get => [.. (GetString(PatternsProperty) ?? string.Empty).Split(System.Environment.NewLine)];
        init => SetProperty(PatternsProperty, value is null || value.Count == 0 ? null : string.Join(System.Environment.NewLine, value));
    }

    /// <summary>
    /// The project GUID from which to download the pipeline artifacts.
    /// </summary>
    [YamlIgnore]
    public string Project
    {
        get => GetString(ProjectProperty) ?? throw new NullReferenceException();
        private init => SetProperty(ProjectProperty, value);
    }

    /// <summary>
    /// The definition ID of the build pipeline.
    /// </summary>
    [YamlIgnore]
    public int Pipeline
    {
        get => GetInt(PipelineProperty) ?? 0;
        private init => SetProperty(PipelineProperty, value.ToString());
    }

    /// <summary>
    /// The build version to download.
    /// </summary>
    [YamlIgnore]
    public RunVersion RunVersion
    {
        get => GetEnum(RunVersionProperty, RunVersion.Latest);
        internal init => SetProperty(RunVersionProperty, value);
    }

    /// <summary>
    /// Specify to filter on branch/ref name.
    /// For example: <c>refs/heads/develop</c>
    /// Argument aliases: branchName
    /// </summary>
    [YamlIgnore]
    public string? RunBranch
    {
        get => GetString(RunBranchProperty);
        internal init => SetProperty(RunBranchProperty, value);
    }

    /// <summary>
    /// Specify to filter on branch/ref name.
    /// For example: refs/heads/develop
    /// Argument aliases: runBranch
    /// </summary>
    [YamlIgnore]
    public string? BranchName
    {
        get => GetString(RunBranchProperty);
        internal init => SetProperty(RunBranchProperty, value);
    }

    /// <summary>
    /// The build from which to download the artifacts.
    /// For example: 1764
    /// Argument aliases: pipelineId, buildId
    /// </summary>
    [YamlIgnore]
    public int RunId
    {
        get => GetInt(RunIdProperty) ?? 0;
        internal init => SetProperty(RunIdProperty, value);
    }

    /// <summary>
    /// The build from which to download the artifacts.
    /// For example: 1764
    /// Argument aliases: runId, buildId
    /// </summary>
    [YamlIgnore]
    public int PipelineId
    {
        get => GetInt(RunIdProperty) ?? 0;
        internal init => SetProperty(RunIdProperty, value);
    }

    /// <summary>
    /// The build from which to download the artifacts.
    /// For example: 1764
    /// Argument aliases: runId, pipelineId
    /// </summary>
    [YamlIgnore]
    public int BuildId
    {
        get => GetInt(RunIdProperty) ?? 0;
        internal init => SetProperty(RunIdProperty, value);
    }

    /// <summary>
    /// A list of tags. Only builds with these tags will be returned.
    /// </summary>
    [YamlIgnore]
    public List<string> Tags
    {
        get => [.. (GetString(TagsProperty) ?? string.Empty).Split(",")];
        init => SetProperty(TagsProperty, string.Join(",", value));
    }

    /// <summary>
    /// A boolean specifying whether to download artifacts from a triggering build.
    /// Defaults to false.
    /// </summary>
    [YamlIgnore]
    public Conditioned<bool>? PreferTriggeringPipeline
    {
        get => GetConditioned<bool>(PreferTriggeringPipelineProperty);
        init => SetProperty(PreferTriggeringPipelineProperty, value);
    }

    /// <summary>
    /// If checked, this build task will try to download artifacts whether the build is succeeded or partially succeeded.
    /// Defaults to false.
    /// </summary>
    [YamlIgnore]
    public Conditioned<bool>? AllowPartiallySucceededBuilds
    {
        get => GetConditioned<bool>(AllowPartiallySucceededBuildsProperty);
        init => SetProperty(AllowPartiallySucceededBuildsProperty, value);
    }

    /// <summary>
    /// If checked, this build task will try to download artifacts whether the build is succeeded or failed.
    /// Defaults to false.
    /// </summary>
    [YamlIgnore]
    public Conditioned<bool>? AllowFailedBuilds
    {
        get => GetConditioned<bool>(AllowFailedBuildsProperty);
        init => SetProperty(AllowFailedBuildsProperty, value);
    }

    /// <summary>
    /// A boolean specifying whether this build task will check that all files are fully downloaded.
    /// Defaults to false.
    /// </summary>
    [YamlIgnore]
    public Conditioned<bool>? CheckDownloadedFiles
    {
        get => GetConditioned<bool>(CheckDownloadedFilesProperty);
        init => SetProperty(CheckDownloadedFilesProperty, value);
    }

    /// <summary>
    /// Number of times to retry downloading a build artifact if the download fails.
    /// Defaults to 4.
    /// </summary>
    [YamlIgnore]
    public int RetryDownloadCount
    {
        get => GetInt(RetryDownloadCountProperty) ?? 0;
        init => SetProperty(RetryDownloadCountProperty, value);
    }
}

/// <summary>
/// The version of the build to download.
/// </summary>
public enum RunVersion
{
    /// <summary>
    /// Latest run.
    /// </summary>
    [YamlMember(Alias = "latest")]
    Latest,

    /// <summary>
    /// Latest run from specific branch and specified Build Tags
    /// </summary>
    [YamlMember(Alias = "latestFromBranch")]
    LatestFromBranch,

    /// <summary>
    /// Specific run.
    /// </summary>
    [YamlMember(Alias = "specific")]
    Specific,
}
