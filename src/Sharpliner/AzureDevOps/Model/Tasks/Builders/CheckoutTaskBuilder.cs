namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating a checkout task.
/// </summary>
public class CheckoutTaskBuilder
{
    /// <summary>
    /// <para>
    /// Configures checkout for this repository.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     Checkout.Self with
    ///     {
    ///         Clean = true,
    ///     }
    /// }
    /// </code>
    /// will generate:
    /// <code lang="yaml">
    /// - checkout: self
    ///   clean: true
    /// </code>
    /// </summary>
    /// <returns>The checkout task.</returns>
    public SelfCheckoutTask Self => new();

    /// <summary>
    /// <para>
    /// Configures no checkout for any repository.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///    Checkout.None
    /// }
    /// </code>
    /// will generate:
    /// <code lang="yaml">
    /// - checkout: none
    /// </code>
    /// </summary>
    /// <returns>The checkout task.</returns>
    public NoneCheckoutTask None => new();

    /// <summary>
    /// <para>
    /// Configures checkout for the designated repository.
    /// </para>
    /// For example:
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///    Checkout.Repository("https://github.com/sharpliner/sharpliner.git") with
    ///    {
    ///        Submodules = SubmoduleCheckout.Recursive,
    ///        Clean = true,
    ///        FetchDepth = 0,
    ///        FetchTags = true,
    ///        Lfs = true,
    ///    }
    /// }
    /// </code>
    /// will generate:
    /// <code lang="yaml">
    /// - checkout: https://github.com/sharpliner/sharpliner.git
    ///   clean: true
    ///   fetchDepth: 0
    ///   fetchTags: true
    ///   submodules: recursive
    /// </code>
    /// </summary>
    /// <param name="repository">The repository to check out.</param>
    /// <returns>The checkout task.</returns>
    public RepositoryCheckoutTask Repository(string repository) => new(repository);

    internal CheckoutTaskBuilder()
    {
    }
}
