using System;
using Sharpliner.AzureDevOps.Expressions;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Base class for Bash steps. Common properties are emitted either as <c>steps.bash</c> shortcut properties
/// or as <c>Bash@3</c> task inputs/step properties, depending on the concrete task type.
/// More details can be found in the official
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/steps-bash">steps.bash schema</see>
/// and <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/bash-v3">Bash@3 task reference</see>.
/// </summary>
public abstract record BashTask : Step
{
    /// <summary>
    /// Specify the working directory in which you want to run the command.
    /// If you leave it empty, the working directory is $(Build.SourcesDirectory).
    /// </summary>
    [YamlMember(Order = 113)]
    public AdoExpression<string>? WorkingDirectory { get; init; }

    /// <summary>
    /// If this is true, this task will fail if any errors are written to stderr.
    /// Default value: `false`.
    /// </summary>
    [YamlMember(Order = 114)]
    public AdoExpression<bool>? FailOnStderr { get; init; }

    /// <summary>
    /// This property is not supported by Azure Pipelines <c>steps.bash</c> or <c>Bash@3</c> and is not emitted.
    /// </summary>
    [Obsolete("NoProfile is not supported by Azure Pipelines steps.bash or Bash@3 and is not emitted.")]
    [YamlIgnore]
    public AdoExpression<bool>? NoProfile { get; init; }

    /// <summary>
    /// This property is not supported by Azure Pipelines <c>steps.bash</c> or <c>Bash@3</c> and is not emitted.
    /// </summary>
    [Obsolete("NoRc is not supported by Azure Pipelines steps.bash or Bash@3 and is not emitted.")]
    [YamlIgnore]
    public AdoExpression<bool>? NoRc { get; init; }

    /// <summary>
    /// Environment in which to run this Bash step. Use <c>"host"</c>, a container name, or a <see cref="StepTarget"/> object.
    /// This is a step property, not a <c>Bash@3</c> task input.
    /// </summary>
    [YamlMember(Order = 215)]
    public AdoExpression<object>? Target { get; init; }

    /// <summary>
    /// Number of retries if this Bash step fails. Default is 0.
    /// This is a step property, not a <c>Bash@3</c> task input.
    /// </summary>
    [YamlMember(Order = 230)]
    public AdoExpression<int>? RetryCountOnTaskFailure { get; init; }
}

/// <summary>
/// Configures the Azure Pipelines step target used by task and script steps.
/// </summary>
public record StepTarget
{
    /// <summary>
    /// Container to target, or <c>host</c> for the agent host.
    /// </summary>
    public AdoExpression<string>? Container { get; init; }

    /// <summary>
    /// Set of allowed logging commands. Defaults to <see cref="StepTargetCommands.Any"/> when omitted by Azure Pipelines.
    /// </summary>
    public AdoExpression<StepTargetCommands>? Commands { get; init; }

    /// <summary>
    /// Restrictions on which variables this step can set. Use <see cref="StepTargetSettableVariables.None"/>
    /// to disable setting variables, or <see cref="StepTargetSettableVariables.Allowed"/> to allow only specific variables.
    /// </summary>
    public AdoExpression<StepTargetSettableVariables>? SettableVariables { get; init; }
}

/// <summary>
/// Restrictions on which variables a targeted Azure Pipelines step can set.
/// </summary>
public sealed class StepTargetSettableVariables : IYamlConvertible
{
    private const string NoneValue = "none";

    private readonly IReadOnlyList<string>? _variables;

    /// <summary>
    /// Disables setting variables from this step.
    /// </summary>
    public static StepTargetSettableVariables None { get; } = new(NoneValue);

    private StepTargetSettableVariables(string value)
    {
        Value = value;
    }

    private StepTargetSettableVariables(IReadOnlyList<string> variables)
    {
        _variables = variables;
    }

    private string? Value { get; }

    /// <summary>
    /// Variable names that this step may set, or <c>null</c> when this instance disables setting variables.
    /// </summary>
    public IReadOnlyList<string>? Variables => _variables;

