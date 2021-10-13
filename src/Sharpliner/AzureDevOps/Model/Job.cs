using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps
{
    /// <summary>
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#job">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    public record Job : IDependsOn
    {
        private static readonly Regex s_nameRegex = new("[a-zA-Z0-9_]+", RegexOptions.Compiled);

        private Conditioned<Pool>? _pool;
        private Conditioned<Strategy>? _strategy;
        private Conditioned<ContainerReference>? _container;

        /// <summary>
        /// Name of the job (A-Z, a-z, 0-9, and underscore).
        /// </summary>
        [YamlMember(Alias = "job", Order = 1, DefaultValuesHandling = DefaultValuesHandling.Preserve)]
        public string Name { get; }

        /// <summary>
        /// Friendly name to display in the UI
        /// </summary>
        [YamlMember(Order = 100)]
        [DisallowNull]
        public string? DisplayName { get; init; }

        /// <summary>
        /// List of names of other jobs this job depends on
        /// </summary>
        [YamlMember(Order = 200)]
        public List<string> DependsOn { get; init; } = new();

        /// <summary>
        /// Specifies which pool to use for a job of the pipeline
        /// A pool specification also holds information about the job's strategy for running.
        /// </summary>
        [YamlMember(Order = 300)]
        public Conditioned<Pool>? Pool { get => _pool; init => _pool = value?.GetRoot(); }

        /// <summary>
        /// Specifies how many jobs with which parameters should run
        /// Can be for example useful for defining a test matrix.
        /// </summary>
        [YamlMember(Order = 400)]
        [DisallowNull]
        public Conditioned<Strategy>? Strategy { get => _strategy; init => _strategy = value?.GetRoot(); }

        /// <summary>
        /// Container to run this job inside of.
        /// </summary>
        [YamlMember(Order = 500)]
        [DisallowNull]
        public Conditioned<ContainerReference>? Container { get => _container; init => _container = value?.GetRoot(); }

        /// <summary>
        /// Specifies variables at the job level
        /// You can add hard-coded values directly, reference variable groups, or insert via variable templates.
        /// </summary>
        [YamlMember(Order = 600)]
        public ConditionedList<VariableBase> Variables { get; init; } = new();

        /// <summary>
        /// A step is a linear sequence of operations that make up a job
        /// Each step runs in its own process on an agent and has access to the pipeline workspace on a local hard drive.
        /// This behavior means environment variables aren't preserved between steps but file system changes are.
        /// </summary>
        [YamlMember(Order = 700)]
        public ConditionedList<Step> Steps { get; init; } = new();

        /// <summary>
        /// How long to run the job before automatically cancelling
        /// </summary>
        [YamlIgnore]
        [DisallowNull]
        public TimeSpan? Timeout { get; init; }

        [YamlMember(Order = 800)]
        public int? TimeoutInMinutes => Timeout != null ? (int)Timeout.Value.TotalMinutes : null;

        /// <summary>
        /// How much time to give 'run always even if cancelled tasks' before killing them
        /// </summary>
        [YamlIgnore]
        [DisallowNull]
        public TimeSpan? CancelTimeout { get; init; }

        [YamlMember(Order = 900)]
        public int? CancelTimeoutInMinutes => CancelTimeout != null ? (int)CancelTimeout.Value.TotalMinutes : null;

        /// <summary>
        /// What to clean up before the job runs
        /// </summary>
        [YamlMember(Order = 1000)]
        [DefaultValue(JobWorkspace.Outputs)]
        public JobWorkspace Workspace { get; init; } = JobWorkspace.Outputs;

        /// <summary>
        /// Container resources to run as a service container.
        /// </summary>
        [YamlMember(Order = 1100)]
        public Dictionary<string, string> Services { get; } = new();

        /// <summary>
        /// Any resources (repos or pools) required by this job that are not already referenced.
        /// </summary>
        [YamlMember(Order = 1200)]
        public Uses? Uses { get; init; }

        [YamlMember(Order = 1100)]
        [DisallowNull]
        public string? Condition { get; init; }

        /// <summary>
        /// 'true' if future jobs should run even if this job fails
        /// defaults to 'false'
        /// </summary>
        [YamlMember(Order = 1200)]
        public bool ContinueOnError { get; init; } = false;

        public Job(string name, string? displayName = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

            if (!s_nameRegex.IsMatch(name))
            {
                throw new ArgumentOutOfRangeException(nameof(name));
            }

            if (displayName != null)
            {
                DisplayName = displayName;
            }
        }
    }
}
