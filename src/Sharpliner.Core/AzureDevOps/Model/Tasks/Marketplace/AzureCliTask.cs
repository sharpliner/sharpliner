using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Runs Azure CLI commands against an Azure subscription using the <c>AzureCLI@2</c> task.
/// More details can be found in the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-cli-v2?view=azure-pipelines">official Azure DevOps pipelines documentation</see>
/// and the <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/9dabcbcbcbc3b5a1a94fd32acaa2766fdf934bd6/Tasks/AzureCLIV2/task.json">official AzureCLIV2 task specification audited on 2026-08-30</see>.
/// </summary>
public abstract record AzureCliTask : AzureDevOpsTask
{
    /// <summary>
    /// Required. Azure Resource Manager service connection for the deployment.
    /// The official input name is <c>connectedServiceNameARM</c>; <c>azureSubscription</c> is its YAML alias and is emitted for compatibility with existing Sharpliner output.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? AzureSubscription
    {
        get => GetExpression<string>("azureSubscription");
        init => SetProperty("azureSubscription", value);
    }

    /// <summary>
    /// Required. Type of script. Allowed values are <c>ps</c> (PowerShell), <c>pscore</c> (PowerShell Core),
    /// <c>batch</c> (Batch), and <c>bash</c> (Shell). Select Shell or PowerShell Core on Linux agents,
    /// Batch, PowerShell, or PowerShell Core on Windows agents; PowerShell Core can run on Linux, macOS, or Windows agents.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<ScriptType>? ScriptType
    {
        get => GetExpression<ScriptType>("scriptType");
        init => SetProperty("scriptType", value);
    }

    /// <summary>
    /// Required. Script location. Allowed values are <c>inlineScript</c> (Inline script) and <c>scriptPath</c> (Script path).
    /// Default value: <c>scriptPath</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<ScriptLocation>? ScriptLocation
    {
        get => GetExpression<ScriptLocation>("scriptLocation");
        init => SetProperty("scriptLocation", value);
    }

    /// <summary>
    /// Optional. Arguments passed to the script.
    /// The official input name is <c>scriptArguments</c>; <c>arguments</c> is its YAML alias and is emitted for compatibility with existing Sharpliner output.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Arguments
    {
        get => GetExpression<string>("arguments");
        init => SetProperty("arguments", value);
    }

    /// <summary>
    /// Optional. Use when <c>scriptType = ps || scriptType = pscore</c>. Allowed values are <c>stop</c>, <c>continue</c>, and <c>silentlyContinue</c>.
    /// Default value: <c>stop</c>. Prepends the line <c>$ErrorActionPreference = 'VALUE'</c> at the top of your PowerShell/PowerShell Core script.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<PowerShellErrorActionPreference>? PowerShellErrorActionPreference
    {
        get => GetExpression<PowerShellErrorActionPreference>("powerShellErrorActionPreference");
        init => SetProperty("powerShellErrorActionPreference", value);
    }

    /// <summary>
    /// Optional. Default value: <c>false</c>. Adds the service principal ID, service principal key, and tenant ID of the Azure endpoint to the script's execution environment.
    /// This is honored only when the Azure endpoint uses the Service Principal authentication scheme.
    /// Access the variables as <c>$env:servicePrincipalId</c> in PowerShell, <c>%servicePrincipalId%</c> in Batch, and <c>$servicePrincipalId</c> in Shell.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>?  AddSpnToEnvironment
    {
        get => GetExpression<bool>("addSpnToEnvironment", false);
        init => SetProperty("addSpnToEnvironment", value);
    }

    /// <summary>
    /// Optional. Default value: <c>false</c>. If this input is false, this task uses its own separate Azure CLI configuration directory.
    /// This can be used to run Azure CLI tasks in parallel releases.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? UseGlobalConfig
    {
        get => GetExpression<bool>("useGlobalConfig", false);
        init => SetProperty("useGlobalConfig", value);
    }

    /// <summary>
    /// Optional. Current working directory where the script is run.
    /// Empty is the root of the repo (build) or artifacts (release), which is <c>$(System.DefaultWorkingDirectory)</c>.
    /// The official input name is <c>cwd</c>; <c>workingDirectory</c> is its YAML alias and is emitted for compatibility with existing Sharpliner output.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? WorkingDirectory
    {
        get => GetExpression<string>("workingDirectory");
        init => SetProperty("workingDirectory", value);
    }

    /// <summary>
    /// Optional. Default value: <c>false</c>. If this input is true, this task fails when any errors are written to the StandardError stream.
    /// Set to false to ignore standard errors and rely on exit codes to determine the status.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? FailOnStandardError
    {
        get => GetExpression<bool>("failOnStandardError", false);
        init => SetProperty("failOnStandardError", value);
    }

    /// <summary>
    /// Optional. Use when <c>scriptType = ps || scriptType = pscore</c>. Default value: <c>false</c>.
    /// If this input is false, the line <c>if ((Test-Path -LiteralPath variable:\LASTEXITCODE)) { exit $LASTEXITCODE }</c> is appended to the end of your script.
    /// This propagates the last exit code from an external command as the exit code of PowerShell.
    /// Otherwise, the line is not appended to the end of your script.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? PowerShellIgnoreLASTEXITCODE
    {
        get => GetExpression<bool>("powerShellIgnoreLASTEXITCODE", false);
        init => SetProperty("powerShellIgnoreLASTEXITCODE", value);
    }

