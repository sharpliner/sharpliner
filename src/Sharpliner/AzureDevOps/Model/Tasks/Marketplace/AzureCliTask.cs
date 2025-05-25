﻿using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-cli-v2?view=azure-pipelines">official Azure DevOps pipelines documentation</see>
/// </summary>
public abstract record AzureCliTask : AzureDevOpsTask
{
    /// <summary>
    /// Alias: connectedServiceNameARM. Required. Azure Resource Manager connection.
    /// </summary>
    [YamlIgnore]
    public Conditioned<string>? AzureSubscription
    {
        get => GetConditioned<string>("azureSubscription");
        init => SetProperty("azureSubscription", value);
    }

    /// <summary>
    /// Required. Type of script. Select a bash or pscore script when running on Linux agent. Or, select a batch, ps, or pscore script when running on Windows agent. A pscore script can run on cross-platform agents (Linux, macOS, or Windows).
    /// </summary>
    [YamlIgnore]
    public Conditioned<ScriptType>? ScriptType
    {
        get => GetConditioned<ScriptType>("scriptType");
        init => SetProperty("scriptType", value);
    }

    /// <summary>
    /// Required. Allowed values: inlineScript (Inline script), scriptPath (Script path). Default value: scriptPath.
    /// </summary>
    [YamlIgnore]
    public Conditioned<ScriptLocation>? ScriptLocation
    {
        get => GetConditioned<ScriptLocation>("scriptLocation");
        init => SetProperty("scriptLocation", value);
    }

    /// <summary>
    /// Input alias: scriptArguments. Arguments passed to the script.
    /// </summary>
    [YamlIgnore]
    public Conditioned<string>? Arguments
    {
        get => GetConditioned<string>("arguments");
        init => SetProperty("arguments", value);
    }

    /// <summary>
    /// Optional. Use when scriptType = ps || scriptType = pscore. Allowed values: stop, continue, silentlyContinue. Default value: stop.
    /// Prepends the line $ErrorActionPreference = 'VALUE' at the top of your PowerShell/PowerShell Core script.
    /// </summary>
    [YamlIgnore]
    public Conditioned<PowerShellErrorActionPreference>? PowerShellErrorActionPreference
    {
        get => GetConditioned<PowerShellErrorActionPreference>("powerShellErrorActionPreference");
        init => SetProperty("powerShellErrorActionPreference", value);
    }

    /// <summary>
    /// Adds the service principal ID, service principal key or workload identity federation token, and tenant ID of the Azure endpoint you chose to the script's execution environment.
    /// You can use the servicePrincipalId, servicePrincipalKey or idToken, and tenantId variables in your script.
    /// </summary>
    [YamlIgnore]
    public Conditioned<bool>?  AddSpnToEnvironment
    {
        get => GetConditioned<bool>("addSpnToEnvironment", false);
        init => SetProperty("addSpnToEnvironment", value);
    }

    /// <summary>
    /// If this input is false, this task will use its own Azure CLI configuration directory.
    /// Use this task to run Azure CLI tasks in parallel releases.
    /// </summary>
    [YamlIgnore]
    public Conditioned<bool>? UseGlobalConfig
    {
        get => GetConditioned<bool>("useGlobalConfig", false);
        init => SetProperty("useGlobalConfig", value);
    }

    /// <summary>
    /// Current working directory where the script is run.
    /// If left blank, this input is the root of the repo (build) or artifacts (release), which is $(System.DefaultWorkingDirectory).
    /// </summary>
    [YamlIgnore]
    public Conditioned<string>? WorkingDirectory
    {
        get => GetConditioned<string>("workingDirectory");
        init => SetProperty("workingDirectory", value);
    }

    /// <summary>
    /// If this input is true, this task will fail when any errors are written to the StandardError stream.
    /// Clear the checkbox to ignore standard errors and instead rely on exit codes to determine the status.
    /// </summary>
    [YamlIgnore]
    public Conditioned<bool>? FailOnStandardError
    {
        get => GetConditioned<bool>("failOnStandardError", false);
        init => SetProperty("failOnStandardError", value);
    }

    /// <summary>
    /// Use when scriptType = ps || scriptType = pscore.
    /// If this input is false, the line if ((Test-Path -LiteralPath variable:\LASTEXITCODE)) { exit $LASTEXITCODE } is appended to the end of your script.
    /// This will propagate the last exit code from an external command as the exit code of PowerShell.
    /// Otherwise, the line is not appended to the end of your script.
    /// </summary>
    [YamlIgnore]
    public Conditioned<bool>? PowerShellIgnoreLASTEXITCODE
    {
        get => GetConditioned<bool>("powerShellIgnoreLASTEXITCODE", false);
        init => SetProperty("powerShellIgnoreLASTEXITCODE", value);
    }

