using System.Collections.Generic;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.Common;

namespace Sharpliner.AzureDevOps.Validation;

public static class ValidationsExtensions
{
    public static IReadOnlyCollection<IDefinitionValidation> GetStageValidations(this ConditionedList<Stage> definitions)
        => new[]
        {
            new StageDependsOnValidation(definitions)
        };

    public static IReadOnlyCollection<IDefinitionValidation> GetJobValidations(this ConditionedList<JobBase> definitions)
        => new[]
        {
            new JobDependsOnValidation(definitions)
        };
}
