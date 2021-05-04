using System;
using System.Collections.Generic;

namespace Sharpliner.Model.AzureDevOps
{
    public abstract record Pool
    {
        public static implicit operator ConditionedDefinition<Pool>(Pool pool) => new(pool);
    }

    public record PoolName : Pool
    {
        public string Name { get; }

        public PoolName(string name) : base()
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }

    public record HostedPool : Pool
    {
        public string? Name { get; }

        public string VmImage { get; }

        public List<string> Demands { get; init; } = new();

        public HostedPool(string vmImage)
        {
            VmImage = vmImage ?? throw new ArgumentNullException(nameof(vmImage));
        }

        public HostedPool(string name, string vmImage)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            VmImage = vmImage ?? throw new ArgumentNullException(nameof(vmImage));
        }
    }
}
