using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Runs Azure CLI commands using the <c>AzureCLI@3</c> task.
/// See the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-cli-v3?view=azure-pipelines">official Azure DevOps task reference</see>
/// and the <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/AzureCLIV3/task.json">official AzureCLIV3 task specification</see>.
/// </summary>
public abstract record AzureCliV3Task : AzureDevOpsTask
{
    /// <summary>Required. The service connection type. Default value: <see cref="AzureCliV3ConnectionType.AzureResourceManager"/>.</summary>
    [YamlIgnore]
    public AdoExpression<AzureCliV3ConnectionType>? ConnectionType
    {
        get => GetExpression<AzureCliV3ConnectionType>("connectionType");
        init => SetProperty("connectionType", value);
    }

    /// <summary>Required when <see cref="ConnectionType"/> is <see cref="AzureCliV3ConnectionType.AzureResourceManager"/>. Azure Resource Manager service connection.</summary>
    [YamlIgnore]
    public AdoExpression<string>? ConnectedServiceNameARM
    {
        get => GetExpression<string>("connectedServiceNameARM");
        init => SetProperty("connectedServiceNameARM", value);
    }

    /// <summary>Required when <see cref="ConnectionType"/> is <see cref="AzureCliV3ConnectionType.AzureDevOps"/>. Azure DevOps service connection.</summary>
    [YamlIgnore]
    public AdoExpression<string>? AzureDevOpsServiceConnection
    {
        get => GetExpression<string>("azureDevOpsServiceConnection");
        init => SetProperty("azureDevOpsServiceConnection", value);
    }

    /// <summary>Required. The script type: PowerShell, PowerShell Core, Batch, or Shell.</summary>
    [YamlIgnore]
    public AdoExpression<ScriptType>? ScriptType
    {
        get => GetExpression<ScriptType>("scriptType");
        init => SetProperty("scriptType", value);
    }

    /// <summary>Required. Whether the script is inline or referenced by path. Default value: <see cref="Tasks.ScriptLocation.ScriptPath"/>.</summary>
    [YamlIgnore]
    public AdoExpression<ScriptLocation>? ScriptLocation
    {
        get => GetExpression<ScriptLocation>("scriptLocation");
        init => SetProperty("scriptLocation", value);
    }

    /// <summary>Optional. Arguments passed to the script.</summary>
    [YamlIgnore]
    public AdoExpression<string>? ScriptArguments
    {
        get => GetExpression<string>("scriptArguments");
        init => SetProperty("scriptArguments", value);
    }

    /// <summary>Optional for PowerShell and PowerShell Core scripts. Default value: <see cref="PowerShellErrorActionPreference.Stop"/>.</summary>
    [YamlIgnore]
    public AdoExpression<PowerShellErrorActionPreference>? PowerShellErrorActionPreference
    {
        get => GetExpression<PowerShellErrorActionPreference>("powerShellErrorActionPreference");
        init => SetProperty("powerShellErrorActionPreference", value);
    }

    /// <summary>Optional. Default value: <c>false</c>. Adds service principal details to the script environment for Service Principal Azure Resource Manager connections.</summary>
    [YamlIgnore]
    public AdoExpression<bool>? AddSpnToEnvironment
    {
        get => GetExpression<bool>("addSpnToEnvironment", false);
        init => SetProperty("addSpnToEnvironment", value);
    }

    /// <summary>Optional. Default value: <c>false</c>. Uses the global Azure CLI configuration rather than a task-specific configuration directory.</summary>
    [YamlIgnore]
    public AdoExpression<bool>? UseGlobalConfig
    {
        get => GetExpression<bool>("useGlobalConfig", false);
        init => SetProperty("useGlobalConfig", value);
    }

    /// <summary>Optional. The directory in which to run the script. An empty value uses <c>$(System.DefaultWorkingDirectory)</c>.</summary>
    [YamlIgnore]
    public AdoExpression<string>? Cwd
    {
        get => GetExpression<string>("cwd");
        init => SetProperty("cwd", value);
    }

    /// <summary>Optional. Default value: <c>false</c>. Fails the task when the script writes to standard error.</summary>
    [YamlIgnore]
    public AdoExpression<bool>? FailOnStandardError
    {
        get => GetExpression<bool>("failOnStandardError", false);
        init => SetProperty("failOnStandardError", value);
    }

    /// <summary>Optional for PowerShell and PowerShell Core scripts. Default value: <c>false</c>. Does not propagate <c>$LASTEXITCODE</c> when true.</summary>
    [YamlIgnore]
    public AdoExpression<bool>? PowerShellIgnoreLASTEXITCODE
    {
        get => GetExpression<bool>("powerShellIgnoreLASTEXITCODE", false);
        init => SetProperty("powerShellIgnoreLASTEXITCODE", value);
    }

