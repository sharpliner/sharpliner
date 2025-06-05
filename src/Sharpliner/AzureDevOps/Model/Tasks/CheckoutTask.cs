using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/steps-checkout">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract record CheckoutTask : Step
{
    /// <summary>
    /// <para>
    /// Configures checkout for the specified repository.
    /// </para>
    /// For more information, see <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/repos/multi-repo-checkout">Check out multiple repositories in your pipeline</see>.
    /// </summary>
    [YamlMember(Order = 1)]
    public abstract string Checkout { get; }

    /// <summary>
    /// <para>
    /// When true, run <c>execute git clean -ffdx &amp;&amp; git reset --hard HEAD</c> before fetching.
    /// </para>
    /// Defaults to <c>false</c>.
    /// </summary>
    [YamlMember(Order = 100)]
    public AdoExpression<bool>? Clean { get; init; }

    /// <summary>
    /// The depth of commits to ask Git to fetch.
    /// Defaults to shallow fetch (= 1).
    /// Set 0 to no limit (full clone).
    /// </summary>
    [YamlMember(Order = 101)]
    public AdoExpression<int>? FetchDepth { get; init; }

    /// <summary>
    /// The depth of commits to ask Git to fetch.
    /// Defaults to shallow fetch (= 1).
    /// Set 0 to no limit (full clone).
    /// </summary>
    [YamlMember(Order = 102)]
    public AdoExpression<string>? FetchFilter { get; init; }

    /// <summary>
    /// Set to 'true' to sync tags when fetching the repo, or 'false' to not sync tags.
    /// See remarks for the default behavior in the official documentation:
    /// https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/steps-checkout#remarks
    /// </summary>
    [YamlMember(Order = 103)]
    public AdoExpression<bool>? FetchTags { get; init; }

    /// <summary>
    /// Whether to download Git-LFS files.
    /// Defaults to false.
    /// </summary>
    [YamlMember(Order = 104)]
    public AdoExpression<bool>? Lfs { get; init; }

    /// <summary>
    /// When true, leave the OAuth token in the Git config after the initial fetch.
    /// Defaults to false.
    /// </summary>
    [YamlMember(Order = 105)]
    public AdoExpression<bool>? PersistCredentials { get; init; }

    /// <summary>
    /// Submodules checkout strategy (single level, recursive to get submodules of submodules).
    /// Defaults to not checking out submodules.
    /// </summary>
    [YamlMember(Order = 106)]
    public AdoExpression<SubmoduleCheckout>? Submodules { get; init; }

    /// <summary>
    /// Path to check out source code, relative to the agent's build directory (e.g. \_work\1).
    /// Defaults to a directory called `s`.
    /// </summary>
    [YamlMember(Order = 107)]
    public AdoExpression<string>? Path { get; init; }

    /// <summary>
    /// Specify a directory to enable sparse checkout in cone mode using directory matching.
    /// Separate multiple directories using a space.
    /// Supported on agent version 3.253.0/4.253.0 and higher with Git 2.25 or higher.
    /// If both sparseCheckoutDirectories and sparseCheckoutPatterns are set, sparseCheckoutDirectories is used and the setting for sparseCheckoutPatterns is disregarded.
    /// </summary>
    [YamlMember(Order = 108)]
    public AdoExpression<string>? SparseCheckoutDirectories { get; init; }

    /// <summary>
    /// Specify a pattern to enable sparse checkout in non-cone mode using pattern matching.
    /// Separate multiple patterns using a space.
    /// Supported on agent version 3.253.0/4.253.0 and higher with Git 2.25 or higher.
    /// If both sparseCheckoutDirectories and sparseCheckoutPatterns are set, sparseCheckoutDirectories is used and the setting for sparseCheckoutPatterns is disregarded.
    /// </summary>
    [YamlMember(Order = 109)]
    public AdoExpression<string>? SparseCheckoutPatterns { get; init; }

    /// <summary>
    /// When true, use the repository root directory as the default working directory for the pipeline. The default is false.
    /// </summary>
    [YamlMember(Order = 110)]
    public AdoExpression<bool>? WorkspaceRepo { get; init; }
}

/// <summary>
/// The current repository is checked out.
/// </summary>
public record SelfCheckoutTask : CheckoutTask
{
    /// <inheritdoc />
    public override string Checkout => "self";
}

/// <summary>
/// No repositories are synced or checked out.
/// </summary>
public record NoneCheckoutTask : Step
{
    /// <inheritdoc />
    [YamlMember(Order = 1)]
    public string Checkout => "none";
}

/// <summary>
/// The designated repository is checked out instead of <c>self</c>.
/// </summary>
public record RepositoryCheckoutTask : CheckoutTask
{
    private readonly string _repository;

    /// <inheritdoc />
    public override string Checkout => _repository;

    /// <summary>
    /// Instantiates a new instance of <see cref="RepositoryCheckoutTask"/> with the specified repository.
    /// </summary>
    /// <param name="repository">The repository to check out.</param>
    public RepositoryCheckoutTask(string repository)
    {
        _repository = repository;
    }
}

/// <summary>
/// Submodules checkout strategy.
/// </summary>
public enum SubmoduleCheckout
{
    /// <summary>
    /// Do not check out submodules.
    /// </summary>
    [YamlMember(Alias = "false")]
    None,

    /// <summary>
    /// Single level of submodules
    /// </summary>
    [YamlMember(Alias = "true")]
    SingleLevel,

    /// <summary>
    /// Recursively check out submodules
    /// </summary>
    [YamlMember(Alias = "recursive")]
    Recursive,
}
