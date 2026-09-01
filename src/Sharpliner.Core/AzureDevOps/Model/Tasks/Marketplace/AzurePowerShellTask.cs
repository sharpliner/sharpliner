using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Runs a PowerShell script within an Azure Resource Manager subscription using the <c>AzurePowerShell@5</c> task.
/// More details can be found in the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-powershell-v5?view=azure-pipelines">official Azure DevOps pipelines documentation</see>
/// and the <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/AzurePowerShellV5/task.json">official AzurePowerShellV5 task specification</see>.
/// </summary>
public abstract record AzurePowerShellTask : AzureDevOpsTask
{
    /// <summary>
    /// Required. Azure Resource Manager service connection used to sign in before the script runs.
    /// The official input name is <c>ConnectedServiceNameARM</c>; <c>azureSubscription</c> is its YAML alias.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? AzureSubscription
    {
        get => GetExpression<string>("azureSubscription");
        init => SetProperty("azureSubscription", value);
    }

    /// <summary>
    /// Required. Whether the script is provided by a file path or inline.
    /// Allowed values are <c>FilePath</c> and <c>InlineScript</c>. Default value: <c>FilePath</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<AzurePowerShellScriptType>? ScriptType
    {
        get => GetExpression<AzurePowerShellScriptType>("ScriptType");
        init => SetProperty("ScriptType", value);
    }

    /// <summary>
    /// Optional. Allowed values are <c>stop</c>, <c>continue</c>, and <c>silentlyContinue</c>. Default value: <c>stop</c>.
    /// Prepends the line <c>$ErrorActionPreference = 'VALUE'</c> at the top of your script.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<PowerShellErrorActionPreference>? ErrorActionPreference
    {
        get => GetExpression<PowerShellErrorActionPreference>("errorActionPreference");
        init => SetProperty("errorActionPreference", value);
    }

    /// <summary>
    /// Optional. Default value: <c>false</c>. If this input is true, this task fails when any errors are written to the error pipeline
    /// or when any data is written to the standard error stream.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? FailOnStandardError
    {
        get => GetExpression<bool>("FailOnStandardError", false);
        init => SetProperty("FailOnStandardError", value);
    }

    /// <summary>
    /// Optional. Which Azure PowerShell version to use. Allowed values are <c>LatestVersion</c> (latest installed version)
    /// and <c>OtherVersion</c> (specify other version). Default value: <c>OtherVersion</c>.
    /// The official input name is <c>TargetAzurePs</c>; <c>azurePowerShellVersion</c> is its YAML alias.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<AzurePowerShellVersion>? AzurePowerShellVersion
    {
        get => GetExpression<AzurePowerShellVersion>("azurePowerShellVersion");
        init => SetProperty("azurePowerShellVersion", value);
    }

    /// <summary>
    /// Required when <c>azurePowerShellVersion = OtherVersion</c>. The Azure PowerShell module version to be used, for example <c>4.1.0</c>.
    /// The official input name is <c>CustomTargetAzurePs</c>; <c>preferredAzurePowerShellVersion</c> is its YAML alias.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PreferredAzurePowerShellVersion
    {
        get => GetExpression<string>("preferredAzurePowerShellVersion");
        init => SetProperty("preferredAzurePowerShellVersion", value);
    }

    /// <summary>
    /// Optional. Default value: <c>false</c>. If this input is true, the script runs in PowerShell Core (<c>pwsh.exe</c>);
    /// otherwise it runs in Windows PowerShell (<c>powershell.exe</c>).
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? Pwsh
    {
        get => GetExpression<bool>("pwsh", false);
        init => SetProperty("pwsh", value);
    }

