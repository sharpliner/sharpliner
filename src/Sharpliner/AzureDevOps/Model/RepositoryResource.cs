﻿using System;
using System.Diagnostics.CodeAnalysis;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// If your pipeline has templates in another repository, or if you want to use multi-repo checkout with a repository that requires a service connection, you must let the system know about that repository.
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/process/resources?view=azure-devops&amp;tabs=schema#define-a-repositories-resource">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record RepositoryResource
{
    /// <summary>
    /// Identifier for the resource used in pipeline resource variables (A-Z, a-z, 0-9, and underscore)
    /// </summary>
    [YamlMember(Alias = "repository")]
    public string Identifier { get; }

    /// <summary>
    /// Project for the source
    /// Optional for current project
    /// </summary>
    [YamlMember(DefaultValuesHandling = DefaultValuesHandling.Preserve)]
    public RepositoryType Type { get; init; } = RepositoryType.Git;

    /// <summary>
    /// Repository name (format depends on `Type`)
    /// </summary>
    [DisallowNull]
    public AdoExpression<string>? Name { get; init; }

    /// <summary>
    /// Ref name to checkout; defaults to <c>refs/heads/main</c>
    /// </summary>
    [DisallowNull]
    public AdoExpression<string>? Ref { get; init; }

    /// <summary>
    /// Name of the service connection to use (for types that aren't Azure Repos)
    /// </summary>
    [DisallowNull]
    public AdoExpression<string>? Endpoint { get; init; }

    /// <summary>
    /// Triggers are not enabled by default unless you add trigger section to the resource
    /// </summary>
    [DisallowNull]
    public AdoExpression<Trigger>? Trigger { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryResource"/> class with the specified identifier.
    /// </summary>
    /// <param name="identifier">The identifier for the resource used in pipeline resource.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="identifier"/> is null.</exception>
    public RepositoryResource(string identifier)
    {
        Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
    }

    /// <summary>
    /// Defines a GitHub repository resource.
    /// </summary>
    /// <param name="identifier">Name of the resource to be referenced later (in the checkout step)</param>
    /// <param name="orgOrUser">Name of the organization / owner of the repository</param>
    /// <param name="repoName">Name of the repository</param>
    /// <param name="endpoint">Endpoint to use to checkout this repository</param>
    /// <param name="refName">Ref to checkout</param>
    public static RepositoryResource GitHub(
        string identifier,
        string orgOrUser,
        string repoName,
        string endpoint,
        string? refName = null)
        => CreateResource(RepositoryType.GitHub, identifier, orgOrUser, repoName, endpoint, refName);

    /// <summary>
    /// Defines a GitHub Enterprise repository resource.
    /// </summary>
    /// <param name="identifier">Name of the resource to be referenced later (in the checkout step)</param>
    /// <param name="orgOrUser">Name of the organization / owner of the repository</param>
    /// <param name="repoName">Name of the repository</param>
    /// <param name="endpoint">Endpoint to use to checkout this repository</param>
    /// <param name="refName">Ref to checkout</param>
    public static RepositoryResource GitHubEnterprise(
        string identifier,
        string orgOrUser,
        string repoName,
        string endpoint,
        string? refName = null)
        => CreateResource(RepositoryType.GitHubEnterprise, identifier, orgOrUser, repoName, endpoint, refName);

    /// <summary>
    /// Defines a Azure DevOps repository resource.
    /// </summary>
    /// <param name="identifier">Name of the resource to be referenced later (in the checkout step)</param>
    /// <param name="project">Name of the Azure DevOps project</param>
    /// <param name="repoName">Name of the repository</param>
    /// <param name="endpoint">Endpoint to use to checkout this repository</param>
    /// <param name="refName">Ref to checkout</param>
    public static RepositoryResource AzureDevOps(
        string identifier,
        string project,
        string repoName,
        string? endpoint = null,
        string? refName = null)
        => CreateResource(RepositoryType.Git, identifier, project, repoName, endpoint, refName);

    /// <summary>
    /// Defines a BitBucket repository resource.
    /// </summary>
    /// <param name="identifier">Name of the resource to be referenced later (in the checkout step)</param>
    /// <param name="orgOrUser">Name of the organization / owner of the repository</param>
    /// <param name="repoName">Name of the repository</param>
    /// <param name="endpoint">Endpoint to use to checkout this repository</param>
    /// <param name="refName">Ref to checkout</param>
    public static RepositoryResource BitBucket(
        string identifier,
        string orgOrUser,
        string repoName,
        string endpoint,
        string? refName = null)
        => CreateResource(RepositoryType.Git, identifier, orgOrUser, repoName, endpoint, refName);

    private static RepositoryResource CreateResource(
        RepositoryType type,
        string identifier,
        string orgOrUser,
        string repoName,
        string? endpoint,
        string? refName)
        => CreateResource(type, identifier, $"{orgOrUser}/{repoName}", endpoint, refName);

    private static RepositoryResource CreateResource(
        RepositoryType type,
        string identifier,
        string name,
        string? endpoint,
        string? refName)
        => new(identifier)
        {
            Type = type,
            Name = name,
            Endpoint = endpoint!,
            Ref = refName!,
        };
}

/// <summary>
/// Type of the repository.
/// </summary>
public enum RepositoryType
{
    /// <summary>
    /// The name value refers to another repository in the same project.
    /// Example name: otherRepo
    /// To refer to a repository in another project within the same organization, prefix the name with that project's name.
    /// Example name: OtherProject/otherRepo.
    /// </summary>
    [YamlMember(Alias = "git")]
    Git,

    /// <summary>
    /// The name value is the full name of the GitHub repository and includes the user or organization.
    /// Example name: Microsoft/vscode
    /// </summary>
    [YamlMember(Alias = "github")]
    GitHub,

    /// <summary>
    /// The name value is the full name of the GitHub Enterprise repository and includes the user or organization.
    /// Example name: Microsoft/vscode
    /// </summary>
    [YamlMember(Alias = "githubenterprise")]
    GitHubEnterprise,

    /// <summary>
    /// The name value is the full name of the Bitbucket Cloud repository and includes the user or organization.
    /// Example name: MyBitbucket/vscode
    /// </summary>
    [YamlMember(Alias = "bitbucket")]
    BitBucket,
}
