using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating <c>Cache@2</c> tasks and strongly typed cache keys.
/// </summary>
public class CacheTaskBuilder
{
    internal CacheTaskBuilder()
    {
    }

    /// <summary>
    /// Gets a fluent builder for a <see cref="CacheKey"/>.
    /// </summary>
    /// <example>
    /// <code lang="csharp">
    /// Cache.Key
    ///     .Literal("npm")
    ///     .Literal("$(Agent.OS)")
    ///     .File("package-lock.json")
    /// </code>
    /// </example>
    public CacheKeyBuilder Key => new();

    /// <summary>
    /// Creates a <see cref="CacheTask"/> for the specified cache path and primary key.
    /// </summary>
    /// <param name="path">The folder path to cache. Wildcards are not supported.</param>
    /// <param name="key">The primary cache key.</param>
    /// <returns>A <c>Cache@2</c> task.</returns>
    public CacheTask Files(AdoExpression<string> path, CacheKey key) => new(key.ToString(), path);

    /// <summary>
    /// Creates a <see cref="CacheTask"/> with restore key prefixes used when the primary key misses.
    /// </summary>
    /// <param name="path">The folder path to cache. Wildcards are not supported.</param>
    /// <param name="key">The primary cache key.</param>
    /// <param name="restoreKeys">Additional restore key prefixes in most-specific-first order.</param>
    /// <returns>A <c>Cache@2</c> task.</returns>
    public CacheTask Files(AdoExpression<string> path, CacheKey key, params CacheKey[] restoreKeys)
        => Files(path, key, (IEnumerable<CacheKey>)restoreKeys);

    /// <summary>
    /// Creates a <see cref="CacheTask"/> with restore key prefixes used when the primary key misses.
    /// </summary>
    /// <param name="path">The folder path to cache. Wildcards are not supported.</param>
    /// <param name="key">The primary cache key.</param>
    /// <param name="restoreKeys">Additional restore key prefixes in most-specific-first order.</param>
    /// <returns>A <c>Cache@2</c> task.</returns>
    public CacheTask Files(AdoExpression<string> path, CacheKey key, IEnumerable<CacheKey> restoreKeys)
    {
        var task = Files(path, key);
        var restoreKeyArray = restoreKeys.Where(restoreKey => restoreKey is not null).ToArray();

        return restoreKeyArray.Length == 0
            ? task
            : task with { RestoreKeys = restoreKeyArray };
    }
}
