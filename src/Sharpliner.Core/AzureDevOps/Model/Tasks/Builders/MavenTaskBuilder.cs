namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Provides methods to create Maven authentication tasks in Azure DevOps pipelines.
/// </summary>
public class MavenTaskBuilder
{
    /// <summary>
    /// Creates a <see cref="MavenAuthenticateTask"/> that adds credentials for Azure Artifacts feeds and/or external Maven repositories
    /// to the current user's <c>settings.xml</c> for the scope of the build.
    /// </summary>
    /// <param name="artifactsFeeds">Optional Azure Artifacts feed names to authenticate with Maven.</param>
    /// <param name="mavenServiceConnections">Optional external Maven service connection names for repositories outside this organization/collection.</param>
    /// <returns>A <see cref="MavenAuthenticateTask"/> instance.</returns>
    /// <example>
    /// <code lang="csharp">
    /// Maven.Authenticate(["MyFeedInOrg1", "MyFeedInOrg2"], ["central", "MavenOrg"])
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: MavenAuthenticate@0
    ///   displayName: Maven Authenticate
    ///   inputs:
    ///     artifactsFeeds: MyFeedInOrg1,MyFeedInOrg2
    ///     mavenServiceConnections: central,MavenOrg
    /// </code>
    /// </example>
    public MavenAuthenticateTask Authenticate(string[]? artifactsFeeds = null, string[]? mavenServiceConnections = null)
    {
        var task = new MavenAuthenticateTask();

        if (artifactsFeeds is not null)
        {
            task = task with { ArtifactsFeeds = artifactsFeeds };
        }

        if (mavenServiceConnections is not null)
        {
            task = task with { MavenServiceConnections = mavenServiceConnections };
        }

        return task;
    }

    /// <summary>
    /// Creates a <see cref="MavenAuthenticateTask"/> that uses an Azure DevOps service connection for Entra Workload Identity authentication.
    /// </summary>
    /// <param name="azureDevOpsServiceConnection">The Azure DevOps service connection name. The official alias is <c>workloadIdentityServiceConnection</c>.</param>
    /// <param name="artifactsFeeds">Optional Azure Artifacts feed names. When this authentication mode is used, feeds can be internal or cross-organization feed names.</param>
    /// <returns>A <see cref="MavenAuthenticateTask"/> instance.</returns>
    /// <example>
    /// <code lang="csharp">
    /// Maven.Authenticate("MyAzureDevOpsServiceConnection", ["MyFeedInOrg1", "CrossOrgFeed"])
    /// </code>
    /// <para>Generated YAML:</para>
    /// <code lang="yaml">
    /// - task: MavenAuthenticate@0
    ///   displayName: Maven Authenticate
    ///   inputs:
    ///     azureDevOpsServiceConnection: MyAzureDevOpsServiceConnection
    ///     artifactsFeeds: MyFeedInOrg1,CrossOrgFeed
    /// </code>
    /// </example>
    public MavenAuthenticateTask Authenticate(string azureDevOpsServiceConnection, string[]? artifactsFeeds = null)
    {
        if (string.IsNullOrWhiteSpace(azureDevOpsServiceConnection))
        {
            throw new System.ArgumentException($"'{nameof(azureDevOpsServiceConnection)}' cannot be null, empty, or whitespace.", nameof(azureDevOpsServiceConnection));
        }

        var task = new MavenAuthenticateTask
        {
            AzureDevOpsServiceConnection = azureDevOpsServiceConnection
        };

        if (artifactsFeeds is not null)
        {
            task = task with { ArtifactsFeeds = artifactsFeeds };
        }

        return task;
    }
}