    /// <summary>
    /// If this is set to true, az login command will output to the task.
    /// Setting it to false will suppress the az login output.
    /// </summary>
    [YamlIgnore]
    public Conditioned<bool>? VisibleAzLogin
    {
        get => GetConditioned<bool>("visibleAzLogin", true);
        init => SetProperty("visibleAzLogin", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureCliTask"/> class with required properties.
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection for the deployment.</param>
    /// <param name="scriptType">Type of script.</param>
    /// <param name="scriptLocation">Path to the script.</param>
    public AzureCliTask(Conditioned<string> azureSubscription, Conditioned<ScriptType> scriptType, Conditioned<ScriptLocation> scriptLocation)
        : base("AzureCLI@2")
    {
        AzureSubscription = azureSubscription;
        ScriptType = scriptType;
        ScriptLocation = scriptLocation;
    }
}

/// <summary>
/// Azure CLI Task with inline script
/// </summary>
public record InlineAzureCliTask : AzureCliTask
{
    /// <summary>
    /// Required when scriptLocation = inlineScript.
    /// You can write your scripts inline here. When using Windows agent, use PowerShell, PowerShell Core, or batch scripting.
    /// Use PowerShell Core or shell scripting when using Linux-based agents. For batch files, use the prefix call before every Azure command.
    /// You can also pass predefined and custom variables to this script by using arguments.
    /// </summary>
    [YamlIgnore]
    public Conditioned<string>? InlineScript
    {
        get => GetConditioned<string>("inlineScript");
        init => SetProperty("inlineScript", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InlineAzureCliTask"/> class with required properties.
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection for the deployment.</param>
    /// <param name="scriptType">Type of script.</param>
    /// <param name="inlineScript">Lines of the script as a string.</param>
    public InlineAzureCliTask(Conditioned<string> azureSubscription, Conditioned<ScriptType> scriptType, Conditioned<string> inlineScript)
    : base(azureSubscription, scriptType, Tasks.ScriptLocation.InlineScript)
    {
        InlineScript = inlineScript;
    }
}

/// <summary>
/// Azure CLI Task with file
/// </summary>
public record AzureCliFileTask : AzureCliTask
{
    /// <summary>
    /// Required when scriptLocation = scriptPath. Fully qualified path of the script. Use .ps1, .bat, or .cmd when using Windows-based agent.
    /// Use .ps1 or .sh when using Linux-based agent or a path relative to the the default working directory.
    /// </summary>
    [YamlIgnore]
    public Conditioned<string>? ScriptPath
    {
        get => GetConditioned<string>("scriptPath");
        init => SetProperty("scriptPath", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureCliFileTask"/> class with required properties.
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection for the deployment.</param>
    /// <param name="scriptType">Type of script.</param>
    /// <param name="scriptPath">Path to the script.</param>
    public AzureCliFileTask(Conditioned<string> azureSubscription, Conditioned<ScriptType> scriptType, Conditioned<string> scriptPath)
    : base(azureSubscription, scriptType, Tasks.ScriptLocation.ScriptPath)
    {
        ScriptPath = scriptPath;
    }
}

/// <summary>
/// Allowed values for ScriptType
/// </summary>
public enum ScriptType
{
    /// <summary>
    /// Powershell
    /// </summary>
    [YamlMember(Alias = "ps")]
    Ps,

    /// <summary>
    /// Powershell Core
    /// </summary>
    [YamlMember(Alias = "pscore")]
    Pscore,

    /// <summary>
    /// Batch (Shell)
    /// </summary>
    [YamlMember(Alias = "batch")]
    Batch,

    /// <summary>
    /// Bash (Shell)
    /// </summary>
    [YamlMember(Alias = "bash")]
    Bash,
}

/// <summary>
/// Allowed values for ScriptLocation
/// </summary>
public enum ScriptLocation
{
    /// <summary>
    /// Default. Script path
    /// </summary>
    [YamlMember(Alias = "scriptPath")]
    ScriptPath,

    /// <summary>
    /// Inline script
    /// </summary>
    [YamlMember(Alias = "inlineScript")]
    InlineScript,
}

/// <summary>
/// Allowed values for PowerShellErrorActionPreference
/// </summary>
public enum PowerShellErrorActionPreference
{
    /// <summary>
    /// Default. Stop
    /// </summary>
    [YamlMember(Alias = "stop")]
    Stop,

    /// <summary>
    /// Continue
    /// </summary>
    [YamlMember(Alias = "continue")]
    Continue,

    /// <summary>
    /// Silently Continue
    /// </summary>
    [YamlMember(Alias = "silentlyContinue")]
    SilentlyContinue,
}

