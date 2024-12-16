using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.Common;

namespace Sharpliner.AzureDevOps.Validation;

internal class NameValidation : IDefinitionValidation
{
    private readonly ValidationSeverity _severity;
    private readonly IReadOnlyCollection<IReadOnlyCollection<string>> _nameGroups;
    private readonly ConditionedList<Parameter> _parameters;

    private NameValidation(IReadOnlyCollection<IReadOnlyCollection<string>> nameGroups, ConditionedList<Parameter> parameters)
    {
        _severity = SharplinerConfiguration.Current.Validations.NameFields;
        _nameGroups = nameGroups;
        _parameters = parameters;
    }

    public NameValidation(ConditionedList<Stage> stages, ConditionedList<Parameter> parameters)
        : this(GetStageAndBuildNameGroups(stages), parameters)
    {
    }

    public NameValidation(ConditionedList<JobBase> jobs, ConditionedList<Parameter> parameters)
        : this([jobs.SelectMany(j => j.FlattenDefinitions()).Select(s => s.Name).ToList()], parameters)
    {
    }

    public IReadOnlyCollection<ValidationError> Validate()
    {
        var errors = new List<ValidationError>();

        if (_severity == ValidationSeverity.Off)
        {
            return errors;
        }

        var invalidNames = _nameGroups.SelectMany(g => g)
            .Where(name => !AzureDevOpsDefinition.NameRegex.IsMatch(name) && !AzureDevOpsDefinition.ParameterReferenceRegex.IsMatch(name));
        foreach (var invalidName in invalidNames)
        {
            errors.Add(new ValidationError(_severity, $"Invalid character found in name `{invalidName}`, only A-Z, a-z, 0-9, and underscore are allowed, or parameter reference syntax `${{{{ parameters['name'] }}}}`"));
        }

        foreach (var group in _nameGroups)
        {
            var duplicate = group.FirstOrDefault(name => group.Count(other => name == other) > 1);
            if (duplicate is not null)
            {
                errors.Add(new(_severity, $"Found duplicate name `{duplicate}`"));
            }
        }

        return errors;
    }

    private static List<List<string>> GetStageAndBuildNameGroups(ConditionedList<Stage> stages)
    {
        var stageNames = stages
            .SelectMany(s => s.FlattenDefinitions())
            .Select(s => s.Name)
            .ToList();

        // For each stage, get its job names
        var jobNames = stages
            .SelectMany(s => s.FlattenDefinitions()
                .Select(s => s.Jobs.SelectMany(j => j.FlattenDefinitions().Select(k => k.Name)).ToList()));

        return jobNames.Prepend(stageNames).ToList();
    }
}