    /// <summary>
    /// Optional. Default value: <c>true</c>. If this is set to true, <c>az login</c> command output is written to the task log.
    /// Setting it to false suppresses the <c>az login</c> output.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? VisibleAzLogin
    {
        get => GetExpression<bool>("visibleAzLogin", true);
        init => SetProperty("visibleAzLogin", value);
    }

    /// <summary>
    /// Optional. Experimental. Default value: <c>false</c>. When enabled, this task continuously signs in to Azure to avoid
    /// <c>AADSTS700024</c> errors when requesting access tokens beyond the ID token expiry date.
    /// Valid only for service connections using the Workload Identity Federation authentication scheme.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? KeepAzSessionActive
    {
        get => GetExpression<bool>("keepAzSessionActive", false);
        init => SetProperty("keepAzSessionActive", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureCliTask"/> class with required properties.
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection for the deployment.</param>
    /// <param name="scriptType">Type of script.</param>
    /// <param name="scriptLocation">Whether the script is provided inline or by file path.</param>
    public AzureCliTask(AdoExpression<string> azureSubscription, AdoExpression<ScriptType> scriptType, AdoExpression<ScriptLocation> scriptLocation)
        : base("AzureCLI@2")
    {
        AzureSubscription = azureSubscription;
        ScriptType = scriptType;
        ScriptLocation = scriptLocation;
    }
}

/// <summary>
/// Azure CLI task with inline script content.
/// </summary>
public record InlineAzureCliTask : AzureCliTask
{
    /// <summary>
    /// Required when <c>scriptLocation = inlineScript</c>.
    /// You can write your scripts inline here. When using Windows agent, use PowerShell, PowerShell Core, or batch scripting.
    /// Use PowerShell Core or shell scripting when using Linux-based agents. For batch files, use the prefix <c>call</c> before every Azure command.
    /// You can also pass predefined and custom variables to this script by using <see cref="AzureCliTask.Arguments"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? InlineScript
    {
        get => GetExpression<string>("inlineScript");
        init => SetProperty("inlineScript", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InlineAzureCliTask"/> class with required properties.
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection for the deployment.</param>
    /// <param name="scriptType">Type of script.</param>
    /// <param name="inlineScript">Lines of the script as a string.</param>
    public InlineAzureCliTask(AdoExpression<string> azureSubscription, AdoExpression<ScriptType> scriptType, AdoExpression<string> inlineScript)
    : base(azureSubscription, scriptType, Tasks.ScriptLocation.InlineScript)
    {
        InlineScript = inlineScript;
    }
}

/// <summary>
/// Azure CLI task with script file path.
/// </summary>
public record AzureCliFileTask : AzureCliTask
{
    /// <summary>
    /// Required when <c>scriptLocation = scriptPath</c>. Fully qualified path of the script, or a path relative to the default working directory.
    /// Use <c>.ps1</c>, <c>.bat</c>, or <c>.cmd</c> when using Windows-based agents; use <c>.ps1</c> or <c>.sh</c> when using Linux-based agents.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ScriptPath
    {
        get => GetExpression<string>("scriptPath");
        init => SetProperty("scriptPath", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureCliFileTask"/> class with required properties.
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection for the deployment.</param>
    /// <param name="scriptType">Type of script.</param>
    /// <param name="scriptPath">Path to the script.</param>
    public AzureCliFileTask(AdoExpression<string> azureSubscription, AdoExpression<ScriptType> scriptType, AdoExpression<string> scriptPath)
    : base(azureSubscription, scriptType, Tasks.ScriptLocation.ScriptPath)
    {
        ScriptPath = scriptPath;
    }
}

/// <summary>
/// Allowed values for Azure CLI task <c>scriptType</c>.
/// </summary>
public enum ScriptType
{
    /// <summary>
    /// PowerShell. Supported on Windows agents.
    /// </summary>
    [YamlMember(Alias = "ps")]
    Ps,

    /// <summary>
    /// PowerShell Core. Supported on Linux, macOS, and Windows agents when PowerShell 6 or later is available.
    /// </summary>
    [YamlMember(Alias = "pscore")]
    Pscore,

    /// <summary>
    /// Batch. Supported on Windows agents.
    /// </summary>
    [YamlMember(Alias = "batch")]
    Batch,

    /// <summary>
    /// Shell. Supported on Linux agents.
    /// </summary>
    [YamlMember(Alias = "bash")]
    Bash,
}

/// <summary>
/// Allowed values for Azure CLI task <c>scriptLocation</c>.
/// </summary>
public enum ScriptLocation
{
    /// <summary>
    /// Default. Script path.
    /// </summary>
    [YamlMember(Alias = "scriptPath")]
    ScriptPath,

    /// <summary>
    /// Inline script.
    /// </summary>
    [YamlMember(Alias = "inlineScript")]
    InlineScript,
}

/// <summary>
/// Allowed values for Azure CLI task <c>powerShellErrorActionPreference</c>.
/// </summary>
public enum PowerShellErrorActionPreference
{
    /// <summary>
    /// Default. Stop.
    /// </summary>
    [YamlMember(Alias = "stop")]
    Stop,

    /// <summary>
    /// Continue.
    /// </summary>
    [YamlMember(Alias = "continue")]
    Continue,

    /// <summary>
    /// SilentlyContinue.
    /// </summary>
    [YamlMember(Alias = "silentlyContinue")]
    SilentlyContinue,
}
