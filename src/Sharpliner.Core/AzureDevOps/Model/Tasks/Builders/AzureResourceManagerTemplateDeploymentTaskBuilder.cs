using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.Common.Model.Tasks;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Fluent builder for <c>AzureResourceManagerTemplateDeployment@3</c>. More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-resource-manager-template-deployment-v3?view=azure-pipelines">official Azure DevOps pipelines documentation</see>.
/// </summary>
public class AzureResourceManagerTemplateDeploymentTaskBuilder : TaskBuilderBase
{
    /// <summary>Starts a management group deployment.</summary>
    public AzureResourceManagerTemplateDeploymentTemplateSourceBuilder ManagementGroup(AdoExpression<string> azureResourceManagerConnection, AdoExpression<string> location)
        => new(AzureResourceManagerTemplateDeploymentScope.ManagementGroup, azureResourceManagerConnection, location);

    /// <summary>Starts a subscription deployment.</summary>
    public AzureResourceManagerTemplateDeploymentTemplateSourceBuilder Subscription(AdoExpression<string> azureResourceManagerConnection, AdoExpression<string> subscription, AdoExpression<string> location)
        => new(AzureResourceManagerTemplateDeploymentScope.Subscription, azureResourceManagerConnection, location, subscription);

    /// <summary>Starts a resource group deployment and selects its action.</summary>
    public AzureResourceManagerTemplateDeploymentResourceGroupBuilder ResourceGroup(AdoExpression<string> azureResourceManagerConnection, AdoExpression<string> subscription, AdoExpression<string> resourceGroupName)
        => new(azureResourceManagerConnection, subscription, resourceGroupName);

    internal AzureResourceManagerTemplateDeploymentTaskBuilder()
    {
    }
}

/// <summary>Selects the action for a resource group deployment.</summary>
public class AzureResourceManagerTemplateDeploymentResourceGroupBuilder
{
    private readonly AdoExpression<string> azureResourceManagerConnection;
    private readonly AdoExpression<string> subscription;
    private readonly AdoExpression<string> resourceGroupName;

    internal AzureResourceManagerTemplateDeploymentResourceGroupBuilder(AdoExpression<string> azureResourceManagerConnection, AdoExpression<string> subscription, AdoExpression<string> resourceGroupName)
    {
        this.azureResourceManagerConnection = azureResourceManagerConnection;
        this.subscription = subscription;
        this.resourceGroupName = resourceGroupName;
    }

    /// <summary>Creates or updates the resource group and then selects the template source.</summary>
    public AzureResourceManagerTemplateDeploymentTemplateSourceBuilder CreateOrUpdate(AdoExpression<string> location)
        => new(AzureResourceManagerTemplateDeploymentScope.ResourceGroup, azureResourceManagerConnection, location, subscription, resourceGroupName);

    /// <summary>Deletes the resource group.</summary>
    public AzureResourceManagerTemplateDeploymentDeleteResourceGroupTask Delete()
        => new(azureResourceManagerConnection, subscription, resourceGroupName);
}

/// <summary>Selects the template source for a create or update deployment.</summary>
public class AzureResourceManagerTemplateDeploymentTemplateSourceBuilder
{
    private readonly AzureResourceManagerTemplateDeploymentScope deploymentScope;
    private readonly AdoExpression<string> azureResourceManagerConnection;
    private readonly AdoExpression<string> location;
    private readonly AdoExpression<string>? subscription;
    private readonly AdoExpression<string>? resourceGroupName;

    internal AzureResourceManagerTemplateDeploymentTemplateSourceBuilder(AzureResourceManagerTemplateDeploymentScope deploymentScope, AdoExpression<string> azureResourceManagerConnection, AdoExpression<string> location, AdoExpression<string>? subscription = null, AdoExpression<string>? resourceGroupName = null)
    {
        this.deploymentScope = deploymentScope;
        this.azureResourceManagerConnection = azureResourceManagerConnection;
        this.location = location;
        this.subscription = subscription;
        this.resourceGroupName = resourceGroupName;
    }

    /// <summary>Uses an ARM template or Bicep file from the linked artifact.</summary>
    public AzureResourceManagerTemplateDeploymentLinkedArtifactTask LinkedArtifact(AdoExpression<string> template, AdoExpression<string>? templateParameters = null)
        => new(deploymentScope, azureResourceManagerConnection, location, template, subscription, resourceGroupName, templateParameters);

    /// <summary>Uses an ARM template file at a URL.</summary>
    public AzureResourceManagerTemplateDeploymentUrlTask Url(AdoExpression<string> templateUrl, AdoExpression<string>? templateParametersUrl = null)
        => new(deploymentScope, azureResourceManagerConnection, location, templateUrl, subscription, resourceGroupName, templateParametersUrl);
}
