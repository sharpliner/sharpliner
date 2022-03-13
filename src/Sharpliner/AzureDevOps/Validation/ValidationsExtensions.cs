using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.Common;

namespace Sharpliner.AzureDevOps.Validation;

public static class ValidationsExtensions
{
    public static IEnumerable<JobBase> GetJobs(this ConditionedList<Stage> stages)
        => stages.SelectMany(s => s.FlattenDefinitions().SelectMany(r => r.Jobs.SelectMany(j => j.FlattenDefinitions())));

    public static IEnumerable<Step> GetSteps(this ConditionedList<JobBase> jobs)
        => jobs.SelectMany(j => j
                .FlattenDefinitions()
                .Where(job => job is Job regularJob)
                .SelectMany(job => ((Job)job).Steps.SelectMany(s => s.FlattenDefinitions())));

    public static IReadOnlyCollection<IDefinitionValidation> GetStageValidations(this ConditionedList<Stage> stages)
        => new IDefinitionValidation[]
        {
            new StageDependsOnValidation(stages),
            new NameValidation(stages),
        };

    public static IReadOnlyCollection<IDefinitionValidation> GetJobValidations(this ConditionedList<JobBase> jobs)
        => new IDefinitionValidation[]
        {
            new JobDependsOnValidation(jobs),
            new NameValidation(jobs),
        };
}
