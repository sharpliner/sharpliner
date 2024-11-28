using System;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks
{
    /// <summary>
    /// Represents the NuGetCommand@2 task for packing NuGet packages in Azure DevOps pipelines.
    /// </summary>
    /// <example>
    /// <code>
    /// var packTask = new NuGetPackCommandTask
    /// {
    ///     PackagesToPack = "**/*.csproj",
    ///     Arguments = "-Properties Configuration=Release"
    /// };
    /// </code>
    /// <para>The corresponding YAML will be:</para>
    /// <code>
    /// - task: NuGetCommand@2
    ///   inputs:
    ///     command: pack
    ///     packagesToPack: '**/*.csproj'
    ///     arguments: '-Properties Configuration=Release'
    /// </code>
    /// </example>
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
        public string PackagesToPack
        {
            get => GetString("packagesToPack")!;
            init => SetProperty("packagesToPack", value);
        }

        public VersioningScheme versioningScheme
        {
            get => GetString("versioningScheme") switch
            {
                "byPrereleaseNumber" => VersioningScheme.ByPrereleaseNumber,
                "byEnvVar" => VersioningScheme.ByEnvVar,
                "byBuildNumber" => VersioningScheme.ByBuildNumber,
                _ => throw new ArgumentOutOfRangeException()
            };
            init => SetProperty("versioningScheme", SharplinerSerializer.Serialize(value));
        }

        /// <summary>
        /// Specifies the configuration to package when using a csproj file.
        /// </summary>
        [YamlIgnore]
        public string? ConfigurationToPack
        {
            get => GetString("configurationToPack");
            init => SetProperty("configurationToPack", value);
        }

        /// <summary>
        /// Specifies the folder where the task creates packages. If the value is empty, the task creates packages at the source root.
        /// </summary>
        [YamlIgnore]
        public string? PackDestination
        {
            get => GetString("packDestination");
            init => SetProperty("packDestination", value);
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

    public enum VersioningScheme
    {
        ByPrereleaseNumber,

        ByEnvVar,

        ByBuildNumber
    }
}
