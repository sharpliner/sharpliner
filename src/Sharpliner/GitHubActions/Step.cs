using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharpliner.GitHubActions
{
    public abstract record Step
    {
        protected Step(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof (id));
            Id = id;
        }
        /// <summary>
        /// Get the unique ID of the step.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Expression that will be evaluated to decide if the step will be executed.
        /// </summary>
        public Expression? If { get; set; }

        /// <summary>
        /// Get/Set the human readable name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Reference to an action to re-use.
        /// </summary>
        public ActionReference? Uses { get; set; }

        /// <summary>
        /// Working directory in which the script will be executed.
        /// </summary>
        public string? WorkingDirectory { get; set; }

        /// <summary>
        /// Get/Set the shell to be use to execute the string.
        /// </summary>
        public Shell Shell { get; set; } = Shell.Default;

        /// <summary>
        /// If Shell is set to Custom, allows to provide the shell to launch.
        /// </summary>
        public string? CustomShell { get; set; }

        /// <summary>
        /// Get/Set if the job should continue after a failure of the step. Defaults to false.
        /// </summary>
        public bool ContinueOnError { get; set; } = false;

        /// <summary>
        /// Get/Set the timeout in minutes. Can be null.
        /// </summary>
        public int? TimeoutMinutes { get; set; }

        /// <summary>
        /// A map of environment variables that are available to all steps of the jobs. When more than one variable
        /// with the same name is used, the latter one will be used.
        /// </summary>
        public Dictionary<string, string>? Variables { get; set; }

        /// <summary>
        /// Method to be override by subclass that will return the script to be executed.
        /// </summary>
        /// <returns></returns>
        internal abstract string GetScript();

    }

    public record InlineStep : Step
    {

        /// <summary>
        /// The contents of the script to execute.
        /// </summary>
        public List<string> Contents { get; init; } = new();

        internal override string GetScript() => string.Join(System.Environment.NewLine, Contents);

        public InlineStep(string id, params string[] scriptLines): base (id)
        {
            Contents = scriptLines.ToList();
        }

    }

}