    /// <summary>
    /// Optional. Working directory where the script is run.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? WorkingDirectory
    {
        get => GetExpression<string>("workingDirectory");
        init => SetProperty("workingDirectory", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzurePowerShellTask"/> class with required properties.
    /// </summary>
    /// <param name="task">Name of the task in the form <c>AzurePowerShell@5</c>.</param>
    /// <param name="azureSubscription">Azure Resource Manager service connection used to sign in before the script runs.</param>
    /// <param name="scriptType">Whether the script is provided by a file path or inline.</param>
    protected AzurePowerShellTask(string task, AdoExpression<string> azureSubscription, AdoExpression<AzurePowerShellScriptType> scriptType)
        : base(task)
    {
        AzureSubscription = azureSubscription;
        ScriptType = scriptType;
    }
}

/// <summary>
/// Azure PowerShell task (<c>AzurePowerShell@5</c>) with inline script content.
/// </summary>
public record InlineAzurePowerShellTask : AzurePowerShellTask
{
    /// <summary>
    /// Required when <c>ScriptType = InlineScript</c>. The script to run.
    /// You can also pass predefined and custom variables to this script.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Inline
    {
        get => GetExpression<string>("Inline");
        init => SetProperty("Inline", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InlineAzurePowerShellTask"/> class with required properties.
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection used to sign in before the script runs.</param>
    /// <param name="inlineScript">Lines of the script as a string.</param>
    public InlineAzurePowerShellTask(AdoExpression<string> azureSubscription, AdoExpression<string> inlineScript)
        : this("AzurePowerShell@5", azureSubscription, inlineScript)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InlineAzurePowerShellTask"/> class for a specific task major version.
    /// </summary>
    /// <param name="task">Name of the task in the form <c>AzurePowerShell@5</c>.</param>
    /// <param name="azureSubscription">Azure Resource Manager service connection used to sign in before the script runs.</param>
    /// <param name="inlineScript">Lines of the script as a string.</param>
    protected InlineAzurePowerShellTask(string task, AdoExpression<string> azureSubscription, AdoExpression<string> inlineScript)
        : base(task, azureSubscription, AzurePowerShellScriptType.InlineScript)
    {
        Inline = inlineScript;
    }
}

/// <summary>
/// Azure PowerShell task (<c>AzurePowerShell@5</c>) referencing a script file.
/// </summary>
public record AzurePowerShellFileTask : AzurePowerShellTask
{
    /// <summary>
    /// Required when <c>ScriptType = FilePath</c>. Path of the script to run.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ScriptPath
    {
        get => GetExpression<string>("ScriptPath");
        init => SetProperty("ScriptPath", value);
    }

    /// <summary>
    /// Optional. Use when <c>ScriptType = FilePath</c>. Additional parameters to pass to the script,
    /// either ordinal parameters or named parameters.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ScriptArguments
    {
        get => GetExpression<string>("ScriptArguments");
        init => SetProperty("ScriptArguments", value);
    }

    /// <summary>
    /// Optional. Use when <c>ScriptType = FilePath</c>. Default value: <c>false</c>.
    /// If this input is true, the task checks that the script is digitally signed and trusted before running it.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? ValidateScriptSignature
    {
        get => GetExpression<bool>("validateScriptSignature", false);
        init => SetProperty("validateScriptSignature", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzurePowerShellFileTask"/> class with required properties.
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection used to sign in before the script runs.</param>
    /// <param name="scriptPath">Path of the script to run.</param>
    public AzurePowerShellFileTask(AdoExpression<string> azureSubscription, AdoExpression<string> scriptPath)
        : this("AzurePowerShell@5", azureSubscription, scriptPath)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzurePowerShellFileTask"/> class for a specific task major version.
    /// </summary>
    /// <param name="task">Name of the task in the form <c>AzurePowerShell@5</c>.</param>
    /// <param name="azureSubscription">Azure Resource Manager service connection used to sign in before the script runs.</param>
    /// <param name="scriptPath">Path of the script to run.</param>
    protected AzurePowerShellFileTask(string task, AdoExpression<string> azureSubscription, AdoExpression<string> scriptPath)
        : base(task, azureSubscription, AzurePowerShellScriptType.FilePath)
    {
        ScriptPath = scriptPath;
    }
}

/// <summary>
/// Azure PowerShell task with inline script content targeting the <c>AzurePowerShell@4</c> major.
/// More details can be found in the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-powershell-v4?view=azure-pipelines">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record InlineAzurePowerShellV4Task : InlineAzurePowerShellTask
{
    /// <summary>
    /// Optional. Default value: <c>false</c>. If this input is true, the Azure context is restricted to the current task
    /// and is not shared with subsequent tasks in the job.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? RestrictContextToCurrentTask
    {
        get => GetExpression<bool>("RestrictContextToCurrentTask", false);
        init => SetProperty("RestrictContextToCurrentTask", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InlineAzurePowerShellV4Task"/> class with required properties.
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection used to sign in before the script runs.</param>
    /// <param name="inlineScript">Lines of the script as a string.</param>
    public InlineAzurePowerShellV4Task(AdoExpression<string> azureSubscription, AdoExpression<string> inlineScript)
        : base("AzurePowerShell@4", azureSubscription, inlineScript)
    {
    }
}

/// <summary>
/// Azure PowerShell task referencing a script file targeting the <c>AzurePowerShell@4</c> major.
/// More details can be found in the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-powershell-v4?view=azure-pipelines">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record AzurePowerShellV4FileTask : AzurePowerShellFileTask
{
    /// <summary>
    /// Optional. Default value: <c>false</c>. If this input is true, the Azure context is restricted to the current task
    /// and is not shared with subsequent tasks in the job.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? RestrictContextToCurrentTask
    {
        get => GetExpression<bool>("RestrictContextToCurrentTask", false);
        init => SetProperty("RestrictContextToCurrentTask", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzurePowerShellV4FileTask"/> class with required properties.
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection used to sign in before the script runs.</param>
    /// <param name="scriptPath">Path of the script to run.</param>
    public AzurePowerShellV4FileTask(AdoExpression<string> azureSubscription, AdoExpression<string> scriptPath)
        : base("AzurePowerShell@4", azureSubscription, scriptPath)
    {
    }
}

/// <summary>
/// Allowed values for the Azure PowerShell task <c>ScriptType</c> input.
/// </summary>
public enum AzurePowerShellScriptType
{
    /// <summary>
    /// Default. Script file path.
    /// </summary>
    [YamlMember(Alias = "FilePath")]
    FilePath,

    /// <summary>
    /// Inline script.
    /// </summary>
    [YamlMember(Alias = "InlineScript")]
    InlineScript,
}

/// <summary>
/// Allowed values for the Azure PowerShell task <c>azurePowerShellVersion</c> input.
/// </summary>
public enum AzurePowerShellVersion
{
    /// <summary>
    /// Latest installed version of Azure PowerShell on the agent.
    /// </summary>
    [YamlMember(Alias = "LatestVersion")]
    LatestVersion,

    /// <summary>
    /// Default. Specify other version through <see cref="AzurePowerShellTask.PreferredAzurePowerShellVersion"/>.
    /// </summary>
    [YamlMember(Alias = "OtherVersion")]
    OtherVersion,
}
