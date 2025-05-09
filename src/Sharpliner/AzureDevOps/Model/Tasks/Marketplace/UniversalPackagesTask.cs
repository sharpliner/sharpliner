using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/universal-packages-v0?view=azure-pipelines">official Azure DevOps pipelines documentation</see>
/// </summary>
public abstract record UniversalPackagesTask : AzureDevOpsTask
{
    /// <summary>
    /// Specifies the Universal Package command to run.
    /// Allowed values: download, publish.
    /// Default value: download.
    /// </summary>
    [YamlIgnore]
    public string? Command
    {
        get => GetString("command");
        init => SetProperty("command", value);
    }

    /// <summary>
    /// Specifies the amount of detail displayed in the output.
    /// Allowed values: None, Trace, Debug, Information, Warning, Error, Critical.
    /// Default value: None.
    /// </summary>
    [YamlIgnore]
    public string? Verbosity
    {
        get => GetString("verbosity");
        init => SetProperty("verbosity", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UniversalPackagesTask"/> class with required properties.
    /// </summary>
    public UniversalPackagesTask(string command)
        : base("UniversalPackages@0")
    {
        SetProperty("command", command);
    }
}
