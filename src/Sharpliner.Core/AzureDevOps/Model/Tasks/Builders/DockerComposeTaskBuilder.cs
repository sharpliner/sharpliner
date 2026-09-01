using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating <c>DockerCompose@1</c> tasks.
/// </summary>
public class DockerComposeTaskBuilder
{
    internal DockerComposeTaskBuilder()
    {
    }

    /// <summary>
    /// <para>
    /// Creates the <c>Build services</c> action of the Docker Compose task.
    /// </para>
    /// <code lang="csharp">
    /// DockerCompose.Build("docker-compose.yml") with
    /// {
    ///     AdditionalImageTags = "latest\n$(Build.BuildNumber)"
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: DockerCompose@1
    ///   displayName: Docker Compose build services
    ///   inputs:
    ///     action: Build services
    ///     dockerComposeFile: docker-compose.yml
    ///     additionalImageTags: |-
    ///       latest
    ///       $(Build.BuildNumber)
    /// </code>
    /// </summary>
    /// <param name="dockerComposeFile">Optional Docker Compose file path. Default: <c>**/docker-compose.yml</c>.</param>
    /// <returns>A <see cref="DockerComposeBuildTask"/> instance.</returns>
    public DockerComposeBuildTask Build(AdoExpression<string>? dockerComposeFile = null)
    {
        var task = new DockerComposeBuildTask();

        if (dockerComposeFile is not null)
        {
            task = task with
            {
                DockerComposeFile = dockerComposeFile,
            };
        }

        return task;
    }

    /// <summary>
    /// <para>
    /// Creates the <c>Push services</c> action of the Docker Compose task.
    /// </para>
    /// <code lang="csharp">
    /// DockerCompose.Push("docker-compose.yml")
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: DockerCompose@1
    ///   displayName: Docker Compose push services
    ///   inputs:
    ///     action: Push services
    ///     dockerComposeFile: docker-compose.yml
    /// </code>
    /// </summary>
    /// <param name="dockerComposeFile">Optional Docker Compose file path. Default: <c>**/docker-compose.yml</c>.</param>
    /// <returns>A <see cref="DockerComposePushTask"/> instance.</returns>
    public DockerComposePushTask Push(AdoExpression<string>? dockerComposeFile = null)
    {
        var task = new DockerComposePushTask();

        if (dockerComposeFile is not null)
        {
            task = task with
            {
                DockerComposeFile = dockerComposeFile,
            };
        }

        return task;
    }

    /// <summary>
    /// <para>
    /// Creates the <c>Run services</c> action of the Docker Compose task.
    /// </para>
    /// <code lang="csharp">
    /// DockerCompose.Run("docker-compose.yml") with
    /// {
    ///     Detached = false,
    ///     AbortOnContainerExit = true,
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: DockerCompose@1
    ///   displayName: Docker Compose run services
    ///   inputs:
    ///     action: Run services
    ///     dockerComposeFile: docker-compose.yml
    ///     detached: false
    ///     abortOnContainerExit: true
    /// </code>
    /// </summary>
    /// <param name="dockerComposeFile">Optional Docker Compose file path. Default: <c>**/docker-compose.yml</c>.</param>
    /// <returns>A <see cref="DockerComposeRunTask"/> instance.</returns>
    public DockerComposeRunTask Run(AdoExpression<string>? dockerComposeFile = null)
    {
        var task = new DockerComposeRunTask();

        if (dockerComposeFile is not null)
        {
            task = task with
            {
                DockerComposeFile = dockerComposeFile,
            };
        }

        return task;
    }

    /// <summary>
    /// <para>
    /// Creates the <c>Run a specific service</c> action of the Docker Compose task.
    /// </para>
    /// <code lang="csharp">
    /// DockerCompose.RunService("web")
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: DockerCompose@1
    ///   displayName: Docker Compose run a specific service
    ///   inputs:
    ///     action: Run a specific service
    ///     serviceName: web
    /// </code>
    /// </summary>
    /// <param name="serviceName">Required Docker Compose service name.</param>
    /// <returns>A <see cref="DockerComposeRunServiceTask"/> instance.</returns>
    public DockerComposeRunServiceTask RunService(string serviceName) => new(serviceName);

