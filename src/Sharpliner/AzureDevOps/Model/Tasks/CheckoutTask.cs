﻿using System.ComponentModel;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#checkout">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract record CheckoutTask : Step
{
    [YamlMember(Order = 1)]
    public abstract string Checkout { get; }

    /// <summary>
    /// When true, run `execute git clean -ffdx &amp;&amp; git reset --hard HEAD` before fetching.
    /// Defaults to false.
    /// </summary>
    [YamlMember(Order = 100)]
    public Conditioned<bool>? Clean { get; init; }

    /// <summary>
    /// The depth of commits to ask Git to fetch.
    /// Defaults to shallow fetch (= 1).
    /// Set 0 to no limit (full clone).
    /// </summary>
    [YamlMember(Order = 101)]
    public Conditioned<int>? FetchDepth { get; init; }

    /// <summary>
    /// Set to 'true' to sync tags when fetching the repo, or 'false' to not sync tags.
    /// See remarks for the default behavior in the official documentation:
    /// https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/steps-checkout#remarks
    /// </summary>
    [YamlMember(Order = 102)]
    public Conditioned<bool>? FetchTags { get; init; }

    /// <summary>
    /// Whether to download Git-LFS files.
    /// Defaults to false.
    /// </summary>
    [YamlMember(Order = 103)]
    public Conditioned<bool>? Lfs { get; init; }

    /// <summary>
    /// Submodules checkout strategy (single level, recursive to get submodules of submodules).
    /// Defaults to not checking out submodules.
    /// </summary>
    [YamlMember(Order = 104)]
    public Conditioned<SubmoduleCheckout>? Submodules { get; init; }

    /// <summary>
    /// Path to check out source code, relative to the agent's build directory (e.g. \_work\1).
    /// Defaults to a directory called `s`.
    /// </summary>
    [YamlMember(Order = 105)]
    public Conditioned<string>? Path { get; init; }

    /// <summary>
    /// When true, leave the OAuth token in the Git config after the initial fetch.
    /// Defaults to false.
    /// </summary>
    [YamlMember(Order = 106)]
    public Conditioned<bool>? PersistCredentials { get; init; }
}

public record SelfCheckoutTask : CheckoutTask
{
    public override string Checkout => "self";
}

public record NoneCheckoutTask : Step
{
    [YamlMember(Order = 1)]
    public string Checkout => "none";
}

public record RepositoryCheckoutTask : CheckoutTask
{
    private readonly string _repository;

    public override string Checkout => _repository;

    public RepositoryCheckoutTask(string repository)
    {
        _repository = repository;
    }
}

public enum SubmoduleCheckout
{
    [YamlMember(Alias = "false")]
    None,

    [YamlMember(Alias = "true")]
    SingleLevel,

    [YamlMember(Alias = "recursive")]
    Recursive,
}
