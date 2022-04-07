using System.Diagnostics.CodeAnalysis;
using System.Linq;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// A scheduled trigger specifies a schedule on which branches are built.
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#scheduled-trigger">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record ScheduledTrigger
{
    /// <summary>
    /// Cron syntax defining a schedule in UTC time
    /// E.g. "0 8,20 * * *" - run at 8:00 and 20:00 (UTC)
    /// </summary>
    [YamlMember(Order = 1, Alias = "cron")]
    [DisallowNull]
    public string? CronSchedule { get; init; }

    /// <summary>
    /// Friendly name given to a specific schedule
    /// </summary>
    [YamlMember(Order = 100)]
    [DisallowNull]
    public string? DisplayName { get; init; }

    [DisallowNull]
    [YamlMember(Order = 200)]
    public InclusionRule? Branches { get; init; }

    /// <summary>
    /// Defines whether to always run the pipeline or only if there have been source code changes since the last successful scheduled run.
    /// Defaults to false
    /// </summary>
    [YamlMember(Order = 300)]
    public bool Always { get; init; } = false;

    public ScheduledTrigger(string cronSchedule, params string[] branches)
    {
        if (branches.Any())
        {
            Branches = new InclusionRule
            {
                Include = branches.ToList()
            };
        }

        CronSchedule = cronSchedule;
    }

    /// <summary>
    /// Creates the cron trigger for the pipeline.
    /// </summary>
    /// <param name="minute">Number or *</param>
    /// <param name="hour">Number or *</param>
    /// <param name="dayOfMonth">Number or *</param>
    /// <param name="month">Number or *</param>
    /// <param name="dayOfWeek">Number or *</param>
    /// <param name="branches">List of branches for which the trigger should run</param>
    public ScheduledTrigger(string minute, string hour, string dayOfMonth, string month, string dayOfWeek, params string[] branches)
        : this($"{minute} {hour} {dayOfMonth} {month} {dayOfWeek}", branches)
    {
    }
}
