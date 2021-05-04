using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.Model.AzureDevOps
{
    // https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#steps
    public abstract record Step
    {
        private string? _name;
        private static readonly Regex s_nameRegex = new("[a-zA-Z0-9_]+", RegexOptions.Compiled);

        /// <summary>
        /// Friendly name displayed in the UI.
        /// </summary>
        [YamlMember(Order = 100)]
        public string DisplayName { get; }

        /// <summary>
        /// Identifier for this step (A-Z, a-z, 0-9, and underscore).
        /// </summary>
        [YamlMember(Order = 150)]
        public string? Name
        {
            get => _name;
            init
            {
                if (value != null && !s_nameRegex.IsMatch(value))
                {
                    throw new ArgumentException(nameof(Name));
                }

                _name = value;
            }
        }

        /// <summary>
        /// Whether to run this step; defaults to 'true'.
        /// </summary>
        [YamlMember(Order = 175)]
        public bool Enabled { get; init; } = true;

        /// <summary>
        /// Condition that must be met to run this step.
        /// </summary>
        [YamlMember(Order = 190)]
        public string? Condition { get; init; }

        /// <summary>
        /// Whether future steps should run even if this step fails.
        /// Defaults to 'false'.
        /// </summary>
        [YamlMember(Order = 200)]
        public bool ContinueOnError { get; init; } = false;

        /// <summary>
        /// Timeout after which the step will be stopped.
        /// </summary>
        [YamlIgnore]
        public TimeSpan? Timeout { get; init; }

        /// <summary>
        /// Timeout after which the step will be stopped.
        /// </summary>
        [YamlMember(Order = 210)]
        public int? TimeoutInMinutes => Timeout != null ? (int)Timeout.Value.TotalMinutes : null;

        /// <summary>
        /// A list of additional items to map into the process's environment.
        /// For example, secret variables are not automatically mapped.
        /// </summary>
        [YamlMember(Order = 220)]
        public IReadOnlyDictionary<string, string>? Env { get; init; }

        protected Step(string displayName)
        {
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }

        /// <summary>
        /// Make step only run when previous steps succeeded.
        /// </summary>
        /// <returns></returns>
        public Step WhenSucceeded() => this with
        {
            Condition = "succeeded()"
        };

        public static implicit operator ConditionedDefinition<Step>(Step step) => new(step);
    }
}
