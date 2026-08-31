using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating a publish task using the <c>publish</c> keyword or the <c>PublishPipelineArtifact@1</c> task.
/// </summary>
public class PublishTaskBuilder
{
    /// <summary>
    /// <para>
    /// Creates a <c>UniversalPackages@1</c> task that publishes a specific package version.
    /// </para>
    /// <para>
    /// For example:
    /// </para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Publish.UniversalPackage("MyProject/my-feed", "tooling-assets", "2.3.4") with
    ///     {
    ///         Directory = "$(Build.ArtifactStagingDirectory)",
    ///         PackageDescription = "Release-ready assets"
    ///     }
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: UniversalPackages@1
    ///   inputs:
    ///     command: publish
    ///     feed: MyProject/my-feed
    ///     packageName: tooling-assets
    ///     packageVersion: 2.3.4
    ///     directory: $(Build.ArtifactStagingDirectory)
    ///     packageDescription: Release-ready assets
    /// </code>
    /// More details can be found in the
    /// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/universal-packages-v1">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    /// <param name="feed">Feed name, or <c>project/feed</c> for project-scoped feeds.</param>
    /// <param name="packageName">Universal Package name.</param>
    /// <param name="packageVersion">Package version to publish.</param>
    public UniversalPackagesV1PublishTask UniversalPackage(
        string feed,
        string packageName,
        string packageVersion) => new(feed, packageName, packageVersion);

    /// <summary>
    /// <para>
    /// Creates a <c>UniversalPackages@1</c> task that publishes a package using automatic version incrementing.
    /// </para>
    /// <para>
    /// For example:
    /// </para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Publish.UniversalPackage("MyProject/my-feed", "tooling-assets", UniversalPackagesV1VersionIncrement.Patch) with
    ///     {
    ///         Directory = "$(Build.ArtifactStagingDirectory)"
    ///     }
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: UniversalPackages@1
    ///   inputs:
    ///     command: publish
    ///     feed: MyProject/my-feed
    ///     packageName: tooling-assets
    ///     versionIncrement: patch
    ///     directory: $(Build.ArtifactStagingDirectory)
    /// </code>
    /// </summary>
    /// <param name="feed">Feed name, or <c>project/feed</c> for project-scoped feeds.</param>
    /// <param name="packageName">Universal Package name.</param>
    /// <param name="versionIncrement">Version component to increment automatically.</param>
    public UniversalPackagesV1PublishTask UniversalPackage(
        string feed,
        string packageName,
        AdoExpression<UniversalPackagesV1VersionIncrement> versionIncrement) => new(feed, packageName, versionIncrement);

    /// <summary>
    /// <para>
    /// Creates a publish step that stores the artifact in Azure Pipelines.
    /// </para>
    /// This uses the <c>publish</c> keyword which is a shortcut for the <c>PublishPipelineArtifact@1</c> task.
    /// The shortcut only supports the <c>publish</c> and <c>artifact</c> properties, use
    /// <see cref="PipelineArtifact(AdoExpression{string}, AdoExpression{string})"/> when you need the other task inputs.
    /// <para>
    /// For example:
    /// </para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Publish.Pipeline("Binary", "bin/Debug/net8.0/") with
    ///     {
    ///         DisplayName = "Publish artifact",
    ///         ContinueOnError = false,
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
    /// </summary>
    /// <param name="artifactName">Artifact name</param>
    /// <param name="targetPath">Path to the folder or file you want to publish</param>
    /// <returns>A new instance of <see cref="PublishTask"/> with the specified parameters</returns>
    public PublishTask Pipeline(AdoExpression<string> artifactName, AdoExpression<string> targetPath)
    {
        return new PublishTask(targetPath, artifactName);
    }

    /// <summary>
    /// <para>
    /// Creates a <c>PublishPipelineArtifact@1</c> task with <see cref="PublishPipelineArtifactTask.ArtifactType"/>
    /// set to <see cref="ArtifactType.Pipeline"/>.
    /// </para>
    /// Compared to <see cref="Pipeline(AdoExpression{string}, AdoExpression{string})"/>, this form allows setting
    /// all of the task inputs such as <see cref="PublishPipelineArtifactTask.Properties"/>.
    /// <para>
    /// For example:
    /// </para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Publish.PipelineArtifact("Binary", "bin/Debug/net8.0/") with
    ///     {
    ///         Properties = "{\"user-type\":\"binary\"}",
    ///     }
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// steps:
    /// - task: PublishPipelineArtifact@1
    ///   inputs:
    ///     targetPath: bin/Debug/net8.0/
    ///     artifact: Binary
    ///     artifactType: pipeline
    ///     properties: '{"user-type":"binary"}'
    /// </code>
    /// </summary>
    /// <param name="artifactName">Artifact name</param>
    /// <param name="targetPath">Path to the folder or file you want to publish</param>
    /// <returns>A new instance of <see cref="PublishPipelineArtifactTask"/> with the specified parameters</returns>
    public PublishPipelineArtifactTask PipelineArtifact(AdoExpression<string> artifactName, AdoExpression<string> targetPath)
    {
        return new PublishPipelineArtifactTask(targetPath, artifactName)
        {
            ArtifactType = ArtifactType.Pipeline,
        };
    }

    /// <summary>
    /// <para>
    /// Creates a <c>PublishPipelineArtifact@1</c> task with <see cref="PublishPipelineArtifactTask.ArtifactType"/>
    /// set to <see cref="ArtifactType.Filepath"/> which copies the artifact to a file share.
    /// </para>
    /// The <c>publish</c> keyword shortcut does not support publishing to a file share so the full task is used.
    /// Publishing artifacts from a Linux or macOS agent to a file share is not supported.
    /// <para>
    /// For example:
    /// </para>
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
    /// - task: PublishPipelineArtifact@1
    ///   inputs:
    ///     targetPath: bin/Debug/netstandard2.0/
    ///     artifact: additional-binary
    ///     artifactType: filepath
    ///     fileSharePath: $(Build.ArtifactStagingDirectory)/additional-binary
    ///     parallel: true
    /// </code>
    /// </summary>
    /// <param name="artifactName">Artifact name</param>
    /// <param name="targetPath">Path to the folder or file you want to publish</param>
    /// <param name="fileSharePath">Path to the file share.</param>
    /// <returns>A new instance of <see cref="PublishPipelineArtifactTask"/> with the specified parameters</returns>
    public PublishPipelineArtifactTask FileShare(AdoExpression<string> artifactName, AdoExpression<string> targetPath, AdoExpression<string> fileSharePath)
    {
        return new PublishPipelineArtifactTask(targetPath, artifactName)
        {
            ArtifactType = ArtifactType.Filepath,
            FileSharePath = fileSharePath,
        };
    }
}
