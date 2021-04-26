﻿using System;
using System.Linq;

namespace Sharpliner.Model.AzureDevOps.Tasks
{
    public abstract record BashTask : Step
    {
        /// <summary>
        /// Specify the working directory in which you want to run the command.
        /// If you leave it empty, the working directory is $(Build.SourcesDirectory).
        /// </summary>
        public string? WorkingDirectory { get; init; }

        /// <summary>
        /// If this is true, this task will fail if any errors are written to stderr.
        /// Default value: `false`.
        /// </summary>
        public bool FailOnStderr { get; init; } = false;

        /// <summary>
        /// Don't load the system-wide startup file **`/etc/profile`** or any of the personal initialization files.
        /// </summary>
        public bool NoProfile { get; init; } = false;

        /// <summary>
        /// If this is true, the task will not process `.bashrc` from the user's home directory.
        /// Default value: `true`.
        /// </summary>
        public bool NoRc { get; init; } = true;

        protected BashTask(string displayName)
            : base(displayName)
        {
        }
    }

    public record InlineBashTask : BashTask
    {
        /// <summary>
        /// Required if Type is inline, contents of the script.
        /// </summary>
        public string Contents { get; }

        public InlineBashTask(string displayName, params string[] scriptLines)
            : base(displayName)
        {
            if (scriptLines is null)
            {
                throw new ArgumentNullException(nameof(scriptLines));
            }

            Contents = string.Join("\n", scriptLines);
        }
    }

    public record BashFileTask : BashTask
    {
        /// <summary>
        /// Path of the script to execute.
        /// Must be a fully qualified path or relative to $(System.DefaultWorkingDirectory).
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Arguments passed to the Bash script.
        /// </summary>
        public string? Arguments { get; init; }

        public BashFileTask(string displayName, string filePath)
            : base(displayName)
        {
            FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        }
    }
}