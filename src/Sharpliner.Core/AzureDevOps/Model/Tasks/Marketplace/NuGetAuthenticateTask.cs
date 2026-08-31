using Sharpliner.AzureDevOps.Expressions;
using System;
using System.Linq;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/nuget-authenticate-v1">NuGetAuthenticate@1</see>
/// task in Azure DevOps pipelines. The task configures NuGet tools to authenticate with Azure Artifacts and other NuGet repositories.
/// It requires NuGet &gt;= 4.8.5385, dotnet &gt;= 6, or MSBuild &gt;= 15.8.166.59604.
/// </summary>
public record NuGetAuthenticateTask : AzureDevOpsTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NuGetAuthenticateTask"/> class.
    /// </summary>
    public NuGetAuthenticateTask() : base("NuGetAuthenticate@1")
    {
        DisplayName = "Authenticate to NuGet feeds";
    }

    /// <summary>
    /// Gets or sets the comma-separated NuGet service connection credentials for feeds outside this organization or collection.
    /// For feeds in this organization or collection, leave this blank; the build's credentials are used automatically.
    /// Not compatible with <see cref="FeedUrl"/> or <see cref="AzureDevOpsServiceConnection"/>.
    /// </summary>
    [YamlIgnore]
    public string[]? NuGetServiceConnections
    {
        get => GetString("nuGetServiceConnections")?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        init => SetProperty("nuGetServiceConnections", ToCommaSeparatedValue(value));
    }

    /// <summary>
    /// Gets or sets the Azure DevOps service connection used with workload identity federation.
    /// This input is available in Azure DevOps Services and has the official task input alias <c>workloadIdentityServiceConnection</c>.
    /// If set, <see cref="FeedUrl"/> is required and all other inputs are ignored.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? AzureDevOpsServiceConnection
    {
        get => GetExpression<string>("azureDevOpsServiceConnection");
        init => SetProperty("azureDevOpsServiceConnection", value);
    }

    /// <summary>
    /// Gets or sets the Azure Artifacts feed URL used with workload identity federation.
    /// If set, <see cref="AzureDevOpsServiceConnection"/> is required and all other inputs are ignored.
    /// The URL should use the NuGet service index format:
    /// <c>https://pkgs.dev.azure.com/{ORG_NAME}/{PROJECT}/_packaging/{FEED_NAME}/nuget/v3/index.json</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? FeedUrl
    {
        get => GetExpression<string>("feedUrl");
        init => SetProperty("feedUrl", value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether to overwrite the task-provided credential provider in the user profile even if it is already installed.
    /// This may upgrade or potentially downgrade the credential provider. Default: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? ForceReinstallCredentialProvider
    {
        get => GetExpression<bool>("forceReinstallCredentialProvider");
        init => SetProperty("forceReinstallCredentialProvider", value);
    }

    private static string? ToCommaSeparatedValue(string[]? values)
    {
        if (values is null)
        {
            return null;
        }

        var normalizedValues = values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .ToArray();

        return normalizedValues.Length == 0 ? null : string.Join(",", normalizedValues);
    }
}
