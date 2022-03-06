using Sharpliner.AzureDevOps.ConditionedExpressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharpliner.AzureDevOps.Validation;

internal abstract class DependsOnValidation
{
    protected static void ValidateDependsOn<T>(ConditionedList<T> definitions) where T : IDependsOn
    {
        ValidateDependsOn(definitions.SelectMany(s => s.FlattenDefinitions()));
    }

    protected static void ValidateDependsOn<T>(IEnumerable<Conditioned<T>> definitions) where T : IDependsOn
    {
        ValidateDependsOn(definitions.SelectMany(s => s.FlattenDefinitions()));
    }

    protected static void ValidateDependsOn<T>(IEnumerable<T> definitions) where T : IDependsOn
    {
        var duplicate = definitions.FirstOrDefault(d => definitions.Count(o => o.Name == d.Name) > 1);
        if (duplicate is not null)
        {
            throw new Exception($"Found duplicate {typeof(T).Name.ToLower()} name '{duplicate.Name}'");
        }

        var invalidName = definitions.FirstOrDefault(d => !DefinitionValidator.NameRegex.IsMatch(d.Name));
        if (invalidName is not null)
        {
            throw new Exception($"Invalid character found in {typeof(T).Name.ToLower()} name '{invalidName.Name}', only A-Z, a-z, 0-9, and underscore are allowed");
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
                    throw new Exception($"{typeof(T).Name} `{definition.Name}` depends on itself");
                }

                // TODO: This check can be disruptive since items can be defined inside templates and then we don't have the visiblity in there
                //if (!allDefs.Any(d => d.Name == dependsOn))
                //{
                //    throw new Exception($"{typeof(T).Name} `{definition.Name}` depends on {typeof(T).Name.ToLower()} `{dependsOn.Definition}` which was not found");
                //}
            }
        }

        // TODO: Validate circular dependencies?
    }
}

/// <summary>
/// Validates that dependsOn field has a matching definition.
/// </summary>
internal class JobsDependsOnValidation : DependsOnValidation, IDefinitionValidation
{
    private readonly IEnumerable<Conditioned<JobBase>> _jobs;

    public JobsDependsOnValidation(IEnumerable<Conditioned<JobBase>> jobs)
    {
        _jobs = jobs;
    }

    public JobsDependsOnValidation(ConditionedList<Stage> stages)
        : this(stages.SelectMany(s => s.FlattenDefinitions().SelectMany(ss => ss.Jobs)))
    {
    }

    public void Validate()
    {
        ValidateDependsOn(_jobs);
    }
}

/// <summary>
/// Validates that dependsOn field has a matching definition.
/// </summary>
internal class StageDependsOnValidation : DependsOnValidation, IDefinitionValidation
{
    private readonly ConditionedList<Stage> _stages;

    public StageDependsOnValidation(ConditionedList<Stage> stages)
    {
        _stages = stages;
    }

    public void Validate()
    {
        ValidateDependsOn(_stages);
    }
}