    /// <summary>
    /// <para>
    /// Creates the <c>Lock services</c> action of the Docker Compose task.
    /// </para>
    /// <code lang="csharp">
    /// DockerCompose.Lock("docker-compose.yml")
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: DockerCompose@1
    ///   displayName: Docker Compose lock services
    ///   inputs:
    ///     action: Lock services
    ///     dockerComposeFile: docker-compose.yml
    /// </code>
    /// </summary>
    /// <param name="dockerComposeFile">Optional Docker Compose file path. Default: <c>**/docker-compose.yml</c>.</param>
    /// <returns>A <see cref="DockerComposeLockTask"/> instance.</returns>
    public DockerComposeLockTask Lock(AdoExpression<string>? dockerComposeFile = null)
    {
        var task = new DockerComposeLockTask();

        if (dockerComposeFile is not null)
        {
            task = task with
            {
                DockerComposeFile = dockerComposeFile,
            };
        }

        return task;
    }

    /// <summary>
    /// <para>
    /// Creates the <c>Write service image digests</c> action of the Docker Compose task.
    /// </para>
    /// <code lang="csharp">
    /// DockerCompose.WriteImageDigests("docker-compose.yml")
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: DockerCompose@1
    ///   displayName: Docker Compose write service image digests
    ///   inputs:
    ///     action: Write service image digests
    ///     dockerComposeFile: docker-compose.yml
    /// </code>
    /// </summary>
    /// <param name="dockerComposeFile">Optional Docker Compose file path. Default: <c>**/docker-compose.yml</c>.</param>
    /// <returns>A <see cref="DockerComposeWriteImageDigestsTask"/> instance.</returns>
    public DockerComposeWriteImageDigestsTask WriteImageDigests(AdoExpression<string>? dockerComposeFile = null)
    {
        var task = new DockerComposeWriteImageDigestsTask();

        if (dockerComposeFile is not null)
        {
            task = task with
            {
                DockerComposeFile = dockerComposeFile,
            };
        }

        return task;
    }

    /// <summary>
    /// <para>
    /// Creates the <c>Combine configuration</c> action of the Docker Compose task.
    /// </para>
    /// <code lang="csharp">
    /// DockerCompose.CombineConfiguration("docker-compose.yml")
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: DockerCompose@1
    ///   displayName: Docker Compose combine configuration
    ///   inputs:
    ///     action: Combine configuration
    ///     dockerComposeFile: docker-compose.yml
    /// </code>
    /// </summary>
    /// <param name="dockerComposeFile">Optional Docker Compose file path. Default: <c>**/docker-compose.yml</c>.</param>
    /// <returns>A <see cref="DockerComposeCombineConfigurationTask"/> instance.</returns>
    public DockerComposeCombineConfigurationTask CombineConfiguration(AdoExpression<string>? dockerComposeFile = null)
    {
        var task = new DockerComposeCombineConfigurationTask();

        if (dockerComposeFile is not null)
        {
            task = task with
            {
                DockerComposeFile = dockerComposeFile,
            };
        }

        return task;
    }

    /// <summary>
    /// <para>
    /// Creates the <c>Run a Docker Compose command</c> action of the Docker Compose task.
    /// </para>
    /// <code lang="csharp">
    /// DockerCompose.Command("config")
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: DockerCompose@1
    ///   displayName: Docker Compose command
    ///   inputs:
    ///     action: Run a Docker Compose command
    ///     dockerComposeCommand: config
    /// </code>
    /// </summary>
    /// <param name="command">Required Docker Compose command.</param>
    /// <returns>A <see cref="DockerComposeCommandTask"/> instance.</returns>
    public DockerComposeCommandTask Command(string command) => new(command);
}
