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
}
