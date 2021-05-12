using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Sharpliner.GitHubActions
{
    /// <summary>
    /// A workflow represents and automatic process in GitHub that have one more more steps.
    /// </summary>
    public record Workflow
    {
        /// <summary>
        /// Contains all the different triggers that have been configured to launch a workflow when a GitHub event
        /// occurs.
        ///
        /// There are three main different types of triggers:
        ///
        /// <list type="bullet">
        ///     <item>
        ///         <term>Manuals</term>
        ///         <description>All those triggers that required manual intervention.</description>
        ///     </item>
        ///     <item>
        ///         <term>Schedules</term>
        ///         <description>All those triggers that will launch a workflow based on a schedule.</description>
        ///     </item>
        ///     <item>
        ///         <term>Webhooks</term>
        ///         <description>All those triggers that will launch a workflow when a webhook event is generated
        ///             from GitHub
        ///         </description>
        ///     </item>
        /// </list>
        ///
        /// </summary>
        public Trigger On { get; } = new ();

        /// <summary>
        /// Allows to set the permissions granted to the Github token that will be used with the workflow. This
        /// setting will apply to all the jobs in a workflow. You can override this setting per job.
        /// </summary>
        public Permissions Permissions { get; init; } = new ();

        /// <summary>
        /// Provide the default settings to be used by all jobs in the workflow.
        /// </summary>
        public Defaults Defaults { get; init; } = new();
    }
}
