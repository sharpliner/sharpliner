using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.Common;

namespace Sharpliner.AzureDevOps.Validation;

internal abstract class DependsOnValidation : IDefinitionValidation
{
    private readonly ValidationSeverity _severity;

    protected DependsOnValidation()
    {
        _severity = SharplinerConfiguration.Current.Validations.DependsOnFields;
    }

    public abstract IReadOnlyCollection<ValidationError> Validate();

    protected IReadOnlyCollection<ValidationError> ValidateDependsOn<T>(IEnumerable<T> definitions) where T : IDependsOn
    {
        var errors = new List<ValidationError>();

        if (_severity == ValidationSeverity.Off)
        {
            return errors;
        }

        foreach (var definition in definitions)
        {
            if (definition.DependsOn is [] or null)
            {
                continue;
            }

            foreach (var dependsOn in definition.DependsOn)
            {
                var dependsOnName = dependsOn.FlattenDefinitions().Where(s => s is not null).FirstOrDefault();

                if (dependsOnName == definition.Name)
                {
                    errors.Add(new ValidationError(_severity, $"{GetTypeName<T>()} `{definition.Name}` depends on itself"));
                    continue;
                }

                if (!definitions.Any(d => d.Name == dependsOnName))
                {
                    // This check can be a false positive since items can be defined inside templates and then we don't have the visibility in there
                    errors.Add(new(ValidationSeverity.Trace, $"{GetTypeName<T>()} `{definition.Name}` depends on {GetTypeName<T>().ToLower()} `{dependsOnName}` which was not found"));
                }
            }
        }

        // TODO: Validate circular dependencies

        return errors;
    }

    private string GetTypeName<T>() => typeof(T).Name.Replace("Base", null);
}

internal class StageDependsOnValidation(AdoExpressionList<Stage> stages) : DependsOnValidation()
{
    private readonly IReadOnlyCollection<Stage> _stages = stages.SelectMany(s => s.FlattenDefinitions()).ToList();

    public override IReadOnlyCollection<ValidationError> Validate()
    {
        var errors = new List<ValidationError>();

        errors.AddRange(ValidateDependsOn(_stages));

        // Validate jobs in each stage
        foreach (var stage in _stages)
        {
            errors.AddRange(ValidateDependsOn(stage.Jobs.SelectMany(job => job.FlattenDefinitions())));
        }

        return errors;
    }
}

internal class JobDependsOnValidation(AdoExpressionList<JobBase> jobs) : DependsOnValidation()
{
    private readonly IReadOnlyCollection<JobBase> _jobs = jobs.SelectMany(s => s.FlattenDefinitions()).ToList();

    public override IReadOnlyCollection<ValidationError> Validate() => ValidateDependsOn(_jobs);
}
