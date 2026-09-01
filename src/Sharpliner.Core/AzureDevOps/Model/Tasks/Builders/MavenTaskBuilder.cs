using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating Azure DevOps <c>Maven</c> tasks.
/// See <see cref="MavenTask"/>, <see cref="MavenV3Task"/>, <see cref="MavenV2Task"/>, and <see cref="MavenV1Task"/>.
/// </summary>
public class MavenTaskBuilder
{
    internal MavenTaskBuilder()
    {
    }

    /// <summary>
    /// Creates a <see cref="MavenTask"/> targeting <c>Maven@4</c>.
    /// Despite the method name, any Maven goals can be supplied through <paramref name="goals"/>.
    /// </summary>
    /// <param name="mavenPOMFile">Optional relative path to the Maven POM file.</param>
    /// <param name="goals">Optional Maven goals to execute.</param>
    /// <param name="options">Optional additional Maven command-line options.</param>
    /// <returns>A <see cref="MavenTask"/> instance.</returns>
    public MavenTask Build(AdoExpression<string>? mavenPOMFile = null, AdoExpression<string>? goals = null, AdoExpression<string>? options = null)
        => Create(new MavenTask(), mavenPOMFile, goals, options);

    /// <summary>
    /// Creates a deprecated <see cref="MavenV3Task"/> targeting <c>Maven@3</c>.
    /// </summary>
    /// <param name="mavenPOMFile">Optional relative path to the Maven POM file.</param>
    /// <param name="goals">Optional Maven goals to execute.</param>
    /// <param name="options">Optional additional Maven command-line options.</param>
    /// <returns>A <see cref="MavenV3Task"/> instance.</returns>
    public MavenV3Task BuildV3(AdoExpression<string>? mavenPOMFile = null, AdoExpression<string>? goals = null, AdoExpression<string>? options = null)
        => Create(new MavenV3Task(), mavenPOMFile, goals, options);

    /// <summary>
    /// Creates a deprecated <see cref="MavenV2Task"/> targeting <c>Maven@2</c>.
    /// </summary>
    /// <param name="mavenPOMFile">Optional relative path to the Maven POM file.</param>
    /// <param name="goals">Optional Maven goals to execute.</param>
    /// <param name="options">Optional additional Maven command-line options.</param>
    /// <returns>A <see cref="MavenV2Task"/> instance.</returns>
    public MavenV2Task BuildV2(AdoExpression<string>? mavenPOMFile = null, AdoExpression<string>? goals = null, AdoExpression<string>? options = null)
        => Create(new MavenV2Task(), mavenPOMFile, goals, options);

    /// <summary>
    /// Creates a deprecated <see cref="MavenV1Task"/> targeting <c>Maven@1</c>.
    /// </summary>
    /// <param name="mavenPOMFile">Optional relative path to the Maven POM file.</param>
    /// <param name="goals">Optional Maven goals to execute.</param>
    /// <param name="options">Optional additional Maven command-line options.</param>
    /// <returns>A <see cref="MavenV1Task"/> instance.</returns>
    public MavenV1Task BuildV1(AdoExpression<string>? mavenPOMFile = null, AdoExpression<string>? goals = null, AdoExpression<string>? options = null)
        => Create(new MavenV1Task(), mavenPOMFile, goals, options);

    private static TTask Create<TTask>(TTask task, AdoExpression<string>? mavenPOMFile, AdoExpression<string>? goals, AdoExpression<string>? options)
        where TTask : MavenTaskBase
        => task with
        {
            MavenPOMFile = mavenPOMFile,
            Goals = goals,
            Options = options,
        };
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
