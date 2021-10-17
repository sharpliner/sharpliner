using System.Diagnostics.CodeAnalysis;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps
{
    /// <summary>
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#pool">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    public record Pool
    {
        /// <summary>
        /// Identifier for this step (A-Z, a-z, 0-9, and underscore).
        /// </summary>
        [YamlMember(Order = 100)]
        public string? Name { get; init; }

        public Pool(string? name)
        {
            Name = name;
        }
    }

    public record HostedPool : Pool
    {
        public HostedPool(string? name = null, string? vmImage = null) : base(name)
        {
            VmImage = vmImage;
        }

        /// <summary>
        /// Identifier for this step (A-Z, a-z, 0-9, and underscore).
        /// </summary>
        [YamlMember(Order = 105)]
        public string? VmImage { get; init; }

        [YamlMember(Order = 110)]
        [DisallowNull]
        public ConditionedList<string> Demands { get; init; } = new();
    }
}
