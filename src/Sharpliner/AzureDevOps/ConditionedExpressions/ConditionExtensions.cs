using System.Linq;
using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

public static class ConditionExtensions
{
    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="condition">Conditioned definition</param>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    public static Conditioned<VariableBase> Variable(this Condition condition, string name, string value)
        => Conditioned.Link<VariableBase>(condition, new Variable(name, value));

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="condition">Conditioned definition</param>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    public static Conditioned<VariableBase> Variable(this Condition condition, string name, bool value)
        => Conditioned.Link<VariableBase>(condition, new Variable(name, value));

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="condition">Conditioned definition</param>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    public static Conditioned<VariableBase> Variable(this Condition condition, string name, int value)
        => Conditioned.Link<VariableBase>(condition, new Variable(name, value));

    /// <summary>
    /// Defines multiple variables at once.
    /// </summary>
    /// <param name="condition">Conditioned definition</param>
    /// <param name="variables">List of (key, value) pairs</param>
    public static Conditioned<VariableBase> Variables(
        this Condition condition,
        params (string name, object value)[] variables)
    {
        var (name, value) = variables.First();
        var conditionedDefinition = value switch
        {
            int number => Conditioned.Link<VariableBase>(condition, new Variable(name, number)),
            bool boolean => Conditioned.Link<VariableBase>(condition, new Variable(name, boolean)),
            string s => Conditioned.Link<VariableBase>(condition, new Variable(name, s)),
            object any => Conditioned.Link<VariableBase>(condition, new Variable(name, any?.ToString() ?? string.Empty)),
        };

        if (variables.Length > 1)
        {
            conditionedDefinition = conditionedDefinition.Variables(variables.Skip(1).ToArray());
        }

        return conditionedDefinition;
    }

    /// <summary>
    /// References a variable group.
    /// </summary>
    public static Conditioned<VariableBase> Group(this Condition condition, string name)
        => Conditioned.Link<VariableBase>(condition, new VariableGroup(name));

    /// <summary>
    /// Creates a new stage.
    /// </summary>
    public static Conditioned<Stage> Stage(this Condition condition, Stage stage)
        => Conditioned.Link(condition, stage);

    /// <summary>
    /// Creates a new stage.
    /// </summary>
    public static Conditioned<Stage> Stage(this Condition condition, Conditioned<Stage> stage)
        => Conditioned.Link(condition, stage);

    /// <summary>
    /// Creates a new job.
    /// </summary>
    public static Conditioned<JobBase> Job(this Condition condition, JobBase job)
        => Conditioned.Link(condition, job);

    /// <summary>
    /// Creates a new job.
    /// </summary>
    public static Conditioned<JobBase> Job(this Condition condition, Conditioned<JobBase> job)
        => Conditioned.Link(condition, job);

    /// <summary>
    /// Creates a new deployment job.
    /// </summary>
    public static Conditioned<JobBase> DeploymentJob(this Condition condition, JobBase job)
        => Conditioned.Link(condition, job);

    /// <summary>
    /// Creates a new deployment job.
    /// </summary>
    public static Conditioned<JobBase> DeploymentJob(this Condition condition, Conditioned<JobBase> job)
        => Conditioned.Link(condition, job);

    /// <summary>
    /// Creates a new step.
    /// </summary>
    public static Conditioned<Step> Step(this Condition condition, Step step)
        => Conditioned.Link(condition, step);

    /// <summary>
    /// Creates a new step.
    /// </summary>
    public static Conditioned<Step> Step(this Condition condition, Conditioned<Step> step)
        => Conditioned.Link(condition, step);

    /// <summary>
    /// Creates a new pool.
    /// </summary>
    public static Conditioned<Pool> Pool(this Condition condition, Pool pool)
        => Conditioned.Link(condition, pool);

    /// <summary>
    /// Creates a new pool.
    /// </summary>
    public static Conditioned<Pool> Pool(this Condition condition, Conditioned<Pool> pool)
        => Conditioned.Link(condition, pool);

    /// <summary>
    /// Reference a YAML stage template.
    /// </summary>
    /// <param name="condition">Conditioned definition</param>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    public static Conditioned<Stage> StageTemplate(this Condition condition, string path, TemplateParameters? parameters = null)
        => Conditioned.Link(condition, new Template<Stage>(condition: condition.ToString(), path: path, parameters ?? new TemplateParameters()));

    /// <summary>
    /// Reference a YAML job template.
    /// </summary>
    /// <param name="condition">Conditioned definition</param>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    public static Conditioned<JobBase> JobTemplate(this Condition condition, string path, TemplateParameters? parameters = null)
        => Conditioned.Link(condition, new Template<JobBase>(condition: condition.ToString(), path: path, parameters ?? new TemplateParameters()));

    /// <summary>
    /// Reference a YAML step template.
    /// </summary>
    /// <param name="condition">Conditioned definition</param>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    public static Conditioned<Step> StepTemplate(this Condition condition, string path, TemplateParameters? parameters = null)
        => Conditioned.Link(condition, new Template<Step>(condition: condition.ToString(), path: path, parameters ?? new TemplateParameters()));

    /// <summary>
    /// Reference a YAML variable template.
    /// </summary>
    /// <param name="condition">Conditioned definition</param>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    public static Conditioned<VariableBase> VariableTemplate(this Condition condition, string path, TemplateParameters? parameters = null)
        => Conditioned.Link(condition, new Template<VariableBase>(condition: condition.ToString(), path: path, parameters ?? new TemplateParameters()));

    /// <summary>
    /// Reference a step library (series of library stages).
    /// </summary>
    public static Conditioned<Stage> StageLibrary(this Condition condition, StageLibrary library)
        => Conditioned.Link(condition, library.Items);

    /// <summary>
    /// Reference a step library (series of library jobs).
    /// </summary>
    public static Conditioned<Job> JobLibrary(this Condition condition, JobLibrary library)
        => Conditioned.Link(condition, library.Items);

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    public static Conditioned<Step> StepLibrary(this Condition condition, StepLibrary library)
        => Conditioned.Link(condition, library.Items);

    /// <summary>
    /// Reference a step library (series of library Variables).
    /// </summary>
    public static Conditioned<VariableBase> VariableLibrary(this Condition condition, VariableLibrary library)
        => Conditioned.Link(condition, library.Items);

    /// <summary>
    /// Reference a step library (series of library stages).
    /// </summary>
    public static Conditioned<Stage> StageLibrary<T>(this Condition condition)
        where T : StageLibrary, new()
        => Conditioned.Link(condition, AzureDevOpsDefinition.CreateInstance<T>().Items);

    /// <summary>
    /// Reference a step library (series of library jobs).
    /// </summary>
    public static Conditioned<Job> JobLibrary<T>(this Condition condition)
        where T : JobLibrary, new()
        => Conditioned.Link(condition, AzureDevOpsDefinition.CreateInstance<T>().Items);

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    public static Conditioned<Step> StepLibrary<T>(this Condition condition)
        where T : StepLibrary, new()
        => Conditioned.Link(condition, AzureDevOpsDefinition.CreateInstance<T>().Items);

    /// <summary>
    /// Reference a step library (series of library Variables).
    /// </summary>
    public static Conditioned<VariableBase> VariableLibrary<T>(this Condition condition)
        where T : VariableLibrary, new()
        => Conditioned.Link(condition, AzureDevOpsDefinition.CreateInstance<T>().Items);

    public static Conditioned<Strategy> Strategy(this Condition condition, Strategy strategy)
        => Conditioned.Link(condition, strategy);

    public static Conditioned<T> Value<T>(this Condition condition, T value)
        => Conditioned.Link(condition, value);
}
