using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Runs <see href="https://github.com/GoogleContainerTools/container-structure-test">container-structure-test</see>
/// against an image from a container registry by using the <c>ContainerStructureTest@0</c> task.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/container-structure-test-v0?view=azure-pipelines">official Azure DevOps pipelines documentation</see>
/// and the
/// <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/ContainerStructureTestV0/task.json">official ContainerStructureTestV0 task specification</see>.
/// </summary>
public record ContainerStructureTestTask : AzureDevOpsTask
{
    private const string DefaultTag = "$(Build.BuildId)";

    /// <summary>
    /// Required <c>connectedService:dockerregistry</c> input.
    /// Docker registry service connection used to authenticate and pull the image.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? DockerRegistryServiceConnection
    {
        get => GetExpression<string>("dockerRegistryServiceConnection");
        init => SetProperty("dockerRegistryServiceConnection", value);
    }

    /// <summary>
    /// Required <c>string</c> input.
    /// Name of the container repository.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Repository
    {
        get => GetExpression<string>("repository");
        init => SetProperty("repository", value);
    }

    /// <summary>
    /// Optional <c>string</c> input.
    /// Image tag used when pulling the image from the selected registry.
    /// Default value: <c>$(Build.BuildId)</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Tag
    {
        get => GetExpression<string>("tag", DefaultTag);
        init => SetProperty("tag", value);
    }

    /// <summary>
    /// Required <c>filePath</c> input.
    /// Path to the container-structure-test configuration file.
    /// Use either <c>.yaml</c> or <c>.json</c> files.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ConfigFile
    {
        get => GetExpression<string>("configFile");
        init => SetProperty("configFile", value);
    }

    /// <summary>
    /// Optional <c>string</c> input.
    /// Name of the Azure DevOps test run where results are published.
    /// Default value: empty string.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? TestRunTitle
    {
        get => GetExpression<string>("testRunTitle");
        init => SetProperty("testRunTitle", value);
    }

    /// <summary>
    /// Optional <c>boolean</c> input.
    /// Set to <c>true</c> to fail the task when test failures are detected.
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? FailTaskOnFailedTests
    {
        get => GetExpression<bool>("failTaskOnFailedTests", false);
        init => SetProperty("failTaskOnFailedTests", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerStructureTestTask"/> class with required inputs.
    /// </summary>
    /// <param name="dockerRegistryServiceConnection">Docker registry service connection used to authenticate and pull the image.</param>
    /// <param name="repository">Container repository name.</param>
    /// <param name="configFile">Path to a <c>.yaml</c> or <c>.json</c> container-structure-test configuration file.</param>
    public ContainerStructureTestTask(
        AdoExpression<string> dockerRegistryServiceConnection,
        AdoExpression<string> repository,
        AdoExpression<string> configFile)
        : base("ContainerStructureTest@0")
    {
        DisplayName = "Container Structure Test";
        DockerRegistryServiceConnection = dockerRegistryServiceConnection;
        Repository = repository;
        ConfigFile = configFile;
    }
}
