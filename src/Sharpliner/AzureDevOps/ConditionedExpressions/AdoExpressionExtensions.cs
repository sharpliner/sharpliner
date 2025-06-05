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
    /// <param name="expression">Expression</param>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    public static AdoExpression<VariableBase> Variable(
        this AdoExpression<VariableBase> expression,
        string name,
        string value)
    {
        expression.Definitions.Add(new AdoExpression<VariableBase>(definition: new Variable(name, value)));
        return expression;
    }

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    public static AdoExpression<VariableBase> Variable(
        this AdoExpression<VariableBase> expression,
        string name,
        bool value)
    {
        expression.Definitions.Add(new AdoExpression<VariableBase>(definition: new Variable(name, value)));
        return expression;
    }

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    public static AdoExpression<VariableBase> Variable(
        this AdoExpression<VariableBase> expression,
        string name,
        int value)
    {
        expression.Definitions.Add(new AdoExpression<VariableBase>(definition: new Variable(name, value)));
        return expression;
    }

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    public static AdoExpression<VariableBase> Variable(
        this AdoExpression<VariableBase> expression,
        string name,
        Enum value)
    {
        expression.Definitions.Add(new AdoExpression<VariableBase>(definition: new Variable(name, value)));
        return expression;
    }

    /// <summary>
    /// Defines multiple variables at once.
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="variables">List of (key, value) pairs</param>
    public static AdoExpression<VariableBase> Variables(
        this AdoExpression<VariableBase> expression,
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

            expression.Definitions.Add(definition);
        }

        return expression;
    }

    /// <summary>
    /// References a variable group.
    /// </summary>
    public static AdoExpression<VariableBase> Group(
        this AdoExpression<VariableBase> expression,
        string name)
    {
        expression.Definitions.Add(new AdoExpression<VariableBase>(definition: new VariableGroup(name)));
        return expression;
    }

    /// <summary>
    /// Creates a new stage.
    /// </summary>
    public static AdoExpression<Stage> Stage(this AdoExpression<Stage> expression, Stage stage)
    {
        expression.Definitions.Add(new AdoExpression<Stage>(definition: stage));
        return expression;
    }

    /// <summary>
    /// Creates a new step.
    /// </summary>
    public static AdoExpression<Step> Step(this AdoExpression<Step> expression, Step step)
    {
        expression.Definitions.Add(new AdoExpression<Step>(definition: step));
        return expression;
    }

    /// <summary>
    /// Creates a new job.
    /// </summary>
    public static AdoExpression<JobBase> Job(this AdoExpression<JobBase> expression, JobBase job)
    {
        expression.Definitions.Add(new AdoExpression<JobBase>(definition: job));
        return expression;
    }

    /// <summary>
    /// Creates a new deployment job.
    /// </summary>
    public static AdoExpression<JobBase> DeploymentJob(this AdoExpression<JobBase> expression, JobBase job)
    {
        expression.Definitions.Add(new AdoExpression<JobBase>(definition: job));
        return expression;
    }

    /// <summary>
    /// Reference a YAML stage template.
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    public static AdoExpression<Stage> StageTemplate(
        this AdoExpression<Stage> expression,
        string path,
        TemplateParameters parameters)
    {
        var template = new Template<Stage>(path: path, parameters);
        expression.Definitions.Add(template);
        return expression;
    }

    /// <summary>
    /// Reference a YAML job template.
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    public static AdoExpression<JobBase> JobTemplate(
        this AdoExpression<JobBase> expression,
        string path,
        TemplateParameters parameters)
    {
        var template = new Template<JobBase>(path: path, parameters);
        expression.Definitions.Add(template);
        return expression;
    }

    /// <summary>
    /// Reference a YAML step template.
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    public static AdoExpression<Step> StepTemplate(
        this AdoExpression<Step> expression,
        string path,
        TemplateParameters parameters)
    {
        var template = new Template<Step>(path: path, parameters);
        expression.Definitions.Add(template);
        return expression;
    }

    /// <summary>
    /// Reference a YAML variable template.
    /// </summary>
    /// <param name="expression">Expression</param>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    public static AdoExpression<VariableBase> VariableTemplate(
        this AdoExpression<VariableBase> expression,
        string path,
        TemplateParameters parameters)
    {
        var template = new Template<VariableBase>(path: path, parameters);
        expression.Definitions.Add(template);
        return expression;
    }

    /// <summary>
    /// Reference a step library (series of library stages).
    /// </summary>
    public static AdoExpression<Stage> StageLibrary(
        this AdoExpression<Stage> expression,
        StageLibrary library)
    {
        expression.Definitions.Add(new LibraryReference<Stage>(library));
        return expression;
    }

    /// <summary>
    /// Reference a step library (series of library jobs).
    /// </summary>
    public static AdoExpression<JobBase> JobLibrary(
        this AdoExpression<JobBase> expression,
        JobLibrary library)
    {
        expression.Definitions.Add(new LibraryReference<JobBase>(library));
        return expression;
    }

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    public static AdoExpression<Step> StepLibrary(
        this AdoExpression<Step> expression,
        StepLibrary library)
    {
        expression.Definitions.Add(new LibraryReference<Step>(library));
        return expression;
    }

    /// <summary>
    /// Reference a step library (series of library Variables).
    /// </summary>
    public static AdoExpression<VariableBase> VariableLibrary(
        this AdoExpression<VariableBase> expression,
        VariableLibrary library)
    {
        expression.Definitions.Add(new LibraryReference<VariableBase>(library));
        return expression;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Stage> Stages(
        this AdoExpression<Stage> expression,
        IEnumerable<AdoExpression<Stage>> stages)
    {
        expression.Definitions.Add(new LibraryReference<Stage>(stages));
        return expression;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Job> Jobs(
        this AdoExpression<Job> expression,
        IEnumerable<AdoExpression<Job>> jobs)
    {
        expression.Definitions.Add(new LibraryReference<Job>(jobs));
        return expression;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Step> Steps(
        this AdoExpression<Step> expression,
        IEnumerable<AdoExpression<Step>> steps)
    {
        expression.Definitions.Add(new LibraryReference<Step>(steps));
        return expression;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<VariableBase> Variables(
        this AdoExpression<VariableBase> expression,
        IEnumerable<AdoExpression<VariableBase>> variables)
    {
        expression.Definitions.Add(new LibraryReference<VariableBase>(variables));
        return expression;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Stage> Stages(
        this AdoExpression<Stage> expression,
        params AdoExpression<Stage>[] stages)
    {
        expression.Definitions.Add(new LibraryReference<Stage>(stages));
        return expression;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Job> Jobs(
        this AdoExpression<Job> expression,
        params AdoExpression<Job>[] jobs)
    {
        expression.Definitions.Add(new LibraryReference<Job>(jobs));
        return expression;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Step> Steps(
        this AdoExpression<Step> expression,
        params AdoExpression<Step>[] steps)
    {
        expression.Definitions.Add(new LibraryReference<Step>(steps));
        return expression;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<VariableBase> Variables(
        this AdoExpression<VariableBase> expression,
        params AdoExpression<VariableBase>[] variables)
    {
        expression.Definitions.Add(new LibraryReference<VariableBase>(variables));
        return expression;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Stage> Stages(
        this AdoExpression<Stage> expression,
        IEnumerable<Stage> stages)
        => expression.Stages(stages.ToArray());

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Job> Jobs(
        this AdoExpression<Job> expression,
        IEnumerable<Job> jobs)
        => expression.Jobs(jobs.ToArray());

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Step> Steps(
        this AdoExpression<Step> expression,
        IEnumerable<Step> steps)
        => expression.Steps(steps.ToArray());

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<VariableBase> Variables(
        this AdoExpression<VariableBase> expression,
        IEnumerable<VariableBase> variables)
        => expression.Variables(variables.ToArray());

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Stage> Stages(
        this AdoExpression<Stage> expression,
        params Stage[] stages)
        => expression.Stages(stages.Select(x => new AdoExpression<Stage>(x)));

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Job> Jobs(
        this AdoExpression<Job> expression,
        params Job[] jobs)
        => expression.Jobs(jobs.Select(x => new AdoExpression<Job>(x)));

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Step> Steps(
        this AdoExpression<Step> expression,
        params Step[] steps)
        => expression.Steps(steps.Select(x => new AdoExpression<Step>(x)));

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<VariableBase> Variables(
        this AdoExpression<VariableBase> expression,
        params VariableBase[] variables)
        => expression.Variables(variables.Select(x => new AdoExpression<VariableBase>(x)));

    /// <summary>
    /// Reference a step library (series of library stages).
    /// </summary>
    public static AdoExpression<Stage> StageLibrary<T>(this AdoExpression<Stage> expression)
        where T : StageLibrary, new()
    {
        expression.Definitions.Add(AzureDevOpsDefinition.CreateLibraryRef<T, Stage>());
        return expression;
    }

    /// <summary>
    /// Reference a step library (series of library jobs).
    /// </summary>
    public static AdoExpression<JobBase> JobLibrary<T>(this AdoExpression<JobBase> expression)
        where T : JobLibrary, new()
    {
        expression.Definitions.Add(AzureDevOpsDefinition.CreateLibraryRef<T, JobBase>());
        return expression;
    }

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    public static AdoExpression<Step> StepLibrary<T>(this AdoExpression<Step> expression)
        where T : StepLibrary, new()
    {
        expression.Definitions.Add(AzureDevOpsDefinition.CreateLibraryRef<T, Step>());
        return expression;
    }

    /// <summary>
    /// Reference a step library (series of library Variables).
    /// </summary>
    public static AdoExpression<VariableBase> VariableLibrary<T>(this AdoExpression<VariableBase> expression)
        where T : VariableLibrary, new()
    {
        expression.Definitions.Add(AzureDevOpsDefinition.CreateLibraryRef<T, VariableBase>());
        return expression;
    }

    internal static AdoExpression<T>? GetRoot<T>(this AdoExpression<T> expression)
    {
        var parent = expression;
        while (parent?.Parent != null)
        {
            parent = parent.Parent as AdoExpression<T>;
        }

        return parent;
    }
}
