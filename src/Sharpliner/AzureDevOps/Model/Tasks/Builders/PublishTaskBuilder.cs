namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating a publish task.
/// </summary>
public class PublishTaskBuilder
{
    /// <summary>
    /// <para>
    /// Creates a publish task with <see cref="PublishTask.ArtifactType"/> set to <see cref="ArtifactType.Pipeline"/>
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Publish.Pipeline("Binary", "bin/Debug/net8.0/") with
    ///     {
    ///         DisplayName = "Publish artifact",
    ///         ContinueOnError = false,
    ///         ArtifactType = ArtifactType.Pipeline,
    ///     }
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// steps:
    /// - publish: bin/Debug/net8.0/
    ///   displayName: Publish artifact
    ///   artifact: Binary
    ///   continueOnError: false
    /// </code>
    /// <param name="artifactName">Artifact name</param>
    /// <param name="filePath">Path to the folder or file you want to publish</param>
    /// <returns>A new instance of <see cref="PublishTask"/> with the specified parameters</returns>
    /// </summary>
    public PublishTask Pipeline(string artifactName, string targetPath)
    {
        return new PublishTask(targetPath, artifactName);
    }

    /// <summary>
    /// <para>
    /// Creates a publish task with <see cref="PublishTask.ArtifactType"/> set to <see cref="ArtifactType.Filepath"/>
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Publish.FileShare("additional-binary", "bin/Debug/netstandard2.0/", $"{variables.Build.ArtifactStagingDirectory}/additional-binary") with
    ///     {
    ///         Parallel = true
    ///     }
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// steps:
    /// - publish: bin/Debug/netstandard2.0/
    ///   artifact: additional-binary
    ///   artifactType: filepath
    ///   fileSharePath: $(Build.ArtifactStagingDirectory)/additional-binary
    ///   parallel: true
    /// </code>
    /// <param name="artifactName">Artifact name</param>
    /// <param name="filePath">Path to the folder or file you want to publish</param>
    /// <param name="fileSharePath"/>Path to the file share.</param>
    /// <returns>A new instance of <see cref="PublishTask"/> with the specified parameters</returns>
    /// </summary>
    public PublishTask FileShare(string artifactName, string targetPath, string fileSharePath)
    {
        return new PublishTask(targetPath, artifactName)
        {
            ArtifactType = ArtifactType.Filepath,
            FileSharePath = fileSharePath,
        };
    }
}
