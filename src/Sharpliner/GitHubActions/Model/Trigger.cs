using System.Collections.Generic;

namespace Sharpliner.GitHubActions;

/// <summary>
/// Representation of all the possible triggers that a GitHub workflow can react too.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal class Trigger
{
    /// <summary>
    /// Collection of triggers that execute the workflow based on a schedule.
    /// </summary>
    public List<Schedule> Schedules { get; } = [];

    /// <summary>
    /// Triggers that required a manual intervention to execute the workflow. The intervention can be done by
    /// a human or an external service.
    /// </summary>
    public Manual Manuals { get; } = new();

    /// <summary>
    /// Collection of triggers that executed the workflow based on a webhook from the GitHup API.
    /// </summary>
    public List<Webhook> Webhooks { get; } = [];
}
