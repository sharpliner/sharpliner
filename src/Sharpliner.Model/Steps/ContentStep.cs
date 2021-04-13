using System;
using System.Collections.Generic;

namespace Sharpliner.Model
{
    public abstract record ContentStep : Step
    {
        public string Contents { get; }

        public ContentStep(
            string displayName,
            string name,
            string contents,
            bool enabled = true,
            bool continueOnError = false,
            TimeSpan? timeout = null,
            string? condition = null,
            IReadOnlyDictionary<string, string>? environmentVariables = null) : base(displayName, name, enabled, continueOnError, timeout, condition, environmentVariables)
        {
            Contents = contents ?? throw new ArgumentNullException(nameof(contents));
        }
    }
}