    /// <summary>
    /// Restricts variable setting to the specified variable names.
    /// </summary>
    /// <param name="variables">Variable names that this step may set.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="variables"/> is empty.</exception>
    public static StepTargetSettableVariables Allowed(params string[] variables)
    {
        ArgumentNullException.ThrowIfNull(variables);

        if (variables.Length == 0)
        {
            throw new ArgumentException("At least one variable name must be specified.", nameof(variables));
        }

        return new(variables);
    }

    void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        => throw new NotImplementedException();

    void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        if (Value is not null)
        {
            emitter.Emit(new Scalar(Value));
            return;
        }

        emitter.Emit(new SequenceStart(AnchorName.Empty, TagName.Empty, true, SequenceStyle.Block));

        foreach (var variable in _variables ?? [])
        {
            emitter.Emit(new Scalar(variable));
        }

        emitter.Emit(new SequenceEnd());
    }
}

/// <summary>
/// Allowed Azure Pipelines logging command modes for a step target.
/// </summary>
public enum StepTargetCommands
{
    /// <summary>
    /// Allow any supported logging command.
    /// </summary>
    [YamlMember(Alias = "any")]
    Any,

    /// <summary>
    /// Restrict logging commands that this step may use.
    /// </summary>
    [YamlMember(Alias = "restricted")]
    Restricted,
}

/// <summary>
/// Task that runs an inline Bash script using the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/steps-bash">steps.bash</see> definition.
/// </summary>
public record InlineBashTask : BashTask
{
    /// <summary>
    /// Inline Bash script content emitted as the required first <c>bash</c> property of the <c>steps.bash</c> shortcut.
    /// </summary>
    [YamlMember(Alias = "bash", Order = 1, ScalarStyle = ScalarStyle.Literal)]
    public string Contents { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InlineBashTask"/> class with the specified script lines.
    /// </summary>
    /// <param name="scriptLines">The lines of the script to execute.</param>
    public InlineBashTask(params string[] scriptLines)
    {
        ArgumentNullException.ThrowIfNull(scriptLines);
        Contents = string.Join("\n", scriptLines);
    }
}

/// <summary>
/// Task that runs a Bash script from a file using the <c>Bash@3</c> task syntax.
/// </summary>
public record BashFileTask : BashTask, IYamlConvertible
{
    /// <summary>
    /// Path of the script to execute.
    /// Must be a fully qualified path or relative to $(System.DefaultWorkingDirectory).
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Arguments passed to the Bash script.
    /// </summary>
    public AdoExpression<string>? Arguments { get; init; }

    /// <summary>
    /// If specified, emits the <c>bashEnvValue</c> input and uses the value as the path of a startup file
    /// that is executed before running the script.
    ///
    /// If the environment variable BASH_ENV has already been defined, the task will override
    /// this variable only for the current task.
    /// This <c>Bash@3</c> input is not available on the <c>steps.bash</c> shortcut; use <see cref="Step.Env"/>
    /// with a <c>BASH_ENV</c> entry for shortcut steps.
    /// </summary>
    public AdoExpression<string>? BashEnv { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BashFileTask"/> class with the specified file path.
    /// </summary>
    /// <param name="filePath">The path of the script to execute.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="filePath"/> is null.</exception>
    public BashFileTask(string filePath)
    {
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        => throw new NotImplementedException();

    // This is unfortunately needed because when referencing a script file, the "bash: ..." variant does not work
    void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        var inputs = new TaskInputs();

        void Add(string key, object? value, object? defaultValue)
        {
            if (value is null)
            {
                return;
            }

            if (!value.Equals(defaultValue))
            {
                inputs![key] = value;
            }
        }

        var defaultValue = new BashFileTask(string.Empty);

        Add("targetType", "filePath", null);
        Add("filePath", FilePath, null);
        Add("arguments", Arguments, defaultValue.Arguments?.Definition);
        Add("workingDirectory", WorkingDirectory, defaultValue.WorkingDirectory?.Definition);
        Add("failOnStderr", FailOnStderr, defaultValue.FailOnStderr);
        Add("bashEnvValue", BashEnv, defaultValue.BashEnv?.Definition);

        nestedObjectSerializer(new AzureDevOpsTask("Bash@3", this)
        {
            Inputs = inputs,
            Target = Target,
            RetryCountOnTaskFailure = RetryCountOnTaskFailure,
        });
    }
}
