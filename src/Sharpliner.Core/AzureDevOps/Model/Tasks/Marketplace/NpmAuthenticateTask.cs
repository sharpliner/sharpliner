using System;
using System.Linq;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/npm-authenticate-v0">npmAuthenticate@0</see>
/// task in Azure DevOps pipelines. The task provides npm credentials to an <c>.npmrc</c> file in your repository for the scope of the build, which
/// enables npm to authenticate with private registries (e.g. Azure Artifacts feeds or other registries declared in the <c>.npmrc</c>).
/// </summary>
public record NpmAuthenticateTask : AzureDevOpsTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NpmAuthenticateTask"/> class.
    /// </summary>
    /// <param name="workingFile">The path to the <c>.npmrc</c> file that lists the registries you want to work with. Select the file, not the folder, such as <c>/packages/mypackage/.npmrc</c>.</param>
    public NpmAuthenticateTask(string workingFile) : base("npmAuthenticate@0")
    {
        WorkingFile = workingFile;
    }

    /// <summary>
    /// Gets or sets the path to the <c>.npmrc</c> file that lists the registries you want to work with.
    /// Select the file, not the folder, such as <c>/packages/mypackage/.npmrc</c>.
    /// </summary>
    [YamlIgnore]
    public string WorkingFile
    {
        get => GetString("workingFile")!;
        init => SetProperty("workingFile", value);
    }

    /// <summary>
    /// Gets or sets the list of names of npm service connections for registries
    /// outside this organization/collection. The <c>.npmrc</c> file must contain registry entries
    /// corresponding to the service connections. If only registries in this organization/collection
    /// are needed, leave this blank. The build's credentials are used automatically.
    /// </summary>
    [YamlIgnore]
    public string[]? CustomEndpoints
    {
        get => GetString("customEndpoint")?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        init => SetProperty("customEndpoint", ToCustomEndpointValue(value));
    }

    private static string? ToCustomEndpointValue(string[]? endpoints)
    {
        if (endpoints is null)
        {
            return null;
        }

        var normalizedEndpoints = endpoints
            .Where(endpoint => !string.IsNullOrWhiteSpace(endpoint))
            .Select(endpoint => endpoint.Trim())
            .ToArray();

        return normalizedEndpoints.Length == 0 ? null : string.Join(",", normalizedEndpoints);
    }
}
