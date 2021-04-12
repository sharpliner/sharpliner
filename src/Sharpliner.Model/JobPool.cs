using System;

namespace Sharpliner.Model
{
    public abstract record JobPool();

    public record JobPoolName : JobPool
    {
        public string Name { get; }

        internal JobPoolName(string name) : base()
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }

    public record HostedJobPool(
        string Name,
        string VmImage,
        params string[] Demands)
        : JobPool;
}
