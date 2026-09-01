using System;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Shared model for Azure Key Vault tasks.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-key-vault-v2?view=azure-pipelines">official Azure DevOps pipelines documentation</see>
/// and the audited task specifications:
/// <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/AzureKeyVaultV2/task.json">AzureKeyVaultV2/task.json</see>,
/// <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/AzureKeyVaultV1/task.json">AzureKeyVaultV1/task.json</see>.
/// </summary>
public abstract record AzureKeyVaultTaskBase : AzureDevOpsTask
{
    /// <summary>
    /// Required. Azure Resource Manager service connection for the key vault.
    /// The official input name is <c>ConnectedServiceName</c>; <c>azureSubscription</c> is its YAML alias and is emitted for compatibility with existing Sharpliner output.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? AzureSubscription
    {
        get => GetExpression<string>("azureSubscription");
        init => SetProperty("azureSubscription", value);
    }

    /// <summary>
    /// Required. Name of an existing Azure Key Vault.
    /// The task validates that the value is a valid key vault name.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? KeyVaultName
    {
        get => GetExpression<string>("KeyVaultName");
        init => SetProperty("KeyVaultName", value);
    }

    /// <summary>
    /// Required. Comma-separated list of secret names to download, or <c>*</c> to download all secrets.
    /// Default value: <c>*</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SecretsFilter
    {
        get => GetExpression<string>("SecretsFilter", "*");
        init => SetProperty("SecretsFilter", value);
    }

    /// <summary>
    /// Required. Run this task before job execution begins and expose downloaded secrets to all tasks in the job.
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? RunAsPreJob
    {
        get => GetExpression<bool>("RunAsPreJob", false);
        init => SetProperty("RunAsPreJob", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureKeyVaultTaskBase"/> class with required properties.
    /// </summary>
    /// <param name="taskVersion">The Azure Key Vault task identity, for example <c>AzureKeyVault@2</c>.</param>
    /// <param name="azureSubscription">Azure Resource Manager service connection for the key vault.</param>
    /// <param name="keyVaultName">Name of an existing Azure Key Vault.</param>
    /// <param name="secretsFilter">Comma-separated list of secret names to download, or <c>*</c> to download all secrets.</param>
    /// <param name="runAsPreJob">Runs this task before job execution starts and exposes secrets to all tasks in the job.</param>
    protected AzureKeyVaultTaskBase(
        string taskVersion,
        AdoExpression<string> azureSubscription,
        AdoExpression<string> keyVaultName,
        AdoExpression<string>? secretsFilter = null,
        AdoExpression<bool>? runAsPreJob = null)
        : base(taskVersion)
    {
        ArgumentNullException.ThrowIfNull(azureSubscription);
        ArgumentNullException.ThrowIfNull(keyVaultName);

        AzureSubscription = azureSubscription;
        KeyVaultName = keyVaultName;
        SecretsFilter = secretsFilter ?? "*";
        RunAsPreJob = runAsPreJob ?? false;
        DisplayName = "Download Azure Key Vault secrets";
    }
}

/// <summary>
/// Downloads Azure Key Vault secrets using the <c>AzureKeyVault@2</c> task.
/// </summary>
public record AzureKeyVaultTask : AzureKeyVaultTaskBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureKeyVaultTask"/> class with required properties.
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection for the key vault.</param>
    /// <param name="keyVaultName">Name of an existing Azure Key Vault.</param>
    /// <param name="secretsFilter">Comma-separated list of secret names to download, or <c>*</c> to download all secrets.</param>
    /// <param name="runAsPreJob">Runs this task before job execution starts and exposes secrets to all tasks in the job.</param>
    public AzureKeyVaultTask(
        AdoExpression<string> azureSubscription,
        AdoExpression<string> keyVaultName,
        AdoExpression<string>? secretsFilter = null,
        AdoExpression<bool>? runAsPreJob = null)
        : base("AzureKeyVault@2", azureSubscription, keyVaultName, secretsFilter, runAsPreJob)
    {
    }
}

/// <summary>
/// Downloads Azure Key Vault secrets using the deprecated <c>AzureKeyVault@1</c> task.
/// </summary>
[Obsolete("AzureKeyVault@1 is deprecated by Microsoft. Prefer AzureKeyVaultTask (AzureKeyVault@2).")]
public record AzureKeyVaultV1Task : AzureKeyVaultTaskBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzureKeyVaultV1Task"/> class with required properties.
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection for the key vault.</param>
    /// <param name="keyVaultName">Name of an existing Azure Key Vault.</param>
    /// <param name="secretsFilter">Comma-separated list of secret names to download, or <c>*</c> to download all secrets.</param>
    /// <param name="runAsPreJob">Runs this task before job execution starts and exposes secrets to all tasks in the job.</param>
    public AzureKeyVaultV1Task(
        AdoExpression<string> azureSubscription,
        AdoExpression<string> keyVaultName,
        AdoExpression<string>? secretsFilter = null,
        AdoExpression<bool>? runAsPreJob = null)
        : base("AzureKeyVault@1", azureSubscription, keyVaultName, secretsFilter, runAsPreJob)
    {
    }
}
