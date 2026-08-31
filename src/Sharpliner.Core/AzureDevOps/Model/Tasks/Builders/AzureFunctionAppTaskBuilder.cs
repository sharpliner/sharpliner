using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>Fluent factory for valid Azure Functions deployment task combinations. See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-function-app-v2?view=azure-pipelines">the official task reference</see>.</summary>
public class AzureFunctionAppTaskBuilder
{
    internal AzureFunctionAppTaskBuilder() { }

    /// <summary>Creates an AzureFunctionApp@2 deployment for a Windows Function App.</summary>
    public AzureFunctionAppV2Task Windows(AdoExpression<string> azureSubscription, AdoExpression<string> appName, AdoExpression<string> package, AdoExpression<AzureFunctionDeploymentMethod>? deploymentMethod = null)
        => new(azureSubscription, AzureFunctionAppType.Windows, appName, package) { DeploymentMethod = deploymentMethod };

    /// <summary>Creates an AzureFunctionApp@2 deployment for a Linux Function App.</summary>
    public AzureFunctionAppV2Task Linux(AdoExpression<string> azureSubscription, AdoExpression<string> appName, AdoExpression<string> package, AdoExpression<AzureFunctionRuntimeStack>? runtimeStack = null, AdoExpression<AzureFunctionDeploymentMethod>? deploymentMethod = null)
        => new(azureSubscription, AzureFunctionAppType.Linux, appName, package) { RuntimeStack = runtimeStack, DeploymentMethod = deploymentMethod };

    /// <summary>Creates an AzureFunctionApp@2 deployment for a Flex Consumption Function App. Flex plans do not accept slots, runtime stacks, or a deployment method.</summary>
    public AzureFunctionAppV2Task FlexConsumption(AdoExpression<string> azureSubscription, AdoExpression<AzureFunctionAppType> appType, AdoExpression<string> appName, AdoExpression<string> package)
        => new(azureSubscription, appType, appName, package) { IsFlexConsumption = true };

    /// <summary>Creates an AzureFunctionApp@1 deployment for a Windows Function App.</summary>
    public AzureFunctionAppV1Task V1Windows(AdoExpression<string> azureSubscription, AdoExpression<string> appName, AdoExpression<string> package, AdoExpression<AzureFunctionDeploymentMethod>? deploymentMethod = null)
        => new(azureSubscription, AzureFunctionAppType.Windows, appName, package) { DeploymentMethod = deploymentMethod };

    /// <summary>Creates an AzureFunctionApp@1 deployment for a Linux Function App. Version 1 does not expose a deployment method for Linux apps.</summary>
    public AzureFunctionAppV1Task V1Linux(AdoExpression<string> azureSubscription, AdoExpression<string> appName, AdoExpression<string> package, AdoExpression<AzureFunctionRuntimeStack>? runtimeStack = null)
        => new(azureSubscription, AzureFunctionAppType.Linux, appName, package) { RuntimeStack = runtimeStack };

    /// <summary>Creates an AzureFunctionAppContainer@1 deployment.</summary>
    public AzureFunctionAppContainerV1Task Container(AdoExpression<string> azureSubscription, AdoExpression<string> appName, AdoExpression<string> imageName)
        => new(azureSubscription, appName, imageName);
}
