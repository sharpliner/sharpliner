using System;

namespace Sharpliner.Model.AzureDevOps.Tasks
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/utility/publish-pipeline-artifact?view=azure-devops
    /// </summary>
    public record PublishPipelineArtifactTask : Step
    {
        /// <summary>
        /// Path to the folder or file you want to publish.
        /// The path must be a fully qualified path or a valid path relative to the root directory of your repository.
        /// </summary>
        public string TargetPath { get; }

        /// <summary>
        /// Your artifact name. You can specify any name you prefer. E.g.: drop
        /// </summary>
        public string ArtifactName { get; init; } = "drop";

        /// <summary>
        /// Artifacts publish location. Choose whether to store the artifact in Azure Pipelines,
        /// or to copy it to a file share that must be accessible from the pipeline agent.
        /// </summary>
        public ArtifactType ArtifactType { get; init; } = ArtifactType.Pipeline;

        /// <summary>
        /// The file share to which the artifact files will be copied.
        /// This can include variables. Required when ArtifactType = FilePath.
        /// </summary>
        public string? FileSharePath { get; init; }

        /// <summary>
        /// Select whether to copy files in parallel using multiple threads for greater potential throughput.
        /// If this setting is not enabled, one thread will be used.
        /// </summary>
        public bool Parallel { get; init; }

        /// <summary>
        /// Enter the degree of parallelism, or number of threads used, to perform the copy.
        /// The value must be at least 1 and not greater than 128.
        /// </summary>
        public uint ParallelCount { get; init; } = 1;

        public PublishPipelineArtifactTask(string displayName, string name, string targetPath)
            : base(displayName, name)
        {
            TargetPath = targetPath ?? throw new ArgumentNullException(nameof(targetPath));
        }
    }

    public enum ArtifactType
    {
        Pipeline,
        Filepath,
    }
}
