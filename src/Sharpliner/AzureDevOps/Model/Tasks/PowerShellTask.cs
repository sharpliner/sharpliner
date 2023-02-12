using System;
using System.ComponentModel;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

public abstract record PowershellTask : Step
{
    /// <summary>
    /// Specify the working directory in which you want to run the command.
    /// If you leave it empty, the working directory is $(Build.SourcesDirectory).
    /// </summary>
    [YamlMember(Order = 113)]
    public Conditioned<string>? WorkingDirectory { get; init; }

    /// <summary>
    /// Prepends the line $ErrorActionPreference = 'VALUE' at the top of your script.
    /// Default value: `stop`.
    /// </summary>
    [YamlMember(Order = 114)]
    [DefaultValue(ActionPreference.Stop)]
    public ActionPreference ErrorActionPreference { get; init; } = ActionPreference.Stop;

    /// <summary>
    /// Prepends the line $WarningPreference = 'VALUE' at the top of your script.
    /// Default value: `continue`.
    /// </summary>
    [YamlMember(Order = 114)]
    [DefaultValue(ActionPreference.Continue)]
    public ActionPreference WarningPreference { get; init; } = ActionPreference.Continue;

    /// <summary>
    /// Prepends the line $InformationPreference = 'VALUE' at the top of your script.
    /// Default value: `continue`.
    /// </summary>
    [YamlMember(Order = 114)]
    [DefaultValue(ActionPreference.Continue)]
    public ActionPreference InformationPreference { get; init; } = ActionPreference.Continue;

    /// <summary>
    /// Prepends the line $VerbosePreference = 'VALUE' at the top of your script.
    /// Default value: `continue`.
    /// </summary>
    [YamlMember(Order = 114)]
    [DefaultValue(ActionPreference.Continue)]
    public ActionPreference VerbosePreference { get; init; } = ActionPreference.Continue;

    /// <summary>
    /// Prepends the line $DebugPreference = 'VALUE' at the top of your script.
    /// Default value: `continue`.
    /// </summary>
    [YamlMember(Order = 114)]
    [DefaultValue(ActionPreference.Continue)]
    public ActionPreference DebugPreference { get; init; } = ActionPreference.Continue;

    /// <summary>
    /// If this is true, this task will fail if any errors are written to the error pipeline, or if any data is written to the Standard Error stream.
    /// Otherwise the task will rely on the exit code to determine failure
    /// Default value: `false`.
    /// </summary>
    [YamlMember(Order = 125)]
    public Conditioned<bool>? FailOnStderr { get; init; }

    /// <summary>
    /// If this is false, the line if ((Test-Path -LiteralPath variable:\\LASTEXITCODE)) { exit $LASTEXITCODE } is appended to the end of your script.
    /// This will cause the last exit code from an external command to be propagated as the exit code of powershell.
    /// Otherwise the line is not appended to the end of your script
    /// Default value: `false`.
    /// </summary>
    [YamlMember(Order = 126)]
    public Conditioned<bool>? IgnoreLASTEXITCODE { get; init; }

    /// <summary>
    /// If this is true, then on Windows the task will use pwsh.exe from your PATH instead of powershell.exe.
    /// Default value: `false`.
    /// </summary>
    [YamlMember(Order = 127)]
    public bool Pwsh { get; init; }
}

public record InlinePowershellTask : PowershellTask
{
    /// <summary>
    /// Required if Type is inline, contents of the script.
    /// </summary>
    [YamlMember(Alias = "powershell", Order = 1, ScalarStyle = ScalarStyle.Literal)]
    public string Contents { get; init; }

    [YamlMember(Order = 2)]
    [DefaultValue("inline")]
    public string TargetType => "inline";

    public InlinePowershellTask(params string[] scriptLines)
    {
        Contents = string.Join(System.Environment.NewLine, scriptLines);
    }
}

public record PowershellFileTask : PowershellTask, IYamlConvertible
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

    public PowershellFileTask(string filePath)
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

        var defaultValue = new PowershellFileTask(string.Empty);

        Add("targetType", "filePath", null);
        Add("filePath", FilePath, null);
        Add("arguments", Arguments, defaultValue.Arguments);
        Add("workingDirectory", WorkingDirectory, defaultValue.WorkingDirectory);
        Add("errorActionPreference", ErrorActionPreference, defaultValue.ErrorActionPreference);
        Add("warningPreference", WarningPreference, defaultValue.WarningPreference);
        Add("informationPreference", InformationPreference, defaultValue.InformationPreference);
        Add("verbosePreference", VerbosePreference, defaultValue.VerbosePreference);
        Add("debugPreference", DebugPreference, defaultValue.DebugPreference);
        Add("failOnStderr", FailOnStderr, defaultValue.FailOnStderr);
        Add("ignoreLASTEXITCODE", IgnoreLASTEXITCODE, defaultValue.IgnoreLASTEXITCODE);
        Add("pwsh", Pwsh, defaultValue.Pwsh);

        nestedObjectSerializer(new AzureDevOpsTask("PowerShell@2", this)
        {
            Inputs = inputs
        });
    }
}

public enum ActionPreference
{
    /// <summary>
    /// (Default) Displays the error message and continues executing.
    /// </summary>
    Continue,

    /// <summary>
    /// Enter the debugger when an error occurs or when an exception is raised.
    /// </summary>
    Break,

    /// <summary>
    /// Suppresses the error message and continues to execute the command. The Ignore value is intended for per-command use, not for use as saved preference. Ignore isn't a valid value for the $ErrorActionPreference variable.
    /// </summary>
    Ignore,

    /// <summary>
    /// Displays the error message and asks you whether you want to continue.
    /// </summary>
    Inquire,

    /// <summary>
    /// No effect. The error message isn't displayed and execution continues without interruption.
    /// </summary>
    SilentlyContinue,

    /// <summary>
    /// Displays the error message and stops executing. In addition to the error generated, the Stop value generates an ActionPreferenceStopException object to the error stream. stream
    /// </summary>
    Stop,

    /// <summary>
    /// Automatically suspends a workflow job to allow for further investigation. After investigation, the workflow can be resumed. The Suspend value is intended for per-command use, not for use as saved preference. Suspend isn't a valid value for the $ErrorActionPreference variable.
    /// </summary>
    Suspend,
}
