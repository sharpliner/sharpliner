using System;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Base class for all PowerShell tasks.
/// It only contains the options which are common to the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/steps-powershell">steps.powershell</see> and
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/steps-pwsh">steps.pwsh</see> shorthands and the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/powershell-v2">PowerShell@2</see> task.
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
    /// When set to anything other than <see cref="ActionPreference.Default"/>, prepends the line $ErrorActionPreference = 'VALUE' at the top of your script.
    /// When set to <see cref="ActionPreference.Default"/>, no line is prepended and PowerShell's own default is used.
    /// When left unset, the task prepends $ErrorActionPreference = 'Stop'.
    /// </summary>
    [YamlMember(Order = 114)]
    public AdoExpression<ActionPreference>? ErrorActionPreference { get; init; }

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
/// The script runs using Windows PowerShell on Windows and <c>pwsh</c> on Linux and macOS.
/// </summary>
/// <remarks>
/// The <c>powershell</c> shorthand only supports a subset of the <c>PowerShell@2</c> inputs.
/// Options such as the warning, information, verbose, debug and progress preferences are only available on <see cref="PowershellFileTask"/>.
/// </remarks>
public record InlinePowershellTask : PowershellTask
{
    /// <summary>
    /// Inline PowerShell script.
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
/// Task that runs a PowerShell script from a file using the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/powershell-v2">PowerShell@2</see> task.
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
    /// Arguments passed to the PowerShell script. Either ordinal parameters or named parameters.
    /// </summary>
    public AdoExpression<string>? Arguments { get; init; }

    /// <summary>
    /// When set to anything other than <see cref="ActionPreference.Default"/>, prepends the line $WarningPreference = 'VALUE' at the top of your script.
    /// When left unset, the task behaves as if <see cref="ActionPreference.Default"/> was used and no line is prepended.
    /// </summary>
    public AdoExpression<ActionPreference>? WarningPreference { get; init; }

    /// <summary>
    /// When set to anything other than <see cref="ActionPreference.Default"/>, prepends the line $InformationPreference = 'VALUE' at the top of your script.
    /// When left unset, the task behaves as if <see cref="ActionPreference.Default"/> was used and no line is prepended.
    /// </summary>
    public AdoExpression<ActionPreference>? InformationPreference { get; init; }

    /// <summary>
    /// When set to anything other than <see cref="ActionPreference.Default"/>, prepends the line $VerbosePreference = 'VALUE' at the top of your script.
    /// When left unset, the task behaves as if <see cref="ActionPreference.Default"/> was used and no line is prepended.
    /// </summary>
    public AdoExpression<ActionPreference>? VerbosePreference { get; init; }

    /// <summary>
    /// When set to anything other than <see cref="ActionPreference.Default"/>, prepends the line $DebugPreference = 'VALUE' at the top of your script.
    /// When left unset, the task behaves as if <see cref="ActionPreference.Default"/> was used and no line is prepended.
    /// </summary>
    public AdoExpression<ActionPreference>? DebugPreference { get; init; }

    /// <summary>
    /// When set to anything other than <see cref="ActionPreference.Default"/>, prepends the line $ProgressPreference = 'VALUE' at the top of your script.
    /// When set to <see cref="ActionPreference.Default"/>, no line is prepended and PowerShell's own default is used.
    /// When left unset, the task prepends $ProgressPreference = 'SilentlyContinue'.
    /// </summary>
    public AdoExpression<ActionPreference>? ProgressPreference { get; init; }

    /// <summary>
    /// If this is true, and your script writes warnings, they are shown as warnings also in the pipeline logs.
    /// Default value: `false`.
    /// </summary>
    public AdoExpression<bool>? ShowWarnings { get; init; }

    /// <summary>
    /// Executes the PowerShell script using the '&amp;' operator instead of the default '.' operator.
    /// When set to true, the script is executed in a separate scope and globally scoped PowerShell variables won't be updated.
    /// Default value: `false`.
    /// </summary>
    public AdoExpression<bool>? RunScriptInSeparateScope { get; init; }

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
        Add("errorActionPreference", ErrorActionPreference, defaultValue.ErrorActionPreference);
        Add("warningPreference", WarningPreference, defaultValue.WarningPreference);
        Add("informationPreference", InformationPreference, defaultValue.InformationPreference);
        Add("verbosePreference", VerbosePreference, defaultValue.VerbosePreference);
        Add("debugPreference", DebugPreference, defaultValue.DebugPreference);
        Add("progressPreference", ProgressPreference, defaultValue.ProgressPreference);
        Add("failOnStderr", FailOnStderr, defaultValue.FailOnStderr);
        Add("showWarnings", ShowWarnings, defaultValue.ShowWarnings);
        Add("ignoreLASTEXITCODE", IgnoreLASTEXITCODE, defaultValue.IgnoreLASTEXITCODE);
        Add("pwsh", _isPwsh, defaultValue._isPwsh);
        Add("workingDirectory", WorkingDirectory, defaultValue.WorkingDirectory);
        Add("runScriptInSeparateScope", RunScriptInSeparateScope, defaultValue.RunScriptInSeparateScope);

        nestedObjectSerializer(new AzureDevOpsTask("PowerShell@2", this)
        {
            Inputs = inputs
        });
    }
}

/// <summary>
/// Task that runs an inline PowerShell Core script using the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/steps-pwsh">steps.pwsh</see> definition.
/// The script runs in PowerShell Core on Windows, macOS and Linux.
/// </summary>
/// <remarks>
/// The <c>pwsh</c> shorthand only supports a subset of the <c>PowerShell@2</c> inputs.
/// Options such as the warning, information, verbose, debug and progress preferences are only available on <see cref="PowershellFileTask"/>.
/// </remarks>
public record InlinePwshTask : PowershellTask
{
    /// <summary>
    /// Inline PowerShell script.
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
/// The PowerShell action preference values accepted by the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/powershell-v2">PowerShell@2</see> task.
/// This is a subset of the <see href="https://learn.microsoft.com/en-us/dotnet/api/system.management.automation.actionpreference">ActionPreference Enum</see>;
/// any other value makes the task fail.
/// </summary>
public enum ActionPreference
{
    /// <summary>
    /// The preference variable is not set by the task and the PowerShell default is used.
    /// </summary>
    Default,

    /// <summary>
    /// Displays the message and continues executing.
    /// </summary>
    Continue,

    /// <summary>
    /// No effect. The message isn't displayed and execution continues without interruption.
    /// </summary>
    SilentlyContinue,

    /// <summary>
    /// Displays the message and stops executing. In addition to the error generated, the Stop value generates an ActionPreferenceStopException object to the error stream.
    /// </summary>
    Stop,
}
