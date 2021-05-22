using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/utility/download-pipeline-artifact?view=azure-devops
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
        /// https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/file-matching-patterns?view=azure-devops
        /// </summary>
        [YamlIgnore]
        public List<string> Patterns { get; init; } = new();

        [YamlMember(Alias = "patterns", Order = 61, ScalarStyle = YamlDotNet.Core.ScalarStyle.Literal)]
        public string _Patterns => string.Join("\n", Patterns);

        /// <summary>
        /// Directory to download the artifact files. Can be relative to the pipeline workspace directory or absolute.
        /// If multi-download option is applied (by leaving an empty artifact name), a sub-directory will be created for each.
        /// Default value: $(Pipeline.Workspace)
        /// https://docs.microsoft.com/en-us/azure/devops/pipelines/artifacts/pipeline-artifacts?view=azure-devops
        /// </summary>
        [YamlMember(Order = 62)]
        [DefaultValue("$(Pipeline.Workspace)")]
        public string Path { get; init; } = "$(Pipeline.Workspace)";

        /// <summary>
        /// A boolean specifying whether to download artifacts from a triggering build.
        /// Defaults to false.
        /// </summary>
        [YamlMember(Order = 100)]
        [DefaultValue(false)]
        public bool PreferTriggeringPipeline { get; init; } = false;

        /// <summary>
        /// A list of tags. Only builds with these tags will be returned.
        /// </summary>
        [YamlIgnore]
        public List<string> Tags { get; init; } = new();

        [YamlMember(Alias = "tags", Order = 104)]
        public string? _Tags => Tags.Any() ? string.Join(",", Tags) : null;

        /// <summary>
        /// If checked, this build task will try to download artifacts whether the build is succeeded or partially succeeded.
        /// Defaults to false.
        /// </summary>
        [YamlMember(Order = 105)]
        [DefaultValue(false)]
        public bool AllowPartiallySucceededBuilds { get; init; } = false;

        /// <summary>
        /// If checked, this build task will try to download artifacts whether the build is succeeded or failed.
        /// Defaults to false.
        /// </summary>
        [YamlMember(Order = 106)]
        [DefaultValue(false)]
        public bool AllowFailedBuilds { get; init; } = false;

        public DownloadTask(string? displayName = null) : base(displayName)
        {
        }
    }

    public record CurrentDownloadTask : DownloadTask
    {
        public override string Download => "current";

        public CurrentDownloadTask(string? displayName = null) : base(displayName)
        {
        }
    }

    public record NoneDownloadTask : Step
    {
        [YamlMember(Order = 1)]
        public string Download => "none";

        public NoneDownloadTask(string? displayName = null) : base(displayName)
        {
        }
    }

    public record SpecificDownloadTask : DownloadTask
    {
        private readonly string _pipelineResourceIdentifier;

        public override string Download => _pipelineResourceIdentifier;

        /// <summary>
        /// The project GUID from which to download the pipeline artifacts.
        /// </summary>
        [YamlMember(Order = 64)]
        public string? Project { get; init; }

        /// <summary>
        /// Specifies which build version to download.
        /// </summary>
        [YamlIgnore]
        public RunVersion RunVersion { get; init; }

        [YamlMember(Alias = "runVersion", Order = 65)]
        public string _RunVersion => RunVersion switch
        {
            RunVersion.LatestFromBranch => "latestFromBranch",
            RunVersion.Specific => "specific",
            _ => "latest",
        };

        /// <summary>
        /// Specify to filter on branch/ref name.
        /// For example: refs/heads/develop
        /// </summary>
        [YamlMember(Alias = "runBranch", Order = 66)]
        [DefaultValue("refs/heads/master")]
        public string BranchName { get; init; } = "refs/heads/master";

        /// <summary>
        /// The build from which to download the artifacts.
        /// For example: 1764
        /// </summary>
        [YamlMember(Alias = "runId", Order = 67)]
        public int PipelineId { get; init; }

        public SpecificDownloadTask(string pipelineResourceIdentifier) : base((string?)null)
        {
            _pipelineResourceIdentifier = pipelineResourceIdentifier;
        }
    }

    public enum RunVersion
    {
        Latest,
        LatestFromBranch,
        Specific,
    }
}
