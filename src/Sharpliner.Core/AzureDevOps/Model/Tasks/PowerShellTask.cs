using System;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Base class for all PowerShell tasks.
/// </summary>
public abstract record PowershellTask : Step
{
    /// <summary>
    /// Specify the working directory in which you want to run the command.
    /// If you leave it empty, the working directory is $(Build.SourcesDirectory).
    /// </summary>
    [YamlMember(Order = 113)]
    public AdoExpression<string>? WorkingDirectory { get; init; }

    /// <summary>
    /// Prepends the line $ErrorActionPreference = 'VALUE' at the top of your script.
    /// Default value: `stop`.
    /// </summary>
    [YamlMember(Order = 114)]
    public AdoExpression<ActionPreference>? ErrorActionPreference { get; init; }

    /// <summary>
    /// Prepends the line $WarningPreference = 'VALUE' at the top of your script.
    /// Default value: `continue`.
    /// </summary>
    [YamlMember(Order = 114)]
    public AdoExpression<ActionPreference>? WarningPreference { get; init; }

    /// <summary>
    /// Prepends the line $InformationPreference = 'VALUE' at the top of your script.
    /// Default value: `continue`.
    /// </summary>
    [YamlMember(Order = 114)]
    public AdoExpression<ActionPreference>? InformationPreference { get; init; }

    /// <summary>
    /// Prepends the line $VerbosePreference = 'VALUE' at the top of your script.
    /// Default value: `continue`.
    /// </summary>
    [YamlMember(Order = 114)]
    public AdoExpression<ActionPreference>? VerbosePreference { get; init; }

    /// <summary>
    /// Prepends the line $DebugPreference = 'VALUE' at the top of your script.
    /// Default value: `continue`.
    /// </summary>
    [YamlMember(Order = 114)]
    public AdoExpression<ActionPreference>? DebugPreference { get; init; }

    /// <summary>
    /// If this is true, this task will fail if any errors are written to the error pipeline, or if any data is written to the Standard Error stream.
    /// Otherwise the task will rely on the exit code to determine failure
    /// Default value: `false`.
    /// </summary>
    [YamlMember(Order = 125)]
    public AdoExpression<bool>? FailOnStderr { get; init; }

    /// <summary>
    /// If this is false, the line if ((Test-Path -LiteralPath variable:\\LASTEXITCODE)) { exit $LASTEXITCODE } is appended to the end of your script.
    /// This will cause the last exit code from an external command to be propagated as the exit code of powershell.
    /// Otherwise the line is not appended to the end of your script
    /// Default value: `false`.
    /// </summary>
    [YamlMember(Order = 126)]
    public AdoExpression<bool>? IgnoreLASTEXITCODE { get; init; }
}

/// <summary>
/// Task that runs an inline PowerShell script using the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/steps-powershell">steps.powershell</see> definition.
/// </summary>
public record InlinePowershellTask : PowershellTask
{
    /// <summary>
    /// Required if Type is inline, contents of the script.
    /// </summary>
    [YamlMember(Alias = "powershell", Order = 1, ScalarStyle = ScalarStyle.Literal)]
    public AdoExpression<string>? Contents { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InlinePowershellTask"/> class with the specified script lines.
    /// </summary>
    /// <param name="scriptLines">Contents of the script (line by line)</param>
    public InlinePowershellTask(params string[] scriptLines)
    {
        Contents = string.Join(System.Environment.NewLine, scriptLines);
    }
}

/// <summary>
/// Task that runs a PowerShell script from a file using the <c>PowerShell</c> task.
/// </summary>
public record PowershellFileTask : PowershellTask, IYamlConvertible
{
    private readonly bool _isPwsh;

    /// <summary>
    /// Path of the script to execute.
    /// Must be a fully qualified path or relative to $(System.DefaultWorkingDirectory).
    /// </summary>
    public AdoExpression<string>? FilePath { get; }

    /// <summary>
    /// Arguments passed to the Bash script.
    /// </summary>
    public AdoExpression<string>? Arguments { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PowershellFileTask"/> class with the specified file path and whether to use PowerShell Core.
    /// </summary>
    /// <param name="filePath">The path to the script file.</param>
    /// <param name="isPwsh">Whether to use PowerShell Core.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public PowershellFileTask(AdoExpression<string> filePath, bool isPwsh)
    {
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        _isPwsh = isPwsh;
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

        var defaultValue = new PowershellFileTask(string.Empty, false);

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
        Add("pwsh", _isPwsh, defaultValue._isPwsh);

        nestedObjectSerializer(new AzureDevOpsTask("PowerShell@2", this)
        {
            Inputs = inputs
        });
    }
}

/// <summary>
/// Task that runs an inline PowerShell Core script using the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/steps-pwsh">steps.pwsh</see> definition.
/// </summary>
public record InlinePwshTask : PowershellTask
{
    /// <summary>
    /// Required if Type is inline, contents of the script.
    /// </summary>
    [YamlMember(Alias = "pwsh", Order = 1, ScalarStyle = ScalarStyle.Literal)]
    public AdoExpression<string>? Contents { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InlinePwshTask"/> class with the specified script lines.
    /// </summary>
    /// <param name="scriptLines"></param>
    public InlinePwshTask(params string[] scriptLines)
    {
        Contents = string.Join(System.Environment.NewLine, scriptLines);
    }
}

/// <summary>
/// The powershell action preference, see <see href="https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.actionpreference">ActionPreference Enum</see> for more details.
/// </summary>
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
