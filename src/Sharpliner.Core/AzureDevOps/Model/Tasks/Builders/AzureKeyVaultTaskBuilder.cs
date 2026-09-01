using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating Azure Key Vault tasks.
/// </summary>
public class AzureKeyVaultTaskBuilder
{
    /// <summary>
    /// Creates an <see cref="AzureKeyVaultTask"/> (<c>AzureKeyVault@2</c>) that downloads secrets from an Azure Key Vault.
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection for the key vault.</param>
    /// <param name="keyVaultName">Name of an existing Azure Key Vault.</param>
    /// <param name="secretsFilter">Comma-separated list of secret names to download, or <c>*</c> to download all secrets.</param>
    /// <param name="runAsPreJob">Runs this task before job execution starts and exposes secrets to all tasks in the job.</param>
    public AzureKeyVaultTask DownloadSecrets(
        AdoExpression<string> azureSubscription,
        AdoExpression<string> keyVaultName,
        AdoExpression<string>? secretsFilter = null,
        AdoExpression<bool>? runAsPreJob = null)
        => new(azureSubscription, keyVaultName, secretsFilter, runAsPreJob);

    /// <summary>
    /// Creates an <see cref="AzureKeyVaultV1Task"/> (<c>AzureKeyVault@1</c>) that downloads secrets from an Azure Key Vault.
    /// This task major is deprecated by Microsoft.
    /// </summary>
    /// <param name="azureSubscription">Azure Resource Manager service connection for the key vault.</param>
    /// <param name="keyVaultName">Name of an existing Azure Key Vault.</param>
    /// <param name="secretsFilter">Comma-separated list of secret names to download, or <c>*</c> to download all secrets.</param>
    /// <param name="runAsPreJob">Runs this task before job execution starts and exposes secrets to all tasks in the job.</param>
    [System.Obsolete("AzureKeyVault@1 is deprecated by Microsoft. Prefer DownloadSecrets for AzureKeyVault@2.")]
    public AzureKeyVaultV1Task DownloadSecretsV1(
        AdoExpression<string> azureSubscription,
        AdoExpression<string> keyVaultName,
        AdoExpression<string>? secretsFilter = null,
        AdoExpression<bool>? runAsPreJob = null)
        => new(azureSubscription, keyVaultName, secretsFilter, runAsPreJob);

    internal AzureKeyVaultTaskBuilder()
    {
    }
}
