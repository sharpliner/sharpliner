using System.Diagnostics.CodeAnalysis;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps
{
    /// <summary>
    /// A deployment job is a special type of job.
    /// It's a collection of steps to run sequentially against the environment.
    /// In YAML pipelines, we recommend that you put your deployment steps in a deployment job.
    ///
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/process/deployment-jobs?view=azure-devops">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    public record DeploymentJob : JobBase
    {
        private Conditioned<DeploymentStrategy>? _strategy;

        /// <summary>
        /// Name of the job (A-Z, a-z, 0-9, and underscore).
        /// </summary>
        [YamlMember(Alias = "deployment", Order = 1, DefaultValuesHandling = DefaultValuesHandling.Preserve)]
        public string JobName => Name;

        /// <summary>
        /// Specifies how many jobs with which parameters should run
        /// Can be for example useful for defining a test matrix.
        /// </summary>
        [YamlMember(Order = 1300)]
        [DisallowNull]
        public Conditioned<DeploymentStrategy>? Strategy { get => _strategy; init => _strategy = value?.GetRoot(); }

        /// <summary>
        /// A deployment job is a special type of job.
        /// It's a collection of steps to run sequentially against the environment.
        /// </summary>
        /// <param name="name">Name of the job (A-Z, a-z, 0-9, and underscore)</param>
        /// <param name="displayName">Friendly name to display in the UI</param>
        public DeploymentJob(string name, string? displayName = null) : base(name, displayName)
        {
        }
    }
}
