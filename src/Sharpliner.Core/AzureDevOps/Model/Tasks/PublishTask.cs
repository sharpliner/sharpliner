using System;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// <para>
/// Represents the <c>publish</c> step shortcut which is a shortcut for the <c>PublishPipelineArtifact@1</c> task.
/// </para>
/// <para>
/// The shortcut only supports the <c>publish</c> and <c>artifact</c> properties (on top of the common step properties).
/// If you need to publish to a file share or to set any of the other task inputs
/// (<c>artifactType</c>, <c>fileSharePath</c>, <c>parallel</c>, <c>parallelCount</c> or <c>properties</c>),
/// use <see cref="PublishPipelineArtifactTask"/> instead.
/// </para>
/// More details can be found in the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/steps-publish">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record PublishTask : Step
{
    /// <summary>
    /// Path to the folder or file you want to publish.
    /// The path must be a fully qualified path or a valid path relative to the root directory of your repository.
    /// Wildcards are not supported.
    /// </summary>
    [YamlMember(Alias = "publish", Order = 1)]
    public AdoExpression<string>? TargetPath { get; }

    /// <summary>
    /// Your artifact name. You can specify any name you prefer. E.g.: drop
    /// </summary>
    [YamlMember(Order = 101)]
    public AdoExpression<string>? Artifact { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PublishTask"/> class with required properties.
    /// </summary>
    /// <param name="targetPath">The path to the folder or file you want to publish.</param>
    /// <param name="artifactName">Your artifact name. You can specify any name you prefer. E.g.: drop</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="targetPath"/> is null.</exception>
    public PublishTask(AdoExpression<string> targetPath, AdoExpression<string>? artifactName = null)
    {
        TargetPath = targetPath ?? throw new ArgumentNullException(nameof(targetPath));
        Artifact = artifactName ?? "drop";
    }
}

/// <summary>
/// <para>
/// Represents the <c>PublishPipelineArtifact@1</c> task which publishes (uploads) a file or a directory
/// as a named artifact for the current run.
/// </para>
/// Unlike the <c>publish</c> shortcut (<see cref="PublishTask"/>), this task also supports publishing to a file share.
/// More details can be found in the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/publish-pipeline-artifact-v1">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record PublishPipelineArtifactTask : AzureDevOpsTask
{
    private const string TargetPathProperty = "targetPath";
    private const string ArtifactProperty = "artifact";
    private const string ArtifactTypeProperty = "artifactType";
    private const string FileSharePathProperty = "fileSharePath";
    private const string ParallelProperty = "parallel";
    private const string ParallelCountProperty = "parallelCount";
    private const string PropertiesProperty = "properties";

    /// <summary>
    /// Path of the file or directory to publish.
    /// Can be absolute or relative to the default working directory. Can include variables, but wildcards are not supported.
    /// <para>
    /// Argument aliases: path
    /// </para>
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string> TargetPath
    {
        get => GetExpression<string>(TargetPathProperty) ?? throw new NullReferenceException();
        init => SetProperty(TargetPathProperty, value);
    }

    /// <summary>
    /// Name of the artifact to publish. You can specify any name you prefer, e.g.: drop.
    /// If not set, the default is a unique ID scoped to the job.
    /// <para>
    /// Argument aliases: artifactName
    /// </para>
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Artifact
    {
        get => GetExpression<string>(ArtifactProperty);
        init => SetProperty(ArtifactProperty, value);
    }

    /// <summary>
    /// Specifies whether to store the artifact in Azure Pipelines or to copy it to a file share
    /// that must be accessible from the pipeline agent.
    /// Defaults to <see cref="Tasks.ArtifactType.Pipeline"/>.
    /// <para>
    /// Argument aliases: publishLocation
    /// </para>
    /// </summary>
    [YamlIgnore]
    public AdoExpression<ArtifactType>? ArtifactType
    {
        get => GetExpression<ArtifactType>(ArtifactTypeProperty);
        init => SetProperty(ArtifactTypeProperty, value);
    }

    /// <summary>
    /// The file share to which the artifact files will be copied.
    /// This can include variables, e.g.: <c>\\my\share\$(Build.DefinitionName)\$(Build.BuildNumber)</c>.
    /// Required when <see cref="ArtifactType"/> = <see cref="Tasks.ArtifactType.Filepath"/>.
    /// Publishing artifacts from a Linux or macOS agent to a file share is not supported.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? FileSharePath
    {
        get => GetExpression<string>(FileSharePathProperty);
        init => SetProperty(FileSharePathProperty, value);
    }

    /// <summary>
    /// Select whether to copy files in parallel using multiple threads for greater potential throughput.
    /// If this setting is not enabled, one thread will be used.
    /// Only used when <see cref="ArtifactType"/> = <see cref="Tasks.ArtifactType.Filepath"/>.
    /// Defaults to <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? Parallel
    {
        get => GetExpression<bool>(ParallelProperty);
        init => SetProperty(ParallelProperty, value);
    }

    /// <summary>
    /// Enter the degree of parallelism, or number of threads used, to perform the copy.
    /// The value must be at least 1 and not greater than 128.
    /// Only used when <see cref="ArtifactType"/> = <see cref="Tasks.ArtifactType.Filepath"/> and <see cref="Parallel"/> = <c>true</c>.
    /// Defaults to 8.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<uint>? ParallelCount
    {
        get => GetExpression<uint>(ParallelCountProperty);
        init => SetProperty(ParallelCountProperty, value);
    }

    /// <summary>
    /// Custom properties to associate with the artifact.
    /// A valid JSON string is expected with all keys having the prefix <c>user-</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Properties
    {
        get => GetExpression<string>(PropertiesProperty);
        init => SetProperty(PropertiesProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PublishPipelineArtifactTask"/> class with required properties.
    /// </summary>
    /// <param name="targetPath">The path of the file or directory to publish.</param>
    /// <param name="artifactName">Name of the artifact to publish. If not set, the default is a unique ID scoped to the job.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="targetPath"/> is null.</exception>
    public PublishPipelineArtifactTask(AdoExpression<string> targetPath, AdoExpression<string>? artifactName = null)
        : base("PublishPipelineArtifact@1")
    {
        TargetPath = targetPath ?? throw new ArgumentNullException(nameof(targetPath));
        Artifact = artifactName;
    }
}

/// <summary>
/// Artifact publish location
/// </summary>
public enum ArtifactType
{
    /// <summary>
    /// Azure Pipelines
    /// </summary>
    [YamlMember(Alias = "pipeline")]
    Pipeline,

    /// <summary>
    /// A file share
    /// </summary>
    [YamlMember(Alias = "filepath")]
    Filepath,
}
