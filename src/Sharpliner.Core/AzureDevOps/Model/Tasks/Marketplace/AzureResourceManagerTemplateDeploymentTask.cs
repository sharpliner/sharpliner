using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Deploys an Azure Resource Manager template by using the <c>AzureResourceManagerTemplateDeployment@3</c> task.
/// More details can be found in the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-resource-manager-template-deployment-v3?view=azure-pipelines">official Azure DevOps pipelines documentation</see>
/// and the <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/AzureResourceManagerTemplateDeploymentV3/task.json">official task specification</see>.
/// </summary>
public abstract record AzureResourceManagerTemplateDeploymentTask : AzureDevOpsTask
{
    /// <summary>Deployment scope.</summary>
    [YamlIgnore]
    public AdoExpression<AzureResourceManagerTemplateDeploymentScope>? DeploymentScope
    {
        get => GetExpression<AzureResourceManagerTemplateDeploymentScope>("deploymentScope");
        init => SetProperty("deploymentScope", value);
    }

    /// <summary>Required. Azure Resource Manager service connection.</summary>
    [YamlIgnore]
    public AdoExpression<string>? AzureResourceManagerConnection
    {
        get => GetExpression<string>("ConnectedServiceName");
        init => SetProperty("ConnectedServiceName", value);
    }

    /// <summary>Required except for management group deployments. Azure subscription.</summary>
    [YamlIgnore]
    public AdoExpression<string>? Subscription
    {
        get => GetExpression<string>("subscriptionName");
        init => SetProperty("subscriptionName", value);
    }

    /// <summary>Required for resource group deployments. Resource group name.</summary>
    [YamlIgnore]
    public AdoExpression<string>? ResourceGroupName
    {
        get => GetExpression<string>("resourceGroupName");
        init => SetProperty("resourceGroupName", value);
    }

    /// <summary>Action for resource group deployments.</summary>
    [YamlIgnore]
    public AdoExpression<AzureResourceManagerTemplateDeploymentAction>? Action
    {
        get => GetExpression<AzureResourceManagerTemplateDeploymentAction>("action");
        init => SetProperty("action", value);
    }

    protected AzureResourceManagerTemplateDeploymentTask(
        AzureResourceManagerTemplateDeploymentScope deploymentScope,
        AdoExpression<string> azureResourceManagerConnection,
        AdoExpression<string>? subscription = null,
        AdoExpression<string>? resourceGroupName = null)
        : base("AzureResourceManagerTemplateDeployment@3")
    {
        DeploymentScope = deploymentScope;
        AzureResourceManagerConnection = azureResourceManagerConnection;
        Subscription = subscription;
        ResourceGroupName = resourceGroupName;
    }
}

/// <summary>Azure Resource Manager template deployment that creates or updates resources.</summary>
public abstract record AzureResourceManagerTemplateDeploymentCreateOrUpdateTask : AzureResourceManagerTemplateDeploymentTask
{
    /// <summary>Required. Template location.</summary>
    [YamlIgnore]
    public AdoExpression<AzureResourceManagerTemplateDeploymentTemplateLocation>? TemplateLocation
    {
        get => GetExpression<AzureResourceManagerTemplateDeploymentTemplateLocation>("templateLocation");
        init => SetProperty("templateLocation", value);
    }

    /// <summary>Required. Location for the resource group or deployment metadata.</summary>
    [YamlIgnore]
    public AdoExpression<string>? Location
    {
        get => GetExpression<string>("location");
        init => SetProperty("location", value);
    }

    /// <summary>Optional override parameters for the template.</summary>
    [YamlIgnore]
    public AdoExpression<string>? OverrideParameters
    {
        get => GetExpression<string>("overrideParameters");
        init => SetProperty("overrideParameters", value);
    }

    /// <summary>Optional deployment mode. Default value: <see cref="AzureResourceManagerTemplateDeploymentMode.Incremental"/>.</summary>
    [YamlIgnore]
    public AdoExpression<AzureResourceManagerTemplateDeploymentMode>? DeploymentMode
    {
        get => GetExpression<AzureResourceManagerTemplateDeploymentMode>("deploymentMode");
        init => SetProperty("deploymentMode", value);
    }

    /// <summary>Optional name for the deployment.</summary>
    [YamlIgnore]
    public AdoExpression<string>? DeploymentName
    {
        get => GetExpression<string>("deploymentName");
        init => SetProperty("deploymentName", value);
    }

    /// <summary>Optional variable name for the deployment outputs.</summary>
    [YamlIgnore]
    public AdoExpression<string>? DeploymentOutputs
    {
        get => GetExpression<string>("deploymentOutputs");
        init => SetProperty("deploymentOutputs", value);
    }

    /// <summary>Optional. Adds service principal details to the override-parameters environment. Default value: <c>false</c>.</summary>
    [YamlIgnore]
    public AdoExpression<bool>? AddSpnToEnvironment
    {
        get => GetExpression<bool>("addSpnToEnvironment", false);
        init => SetProperty("addSpnToEnvironment", value);
    }

    /// <summary>Optional. Emits individual output values without JSON stringification. Default value: <c>false</c>.</summary>
    [YamlIgnore]
    public AdoExpression<bool>? UseWithoutJSON
    {
        get => GetExpression<bool>("useWithoutJSON", false);
        init => SetProperty("useWithoutJSON", value);
    }

    protected AzureResourceManagerTemplateDeploymentCreateOrUpdateTask(
        AzureResourceManagerTemplateDeploymentScope deploymentScope,
        AdoExpression<string> azureResourceManagerConnection,
        AdoExpression<string> location,
        AdoExpression<string>? subscription = null,
        AdoExpression<string>? resourceGroupName = null)
        : base(deploymentScope, azureResourceManagerConnection, subscription, resourceGroupName)
    {
        Location = location;
        if (deploymentScope == AzureResourceManagerTemplateDeploymentScope.ResourceGroup)
        {
            SetProperty("action", AzureResourceManagerTemplateDeploymentAction.CreateOrUpdateResourceGroup);
        }
    }
}

