using System;
using System.ComponentModel;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/utility/publish-pipeline-artifact?view=azure-devops">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record PublishTask : Step
{
    /// <summary>
    /// Path to the folder or file you want to publish.
    /// The path must be a fully qualified path or a valid path relative to the root directory of your repository.
    /// </summary>
    [YamlMember(Alias = "publish", Order = 1)]
    public string TargetPath { get; }

    /// <summary>
    /// Your artifact name. You can specify any name you prefer. E.g.: drop
    /// </summary>
    [YamlMember(Order = 101)]
    public Conditioned<string>? Artifact { get; init; }

    /// <summary>
    /// Artifacts publish location. Choose whether to store the artifact in Azure Pipelines,
    /// or to copy it to a file share that must be accessible from the pipeline agent.
    /// </summary>
    [YamlMember(Order = 102)]
    [DefaultValue(ArtifactType.Pipeline)]
    public ArtifactType ArtifactType { get; init; } = ArtifactType.Pipeline;

    /// <summary>
    /// The file share to which the artifact files will be copied.
    /// This can include variables. Required when ArtifactType = FilePath.
    /// </summary>
    [YamlMember(Order = 211)]
    public Conditioned<string>? FileSharePath { get; init; }

    /// <summary>
    /// Select whether to copy files in parallel using multiple threads for greater potential throughput.
    /// If this setting is not enabled, one thread will be used.
    /// </summary>
    [YamlMember(Order = 212)]
    public Conditioned<bool>? Parallel { get; init; }

    /// <summary>
    /// Enter the degree of parallelism, or number of threads used, to perform the copy.
    /// The value must be at least 1 and not greater than 128.
    /// </summary>
    [YamlMember(Order = 213)]
    [DefaultValue(1u)]
    public uint ParallelCount { get; init; } = 1u;

    public PublishTask(string targetPath, string artifactName = "drop")
    {
        TargetPath = targetPath ?? throw new ArgumentNullException(nameof(targetPath));
        Artifact = artifactName ?? throw new ArgumentNullException(nameof(artifactName));
    }
}

public enum ArtifactType
{
    Pipeline,
    Filepath,
    Container,
}
