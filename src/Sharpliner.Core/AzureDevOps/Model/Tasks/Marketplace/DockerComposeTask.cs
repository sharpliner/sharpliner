using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <c>DockerCompose@1</c> Azure DevOps task, which builds, pushes, runs, locks, and otherwise operates on multi-container Docker Compose applications.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/docker-compose-v1">official Azure DevOps pipelines documentation</see>
/// and the
/// <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/DockerComposeV1/task.json">official DockerComposeV1 task specification</see>.
/// </summary>
public record DockerComposeTask : AzureDevOpsTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeTask"/> class with the specified action.
    /// </summary>
    /// <param name="action">
    /// The Docker Compose action. Allowed values in <c>DockerCompose@1</c> are
    /// <c>Build services</c>, <c>Push services</c>, <c>Run services</c>, <c>Run a specific service</c>,
    /// <c>Lock services</c>, <c>Write service image digests</c>, <c>Combine configuration</c>,
    /// and <c>Run a Docker Compose command</c>.
    /// </param>
    public DockerComposeTask(string action) : base("DockerCompose@1")
    {
        SetProperty("action", action);
    }

    /// <summary>
    /// Container registry mode used by the task.
    /// Allowed values are <see cref="DockerComposeContainerRegistryType.AzureContainerRegistry"/> and
    /// <see cref="DockerComposeContainerRegistryType.ContainerRegistry"/>.
    /// Default: <see cref="DockerComposeContainerRegistryType.AzureContainerRegistry"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<DockerComposeContainerRegistryType>? ContainerRegistryType
    {
        get => GetExpression<DockerComposeContainerRegistryType>("containerregistrytype");
        init => SetProperty("containerregistrytype", value);
    }

    /// <summary>
    /// Docker registry service connection used when <see cref="ContainerRegistryType"/> is
    /// <see cref="DockerComposeContainerRegistryType.ContainerRegistry"/>.
    /// The official DockerCompose@1 input is <c>dockerRegistryEndpoint</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? DockerRegistryEndpoint
    {
        get => GetExpression<string>("dockerRegistryEndpoint");
        init => SetProperty("dockerRegistryEndpoint", value);
    }

    /// <summary>
    /// Azure Resource Manager service connection used when <see cref="ContainerRegistryType"/> is
    /// <see cref="DockerComposeContainerRegistryType.AzureContainerRegistry"/>.
    /// The official DockerCompose@1 input is <c>azureSubscriptionEndpoint</c>; <c>azureSubscription</c> is its documented alias.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? AzureSubscription
    {
        get => GetExpression<string>("azureSubscriptionEndpoint");
        init => SetProperty("azureSubscriptionEndpoint", value);
    }

    /// <summary>
    /// Azure Container Registry name used when <see cref="ContainerRegistryType"/> is
    /// <see cref="DockerComposeContainerRegistryType.AzureContainerRegistry"/>.
    /// Default: empty.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? AzureContainerRegistry
    {
        get => GetExpression<string>("azureContainerRegistry");
        init => SetProperty("azureContainerRegistry", value);
    }

    /// <summary>
    /// Required Docker Compose file path.
    /// Supports minimatch patterns and defaults to <c>**/docker-compose.yml</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? DockerComposeFile
    {
        get => GetExpression<string>("dockerComposeFile");
        init => SetProperty("dockerComposeFile", value);
    }

    /// <summary>
    /// Optional additional Docker Compose files.
    /// Provide multiple files as a new-line separated <c>multiLine</c> value.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? AdditionalDockerComposeFiles
    {
        get => GetExpression<string>("additionalDockerComposeFiles");
        init => SetProperty("additionalDockerComposeFiles", value);
    }

    /// <summary>
    /// Optional environment variables passed as Docker Compose file arguments.
    /// This maps to DockerCompose@1 input <c>dockerComposeFileArgs</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? DockerComposeFileArgs
    {
        get => GetExpression<string>("dockerComposeFileArgs");
        init => SetProperty("dockerComposeFileArgs", value);
    }

    /// <summary>
    /// Optional Docker Compose project name.
    /// Default: <c>$(Build.Repository.Name)</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ProjectName
    {
        get => GetExpression<string>("projectName");
        init => SetProperty("projectName", value);
    }

    /// <summary>
    /// Optional flag controlling whether image names are qualified with the selected registry.
    /// Default: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? QualifyImageNames
    {
        get => GetExpression<bool>("qualifyImageNames");
        init => SetProperty("qualifyImageNames", value);
    }

    /// <summary>
    /// Optional extra arguments passed to the action.
    /// DockerCompose@1 supports this input for every action except <c>Lock services</c>,
    /// <c>Combine configuration</c>, and <c>Write service image digests</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Arguments
    {
        get => GetExpression<string>("arguments");
        init => SetProperty("arguments", value);
    }

    /// <summary>
    /// Optional Docker host service connection.
    /// This advanced DockerCompose@1 input is <c>dockerHostEndpoint</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? DockerHostEndpoint
    {
        get => GetExpression<string>("dockerHostEndpoint");
        init => SetProperty("dockerHostEndpoint", value);
    }

    /// <summary>
    /// Optional advanced flag that turns the task into a no-op when no Docker Compose file is found.
    /// Default: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? NopIfNoDockerComposeFile
    {
        get => GetExpression<bool>("nopIfNoDockerComposeFile");
        init => SetProperty("nopIfNoDockerComposeFile", value);
    }

    /// <summary>
    /// Optional advanced flag that fails the task when the files listed in <see cref="AdditionalDockerComposeFiles"/>
    /// are missing.
    /// Default: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? RequireAdditionalDockerComposeFiles
    {
        get => GetExpression<bool>("requireAdditionalDockerComposeFiles");
        init => SetProperty("requireAdditionalDockerComposeFiles", value);
    }

    /// <summary>
    /// Optional advanced current working directory.
    /// The official DockerCompose@1 input is <c>cwd</c>; <c>currentWorkingDirectory</c> is its documented alias.
    /// Default: <c>$(System.DefaultWorkingDirectory)</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? CurrentWorkingDirectory
    {
        get => GetExpression<string>("cwd");
        init => SetProperty("cwd", value);
    }

    /// <summary>
    /// Optional path to the Docker Compose executable.
    /// Default: empty, which lets the task resolve Docker Compose from the agent environment.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? DockerComposePath
    {
        get => GetExpression<string>("dockerComposePath");
        init => SetProperty("dockerComposePath", value);
    }
}

/// <summary>
/// Container registry types supported by the <c>DockerCompose@1</c> task.
/// </summary>
public enum DockerComposeContainerRegistryType
{
    /// <summary>
    /// Use an Azure Container Registry selected through Azure Resource Manager.
    /// Serialized as <c>Azure Container Registry</c>.
    /// </summary>
    [YamlMember(Alias = "Azure Container Registry")]
    AzureContainerRegistry,

    /// <summary>
    /// Use a generic Docker registry service connection.
    /// Serialized as <c>Container Registry</c>.
    /// </summary>
    [YamlMember(Alias = "Container Registry")]
    ContainerRegistry,
}
