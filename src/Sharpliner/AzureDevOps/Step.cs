﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps
{
    /// <summary>
    /// A step is a linear sequence of operations that make up a job.
    /// Each step runs in its own process on an agent and has access to the pipeline workspace on a local hard drive.
    /// This behavior means environment variables aren't preserved between steps but file system changes are.
    /// https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#steps
    /// </summary>
    public abstract record Step
    {
        private string? _name;
        private static readonly Regex s_nameRegex = new("[a-zA-Z0-9_]+", RegexOptions.Compiled);

        /// <summary>
        /// Friendly name displayed in the UI.
        /// </summary>
        [YamlMember(Order = 100)]
        [DisallowNull]
        public string? DisplayName { get; init; }

        /// <summary>
        /// Identifier for this step (A-Z, a-z, 0-9, and underscore).
        /// </summary>
        [YamlMember(Order = 150)]
        [DisallowNull]
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
        [DefaultValue(true)]
        public bool Enabled { get; init; } = true;

        /// <summary>
        /// Condition that must be met to run this step.
        /// </summary>
        [YamlMember(Order = 190)]
        [DisallowNull]
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
        [DisallowNull]
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
        [DisallowNull]
        public IReadOnlyDictionary<string, string>? Env { get; init; }

        /// <summary>
        /// Make step only run when a condition is met.
        /// </summary>
        public Step When(string condition) => this with
        {
            Condition = condition
        };

        /// <summary>
        /// Make step only run when previous steps succeeded ('succeeded()').
        /// </summary>
        public Step WhenSucceeded() => When("succeeded()");

        /// <summary>
        /// Make step run even when previous steps failed ('succeededOrFailed()').
        /// </summary>
        public Step WhenSucceededOrFailed() => When("succeededOrFailed()");

        public static implicit operator ConditionedDefinition<Step>(Step step) => new(step);
    }
}
