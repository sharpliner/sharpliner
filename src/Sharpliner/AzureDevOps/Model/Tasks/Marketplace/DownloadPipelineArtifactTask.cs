using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/download-pipeline-artifact-v2?view=azure-pipelines">official Azure DevOps pipelines documentation</see>
/// </summary>
public record DownloadPipelineArtifactTask : AzureDevOpsTask
{
    /// <summary>
    /// Downloads artifacts produced by the current pipeline run or from a specific pipeline run.
    /// Defaults to <code>current</code>
    /// </summary>
    [YamlIgnore]
    public BuildType BuildType
    {
        get => GetEnum("buildType", BuildType.Current);
        init => SetProperty("buildType", value);
    }

    /// <summary>
    /// Specifies the project name or GUID from which to download the pipeline artifacts.
    /// </summary>
    [YamlIgnore]
    public string? Project
    {
        get => GetString("project");
        init => SetProperty("project", value);
    }

    /// <summary>
    /// The definition ID of the pipeline. Required when source == specific.
    /// </summary>
    [YamlIgnore]
    public string? Definition
    {
        get => GetString("definition");
        init => SetProperty("definition", value);
    }

    /// <summary>
    /// When appropriate, download artifacts from the triggering build.
    /// If checked, the task downloads artifacts from the triggering build.
    /// If there is no triggering build from the specified pipeline, the task downloads artifacts from the build specified in the options below.
    /// Defaults to <code>false</code>
    /// </summary>
    [YamlIgnore]
    public bool SpecificBuildWithTriggering
    {
        get => GetBool("specificBuildWithTriggering", false);
        init => SetProperty("specificBuildWithTriggering", value);
    }

    /// <summary>
    /// Required when source == specific.
    /// Allowed values: latest, latestFromBranch (Latest from specific branch and specified Build Tags), specific (Specific version)
    /// Defaults to <code>latest</code>
    /// </summary>
    [YamlIgnore]
    public BuildVersionToDownload BuildVersionToDownload
    {
        get => GetEnum("buildVersionToDownload", BuildVersionToDownload.Latest);
        init => SetProperty("buildVersionToDownload", value);
    }

    /// <summary>
    /// Specifies the filter on the branch/ref name
    /// Required when source == specific and runVersion == latestFromBranch.
    /// Defaults to <code>refs/heads/master</code>
    /// </summary>
    [YamlIgnore]
    public string? BranchName
    {
        get => GetString("branchName");
        init => SetProperty("branchName", value);
    }

    /// <summary>
    /// Required when source == specific and runVersion == specific
    /// The identifier of the pipeline run from which to download the artifacts
    /// </summary>
    [YamlIgnore]
    public string? PipelineId
    {
        get => GetString("pipelineId");
        init => SetProperty("pipelineId", value);
    }

    /// <summary>
    /// Optional. Use when source == specific and runVersion != specific.
    /// The comma-delimited list of tags that the task uses to return tagged builds. Untagged builds are not returned.
    /// </summary>
    [YamlIgnore]
    public string? Tags
    {
        get => GetString("tags");
        init => SetProperty("tags", value);
    }

    /// <summary>
    /// Optional. Use when source == specific and runVersion != specific.
    /// Specifies if the build task downloads artifacts whether the build succeeds or partially succeeds.
    /// Defaults to <code>false</code>
    /// </summary>
    [YamlIgnore]
    public bool AllowPartiallySucceededBuilds
    {
        get => GetBool("allowPartiallySucceededBuilds", false);
        init => SetProperty("allowPartiallySucceededBuilds", value);
    }

    /// <summary>
    /// Optional. Use when source == specific and runVersion != specific.
    /// If checked, the build task downloads artifacts whether the build succeeds or fails.
    /// Defaults to <code>false</code>
    /// </summary>
    [YamlIgnore]
    public bool AllowFailedBuilds
    {
        get => GetBool("allowFailedBuilds", false);
        init => SetProperty("allowFailedBuilds", value);
    }

    /// <summary>
    /// Specifies the name of the artifact to download. If the value is left empty, the task downloads all artifacts associated with the pipeline run.
    /// If checked, the build task downloads artifacts whether the build succeeds or fails.
    /// Defaults to <code>false</code>
    /// </summary>
    [YamlIgnore]
    public string? ArtifactName
    {
        get => GetString("artifactName");
        init => SetProperty("artifactName", value);
    }

    /// <summary>
    /// The file matching patterns that limit downloaded files. The value can be one or more file matching patterns that are new line delimited.
    /// Defaults to <code>**</code>
    /// </summary>
    [YamlIgnore]
    public string? ItemPattern
    {
        get => GetString("itemPattern");
        init => SetProperty("itemPattern", value);
    }

    /// <summary>
    /// Specifies either a relative or absolute path on the agent machine where the artifacts will download.
    /// If the multi-download option is applied (by leaving an empty artifact name), a sub-directory will be created for each download.
    /// Defaults to <code>$(Pipeline.Workspace)</code>
    /// </summary>
    [YamlIgnore]
    public string? TargetPath
    {
        get => GetString("targetPath");
        init => SetProperty("targetPath", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DownloadPipelineArtifactTask"/> class with required properties.
    /// </summary>
    public DownloadPipelineArtifactTask()
        : base("DownloadPipelineArtifact@2")
    {
    }
}

/// <summary>
/// Allowed values for BuildType
/// </summary>
public enum BuildType
{
    /// <summary>
    /// Default. Current run
    /// </summary>
    [YamlMember(Alias = "current")]
    Current,

    /// <summary>
    /// Specific run
    /// </summary>
    [YamlMember(Alias = "specific")]
    Specific,
}

/// <summary>
/// Allowed values for BuildVersionToDownload
/// </summary>
public enum BuildVersionToDownload
{
    /// <summary>
    /// Default
    /// </summary>
    [YamlMember(Alias = "latest")]
    Latest,

    /// <summary>
    /// Latest from specific branch and specified Build Tags
    /// </summary>
    [YamlMember(Alias = "latestFromBranch")]
    LatestFromBranch,

    /// <summary>
    /// Specific version
    /// </summary>
    [YamlMember(Alias = "specific")]
    Specific,
}
