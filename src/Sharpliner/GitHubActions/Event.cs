namespace Sharpliner.GitHubActions;

/// <summary>
/// Abstract class that represents all the possible events that will trigger a workflow.
/// </summary>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record Event;

/// <summary>
/// Event that allows to trigger a workflow at a schedule time using POSIX cron syntax.
/// </summary>
/// <param name="Cron">The POSIX cron syntax that will be used to decide when to trigger the workflow.</param>
// TODO (GitHub Actions): Made internal until we get to a more complete API
internal record Schedule(string Cron) : Event;