    /// <summary>Optional. Default value: <c>true</c>. Writes <c>az login</c> output to the task log.</summary>
    [YamlIgnore]
    public AdoExpression<bool>? VisibleAzLogin
    {
        get => GetExpression<bool>("visibleAzLogin", true);
        init => SetProperty("visibleAzLogin", value);
    }

    /// <summary>Optional. Default value: <c>false</c>. Allows identities with no Azure subscription access.</summary>
    [YamlIgnore]
    public AdoExpression<bool>? AllowNoSubscriptions
    {
        get => GetExpression<bool>("allowNoSubscriptions", false);
        init => SetProperty("allowNoSubscriptions", value);
    }

    /// <summary>Optional. Experimental. Default value: <c>false</c>. Keeps an Azure CLI session active for workload identity federation Azure Resource Manager connections.</summary>
    [YamlIgnore]
    public AdoExpression<bool>? KeepAzSessionActive
    {
        get => GetExpression<bool>("keepAzSessionActive", false);
        init => SetProperty("keepAzSessionActive", value);
    }

    /// <summary>Initializes the common AzureCLI@3 inputs.</summary>
    /// <param name="connectionType">Service connection type.</param>
    /// <param name="serviceConnection">Service connection matching <paramref name="connectionType"/>.</param>
    /// <param name="scriptType">Type of script.</param>
    /// <param name="scriptLocation">Script location.</param>
    protected AzureCliV3Task(AzureCliV3ConnectionType connectionType, AdoExpression<string> serviceConnection, AdoExpression<ScriptType> scriptType, AdoExpression<ScriptLocation> scriptLocation)
        : base("AzureCLI@3")
    {
        ConnectionType = connectionType;
        if (connectionType == AzureCliV3ConnectionType.AzureResourceManager)
            ConnectedServiceNameARM = serviceConnection;
        else
            AzureDevOpsServiceConnection = serviceConnection;
        ScriptType = scriptType;
        ScriptLocation = scriptLocation;
    }
}

/// <summary>AzureCLI@3 task with an inline script. See the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-cli-v3?view=azure-pipelines">official task reference</see>.</summary>
public record InlineAzureCliV3Task : AzureCliV3Task
{
    /// <summary>Required when <see cref="AzureCliV3Task.ScriptLocation"/> is inline. The script contents.</summary>
    [YamlIgnore]
    public AdoExpression<string>? InlineScript
    {
        get => GetExpression<string>("inlineScript");
        init => SetProperty("inlineScript", value);
    }

    /// <summary>Initializes an inline AzureCLI@3 task.</summary>
    public InlineAzureCliV3Task(AzureCliV3ConnectionType connectionType, AdoExpression<string> serviceConnection, AdoExpression<ScriptType> scriptType, AdoExpression<string> inlineScript)
        : base(connectionType, serviceConnection, scriptType, Tasks.ScriptLocation.InlineScript) => InlineScript = inlineScript;
}

/// <summary>AzureCLI@3 task that references a script file. See the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-cli-v3?view=azure-pipelines">official task reference</see>.</summary>
public record AzureCliV3FileTask : AzureCliV3Task
{
    /// <summary>Required when <see cref="AzureCliV3Task.ScriptLocation"/> is a script path. A fully qualified or working-directory-relative script path.</summary>
    [YamlIgnore]
    public AdoExpression<string>? ScriptPath
    {
        get => GetExpression<string>("scriptPath");
        init => SetProperty("scriptPath", value);
    }

    /// <summary>Initializes a file-based AzureCLI@3 task.</summary>
    public AzureCliV3FileTask(AzureCliV3ConnectionType connectionType, AdoExpression<string> serviceConnection, AdoExpression<ScriptType> scriptType, AdoExpression<string> scriptPath)
        : base(connectionType, serviceConnection, scriptType, Tasks.ScriptLocation.ScriptPath) => ScriptPath = scriptPath;
}

/// <summary>Allowed values for the AzureCLI@3 <c>connectionType</c> input.</summary>
public enum AzureCliV3ConnectionType
{
    /// <summary>Azure Resource Manager service connection.</summary>
    [YamlMember(Alias = "azureRM")]
    AzureResourceManager,

    /// <summary>Azure DevOps service connection.</summary>
    [YamlMember(Alias = "azureDevOps")]
    AzureDevOps,
}
