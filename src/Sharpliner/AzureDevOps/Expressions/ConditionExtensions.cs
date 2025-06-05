using System;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Extension methods for creating conditioned definitions.
/// </summary>
public static class ConditionExtensions
{
    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="condition">Expression</param>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    public static AdoExpression<VariableBase> Variable(this IfCondition condition, string name, string value)
        => AdoExpression.Link<VariableBase>(condition, new Variable(name, value));

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="condition">Expression</param>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    public static AdoExpression<VariableBase> Variable(this IfCondition condition, string name, bool value)
        => AdoExpression.Link<VariableBase>(condition, new Variable(name, value));

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="condition">Expression</param>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    public static AdoExpression<VariableBase> Variable(this IfCondition condition, string name, int value)
        => AdoExpression.Link<VariableBase>(condition, new Variable(name, value));

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="condition">Expression</param>
    /// <param name="name">Variable name</param>
    /// <param name="value">Variable value</param>
    public static AdoExpression<VariableBase> Variable(this IfCondition condition, string name, Enum value)
        => AdoExpression.Link<VariableBase>(condition, new Variable(name, value));

    /// <summary>
    /// Defines a variable.
    /// </summary>
    /// <param name="condition">Expression</param>
    /// <param name="variable">Variable instance</param>
    public static AdoExpression<VariableBase> Variable(this IfCondition condition, Variable variable)
        => AdoExpression.Link<VariableBase>(condition, variable);

    /// <summary>
    /// Defines multiple variables at once.
    /// </summary>
    /// <param name="condition">Expression</param>
    /// <param name="variables">List of (key, value) pairs</param>
    public static AdoExpression<VariableBase> Variables(
        this IfCondition condition,
        params (string name, object value)[] variables)
    {
        var (name, value) = variables.First();
        var expression = value switch
        {
            int number => AdoExpression.Link<VariableBase>(condition, new Variable(name, number)),
            bool boolean => AdoExpression.Link<VariableBase>(condition, new Variable(name, boolean)),
            string s => AdoExpression.Link<VariableBase>(condition, new Variable(name, s)),
            object any => AdoExpression.Link<VariableBase>(condition, new Variable(name, any?.ToString() ?? string.Empty)),
        };

        if (variables.Length > 1)
        {
            expression = expression.Variables(variables.Skip(1).ToArray());
        }

        return expression;
    }

    /// <summary>
    /// References a variable group.
    /// </summary>
    public static AdoExpression<VariableBase> Group(this IfCondition condition, string name)
        => AdoExpression.Link<VariableBase>(condition, new VariableGroup(name));

    /// <summary>
    /// Creates a new stage.
    /// </summary>
    public static AdoExpression<Stage> Stage(this IfCondition condition, Stage stage)
        => AdoExpression.Link(condition, stage);

    /// <summary>
    /// Creates a new stage.
    /// </summary>
    public static AdoExpression<Stage> Stage(this IfCondition condition, AdoExpression<Stage> stage)
        => AdoExpression.Link(condition, stage);

    /// <summary>
    /// Creates a new job.
    /// </summary>
    public static AdoExpression<JobBase> Job(this IfCondition condition, JobBase job)
        => AdoExpression.Link(condition, job);

    /// <summary>
    /// Creates a new job.
    /// </summary>
    public static AdoExpression<JobBase> Job(this IfCondition condition, AdoExpression<JobBase> job)
        => AdoExpression.Link(condition, job);

    /// <summary>
    /// Creates a new deployment job.
    /// </summary>
    public static AdoExpression<JobBase> DeploymentJob(this IfCondition condition, JobBase job)
        => AdoExpression.Link(condition, job);

    /// <summary>
    /// Creates a new deployment job.
    /// </summary>
    public static AdoExpression<JobBase> DeploymentJob(this IfCondition condition, AdoExpression<JobBase> job)
        => AdoExpression.Link(condition, job);

    /// <summary>
    /// Creates a new step.
    /// </summary>
    public static AdoExpression<Step> Step(this IfCondition condition, Step step)
        => AdoExpression.Link(condition, step);

    /// <summary>
    /// Creates a new step.
    /// </summary>
    public static AdoExpression<Step> Step(this IfCondition condition, AdoExpression<Step> step)
        => AdoExpression.Link(condition, step);

    /// <summary>
    /// Creates a new pool.
    /// </summary>
    public static AdoExpression<Pool> Pool(this IfCondition condition, Pool pool)
        => AdoExpression.Link(condition, pool);

