using System.Diagnostics.CodeAnalysis;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps
{
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
