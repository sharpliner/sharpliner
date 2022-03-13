using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.Common;

namespace Sharpliner.AzureDevOps.Validation;

internal abstract class DependsOnValidation : IDefinitionValidation
{
    private readonly ValidationSeverity _severity;

    public DependsOnValidation(ValidationSeverity severity)
    {
        _severity = severity;
    }

    public abstract IReadOnlyCollection<ValidationError> Validate();

    protected IReadOnlyCollection<ValidationError> ValidateDependsOn<T>(ConditionedList<T> definitions) where T : IDependsOn
    {
        return ValidateDependsOn(definitions.SelectMany(s => s.FlattenDefinitions()));
    }

    protected IReadOnlyCollection<ValidationError> ValidateDependsOn<T>(IEnumerable<Conditioned<T>> definitions) where T : IDependsOn
    {
        return ValidateDependsOn(definitions.SelectMany(s => s.FlattenDefinitions()));
    }

    protected IReadOnlyCollection<ValidationError> ValidateDependsOn<T>(IEnumerable<T> definitions) where T : IDependsOn
    {
        var errors = new List<ValidationError>();

        if (_severity == ValidationSeverity.Off)
        {
            return errors;
        }

        var duplicate = definitions.FirstOrDefault(d => definitions.Count(o => o.Name == d.Name) > 1);
        if (duplicate is not null)
        {
            errors.Add(new(_severity, $"Found duplicate {typeof(T).Name.ToLower()} name '{duplicate.Name}'"));
        }

        var invalidName = definitions.FirstOrDefault(d => !AzureDevOpsDefinition.NameRegex.IsMatch(d.Name));
        if (invalidName is not null)
        {
            errors.Add(new ValidationError(_severity, $"Invalid character found in {typeof(T).Name.ToLower()} name '{invalidName.Name}', only A-Z, a-z, 0-9, and underscore are allowed"));
        }

        foreach (var definition in definitions)
        {
            if (definition.DependsOn is EmptyDependsOn)
            {
                continue;
            }

            foreach (var dependsOn in definition.DependsOn)
            {
                if (dependsOn.Definition == definition.Name)
                {
                    errors.Add(new ValidationError(_severity, $"{typeof(T).Name} `{definition.Name}` depends on itself"));
                    continue;
                }

                if (!definitions.Any(d => d.Name == dependsOn))
                {
                    // This check can be a false positive since items can be defined inside templates and then we don't have the visiblity in there
                    errors.Add(new(ValidationSeverity.Trace, $"{typeof(T).Name} `{definition.Name}` depends on {typeof(T).Name.ToLower()} `{dependsOn.Definition}` which was not found"));
                }
            }
        }

        // TODO: Validate circular dependencies

        return errors;
    }
}

/// <summary>
/// Validates that dependsOn field has a matching definition.
/// </summary>
internal class JobsDependsOnValidation : DependsOnValidation
{
    private readonly IEnumerable<Conditioned<JobBase>> _jobs;

    public JobsDependsOnValidation(IEnumerable<Conditioned<JobBase>> jobs)
        : base(SharplinerConfiguration.Current.Validations.DependsOn)
    {
        _jobs = jobs;
    }

    public JobsDependsOnValidation(ConditionedList<Stage> stages)
        : this(stages.SelectMany(s => s.FlattenDefinitions().SelectMany(ss => ss.Jobs)))
    {
    }

    public override IReadOnlyCollection<ValidationError> Validate()
    {
        return ValidateDependsOn(_jobs);
    }
}

/// <summary>
/// Validates that dependsOn field has a matching definition.
/// </summary>
internal class StageDependsOnValidation : DependsOnValidation
{
    private readonly ConditionedList<Stage> _stages;

    public StageDependsOnValidation(ConditionedList<Stage> stages)
        : base(SharplinerConfiguration.Current.Validations.DependsOn)
    {
        _stages = stages;
    }

    public override IReadOnlyCollection<ValidationError> Validate()
    {
        return ValidateDependsOn(_stages);
    }
}
