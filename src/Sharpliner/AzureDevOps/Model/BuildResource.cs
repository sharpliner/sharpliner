﻿using System.Diagnostics.CodeAnalysis;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// If you have an external CI build system that produces artifacts, you can consume artifacts with a builds resource.
/// A builds resource can be any external CI systems like Jenkins, TeamCity, CircleCI, and so on.
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/process/resources?view=azure-devops&amp;tabs=schema#define-a-builds-resource">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record BuildResource
{
    /// <summary>
    /// Identifier for the resource used in build resource variables
    /// </summary>
    [YamlMember(Alias = "build")]
    public string Identifier { get; }

    /// <summary>
    /// The type of your build service like Jenkins, circleCI etc.
    /// </summary>
    [DisallowNull]
    public AdoExpression<string>? Type { get; init; }

    /// <summary>
    /// Service connection for your build service
    /// </summary>
    [DisallowNull]
    public AdoExpression<string>? Connection { get; init; }

    /// <summary>
    /// Source definition of the build
    /// </summary>
    [DisallowNull]
    public AdoExpression<string>? Source { get; init; }

    /// <summary>
    /// The build number to pick the artifact, defaults to latest successful build
    /// </summary>
    [DisallowNull]
    public AdoExpression<string>? Version { get; init; }

    /// <summary>
    /// Triggers aren't enabled by default and should be explicitly set
    /// </summary>
    [DisallowNull]
    public AdoExpression<bool>? Trigger { get; init; } = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildResource"/> class with the specified identifier.
    /// </summary>
    /// <param name="identifier">The alias or name of build artifact. Acceptable values: [-_A-Za-z0-9]*.</param>
    public BuildResource(string identifier)
    {
        Identifier = identifier ?? throw new System.ArgumentNullException(nameof(identifier));
    }
}
