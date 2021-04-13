using System;
using System.Collections.Generic;

namespace Sharpliner.Model
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
        public string? ArtifactName { get; }

        /// <summary>
        /// Artifacts publish location. Choose whether to store the artifact in Azure Pipelines,
        /// or to copy it to a file share that must be accessible from the pipeline agent.
        /// </summary>
        public ArtifactType ArtifactType { get; }

        /// <summary>
        /// The file share to which the artifact files will be copied.
        /// This can include variables. Required when ArtifactType = FilePath.
        /// </summary>
        public string? FileSharePath { get; }

        /// <summary>
        /// Select whether to copy files in parallel using multiple threads for greater potential throughput.
        /// If this setting is not enabled, one thread will be used.
        /// </summary>
        public bool Parallel { get; }

        /// <summary>
        /// Enter the degree of parallelism, or number of threads used, to perform the copy.
        /// The value must be at least 1 and not greater than 128.
        /// </summary>
        public uint ParallelCount { get; }

        public PublishPipelineArtifactTask(
            string displayName,
            string name,
            string targetPath,
            string? artifactName = "drop",
            ArtifactType artifactType = ArtifactType.Pipeline,
            string? fileSharePath = null,
            bool parallel = false,
            uint parallelCount = 1,
            bool enabled = true,
            bool continueOnError = false,
            TimeSpan? timeout = null,
            string? condition = null,
            IReadOnlyDictionary<string, string>? environmentVariables = null)
            : base(displayName, name, enabled, continueOnError, timeout, condition, environmentVariables)
        {
            TargetPath = targetPath ?? throw new ArgumentNullException(nameof(targetPath));
            ArtifactName = artifactName;
            ArtifactType = artifactType;
            FileSharePath = fileSharePath;
            Parallel = parallel;
            ParallelCount = parallelCount;
        }
    }

    public enum ArtifactType
    {
        Pipeline,
        Filepath,
    }
}
