using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.Common;

namespace Sharpliner.AzureDevOps.Validation;

/// <summary>
/// Utility class for validation extensions.
/// </summary>
public static class ValidationsExtensions
{
    /// <summary>
    /// Get all jobs from the stages.
    /// </summary>
    /// <param name="stages">The input stages.</param>
    /// <returns>A flattened list of all jobs.</returns>
    public static IEnumerable<JobBase> GetJobs(this ConditionedList<Stage> stages)
        => stages.SelectMany(s => s.FlattenDefinitions().SelectMany(r => r.Jobs.SelectMany(j => j.FlattenDefinitions())));

    /// <summary>
    /// Get all steps from the jobs, this includes only jobs of type <see cref="Job"/>.
    /// </summary>
    /// <param name="jobs">The input jobs.</param>
    /// <returns>A flattened list of all steps.</returns>
    public static IEnumerable<Step> GetSteps(this ConditionedList<JobBase> jobs)
        => jobs.SelectMany(j => j
                .FlattenDefinitions()
                .OfType<Job>()
                .SelectMany(job => job.Steps.SelectMany(s => s.FlattenDefinitions())));

    /// <summary>
    /// Gets the required validations for a list of stages.
    /// </summary>
    /// <param name="stages">The input stages.</param>
    /// <returns>A list of validations for the input stages.</returns>
    public static IReadOnlyCollection<IDefinitionValidation> GetStageValidations(this ConditionedList<Stage> stages) =>
    [
        new StageDependsOnValidation(stages),
        new NameValidation(stages),
    ];

    /// <summary>
    /// Gets the required validations for a list of jobs.
    /// </summary>
    /// <param name="jobs">The input jobs.</param>
    /// <returns>A list of validations for the input jobs.</returns>
    public static IReadOnlyCollection<IDefinitionValidation> GetJobValidations(this ConditionedList<JobBase> jobs) =>
    [
        new JobDependsOnValidation(jobs),
        new NameValidation(jobs),
    ];
}
