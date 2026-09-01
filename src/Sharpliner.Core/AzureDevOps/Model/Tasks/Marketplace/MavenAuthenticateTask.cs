using System;
using System.Linq;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/maven-authenticate-v0">MavenAuthenticate@0</see>
/// task in Azure DevOps pipelines.
/// </summary>
/// <remarks>
/// <para>
/// Audited against the <c>MavenAuthenticateV0</c> task specification from
/// <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/MavenAuthenticateV0/task.json">microsoft/azure-pipelines-tasks</see>
/// (task.json version <c>0.280.0</c>, retrieved <c>2026-08-31</c>) and
/// the current <see href="https://raw.githubusercontent.com/MicrosoftDocs/azure-devops-yaml-schema/main/task-reference/maven-authenticate-v0.md">Microsoft Learn YAML task reference</see>
/// (<c>ms.date: 07/28/2026</c>).
/// </para>
/// <para>
/// The task adds credentials for Azure Artifacts feeds and external Maven repositories to the current user's <c>settings.xml</c>.
/// By default, it reads/writes <c>$HOME/.m2/settings.xml</c> on Linux and macOS and <c>%USERPROFILE%\.m2\settings.xml</c> on Windows;
/// if no file exists, it creates one.
/// </para>
/// <para>
/// If your build uses a custom settings file via <c>mvn -s</c>, this task does not update that file directly. In that scenario,
/// add a matching <c>&lt;server&gt;</c> entry that uses <c>${env.SYSTEM_ACCESSTOKEN}</c> in your custom <c>settings.xml</c>.
/// </para>
/// </remarks>
public record MavenAuthenticateTask : AzureDevOpsTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MavenAuthenticateTask"/> class.
    /// </summary>
    public MavenAuthenticateTask() : base("MavenAuthenticate@0")
    {
        DisplayName = "Maven Authenticate";
    }

    /// <summary>
    /// Gets or sets the Azure Artifacts feed names to authenticate with Maven.
    /// Specify feed names as a list; the task emits the official comma-separated <c>artifactsFeeds</c> input.
    /// </summary>
    /// <remarks>
    /// If you only need authentication for external Maven repositories, leave this value blank.
    /// When <see cref="AzureDevOpsServiceConnection"/> is set, these can be internal or cross-organization feed names.
    /// </remarks>
    [YamlIgnore]
    public string[]? ArtifactsFeeds
    {
        get => GetString("artifactsFeeds")?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        init => SetProperty("artifactsFeeds", ToCommaSeparatedValue(value));
    }

    /// <summary>
    /// Gets or sets the external Maven service connection names used for repositories outside this organization/collection.
    /// Specify service connection names as a list; the task emits the official comma-separated <c>mavenServiceConnections</c> input.
    /// </summary>
    /// <remarks>
    /// If you only need authentication for Azure Artifacts feeds, leave this value blank.
    /// When <see cref="AzureDevOpsServiceConnection"/> is set, this input is ignored by the task.
    /// </remarks>
    [YamlIgnore]
    public string[]? MavenServiceConnections
    {
        get => GetString("mavenServiceConnections")?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        init => SetProperty("mavenServiceConnections", ToCommaSeparatedValue(value));
    }

    /// <summary>
    /// Gets or sets the Azure DevOps service connection used for Entra Workload Identity authentication.
    /// This input has the official alias <c>workloadIdentityServiceConnection</c>.
    /// </summary>
    /// <remarks>
    /// If this value is set, <see cref="MavenServiceConnections"/> is ignored by the task.
    /// </remarks>
    [YamlIgnore]
    public string? AzureDevOpsServiceConnection
    {
        get => GetString("azureDevOpsServiceConnection");
        init => SetOptionalStringProperty("azureDevOpsServiceConnection", value);
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
