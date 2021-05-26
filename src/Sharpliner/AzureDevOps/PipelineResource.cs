using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#pipeline-resource
    /// </summary>
    public record PipelineResource
    {
        /// <summary>
        /// Identifier for the resource used in pipeline resource variables
        /// </summary>
        [YamlMember(Alias = "pipeline")]
        public string Identifier { get; }

        /// <summary>
        /// Project for the source
        /// Optional for current project
        /// </summary>
        [DisallowNull]
        public string? Project { get; init; }

        /// <summary>
        /// Name of the pipeline that produces an artifact
        /// </summary>
        [DisallowNull]
        public string? Source { get; init; }

        /// <summary>
        /// The pipeline run number to pick the artifact
        /// Defaults to latest pipeline successful across all stages
        /// Used only for manual or scheduled triggers
        /// </summary>
        [DisallowNull]
        public string? Version { get; init; }

        /// <summary>
        /// Branch to pick the artifact
        /// Optional, defaults to all branches
        /// Used only for manual or scheduled triggers
        /// </summary>
        [DisallowNull]
        public string? Branch { get; init; }

        /// <summary>
        /// List of tags required on the pipeline to pickup default artifacts
        /// Optional, used only for manual or scheduled triggers
        /// </summary>
        public List<string> Tags { get; init; } = new();

        /// <summary>
        /// Triggers are not enabled by default unless you add trigger section to the resource
        /// </summary>
        [DisallowNull]
        public PipelineTrigger? Trigger { get; init; }

        public PipelineResource(string identifier)
        {
            Identifier = identifier ?? throw new System.ArgumentNullException(nameof(identifier));
        }
    }
}
