using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks
{
    /// <summary>
    /// Represents the NuGetCommand@2 task for packing NuGet packages in Azure DevOps pipelines.
    /// </summary>
    public record NuGetPackCommandTask : NuGetCommandTask
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NuGetPackCommandTask"/> class.
        /// </summary>
        public NuGetPackCommandTask() : base("pack")
        {
        }

        /// <summary>
        /// Gets or sets the pattern to search for csproj or nuspec files to pack.
        /// </summary>
        [YamlIgnore]
        public string? PackagesToPack
        {
            get => GetString("packagesToPack");
            init => SetProperty("packagesToPack", value);
        }

        /// <summary>
        /// Gets or sets the additional arguments for the pack command.
        /// </summary>
        [YamlIgnore]
        public string? Arguments
        {
            get => GetString("arguments");
            init => SetProperty("arguments", value);
        }
    }
}
