using System.Collections.Generic;
using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating a <c>Docker@2</c> task for the <c>build</c>, <c>push</c>, <c>buildAndPush</c>, <c>login</c>,
/// <c>logout</c>, <c>start</c>, and <c>stop</c> commands.
/// </summary>
public class DockerTaskBuilder
{
    internal DockerTaskBuilder()
    {
    }

    /// <summary>
    /// <para>
    /// Creates the <c>build</c> command version of the Docker task.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Docker.Build("Dockerfile", repository: "contoso/my-app", tags: ["$(Build.BuildId)", "latest"])
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: Docker@2
    ///   inputs:
    ///     command: build
    ///     Dockerfile: Dockerfile
    ///     repository: contoso/my-app
    ///     tags: |-
    ///       $(Build.BuildId)
    ///       latest
    /// </code>
    /// </summary>
    /// <param name="dockerfile">Path to the Dockerfile</param>
    /// <param name="repository">Name of the repository</param>
    /// <param name="tags">A list of tags to apply to the built image</param>
    /// <param name="buildContext">Path to the build context. Use <c>**</c> to specify the directory that contains the Dockerfile</param>
    /// <param name="arguments">Additional Docker command options</param>
    /// <returns>A new instance of the <see cref="DockerBuildTask"/> with the specified arguments</returns>
    public DockerBuildTask Build(
        AdoExpression<string> dockerfile,
        AdoExpression<string>? repository = null,
        IReadOnlyList<string>? tags = null,
        AdoExpression<string>? buildContext = null,
        AdoExpression<string>? arguments = null) => new()
    {
        Dockerfile = dockerfile,
        Repository = repository,
        Tags = tags,
        BuildContext = buildContext,
        Arguments = arguments,
    };

    /// <summary>
    /// <para>
    /// Creates the <c>push</c> command version of the Docker task.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Docker.Push("contoso/my-app", tags: ["$(Build.BuildId)"])
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: Docker@2
    ///   inputs:
    ///     command: push
    ///     repository: contoso/my-app
    ///     tags: $(Build.BuildId)
    /// </code>
    /// </summary>
    /// <param name="repository">Name of the repository</param>
    /// <param name="tags">A list of tags of the image(s) to push</param>
    /// <param name="arguments">Additional Docker command options</param>
    /// <returns>A new instance of the <see cref="DockerPushTask"/> with the specified arguments</returns>
    public DockerPushTask Push(
        AdoExpression<string> repository,
        IReadOnlyList<string>? tags = null,
        AdoExpression<string>? arguments = null) => new()
    {
        Repository = repository,
        Tags = tags,
        Arguments = arguments,
    };

    /// <summary>
    /// <para>
    /// Creates the <c>buildAndPush</c> command version of the Docker task. Note that Docker@2 ignores the
    /// <c>arguments</c> input for this command.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Docker.BuildAndPush("Dockerfile", "contoso/my-app", tags: ["$(Build.BuildId)"])
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: Docker@2
    ///   inputs:
    ///     command: buildAndPush
    ///     Dockerfile: Dockerfile
    ///     repository: contoso/my-app
    ///     tags: $(Build.BuildId)
    /// </code>
    /// </summary>
    /// <param name="dockerfile">Path to the Dockerfile</param>
    /// <param name="repository">Name of the repository</param>
    /// <param name="tags">A list of tags to apply to the built image</param>
    /// <param name="buildContext">Path to the build context. Use <c>**</c> to specify the directory that contains the Dockerfile</param>
    /// <returns>A new instance of the <see cref="DockerBuildAndPushTask"/> with the specified arguments</returns>
    public DockerBuildAndPushTask BuildAndPush(
        AdoExpression<string> dockerfile,
        AdoExpression<string> repository,
        IReadOnlyList<string>? tags = null,
        AdoExpression<string>? buildContext = null) => new()
    {
        Dockerfile = dockerfile,
        Repository = repository,
        Tags = tags,
        BuildContext = buildContext,
    };

    /// <summary>
    /// <para>
    /// Creates the <c>login</c> command version of the Docker task, which authenticates with a container registry.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Docker.Login("my-registry-service-connection")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: Docker@2
    ///   inputs:
    ///     command: login
    ///     containerRegistry: my-registry-service-connection
    /// </code>
    /// </summary>
    /// <param name="containerRegistry">Docker registry service connection to authenticate with</param>
    /// <returns>A new instance of the <see cref="DockerLoginTask"/> with the specified arguments</returns>
    public DockerLoginTask Login(AdoExpression<string> containerRegistry) => new()
    {
        ContainerRegistry = containerRegistry,
    };

    /// <summary>
    /// <para>
    /// Creates the <c>logout</c> command version of the Docker task, which removes authentication for a container registry.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Docker.Logout("my-registry-service-connection")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: Docker@2
    ///   inputs:
    ///     command: logout
    ///     containerRegistry: my-registry-service-connection
    /// </code>
    /// </summary>
    /// <param name="containerRegistry">
    /// Docker registry service connection to log out of. When omitted, Docker@2 removes all authentication data
    /// from the temporary Docker config instead.
    /// </param>
    /// <returns>A new instance of the <see cref="DockerLogoutTask"/> with the specified arguments</returns>
    public DockerLogoutTask Logout(AdoExpression<string>? containerRegistry = null) => new()
    {
        ContainerRegistry = containerRegistry,
    };

    /// <summary>
    /// <para>
    /// Creates the <c>start</c> command version of the Docker task, which starts a container.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Docker.Start("my-container")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: Docker@2
    ///   inputs:
    ///     command: start
    ///     container: my-container
    /// </code>
    /// </summary>
    /// <param name="container">Name of the container</param>
    /// <param name="arguments">Additional Docker command options</param>
    /// <returns>A new instance of the <see cref="DockerStartTask"/> with the specified arguments</returns>
    public DockerStartTask Start(AdoExpression<string> container, AdoExpression<string>? arguments = null) => new()
    {
        Container = container,
        Arguments = arguments,
    };

    /// <summary>
    /// <para>
    /// Creates the <c>stop</c> command version of the Docker task, which stops a container.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Docker.Stop("my-container")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: Docker@2
    ///   inputs:
    ///     command: stop
    ///     container: my-container
    /// </code>
    /// </summary>
    /// <param name="container">Name of the container</param>
    /// <param name="arguments">Additional Docker command options</param>
    /// <returns>A new instance of the <see cref="DockerStopTask"/> with the specified arguments</returns>
    public DockerStopTask Stop(AdoExpression<string> container, AdoExpression<string>? arguments = null) => new()
    {
        Container = container,
        Arguments = arguments,
    };
}
