using System;
using System.Collections.Generic;

namespace Sharpliner.Model
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

    public record HostedPool(
        string Name,
        string VmImage,
        IEnumerable<string>? Demands = null)
        : Pool;
}
