using System;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Allows better syntax inside of the condition tree.
/// </summary>
public static class AdoExpressionExtensions
{
    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="conditionedDefinition">Conditioned definition</param>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    public static AdoExpression<VariableBase> Variable(
        this AdoExpression<VariableBase> conditionedDefinition,
        string name,
        string value)
    {
        conditionedDefinition.Definitions.Add(new AdoExpression<VariableBase>(definition: new Variable(name, value)));
        return conditionedDefinition;
    }

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="conditionedDefinition">Conditioned definition</param>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    public static AdoExpression<VariableBase> Variable(
        this AdoExpression<VariableBase> conditionedDefinition,
        string name,
        bool value)
    {
        conditionedDefinition.Definitions.Add(new AdoExpression<VariableBase>(definition: new Variable(name, value)));
        return conditionedDefinition;
    }

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="conditionedDefinition">Conditioned definition</param>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    public static AdoExpression<VariableBase> Variable(
        this AdoExpression<VariableBase> conditionedDefinition,
        string name,
        int value)
    {
        conditionedDefinition.Definitions.Add(new AdoExpression<VariableBase>(definition: new Variable(name, value)));
        return conditionedDefinition;
    }

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="conditionedDefinition">Conditioned definition</param>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    public static AdoExpression<VariableBase> Variable(
        this AdoExpression<VariableBase> conditionedDefinition,
        string name,
        Enum value)
    {
        conditionedDefinition.Definitions.Add(new AdoExpression<VariableBase>(definition: new Variable(name, value)));
        return conditionedDefinition;
    }

    /// <summary>
    /// Defines multiple variables at once.
    /// </summary>
    /// <param name="conditionedDefinition">Conditioned definition</param>
    /// <param name="variables">List of (key, value) pairs</param>
    public static AdoExpression<VariableBase> Variables(
        this AdoExpression<VariableBase> conditionedDefinition,
        params (string name, object value)[] variables)
    {
        foreach (var variable in variables)
        {
            AdoExpression<VariableBase> definition = variable.value switch
            {
                int number => new AdoExpression<VariableBase>(definition: new Variable(variable.name, number)),
                bool boolean => new AdoExpression<VariableBase>(definition: new Variable(variable.name, boolean)),
                string s => new AdoExpression<VariableBase>(definition: new Variable(variable.name, s)),
                object any => new AdoExpression<VariableBase>(definition: new Variable(variable.name, any?.ToString() ?? string.Empty)),
            };

            conditionedDefinition.Definitions.Add(definition);
        }

        return conditionedDefinition;
    }

    /// <summary>
    /// References a variable group.
    /// </summary>
    public static AdoExpression<VariableBase> Group(
        this AdoExpression<VariableBase> conditionedDefinition,
        string name)
    {
        conditionedDefinition.Definitions.Add(new AdoExpression<VariableBase>(definition: new VariableGroup(name)));
        return conditionedDefinition;
    }

    /// <summary>
    /// Creates a new stage.
    /// </summary>
    public static AdoExpression<Stage> Stage(this AdoExpression<Stage> conditionedDefinition, Stage stage)
    {
        conditionedDefinition.Definitions.Add(new AdoExpression<Stage>(definition: stage));
        return conditionedDefinition;
    }

    /// <summary>
    /// Creates a new step.
    /// </summary>
    public static AdoExpression<Step> Step(this AdoExpression<Step> conditionedDefinition, Step step)
    {
        conditionedDefinition.Definitions.Add(new AdoExpression<Step>(definition: step));
        return conditionedDefinition;
    }

    /// <summary>
    /// Creates a new job.
    /// </summary>
    public static AdoExpression<JobBase> Job(this AdoExpression<JobBase> conditionedDefinition, JobBase job)
    {
        conditionedDefinition.Definitions.Add(new AdoExpression<JobBase>(definition: job));
        return conditionedDefinition;
    }