    /// <summary>
    /// Creates a new pool.
    /// </summary>
    public static AdoExpression<Pool> Pool(this IfCondition condition, AdoExpression<Pool> pool)
        => AdoExpression.Link(condition, pool);

    /// <summary>
    /// Reference a YAML stage template.
    /// </summary>
    /// <param name="condition">Expression</param>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    public static AdoExpression<Stage> StageTemplate(this IfCondition condition, string path, TemplateParameters? parameters = null)
        => AdoExpression.Link(condition, new Template<Stage>(condition: condition, path: path, parameters ?? []));

    /// <summary>
    /// Reference a YAML job template.
    /// </summary>
    /// <param name="condition">Expression</param>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    public static AdoExpression<JobBase> JobTemplate(this IfCondition condition, string path, TemplateParameters? parameters = null)
        => AdoExpression.Link(condition, new Template<JobBase>(condition: condition, path: path, parameters ?? []));

    /// <summary>
    /// Reference a YAML step template.
    /// </summary>
    /// <param name="condition">Expression</param>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    public static AdoExpression<Step> StepTemplate(this IfCondition condition, string path, TemplateParameters? parameters = null)
        => AdoExpression.Link(condition, new Template<Step>(condition: condition, path: path, parameters ?? []));

    /// <summary>
    /// Reference a YAML variable template.
    /// </summary>
    /// <param name="condition">Expression</param>
    /// <param name="path">Relative path to the YAML file with the template</param>
    /// <param name="parameters">Values for template parameters</param>
    public static AdoExpression<VariableBase> VariableTemplate(this IfCondition condition, string path, TemplateParameters? parameters = null)
        => AdoExpression.Link(condition, new Template<VariableBase>(condition: condition, path: path, parameters ?? []));

    /// <summary>
    /// Reference a stage library (series of library stages).
    /// </summary>
    public static AdoExpression<Stage> StageLibrary(this IfCondition condition, StageLibrary library)
        => AdoExpression.Link(condition, library.Items);

    /// <summary>
    /// Reference a job library (series of library jobs).
    /// </summary>
    public static AdoExpression<JobBase> JobLibrary(this IfCondition condition, JobLibrary library)
        => AdoExpression.Link(condition, library.Items);

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    public static AdoExpression<Step> StepLibrary(this IfCondition condition, StepLibrary library)
        => AdoExpression.Link(condition, library.Items);

    /// <summary>
    /// Reference a variable library (series of library Variables).
    /// </summary>
    public static AdoExpression<VariableBase> VariableLibrary(this IfCondition condition, VariableLibrary library)
        => AdoExpression.Link(condition, library.Items);

    /// <summary>
    /// Reference a step library (series of library stages).
    /// </summary>
    public static AdoExpression<Stage> StageLibrary<T>(this IfCondition condition) where T : StageLibrary, new()
        => AdoExpression.Link(condition, new T().Items);

    /// <summary>
    /// Reference a step library (series of library jobs).
    /// </summary>
    public static AdoExpression<JobBase> JobLibrary<T>(this IfCondition condition) where T : JobLibrary, new()
        => AdoExpression.Link(condition, new T().Items);

    /// <summary>
    /// Reference a step library (series of library steps).
    /// </summary>
    public static AdoExpression<Step> StepLibrary<T>(this IfCondition condition) where T : StepLibrary, new()
        => AdoExpression.Link(condition, new T().Items);

    /// <summary>
    /// Reference a step library (series of library Variables).
    /// </summary>
    public static AdoExpression<VariableBase> VariableLibrary<T>(this IfCondition condition) where T : VariableLibrary, new()
        => AdoExpression.Link(condition, new T().Items);

    /// <summary>
    /// Include a set of stages.
    /// </summary>
    public static AdoExpression<Stage> Stages(this IfCondition condition, params AdoExpression<Stage>[] stages)
        => AdoExpression.Link<Stage>(condition, stages);

    /// <summary>
    /// Include a set of jobs.
    /// </summary>
    public static AdoExpression<Job> Jobs(this IfCondition condition, params AdoExpression<Job>[] jobs)
        => AdoExpression.Link<Job>(condition, jobs);

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Step> Steps(this IfCondition condition, params AdoExpression<Step>[] steps)
        => AdoExpression.Link<Step>(condition, steps);

