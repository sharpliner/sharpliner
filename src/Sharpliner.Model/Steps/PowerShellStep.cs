using System;
using System.Collections.Generic;

namespace Sharpliner.Model
{
    public record PowerShellStep : ContentStep
    {
        public PowerShellStep(
            string displayName,
            string name,
            string contents,
            bool enabled = true,
            ErrorActionPreference errorActionPreference = ErrorActionPreference.Continue,
            bool continueOnError = false,
            TimeSpan? timeout = null,
            string? condition = null,
            IReadOnlyDictionary<string, string>? environmentVariables = null) : base(displayName, name, contents, enabled, continueOnError, timeout, condition, environmentVariables)
        {
        }
    }

    public enum ErrorActionPreference
    {
        /// <summary>
        /// (Default) Displays the error message and continues executing.
        /// </summary>
        Continue,

        /// <summary>
        /// Enter the debugger when an error occurs or when an exception is raised.
        /// </summary>
        Break,

        /// <summary>
        /// Suppresses the error message and continues to execute the command. The Ignore value is intended for per-command use, not for use as saved preference. Ignore isn't a valid value for the $ErrorActionPreference variable.
        /// </summary>
        Ignore,

        /// <summary>
        /// Displays the error message and asks you whether you want to continue.
        /// </summary>
        Inquire,

        /// <summary>
        /// No effect. The error message isn't displayed and execution continues without interruption.
        /// </summary>
        SilentlyContinue,

        /// <summary>
        /// Displays the error message and stops executing. In addition to the error generated, the Stop value generates an ActionPreferenceStopException object to the error stream. stream
        /// </summary>
        Stop,

        /// <summary>
        /// Automatically suspends a workflow job to allow for further investigation. After investigation, the workflow can be resumed. The Suspend value is intended for per-command use, not for use as saved preference. Suspend isn't a valid value for the $ErrorActionPreference variable.
        /// </summary>
        Suspend,
    }
}
