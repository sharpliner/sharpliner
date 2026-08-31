using System;
using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating Azure Container Apps deployment tasks.
/// Uses major-version-specific builders to keep task versions explicit.
/// </summary>
public class AzureContainerAppsTaskBuilder
{
    /// <summary>
    /// Creates Azure Container Apps deployment tasks using <c>AzureContainerApps@1</c>.
    /// </summary>
    public AzureContainerAppsV1TaskBuilder V1 => new();

    /// <summary>
    /// Creates Azure Container Apps deployment tasks using deprecated <c>AzureContainerApps@0</c>.
    /// </summary>
    [Obsolete("AzureContainerApps@0 is deprecated by Azure Pipelines. Prefer AzureContainerApps@1.")]
    public AzureContainerAppsV0TaskBuilder V0 => new();

    internal AzureContainerAppsTaskBuilder()
    {
    }
}

/// <summary>
/// Builder for valid deployment paths with <c>AzureContainerApps@1</c>.
/// </summary>
public class AzureContainerAppsV1TaskBuilder
{
    /// <summary>
    /// Creates a source-based deployment task where an image is built and pushed before deployment.
    /// Requires <paramref name="appSourcePath"/> and <paramref name="acrName"/>.
    /// </summary>
    public AzureContainerAppsV1FromSourceTask FromSource(AdoExpression<string> azureSubscription, AdoExpression<string> appSourcePath, AdoExpression<string> acrName)
        => new(azureSubscription, appSourcePath, acrName);

    /// <summary>
    /// Creates an image-based deployment task where an existing image is deployed.
    /// Requires <paramref name="imageToDeploy"/>.
    /// </summary>
    public AzureContainerAppsV1FromImageTask FromImage(AdoExpression<string> azureSubscription, AdoExpression<string> imageToDeploy)
        => new(azureSubscription, imageToDeploy);

    /// <summary>
    /// Creates a YAML-based deployment task where Container App configuration is read from a YAML file.
    /// Requires <paramref name="yamlConfigPath"/>.
    /// </summary>
    public AzureContainerAppsV1FromYamlTask FromYaml(AdoExpression<string> azureSubscription, AdoExpression<string> yamlConfigPath)
        => new(azureSubscription, yamlConfigPath);
}

/// <summary>
/// Builder for valid deployment paths with deprecated <c>AzureContainerApps@0</c>.
/// </summary>
[Obsolete("AzureContainerApps@0 is deprecated by Azure Pipelines. Prefer AzureContainerApps@1.")]
public class AzureContainerAppsV0TaskBuilder
{
    /// <summary>
    /// Creates a source-based deployment task where an image is built and pushed before deployment.
    /// Requires <paramref name="appSourcePath"/> and <paramref name="acrName"/>.
    /// </summary>
    public AzureContainerAppsV0FromSourceTask FromSource(AdoExpression<string> azureSubscription, AdoExpression<string> appSourcePath, AdoExpression<string> acrName)
        => new(azureSubscription, appSourcePath, acrName);

    /// <summary>
    /// Creates an image-based deployment task where an existing image is deployed.
    /// Requires <paramref name="imageToDeploy"/>.
    /// </summary>
    public AzureContainerAppsV0FromImageTask FromImage(AdoExpression<string> azureSubscription, AdoExpression<string> imageToDeploy)
        => new(azureSubscription, imageToDeploy);

    /// <summary>
    /// Creates a YAML-based deployment task where Container App configuration is read from a YAML file.
    /// Requires <paramref name="yamlConfigPath"/>.
    /// </summary>
    public AzureContainerAppsV0FromYamlTask FromYaml(AdoExpression<string> azureSubscription, AdoExpression<string> yamlConfigPath)
        => new(azureSubscription, yamlConfigPath);
}