    /// <summary>
    /// Include a set of variables.
    /// </summary>
    public static AdoExpression<VariableBase> Variables(this IfCondition condition, params AdoExpression<VariableBase>[] variables)
        => AdoExpression.Link<VariableBase>(condition, variables);

    /// <summary>
    /// Include a set of stages.
    /// </summary>
    public static AdoExpression<Stage> Stages(this IfCondition condition, IEnumerable<AdoExpression<Stage>> stages)
        => AdoExpression.Link(condition, stages);

    /// <summary>
    /// Include a set of jobs.
    /// </summary>
    public static AdoExpression<Job> Jobs(this IfCondition condition, IEnumerable<AdoExpression<Job>> jobs)
        => AdoExpression.Link(condition, jobs);

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Step> Steps(this IfCondition condition, IEnumerable<AdoExpression<Step>> steps)
        => AdoExpression.Link(condition, steps);

    /// <summary>
    /// Include a set of variables.
    /// </summary>
    public static AdoExpression<VariableBase> Variables(this IfCondition condition, IEnumerable<AdoExpression<VariableBase>> variables)
        => AdoExpression.Link(condition, variables);

    /// <summary>
    /// Include a set of stages.
    /// </summary>
    public static AdoExpression<Stage> Stages(this IfCondition condition, params Stage[] stages)
        => AdoExpression.Link(condition, stages.Select(x => new AdoExpression<Stage>(x)));

    /// <summary>
    /// Include a set of jobs.
    /// </summary>
    public static AdoExpression<Job> Jobs(this IfCondition condition, params Job[] jobs)
        => AdoExpression.Link(condition, jobs.Select(x => new AdoExpression<Job>(x)));

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Step> Steps(this IfCondition condition, params Step[] steps)
        => AdoExpression.Link(condition, steps.Select(x => new AdoExpression<Step>(x)));

    /// <summary>
    /// Include a set of variables.
    /// </summary>
    public static AdoExpression<VariableBase> Variables(this IfCondition condition, params VariableBase[] variables)
        => AdoExpression.Link(condition, variables.Select(x => new AdoExpression<VariableBase>(x)));

    /// <summary>
    /// Include a set of stages.
    /// </summary>
    public static AdoExpression<Stage> Stages(this IfCondition condition, IEnumerable<Stage> stages)
        => condition.Stages(stages.ToArray());

    /// <summary>
    /// Include a set of jobs.
    /// </summary>
    public static AdoExpression<Job> Jobs(this IfCondition condition, IEnumerable<Job> jobs)
        => condition.Jobs(jobs.ToArray());

    /// <summary>
    /// Include a set of steps.
    /// </summary>
    public static AdoExpression<Step> Steps(this IfCondition condition, IEnumerable<Step> steps)
        => condition.Steps(steps.ToArray());

    /// <summary>
    /// Include a set of variables.
    /// </summary>
    public static AdoExpression<VariableBase> Variables(this IfCondition condition, IEnumerable<VariableBase> variables)
        => condition.Variables(variables.ToArray());

    /// <summary>
    /// Includes a strategy.
    /// </summary>
    public static AdoExpression<Strategy> Strategy(this IfCondition condition, Strategy strategy)
        => AdoExpression.Link(condition, strategy);

    /// <summary>
    /// Includes a generic object.
    /// </summary>
    public static AdoExpression<T> Value<T>(this IfCondition condition, T value)
        => AdoExpression.Link(condition, value);

    /// <summary>
    /// Starts an <c>${{ each var in collection }}</c> section.
    /// For example:
    /// <code lang="csharp">
    /// If.IsBranch("main")
    ///     .Each("env", "parameters.environments")
    ///         .Stage(new Stage("stage-${{ env.name }}")
    ///         {
    ///         })
    /// </code>
    /// will generate:
    /// <code lang="yaml">
    /// - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/main') }}:
    ///   - ${{ each env in parameters.stages }}:
    ///     - stage: stage-${{ env.name }}
    /// </code>
    /// </summary>
    /// <param name="condition">The current condition.</param>
    /// <param name="iterator">Name of the iterator variable.</param>
    /// <param name="collection">Collection to iterate over.</param>
    /// <returns>An if condition with an <c>each</c> expression appended.</returns>
    public static IfCondition Each(this IfCondition condition, string iterator, string collection)
    {
        condition.EachExpression = new(iterator, collection);
        return condition;
    }
}
