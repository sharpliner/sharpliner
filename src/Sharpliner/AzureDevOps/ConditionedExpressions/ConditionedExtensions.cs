using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Allows better syntax inside of the condition tree.
/// </summary>
public static class ConditionedExtensions
{
    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="conditionedDefinition">Conditioned definition</param>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    public static Conditioned<VariableBase> Variable(
        this Conditioned<VariableBase> conditionedDefinition,
        string name,
        string value)
    {
        conditionedDefinition.Definitions.Add(new Conditioned<VariableBase>(definition: new Variable(name, value)));
        return conditionedDefinition;
    }

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="conditionedDefinition">Conditioned definition</param>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    public static Conditioned<VariableBase> Variable(
        this Conditioned<VariableBase> conditionedDefinition,
        string name,
        bool value)
    {
        conditionedDefinition.Definitions.Add(new Conditioned<VariableBase>(definition: new Variable(name, value)));
        return conditionedDefinition;
    }

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="conditionedDefinition">Conditioned definition</param>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    public static Conditioned<VariableBase> Variable(
        this Conditioned<VariableBase> conditionedDefinition,
        string name,
        int value)
    {
        conditionedDefinition.Definitions.Add(new Conditioned<VariableBase>(definition: new Variable(name, value)));
        return conditionedDefinition;
    }

    /// <summary>
    /// Defines multiple variables at once.
    /// </summary>
    /// <param name="conditionedDefinition">Conditioned definition</param>
    /// <param name="variables">List of (key, value) pairs</param>
    public static Conditioned<VariableBase> Variables(
        this Conditioned<VariableBase> conditionedDefinition,
        params (string name, object value)[] variables)
    {
        foreach (var variable in variables)
        {
            Conditioned<VariableBase> definition = variable.value switch
            {
                int number => new Conditioned<VariableBase>(definition: new Variable(variable.name, number)),
                bool boolean => new Conditioned<VariableBase>(definition: new Variable(variable.name, boolean)),
                string s => new Conditioned<VariableBase>(definition: new Variable(variable.name, s)),
                object any => new Conditioned<VariableBase>(definition: new Variable(variable.name, any?.ToString() ?? string.Empty)),
            };

            conditionedDefinition.Definitions.Add(definition);
        }

        return conditionedDefinition;
    }

    /// <summary>
    /// References a variable group.
    /// </summary>
    public static Conditioned<VariableBase> Group(
        this Conditioned<VariableBase> conditionedDefinition,
        string name)
    {
        conditionedDefinition.Definitions.Add(new Conditioned<VariableBase>(definition: new VariableGroup(name)));
        return conditionedDefinition;
    }

    /// <summary>
    /// References a variable template.
    /// </summary>
    public static Conditioned<VariableBase> Template(
        this Conditioned<VariableBase> conditionedDefinition,
        string name)
    {
        conditionedDefinition.Definitions.Add(new Conditioned<VariableBase>(definition: new VariableTemplate(name)));
        return conditionedDefinition;
    }

    /// <summary>
    /// Creates a new stage.
    /// </summary>
    public static Conditioned<Stage> Stage(this Conditioned<Stage> condition, Stage stage)
    {
        condition.Definitions.Add(new Conditioned<Stage>(definition: stage));
        return condition;
    }

    /// <summary>
    /// Creates a new step.
    /// </summary>
    public static Conditioned<Step> Step(this Conditioned<Step> condition, Step step)
    {
        condition.Definitions.Add(new Conditioned<Step>(definition: step));
        return condition;
    }

    /// <summary>
    /// Creates a new job.
    /// </summary>
    public static Conditioned<JobBase> Job(this Conditioned<JobBase> condition, JobBase job)
    {
        condition.Definitions.Add(new Conditioned<JobBase>(definition: job));
        return condition;
    }

    /// <summary>
    /// Creates a new deployment job.
    /// </summary>
    public static Conditioned<JobBase> DeploymentJob(this Conditioned<JobBase> condition, JobBase job)
    {
        condition.Definitions.Add(new Conditioned<JobBase>(definition: job));
        return condition;
    }

