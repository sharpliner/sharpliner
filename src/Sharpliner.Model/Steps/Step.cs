using System;
using System.Collections.Generic;

namespace Sharpliner.Model
{
    // https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#steps
    public abstract record Step
    {
        public string DisplayName { get; }

        public string Name { get; }

        public bool Enabled { get; init; }

        public bool ContinueOnError { get; init; }

        public string? Condition { get; init; }

        public int? TimeoutInMinutes { get; protected set; }

        /// <summary>
        /// A list of additional items to map into the process's environment.
        /// For example, secret variables are not automatically mapped.
        /// </summary>
        public IReadOnlyDictionary<string, string>? Env { get; init; }

        protected Step(string displayName, string name)
        {
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