    /// <summary>
    /// Creates a new deployment job.
    /// </summary>
    public static AdoExpression<JobBase> DeploymentJob(this AdoExpression<JobBase> conditionedDefinition, JobBase job)
    {
        conditionedDefinition.Definitions.Add(new AdoExpression<JobBase>(definition: job));
        return conditionedDefinition;
    }

    /// <summary>
    /// Reference a YAML stage template.
    /// </summary>
    /// <param name="conditionedDefinition">Conditioned definition</param>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    public static AdoExpression<Stage> StageTemplate(
        this AdoExpression<Stage> conditionedDefinition,
        string path,
        TemplateParameters parameters)
    {
        var template = new Template<Stage>(path: path, parameters);
        conditionedDefinition.Definitions.Add(template);
        return conditionedDefinition;
    }

    /// <summary>
    /// Reference a YAML job template.
    /// </summary>
    /// <param name="conditionedDefinition">Conditioned definition</param>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    public static AdoExpression<JobBase> JobTemplate(
        this AdoExpression<JobBase> conditionedDefinition,
        string path,
        TemplateParameters parameters)
    {
        var template = new Template<JobBase>(path: path, parameters);
        conditionedDefinition.Definitions.Add(template);
        return conditionedDefinition;
    }

    /// <summary>
    /// Reference a YAML step template.
    /// </summary>
    /// <param name="conditionedDefinition">Conditioned definition</param>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    public static AdoExpression<Step> StepTemplate(
        this AdoExpression<Step> conditionedDefinition,
        string path,
        TemplateParameters parameters)
    {
        var template = new Template<Step>(path: path, parameters);
        conditionedDefinition.Definitions.Add(template);
        return conditionedDefinition;
    }

    /// <summary>
    /// Reference a YAML variable template.
    /// </summary>
    /// <param name="conditionedDefinition">Conditioned definition</param>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    public static AdoExpression<VariableBase> VariableTemplate(
        this AdoExpression<VariableBase> conditionedDefinition,
        string path,
        TemplateParameters parameters)
    {
        var template = new Template<VariableBase>(path: path, parameters);
        conditionedDefinition.Definitions.Add(template);
        return conditionedDefinition;
    }

    /// <summary>
    /// Reference a step library (series of library stages).
    /// </summary>
    public static AdoExpression<Stage> StageLibrary(
        this AdoExpression<Stage> conditionedDefinition,
        StageLibrary library)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<Stage>(library));
        return conditionedDefinition;
    }

    /// <summary>
    /// Reference a step library (series of library jobs).
    /// </summary>
    public static AdoExpression<JobBase> JobLibrary(
        this AdoExpression<JobBase> conditionedDefinition,
        JobLibrary library)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<JobBase>(library));
        return conditionedDefinition;
    }

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    public static AdoExpression<Step> StepLibrary(
        this AdoExpression<Step> conditionedDefinition,
        StepLibrary library)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<Step>(library));
        return conditionedDefinition;
    }

    /// <summary>
    /// Reference a step library (series of library Variables).
    /// </summary>
    public static AdoExpression<VariableBase> VariableLibrary(
        this AdoExpression<VariableBase> conditionedDefinition,
        VariableLibrary library)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<VariableBase>(library));
        return conditionedDefinition;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Stage> Stages(
        this AdoExpression<Stage> conditionedDefinition,
        IEnumerable<AdoExpression<Stage>> stages)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<Stage>(stages));
        return conditionedDefinition;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Job> Jobs(
        this AdoExpression<Job> conditionedDefinition,
        IEnumerable<AdoExpression<Job>> jobs)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<Job>(jobs));
        return conditionedDefinition;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Step> Steps(
        this AdoExpression<Step> conditionedDefinition,
        IEnumerable<AdoExpression<Step>> steps)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<Step>(steps));
        return conditionedDefinition;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<VariableBase> Variables(
        this AdoExpression<VariableBase> conditionedDefinition,
        IEnumerable<AdoExpression<VariableBase>> variables)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<VariableBase>(variables));
        return conditionedDefinition;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Stage> Stages(
        this AdoExpression<Stage> conditionedDefinition,
        params AdoExpression<Stage>[] stages)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<Stage>(stages));
        return conditionedDefinition;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Job> Jobs(
        this AdoExpression<Job> conditionedDefinition,
        params AdoExpression<Job>[] jobs)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<Job>(jobs));
        return conditionedDefinition;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Step> Steps(
        this AdoExpression<Step> conditionedDefinition,
        params AdoExpression<Step>[] steps)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<Step>(steps));
        return conditionedDefinition;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<VariableBase> Variables(
        this AdoExpression<VariableBase> conditionedDefinition,
        params AdoExpression<VariableBase>[] variables)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<VariableBase>(variables));
        return conditionedDefinition;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Stage> Stages(
        this AdoExpression<Stage> conditionedDefinition,
        IEnumerable<Stage> stages)
        => conditionedDefinition.Stages(stages.ToArray());

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Job> Jobs(
        this AdoExpression<Job> conditionedDefinition,
        IEnumerable<Job> jobs)
        => conditionedDefinition.Jobs(jobs.ToArray());

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Step> Steps(
        this AdoExpression<Step> conditionedDefinition,
        IEnumerable<Step> steps)
        => conditionedDefinition.Steps(steps.ToArray());

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<VariableBase> Variables(
        this AdoExpression<VariableBase> conditionedDefinition,
        IEnumerable<VariableBase> variables)
        => conditionedDefinition.Variables(variables.ToArray());

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Stage> Stages(
        this AdoExpression<Stage> conditionedDefinition,
        params Stage[] stages)
        => conditionedDefinition.Stages(stages.Select(x => new AdoExpression<Stage>(x)));

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Job> Jobs(
        this AdoExpression<Job> conditionedDefinition,
        params Job[] jobs)
        => conditionedDefinition.Jobs(jobs.Select(x => new AdoExpression<Job>(x)));

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Step> Steps(
        this AdoExpression<Step> conditionedDefinition,
        params Step[] steps)
        => conditionedDefinition.Steps(steps.Select(x => new AdoExpression<Step>(x)));

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<VariableBase> Variables(
        this AdoExpression<VariableBase> conditionedDefinition,
        params VariableBase[] variables)
        => conditionedDefinition.Variables(variables.Select(x => new AdoExpression<VariableBase>(x)));

    /// <summary>
    /// Reference a step library (series of library stages).
    /// </summary>
    public static AdoExpression<Stage> StageLibrary<T>(this AdoExpression<Stage> conditionedDefinition)
        where T : StageLibrary, new()
    {
        conditionedDefinition.Definitions.Add(AzureDevOpsDefinition.CreateLibraryRef<T, Stage>());
        return conditionedDefinition;
    }

    /// <summary>
    /// Reference a step library (series of library jobs).
    /// </summary>
    public static AdoExpression<JobBase> JobLibrary<T>(this AdoExpression<JobBase> conditionedDefinition)
        where T : JobLibrary, new()
    {
        conditionedDefinition.Definitions.Add(AzureDevOpsDefinition.CreateLibraryRef<T, JobBase>());
        return conditionedDefinition;
    }

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    public static AdoExpression<Step> StepLibrary<T>(this AdoExpression<Step> conditionedDefinition)
        where T : StepLibrary, new()
    {
        conditionedDefinition.Definitions.Add(AzureDevOpsDefinition.CreateLibraryRef<T, Step>());
        return conditionedDefinition;
    }

    /// <summary>
    /// Reference a step library (series of library Variables).
    /// </summary>
    public static AdoExpression<VariableBase> VariableLibrary<T>(this AdoExpression<VariableBase> conditionedDefinition)
        where T : VariableLibrary, new()
    {
        conditionedDefinition.Definitions.Add(AzureDevOpsDefinition.CreateLibraryRef<T, VariableBase>());
        return conditionedDefinition;
    }

    internal static AdoExpression<T>? GetRoot<T>(this AdoExpression<T> conditionedDefinition)
    {
        var parent = conditionedDefinition;
        while (parent?.Parent != null)
        {
            parent = parent.Parent as AdoExpression<T>;
        }

        return parent;
    }
}
