using System;
using System.Collections.Generic;

namespace Sharpliner.Model.AzureDevOps
{
    // https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#steps
    public abstract record Step
    {
        private string? _name;

        /// <summary>
        /// Friendly name displayed in the UI.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Identifier for this step (A-Z, a-z, 0-9, and underscore).
        /// </summary>
        public string? Name
        {
            get => _name;
            init => _name = value; // TODO: Validate format
        }

        /// <summary>
        /// Whether to run this step; defaults to 'true'.
        /// </summary>
        public bool Enabled { get; init; } = true;

        /// <summary>
        /// Whether future steps should run even if this step fails.
        /// Defaults to 'false'.
        /// </summary>
        public bool ContinueOnError { get; init; } = false;

        /// <summary>
        /// Condition that must be met to run this step.
        /// </summary>
        public string? Condition { get; init; }

        /// <summary>
        /// Timeout after which the step will be stopped.
        /// </summary>
        public TimeSpan? Timeout { get; protected set; }

        /// <summary>
        /// A list of additional items to map into the process's environment.
        /// For example, secret variables are not automatically mapped.
        /// </summary>
        public IReadOnlyDictionary<string, string>? Env { get; init; }

        protected Step(string displayName)
        {
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }
    }
}