    /// <summary>
    /// Reference a YAML stage template.
    /// </summary>
    /// <param name="conditionedDefinition">Conditioned definition</param>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    public static Conditioned<Stage> StageTemplate(
        this Conditioned<Stage> conditionedDefinition,
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
    public static Conditioned<JobBase> JobTemplate(
        this Conditioned<JobBase> conditionedDefinition,
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
    public static Conditioned<Step> StepTemplate(
        this Conditioned<Step> conditionedDefinition,
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
    public static Conditioned<VariableBase> VariableTemplate(
        this Conditioned<VariableBase> conditionedDefinition,
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
    public static Conditioned<Stage> StageLibrary(
        this Conditioned<Stage> conditionedDefinition,
        StageLibrary library)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<Stage>(library));
        return conditionedDefinition;
    }

    /// <summary>
    /// Reference a step library (series of library jobs).
    /// </summary>
    public static Conditioned<JobBase> JobLibrary(
        this Conditioned<JobBase> conditionedDefinition,
        JobLibrary library)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<JobBase>(library));
        return conditionedDefinition;
    }

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    public static Conditioned<Step> StepLibrary(
        this Conditioned<Step> conditionedDefinition,
        StepLibrary library)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<Step>(library));
        return conditionedDefinition;
    }

    /// <summary>
    /// Reference a step library (series of library Variables).
    /// </summary>
    public static Conditioned<VariableBase> VariableLibrary(
        this Conditioned<VariableBase> conditionedDefinition,
        VariableLibrary library)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<VariableBase>(library));
        return conditionedDefinition;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static Conditioned<Stage> Stages(
        this Conditioned<Stage> conditionedDefinition,
        IEnumerable<Conditioned<Stage>> stages)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<Stage>(stages));
        return conditionedDefinition;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static Conditioned<Job> Jobs(
        this Conditioned<Job> conditionedDefinition,
        IEnumerable<Conditioned<Job>> jobs)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<Job>(jobs));
        return conditionedDefinition;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static Conditioned<Step> Steps(
        this Conditioned<Step> conditionedDefinition,
        IEnumerable<Conditioned<Step>> steps)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<Step>(steps));
        return conditionedDefinition;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static Conditioned<VariableBase> Variables(
        this Conditioned<VariableBase> conditionedDefinition,
        IEnumerable<Conditioned<VariableBase>> variables)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<VariableBase>(variables));
        return conditionedDefinition;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static Conditioned<Stage> Stages(
        this Conditioned<Stage> conditionedDefinition,
        params Conditioned<Stage>[] stages)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<Stage>(stages));
        return conditionedDefinition;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static Conditioned<Job> Jobs(
        this Conditioned<Job> conditionedDefinition,
        params Conditioned<Job>[] jobs)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<Job>(jobs));
        return conditionedDefinition;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static Conditioned<Step> Steps(
        this Conditioned<Step> conditionedDefinition,
        params Conditioned<Step>[] steps)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<Step>(steps));
        return conditionedDefinition;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static Conditioned<VariableBase> Variables(
        this Conditioned<VariableBase> conditionedDefinition,
        params Conditioned<VariableBase>[] variables)
    {
        conditionedDefinition.Definitions.Add(new LibraryReference<VariableBase>(variables));
        return conditionedDefinition;
    }

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static Conditioned<Stage> Stages(
        this Conditioned<Stage> conditionedDefinition,
        IEnumerable<Stage> stages)
        => conditionedDefinition.Stages(stages.ToArray());

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static Conditioned<Job> Jobs(
        this Conditioned<Job> conditionedDefinition,
        IEnumerable<Job> jobs)
        => conditionedDefinition.Jobs(jobs.ToArray());

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static Conditioned<Step> Steps(
        this Conditioned<Step> conditionedDefinition,
        IEnumerable<Step> steps)
        => conditionedDefinition.Steps(steps.ToArray());

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static Conditioned<VariableBase> Variables(
        this Conditioned<VariableBase> conditionedDefinition,
        IEnumerable<VariableBase> variables)
        => conditionedDefinition.Variables(variables.ToArray());

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static Conditioned<Stage> Stages(
        this Conditioned<Stage> conditionedDefinition,
        params Stage[] stages)
        => conditionedDefinition.Stages(stages.Select(x => new Conditioned<Stage>(x)));

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static Conditioned<Job> Jobs(
        this Conditioned<Job> conditionedDefinition,
        params Job[] jobs)
        => conditionedDefinition.Jobs(jobs.Select(x => new Conditioned<Job>(x)));

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static Conditioned<Step> Steps(
        this Conditioned<Step> conditionedDefinition,
        params Step[] steps)
        => conditionedDefinition.Steps(steps.Select(x => new Conditioned<Step>(x)));

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static Conditioned<VariableBase> Variables(
        this Conditioned<VariableBase> conditionedDefinition,
        params VariableBase[] variables)
        => conditionedDefinition.Variables(variables.Select(x => new Conditioned<VariableBase>(x)));

    /// <summary>
    /// Reference a step library (series of library stages).
    /// </summary>
    public static Conditioned<Stage> StageLibrary<T>(this Conditioned<Stage> conditionedDefinition)
        where T : StageLibrary, new()
    {
        conditionedDefinition.Definitions.Add(AzureDevOpsDefinition.CreateLibraryRef<T, Stage>());
        return conditionedDefinition;
    }

    /// <summary>
    /// Reference a step library (series of library jobs).
    /// </summary>
    public static Conditioned<JobBase> JobLibrary<T>(this Conditioned<JobBase> conditionedDefinition)
        where T : JobLibrary, new()
    {
        conditionedDefinition.Definitions.Add(AzureDevOpsDefinition.CreateLibraryRef<T, JobBase>());
        return conditionedDefinition;
    }

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    public static Conditioned<Step> StepLibrary<T>(this Conditioned<Step> conditionedDefinition)
        where T : StepLibrary, new()
    {
        conditionedDefinition.Definitions.Add(AzureDevOpsDefinition.CreateLibraryRef<T, Step>());
        return conditionedDefinition;
    }

    /// <summary>
    /// Reference a step library (series of library Variables).
    /// </summary>
    public static Conditioned<VariableBase> VariableLibrary<T>(this Conditioned<VariableBase> conditionedDefinition)
        where T : VariableLibrary, new()
    {
        conditionedDefinition.Definitions.Add(AzureDevOpsDefinition.CreateLibraryRef<T, VariableBase>());
        return conditionedDefinition;
    }

    internal static Conditioned<T>? GetRoot<T>(this Conditioned<T> conditionedDefinition)
    {
        var parent = conditionedDefinition;
        while (parent?.Parent != null)
        {
            parent = parent.Parent as Conditioned<T>;
        }

        return parent;
    }
}
