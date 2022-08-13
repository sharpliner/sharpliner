﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/utility/download-pipeline-artifact?view=azure-devops">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract record DownloadTask : Step
{
    [YamlMember(Order = 1)]
    public abstract string Download { get; }

    /// <summary>
    /// The name of the artifact to download. If left empty, all artifacts associated to the pipeline run will be downloaded.
    /// </summary>
    [YamlMember(Order = 60)]
    public string? Artifact { get; init; } = null;

    /// <summary>
    /// One or more file matching patterns (new line delimited) that limit which files get downloaded.
    /// Default value: **
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/file-matching-patterns?view=azure-devops">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    [YamlIgnore]
    public List<string> Patterns { get; init; } = new();

    [YamlMember(Alias = "patterns", Order = 61, ScalarStyle = YamlDotNet.Core.ScalarStyle.Literal)]
    public string _Patterns => string.Join("\n", Patterns);

    /// <summary>
    /// Directory to download the artifact files. Can be relative to the pipeline workspace directory or absolute.
    /// If multi-download option is applied (by leaving an empty artifact name), a sub-directory will be created for each.
    /// Default value: $(Pipeline.Workspace)
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/artifacts/pipeline-artifacts?view=azure-devops">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    [YamlMember(Order = 62)]
    [DefaultValue("$(Pipeline.Workspace)")]
    public string Path { get; init; } = "$(Pipeline.Workspace)";

    /// <summary>
    /// A list of tags. Only builds with these tags will be returned.
    /// </summary>
    [YamlIgnore]
    public List<string> Tags { get; init; } = new();

    [YamlMember(Alias = "tags", Order = 104)]
    public string? _Tags => Tags.Any() ? string.Join(",", Tags) : null;
}

public record CurrentDownloadTask : DownloadTask
{
    [YamlMember(Order = 1)]
    public override string Download => "current";
}

public record NoneDownloadTask : Step
{
    [YamlMember(Order = 1)]
    public string Download => "none";
}

public record SpecificDownloadTask : AzureDevOpsTask
{
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

    public SpecificDownloadTask(string project, int pipeline) : base("DownloadPipelineArtifact@2")
    {
        SetProperty(RunVersionProperty, "specific");
        Project = project;
        Pipeline = pipeline;
    }

    /// <summary>
    /// The project GUID from which to download the pipeline artifacts.
    /// </summary>
    [YamlIgnore]
    public string Project
    {
        get => GetString(ProjectProperty) ?? throw new NullReferenceException();
        init => SetProperty(ProjectProperty, value);
    }

    /// <summary>
    /// The definition ID of the build pipeline.
    /// </summary>
    [YamlIgnore]
    public int Pipeline
    {
        get => GetInt(PipelineProperty) ?? 0;
        init => SetProperty(PipelineProperty, value.ToString());
    }

    [YamlIgnore]
    public RunVersion RunVersion { get; } = RunVersion.Specific;

    /// <summary>
    /// Specify to filter on branch/ref name.
    /// For example: refs/heads/develop
    /// Argument aliases: branchName
    /// </summary>
    [YamlIgnore]
    public string RunBranch
    {
        get => GetString(RunBranchProperty)!;
        init => SetProperty(RunBranchProperty, value);
    }

    /// <summary>
    /// Specify to filter on branch/ref name.
    /// For example: refs/heads/develop
    /// Argument aliases: runBranch
    /// </summary>
    [YamlIgnore]
    public string BranchName
    {
        get => GetString(RunBranchProperty)!;
        init => SetProperty(RunBranchProperty, value);
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
        init => SetProperty(RunIdProperty, value.ToString());
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
        init => SetProperty(RunIdProperty, value.ToString());
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
        init => SetProperty(RunIdProperty, value.ToString());
    }

    /// <summary>
    /// A list of tags. Only builds with these tags will be returned.
    /// </summary>
    [YamlIgnore]
    public List<string> Tags
    {
        get => (GetString(TagsProperty) ?? string.Empty).Split(",").ToList();
        init => SetProperty(TagsProperty, string.Join(",", value));
    }

    /// <summary>
    /// A boolean specifying whether to download artifacts from a triggering build.
    /// Defaults to false.
    /// </summary>
    [YamlIgnore]
    public bool PreferTriggeringPipeline
    {
        get => GetBool(PreferTriggeringPipelineProperty, false);
        init => SetProperty(PreferTriggeringPipelineProperty, value ? "true" : "false");
    }

    /// <summary>
    /// If checked, this build task will try to download artifacts whether the build is succeeded or partially succeeded.
    /// Defaults to false.
    /// </summary>
    [YamlIgnore]
    public bool AllowPartiallySucceededBuilds
    {
        get => GetBool(AllowPartiallySucceededBuildsProperty, false);
        init => SetProperty(AllowPartiallySucceededBuildsProperty, value ? "true" : "false");
    }

    /// <summary>
    /// If checked, this build task will try to download artifacts whether the build is succeeded or failed.
    /// Defaults to false.
    /// </summary>
    [YamlIgnore]
    public bool AllowFailedBuilds
    {
        get => GetBool(AllowFailedBuildsProperty, false);
        init => SetProperty(AllowFailedBuildsProperty, value ? "true" : "false");
    }

    /// <summary>
    /// A boolean specifying whether this build task will check that all files are fully downloaded.
    /// Defaults to false.
    /// </summary>
    [YamlIgnore]
    public bool CheckDownloadedFiles
    {
        get => GetBool(CheckDownloadedFilesProperty, false);
        init => SetProperty(CheckDownloadedFilesProperty, value ? "true" : "false");
    }

    /// <summary>
    /// Number of times to retry downloading a build artifact if the download fails.
    /// Defaults to 4.
    /// </summary>
    [YamlIgnore]
    public int RetryDownloadCount
    {
        get => GetInt(RetryDownloadCountProperty) ?? 0;
        init => SetProperty(RetryDownloadCountProperty, value.ToString());
    }
}

public enum RunVersion
{
    Latest,
    LatestFromBranch,
    Specific,
}
