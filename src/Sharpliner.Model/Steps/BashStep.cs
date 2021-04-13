using System;
using System.Collections.Generic;

namespace Sharpliner.Model
{
    public record BashStep : ContentStep
    {
        public BashStep(
            string displayName,
            string name,
            string contents,
            bool enabled = true,
            bool continueOnError = false,
            TimeSpan? timeout = null,
            string? condition = null,
            IReadOnlyDictionary<string, string>? environmentVariables = null) : base(displayName, name, contents, enabled, continueOnError, timeout, condition, environmentVariables)
        {
        }
    }
}
