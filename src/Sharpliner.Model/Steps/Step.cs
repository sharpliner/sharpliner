using System;
using System.Collections.Generic;

namespace Sharpliner.Model
{
    // https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#steps
    public abstract record Step
    {
        public string DisplayName { get; }
        public string Name { get; }
        public bool Enabled { get; }
        public bool ContinueOnError { get; }
        public string? Condition { get; }
        public IReadOnlyDictionary<string, string>? EnvironmentVariables { get; }
        public int? TimeoutInMinutes { get; }

        public Step(
            string displayName,
            string name,
            bool enabled = true,
            bool continueOnError = false,
            TimeSpan? timeout = null,
            string? condition = null,
            IReadOnlyDictionary<string, string>? environmentVariables = null)
        {
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Enabled = enabled;
            ContinueOnError = continueOnError;
            TimeoutInMinutes = timeout.HasValue ? (int)timeout.Value.TotalMinutes : null;
            Condition = condition;
            EnvironmentVariables = environmentVariables;
        }
    }
}
