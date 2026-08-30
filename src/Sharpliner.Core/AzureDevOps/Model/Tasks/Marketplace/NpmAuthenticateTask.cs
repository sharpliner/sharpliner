using System;
using System.Linq;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/npm-authenticate-v0">npmAuthenticate@0</see>
/// task in Azure DevOps pipelines.
/// </summary>
/// <remarks>
/// <para>
/// Audited against the <c>NpmAuthenticateV0</c> task specification from
/// <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/9dabcbcbcbc3b5a1a94fd32acaa2766fdf934bd6/Tasks/NpmAuthenticateV0/task.json">microsoft/azure-pipelines-tasks</see>
/// at revision <c>9dabcbcbcbc3b5a1a94fd32acaa2766fdf934bd6</c> (<c>2026-08-28</c>, task.json version <c>0.277.0</c>) and
/// the current <see href="https://raw.githubusercontent.com/MicrosoftDocs/azure-devops-yaml-schema/main/task-reference/npm-authenticate-v0.md">Microsoft Learn YAML task reference</see>
/// (<c>ms.date: 07/28/2026</c>).
/// </para>
/// <para>
/// The task provides npm credentials to an <c>.npmrc</c> file in your repository for the scope of the build. It is intended for npm task runners such as
/// gulp and Grunt; do not use it when the pipeline also uses the Azure Pipelines npm task, which handles authentication itself.
/// </para>
/// <para>
/// The task appends authentication entries to the selected <c>.npmrc</c> during the job and restores the file in post-job cleanup. Keep credentials out of
/// source-controlled <c>.npmrc</c> files and store external registry credentials in npm service connections.
/// </para>
/// <para>
/// Official task inputs are <c>workingFile</c> (<c>filePath</c>, required), <c>azureDevOpsServiceConnection</c>
/// (alias <c>workloadIdentityServiceConnection</c>, optional), <c>feedUrl</c> (optional), and <c>customEndpoint</c>
/// (<c>connectedService:externalnpmregistry</c>, optional multi-select list). These inputs define no default values, conditions, or visibility rules.
/// </para>
/// </remarks>
public record NpmAuthenticateTask : AzureDevOpsTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NpmAuthenticateTask"/> class.
    /// </summary>
    /// <param name="workingFile">The path to the <c>.npmrc</c> file that specifies the registries you want to work with. Select the file, not the folder, such as <c>/packages/mypackage/.npmrc</c>.</param>
    public NpmAuthenticateTask(string workingFile) : base("npmAuthenticate@0")
    {
        WorkingFile = workingFile;
    }

    /// <summary>
    /// Gets or sets the path to the <c>.npmrc</c> file that specifies the registries you want to work with.
    /// Select the file, not the folder, such as <c>/packages/mypackage/.npmrc</c>.
    /// </summary>
    [YamlIgnore]
    public string WorkingFile
    {
        get => GetString("workingFile")!;
        init => SetProperty("workingFile", value);
    }

    /// <summary>
    /// Gets or sets the Azure DevOps service connection used to authenticate to an Azure Artifacts feed with Workload Identity Federation.
    /// </summary>
    /// <remarks>
    /// This input is available in Azure Pipelines and has the official YAML alias <c>workloadIdentityServiceConnection</c>.
    /// If this value is set, <see cref="FeedUrl"/> is required. This input is not compatible with <see cref="CustomEndpoints"/>.
    /// </remarks>
    [YamlIgnore]
    public string? AzureDevOpsServiceConnection
    {
        get => GetString("azureDevOpsServiceConnection");
        init => SetOptionalStringProperty("azureDevOpsServiceConnection", value);
    }

    /// <summary>
    /// Gets or sets the Azure Artifacts feed URL, in npm registry format, to use with <see cref="AzureDevOpsServiceConnection"/>.
    /// </summary>
    /// <remarks>
    /// If this value is set, <see cref="AzureDevOpsServiceConnection"/> is required. This input is not compatible with <see cref="CustomEndpoints"/>.
    /// Example format: <c>https://pkgs.dev.azure.com/{ORG_NAME}/{PROJECT}/_packaging/{FEED_NAME}/npm/registry/</c>.
    /// </remarks>
    [YamlIgnore]
    public string? FeedUrl
    {
        get => GetString("feedUrl");
        init => SetOptionalStringProperty("feedUrl", value);
    }

    /// <summary>
    /// Gets or sets the npm service connection names to use for external registries located in the selected <c>.npmrc</c>.
    /// For registries in this organization/collection, leave this blank; the build's credentials are used automatically.
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

    private void SetOptionalStringProperty(string name, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Inputs.Remove(name);
        }
        else
        {
            SetProperty(name, value);
        }
    }
}