/// <summary>Azure Resource Manager template deployment using files from the linked artifact.</summary>
public record AzureResourceManagerTemplateDeploymentLinkedArtifactTask : AzureResourceManagerTemplateDeploymentCreateOrUpdateTask
{
    /// <summary>Required. Path or pattern for the ARM template or Bicep file.</summary>
    [YamlIgnore]
    public AdoExpression<string>? Template
    {
        get => GetExpression<string>("csmFile");
        init => SetProperty("csmFile", value);
    }

    /// <summary>Optional path or pattern for the template parameters file.</summary>
    [YamlIgnore]
    public AdoExpression<string>? TemplateParameters
    {
        get => GetExpression<string>("csmParametersFile");
        init => SetProperty("csmParametersFile", value);
    }

    internal AzureResourceManagerTemplateDeploymentLinkedArtifactTask(AzureResourceManagerTemplateDeploymentScope deploymentScope, AdoExpression<string> azureResourceManagerConnection, AdoExpression<string> location, AdoExpression<string> template, AdoExpression<string>? subscription = null, AdoExpression<string>? resourceGroupName = null, AdoExpression<string>? templateParameters = null)
        : base(deploymentScope, azureResourceManagerConnection, location, subscription, resourceGroupName)
    {
        SetProperty("templateLocation", AzureResourceManagerTemplateDeploymentTemplateLocation.LinkedArtifact);
        Template = template;
        TemplateParameters = templateParameters;
    }
}

/// <summary>Azure Resource Manager template deployment using template files at URLs.</summary>
public record AzureResourceManagerTemplateDeploymentUrlTask : AzureResourceManagerTemplateDeploymentCreateOrUpdateTask
{
    /// <summary>Required URL of the ARM template.</summary>
    [YamlIgnore]
    public AdoExpression<string>? TemplateUrl
    {
        get => GetExpression<string>("csmFileLink");
        init => SetProperty("csmFileLink", value);
    }

    /// <summary>Optional URL of the template parameters file.</summary>
    [YamlIgnore]
    public AdoExpression<string>? TemplateParametersUrl
    {
        get => GetExpression<string>("csmParametersFileLink");
        init => SetProperty("csmParametersFileLink", value);
    }

    internal AzureResourceManagerTemplateDeploymentUrlTask(AzureResourceManagerTemplateDeploymentScope deploymentScope, AdoExpression<string> azureResourceManagerConnection, AdoExpression<string> location, AdoExpression<string> templateUrl, AdoExpression<string>? subscription = null, AdoExpression<string>? resourceGroupName = null, AdoExpression<string>? templateParametersUrl = null)
        : base(deploymentScope, azureResourceManagerConnection, location, subscription, resourceGroupName)
    {
        SetProperty("templateLocation", AzureResourceManagerTemplateDeploymentTemplateLocation.UrlOfTheFile);
        TemplateUrl = templateUrl;
        TemplateParametersUrl = templateParametersUrl;
    }
}

/// <summary>Deletes a resource group using AzureResourceManagerTemplateDeployment@3.</summary>
public record AzureResourceManagerTemplateDeploymentDeleteResourceGroupTask : AzureResourceManagerTemplateDeploymentTask
{
    internal AzureResourceManagerTemplateDeploymentDeleteResourceGroupTask(AdoExpression<string> azureResourceManagerConnection, AdoExpression<string> subscription, AdoExpression<string> resourceGroupName)
        : base(AzureResourceManagerTemplateDeploymentScope.ResourceGroup, azureResourceManagerConnection, subscription, resourceGroupName)
    {
        SetProperty("action", AzureResourceManagerTemplateDeploymentAction.DeleteResourceGroup);
    }
}

/// <summary>Deployment scopes supported by AzureResourceManagerTemplateDeployment@3.</summary>
public enum AzureResourceManagerTemplateDeploymentScope
{
    /// <summary>Management group deployment.</summary>
    [YamlMember(Alias = "Management Group")]
    ManagementGroup,
    /// <summary>Subscription deployment.</summary>
    Subscription,
    /// <summary>Resource group deployment.</summary>
    [YamlMember(Alias = "Resource Group")]
    ResourceGroup,
}

/// <summary>Actions supported for resource group deployments.</summary>
public enum AzureResourceManagerTemplateDeploymentAction
{
    /// <summary>Create or update the resource group and deploy the template.</summary>
    [YamlMember(Alias = "Create Or Update Resource Group")]
    CreateOrUpdateResourceGroup,
    /// <summary>Delete the resource group.</summary>
    [YamlMember(Alias = "DeleteRG")]
    DeleteResourceGroup,
}

/// <summary>Template locations supported by AzureResourceManagerTemplateDeployment@3.</summary>
public enum AzureResourceManagerTemplateDeploymentTemplateLocation
{
    /// <summary>Files from the linked artifact.</summary>
    [YamlMember(Alias = "Linked artifact")]
    LinkedArtifact,
    /// <summary>URLs of the template files.</summary>
    [YamlMember(Alias = "URL of the file")]
    UrlOfTheFile,
}

/// <summary>Deployment modes supported by AzureResourceManagerTemplateDeployment@3.</summary>
public enum AzureResourceManagerTemplateDeploymentMode
{
    /// <summary>Incremental deployment.</summary>
    Incremental,
    /// <summary>Complete deployment.</summary>
    Complete,
    /// <summary>Validate the deployment without deploying resources.</summary>
    [YamlMember(Alias = "Validation")]
    Validation,
}
