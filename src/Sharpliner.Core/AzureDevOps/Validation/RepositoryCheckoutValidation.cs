using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.Tasks;
using Sharpliner.Common;

namespace Sharpliner.AzureDevOps.Validation;

internal class RepositoryCheckoutValidation : IDefinitionValidation
{
    private readonly IEnumerable<Resources> _resources;
    private readonly IEnumerable<Step> _steps;
    private readonly ValidationSeverity _severity;

    public RepositoryCheckoutValidation(SingleStagePipeline pipeline)
    {
        _resources = pipeline.Resources?.FlattenDefinitions() ?? [];
        _steps = pipeline.Jobs.GetSteps();
        _severity = SharplinerConfiguration.Current.Validations.RepositoryCheckouts;
    }

    public RepositoryCheckoutValidation(Pipeline pipeline)
    {
        _resources = pipeline.Resources?.FlattenDefinitions() ?? [];
        _steps = pipeline.Stages
            .SelectMany(stage => stage.FlattenDefinitions())
            .SelectMany(stage => stage.Jobs.GetSteps());
        _severity = SharplinerConfiguration.Current.Validations.RepositoryCheckouts;
    }

    public IReadOnlyCollection<ValidationError> Validate()
    {
        var errors = new List<ValidationError>();

        if (_severity == ValidationSeverity.Off)
        {
            return errors;
        }

        var repositories = _steps
            .OfType<RepositoryCheckoutTask>()
            .Select(rct => rct.Checkout)
            .Where(repo => repo != "self" && repo != "none")
            .ToList();

        if (repositories.Count == 0)
        {
            return errors;
        }

        var resources = _resources.SelectMany(
                resource => resource.Repositories.SelectMany(res => res.FlattenDefinitions().Select(r => r.Identifier)))
            .ToList();

        var missingRepositories = repositories.Where(repo => !resources.Contains(repo));

        errors.AddRange(missingRepositories.Select(repo =>
            new ValidationError(_severity, $"Checked out repository `{repo}` needs to be declared in pipeline resources")));

        return errors;
    }
}
