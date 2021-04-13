using System;
using System.Collections.Generic;

namespace Sharpliner.Model
{
    public abstract record PowerShellStep : Step
    {
        /// <summary>
        /// Specify the working directory in which you want to run the command.
        /// If you leave it empty, the working directory is $(Build.SourcesDirectory).
        /// </summary>
        public string? WorkingDirectory { get; }

        /// <summary>
        /// Prepends the line $ErrorActionPreference = 'VALUE' at the top of your script.
        /// Default value: `stop`.
        /// </summary>
        public ErrorActionPreference ErrorActionPreference { get; } = ErrorActionPreference.Stop;

        /// <summary>
        /// If this is true, this task will fail if any errors are written to the error pipeline, or if any data is written to the Standard Error stream.
        /// Otherwise the task will rely on the exit code to determine failure
        /// Default value: `false`.
        /// </summary>
        public bool FailOnStderr { get; } = false;

        /// <summary>
        /// If this is false, the line if ((Test-Path -LiteralPath variable:\\LASTEXITCODE)) { exit $LASTEXITCODE } is appended to the end of your script.
        /// This will cause the last exit code from an external command to be propagated as the exit code of powershell.
        /// Otherwise the line is not appended to the end of your script
        /// Default value: `false`.
        /// </summary>
        public bool IgnoreLASTEXITCODE { get; } = false;

        /// <summary>
        /// If this is true, then on Windows the task will use pwsh.exe from your PATH instead of powershell.exe.
        /// Default value: `false`.
        /// </summary>
        public bool Pwsh { get; } = false;

        public PowerShellStep(
            string displayName,
            string name,
            string? workingDirectory = null,
            ErrorActionPreference errorActionPreference = ErrorActionPreference.Continue,
            bool failOnStderr = false,
            bool ignoreLASTEXITCODE = false,
            bool pwsh = false,
            bool enabled = true,
            bool continueOnError = false,
            TimeSpan? timeout = null,
            string? condition = null,
            IReadOnlyDictionary<string, string>? environmentVariables = null)
            : base(displayName, name, enabled, continueOnError, timeout, condition, environmentVariables)
        {
            WorkingDirectory = workingDirectory;
            ErrorActionPreference = errorActionPreference;
            FailOnStderr = failOnStderr;
            IgnoreLASTEXITCODE = ignoreLASTEXITCODE;
            Pwsh = pwsh;
        }
    }

    public record InlinePowerShellStep : PowerShellStep
    {
        /// <summary>
        /// Required if Type is inline, contents of the script.
        /// </summary>
        public string Contents { get; }

        public InlinePowerShellStep(
            string displayName,
            string name,
            string scriptContents,
            string? workingDirectory = null,
            ErrorActionPreference errorActionPreference = ErrorActionPreference.Continue,
            bool failOnStderr = false,
            bool ignoreLASTEXITCODE = false,
            bool pwsh = false,
            bool enabled = true,
            bool continueOnError = false,
            TimeSpan? timeout = null,
            string? condition = null,
            IReadOnlyDictionary<string, string>? environmentVariables = null)
            : base(displayName, name, workingDirectory, errorActionPreference, failOnStderr, ignoreLASTEXITCODE, pwsh, enabled, continueOnError, timeout, condition, environmentVariables)
        {
            Contents = scriptContents;
        }
    }

    public record PowerShellFileStep : PowerShellStep
    {
        /// <summary>
        /// Path of the script to execute.
        /// Must be a fully qualified path or relative to $(System.DefaultWorkingDirectory).
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Arguments passed to the Bash script.
        /// </summary>
        public string? Arguments { get; }

        public PowerShellFileStep(
            string displayName,
            string name,
            string filePath,
            string? arguments = null,
            string? workingDirectory = null,
            ErrorActionPreference errorActionPreference = ErrorActionPreference.Continue,
            bool failOnStderr = false,
            bool ignoreLASTEXITCODE = false,
            bool pwsh = false,
            bool enabled = true,
            bool continueOnError = false,
            TimeSpan? timeout = null,
            string? condition = null,
            IReadOnlyDictionary<string, string>? environmentVariables = null) : base(displayName, name, workingDirectory, errorActionPreference, failOnStderr, ignoreLASTEXITCODE, pwsh, enabled, continueOnError, timeout, condition, environmentVariables)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            Arguments = arguments;
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
