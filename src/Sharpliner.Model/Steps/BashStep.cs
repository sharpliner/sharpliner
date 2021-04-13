using System;
using System.Collections.Generic;

namespace Sharpliner.Model
{
    public abstract record BashStep : Step
    {
        /// <summary>
        /// Specify the working directory in which you want to run the command.
        /// If you leave it empty, the working directory is $(Build.SourcesDirectory).
        /// </summary>
        public string? WorkingDirectory { get; }

        /// <summary>
        /// If this is true, this task will fail if any errors are written to stderr.
        /// Default value: `false`.
        /// </summary>
        public bool FailOnStderr { get; } = false;

        /// <summary>
        /// Don't load the system-wide startup file **`/etc/profile`** or any of the personal initialization files.
        /// </summary>
        public bool NoProfile { get; } = false;

        /// <summary>
        /// If this is true, the task will not process `.bashrc` from the user's home directory.
        /// Default value: `true`.
        /// </summary>
        public bool NoRc { get; } = true;

        public BashStep(
            string displayName,
            string name,
            string? workingDirectory = null,
            bool failOnStderr = false,
            bool noProfile = false,
            bool noRc = false,
            bool enabled = true,
            bool continueOnError = false,
            TimeSpan? timeout = null,
            string? condition = null,
            IReadOnlyDictionary<string, string>? environmentVariables = null)
            : base(displayName, name, enabled, continueOnError, timeout, condition, environmentVariables)
        {
            WorkingDirectory = workingDirectory;
            FailOnStderr = failOnStderr;
            NoProfile = noProfile;
            NoRc = noRc;
        }
    }

    public record InlineBashStep : BashStep
    {
        /// <summary>
        /// Required if Type is inline, contents of the script.
        /// </summary>
        public string Contents { get; }

        public InlineBashStep(
            string displayName,
            string name,
            string scriptContents,
            string? workingDirectory = null,
            bool failOnStderr = false,
            bool noProfile = false,
            bool noRc = false,
            bool enabled = true,
            bool continueOnError = false,
            TimeSpan? timeout = null,
            string? condition = null,
            IReadOnlyDictionary<string, string>? environmentVariables = null)
            : base(displayName, name, workingDirectory, failOnStderr, noProfile, noRc, enabled, continueOnError, timeout, condition, environmentVariables)
        {
            Contents = scriptContents;
        }
    }

    public record BashFileStep : BashStep
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

        public BashFileStep(
            string displayName,
            string name,
            string filePath,
            string? arguments = null,
            string? workingDirectory = null,
            bool failOnStderr = false,
            bool noProfile = false,
            bool noRc = false,
            bool enabled = true,
            bool continueOnError = false,
            TimeSpan? timeout = null,
            string? condition = null,
            IReadOnlyDictionary<string, string>? environmentVariables = null) : base(displayName, name, workingDirectory, failOnStderr, noProfile, noRc, enabled, continueOnError, timeout, condition, environmentVariables)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            Arguments = arguments;
        }
    }
}
