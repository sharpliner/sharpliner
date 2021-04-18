using System;
using System.Collections.Generic;

namespace Sharpliner.Model.AzureDevOps
{
    public abstract record Pool();

    public record PoolName : Pool
    {
        public string Name { get; }

        internal PoolName(string name) : base()
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }

    public record HostedPool(string Name, string VmImage) : Pool
    {
        public List<string> Demands { get; init; } = new();
    }
}
