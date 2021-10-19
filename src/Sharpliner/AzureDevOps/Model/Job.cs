using System.Diagnostics.CodeAnalysis;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps
{
    /// <summary>
    /// A job is a collection of steps run by an agent or on a server.
    /// Jobs can run conditionally and might depend on earlier jobs.
    ///
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#job">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    public record Job : JobBase
    {
        private Conditioned<Strategy>? _strategy;

        /// <summary>
        /// Name of the job (A-Z, a-z, 0-9, and underscore).
        /// </summary>
        [YamlMember(Alias = "job", Order = 1, DefaultValuesHandling = DefaultValuesHandling.Preserve)]
        public string JobName => Name;

        /// <summary>
        /// Specifies how many jobs with which parameters should run
        /// Can be for example useful for defining a test matrix.
        /// </summary>
        [YamlMember(Order = 400)]
        [DisallowNull]
        public Conditioned<Strategy>? Strategy { get => _strategy; init => _strategy = value?.GetRoot(); }

        /// <summary>
        /// Any resources (repos or pools) required by this job that are not already referenced.
        /// </summary>
        [YamlMember(Order = 1200)]
        public Uses? Uses { get; init; }

        /// <summary>
        /// A job is a collection of steps run by an agent or on a server.
        /// </summary>
        /// <param name="name">Name of the job (A-Z, a-z, 0-9, and underscore)</param>
        /// <param name="displayName">Friendly name to display in the UI</param>
        public Job(string name, string? displayName = null) : base(name, displayName)
        {
        }
    }
}
