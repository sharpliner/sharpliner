namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <c>HelmDeploy@1</c> task with <c>command: ls</c>, which lists the releases in the cluster.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/helm-deploy-v1">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record HelmDeployLsTask : HelmDeployCommandTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelmDeployLsTask"/> record.
    /// </summary>
    public HelmDeployLsTask() : base("ls")
    {
    }
}

/// <summary>
/// Represents the <c>HelmDeploy@1</c> task with <c>command: get</c>, which downloads the information of a named release.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/helm-deploy-v1">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record HelmDeployGetTask : HelmDeployCommandTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelmDeployGetTask"/> record.
    /// </summary>
    public HelmDeployGetTask() : base("get")
    {
    }
}

/// <summary>
/// Represents the <c>HelmDeploy@1</c> task with <c>command: expose</c>, which exposes a release through a Kubernetes service.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/helm-deploy-v1">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record HelmDeployExposeTask : HelmDeployCommandTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelmDeployExposeTask"/> record.
    /// </summary>
    public HelmDeployExposeTask() : base("expose")
    {
    }
}

/// <summary>
/// Represents the <c>HelmDeploy@1</c> task with <c>command: login</c>, which logs in to an Azure Container Registry.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/helm-deploy-v1">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record HelmDeployLoginTask : HelmDeployTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelmDeployLoginTask"/> record.
    /// </summary>
    public HelmDeployLoginTask() : base("login")
    {
    }
}

/// <summary>
/// Represents the <c>HelmDeploy@1</c> task with <c>command: logout</c>, which logs out of an Azure Container Registry.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/helm-deploy-v1">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record HelmDeployLogoutTask : HelmDeployTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HelmDeployLogoutTask"/> record.
    /// </summary>
    public HelmDeployLogoutTask() : base("logout")
    {
    }
}
