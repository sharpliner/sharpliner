using System;
using System.ComponentModel;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://github.com/MicrosoftDocs/azure-devops-docs/blob/master/docs/pipelines/tasks/utility/bash.md">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract record BashTask : Step
{
    /// <summary>
    /// Specify the working directory in which you want to run the command.
    /// If you leave it empty, the working directory is $(Build.SourcesDirectory).
    /// </summary>
    [YamlMember(Order = 113)]
    public Conditioned<string>? WorkingDirectory { get; init; }

    /// <summary>
    /// If this is true, this task will fail if any errors are written to stderr.
    /// Default value: `false`.
    /// </summary>
    [YamlMember(Order = 114)]
    public Conditioned<bool>? FailOnStderr { get; init; }

    /// <summary>
    /// Don't load the system-wide startup file **`/etc/profile`** or any of the personal initialization files.
    /// </summary>
    [YamlMember(Order = 115)]
    public Conditioned<bool>? NoProfile { get; init; }

    /// <summary>
    /// If this is true, the task will not process `.bashrc` from the user's home directory.
    /// Default value: `true`.
    /// </summary>
    [YamlMember(Order = 200)]
    [DefaultValue(true)]
    public Conditioned<bool>? NoRc { get; init; } = true;
}

/// <summary>
/// Task that runs an inline Bash script using the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/steps-bash">steps.bash</see> definition.
/// </summary>
public record InlineBashTask : BashTask
{
    /// <summary>
    /// Required if Type is inline, contents of the script.
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
/// Task that runs a bash script from a file using the <c>Bash</c> task.
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
    public Conditioned<string>? Arguments { get; init; }

    /// <summary>
    /// If the related input is specified, the value will be used as the path of a startup file
    /// that will be executed before running the script.
    ///
    /// If the environment variable BASH_ENV has already been defined, the task will override
    /// this variable only for the current task.
    /// </summary>
    public Conditioned<string>? BashEnv { get; init; }

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

    // This is unfortunately needed because when referencing a script file, the "powershell: ..." variant does not work
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
            Inputs = inputs
        });
    }
}
