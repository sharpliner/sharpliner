using System;
using System.Collections.Generic;

namespace Sharpliner.AzureDevOps
{
    public record Pool(string? Name)
    {
        public static implicit operator ConditionedDefinition<Pool>(Pool pool) => new(pool);
    }

    public record HostedPool : Pool
    {
        public string VmImage { get; }

        public List<string> Demands { get; init; } = new();

        public HostedPool(string vmImage) : this(null!, vmImage)
        {
        }

        public HostedPool(string name, string vmImage) : base(name)
        {
            VmImage = vmImage ?? throw new ArgumentNullException(nameof(vmImage));
        }
    }
}
