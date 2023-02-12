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
    public bool? FailOnStderr { get; init; }

    /// <summary>
    /// Don't load the system-wide startup file **`/etc/profile`** or any of the personal initialization files.
    /// </summary>
    [YamlMember(Order = 115)]
    public bool? NoProfile { get; init; }

    /// <summary>
    /// If this is true, the task will not process `.bashrc` from the user's home directory.
    /// Default value: `true`.
    /// </summary>
    [YamlMember(Order = 200)]
    [DefaultValue(true)]
    public bool NoRc { get; init; } = true;
}

public record InlineBashTask : BashTask
{
    /// <summary>
    /// Required if Type is inline, contents of the script.
    /// </summary>
    [YamlMember(Alias = "bash", Order = 1, ScalarStyle = ScalarStyle.Literal)]
    public string Contents { get; }

    public InlineBashTask(params string[] scriptLines)
    {
        if (scriptLines is null)
        {
            throw new ArgumentNullException(nameof(scriptLines));
        }

        Contents = string.Join("\n", scriptLines);
    }
}

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

    public BashFileTask(string filePath)
    {
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
    }

    public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        => throw new NotImplementedException();

    // This is unfortunately needed because when referencing a script file, the "powershell: ..." variant does not work
    public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
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
