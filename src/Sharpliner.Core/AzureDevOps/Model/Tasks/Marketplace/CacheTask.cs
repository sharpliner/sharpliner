using System;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents one segment in a <see cref="CacheKey"/>.
/// </summary>
/// <remarks>
/// Cache key segments are separated with <c>|</c>. Literal string segments are emitted in double quotes so that Azure Pipelines
/// does not interpret strings containing dots as file paths. File and file pattern segments are emitted without quotes so that
/// Azure Pipelines can hash their matching file contents.
/// </remarks>
public sealed class CacheKeySegment
{
    private readonly string _value;

    private CacheKeySegment(string value, bool quote)
    {
        ValidateSegment(value);

        _value = quote ? Quote(value) : value;
    }

    /// <summary>
    /// Creates a literal string cache key segment. The value is emitted in double quotes.
    /// </summary>
    /// <param name="value">The literal key segment value.</param>
    /// <returns>A cache key segment.</returns>
    public static CacheKeySegment Literal(string value) => new(value, quote: true);

    /// <summary>
    /// Creates a file path cache key segment. Azure Pipelines hashes the referenced file when resolving the cache key.
    /// </summary>
    /// <param name="path">The absolute path or path relative to <c>$(System.DefaultWorkingDirectory)</c>.</param>
    /// <returns>A cache key segment.</returns>
    public static CacheKeySegment File(string path) => new(path, quote: false);

    /// <summary>
    /// Creates a file pattern cache key segment. Azure Pipelines hashes the files matching the pattern when resolving the cache key.
    /// </summary>
    /// <param name="pattern">The file pattern to hash.</param>
    /// <returns>A cache key segment.</returns>
    public static CacheKeySegment FilePattern(string pattern) => new(pattern, quote: false);

    /// <summary>
    /// Converts the segment to the string form Azure Pipelines expects in the <c>key</c> or <c>restoreKeys</c> input.
    /// </summary>
    /// <returns>The serialized cache key segment.</returns>
    public override string ToString() => _value;

    private static void ValidateSegment(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Cache key segments cannot be null, empty, or whitespace.", nameof(value));
        }

        if (value.Contains('|', StringComparison.Ordinal) || value.Contains('\n', StringComparison.Ordinal) || value.Contains('\r', StringComparison.Ordinal))
        {
            throw new ArgumentException("Cache key segments cannot contain '|', carriage return, or newline characters.", nameof(value));
        }
    }

    private static string Quote(string value)
        => $"\"{value.Replace("\"", "\\\"", StringComparison.Ordinal)}\"";
}

/// <summary>
/// Represents a strongly typed <c>Cache@2</c> key or restore key prefix.
/// </summary>
/// <remarks>
/// More details about cache key segments can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/release/caching?view=azure-devops#configure-the-cache-task">official Azure Pipelines caching documentation</see>.
/// </remarks>
public sealed class CacheKey
{
    private readonly string _value;

    private CacheKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Cache keys cannot be null, empty, or whitespace.", nameof(value));
        }

        if (value.Contains('\n', StringComparison.Ordinal) || value.Contains('\r', StringComparison.Ordinal))
        {
            throw new ArgumentException("Cache keys cannot contain carriage return or newline characters.", nameof(value));
        }

        _value = value;
    }

    /// <summary>
    /// Creates a cache key from one or more typed segments.
    /// </summary>
    /// <param name="segments">The key segments to join with <c>|</c>.</param>
    /// <returns>A cache key.</returns>
    /// <exception cref="ArgumentException">Thrown when no segments are supplied.</exception>
    public static CacheKey FromSegments(params CacheKeySegment[] segments)
    {
        if (segments is null || segments.Length == 0)
        {
            throw new ArgumentException("Cache keys must contain at least one segment.", nameof(segments));
        }

        if (segments.Any(segment => segment is null))
        {
            throw new ArgumentException("Cache key segments cannot contain null values.", nameof(segments));
        }

        return new(string.Join(" | ", segments.Select(segment => segment.ToString())));
    }

    /// <summary>
    /// Creates a cache key from an already formatted Azure Pipelines cache key string.
    /// </summary>
    /// <param name="value">The formatted cache key.</param>
    /// <returns>A cache key.</returns>
    public static CacheKey Raw(string value) => new(value);

    /// <summary>
    /// Creates a literal string cache key segment.
    /// </summary>
    /// <param name="value">The literal key segment value.</param>
    /// <returns>A cache key segment.</returns>
    public static CacheKeySegment Literal(string value) => CacheKeySegment.Literal(value);

    /// <summary>
    /// Creates a file path cache key segment.
    /// </summary>
    /// <param name="path">The file path to hash.</param>
    /// <returns>A cache key segment.</returns>
    public static CacheKeySegment File(string path) => CacheKeySegment.File(path);

    /// <summary>
    /// Creates a file pattern cache key segment.
    /// </summary>
    /// <param name="pattern">The file pattern to hash.</param>
    /// <returns>A cache key segment.</returns>
    public static CacheKeySegment FilePattern(string pattern) => CacheKeySegment.FilePattern(pattern);

    /// <summary>
    /// Converts a cache key to the string form Azure Pipelines expects.
    /// </summary>
    /// <returns>The serialized cache key.</returns>
    public override string ToString() => _value;

    /// <summary>
    /// Converts a cache key to the string form Azure Pipelines expects.
    /// </summary>
    /// <param name="key">The cache key.</param>
    public static implicit operator string(CacheKey key) => key.ToString();
}

/// <summary>
/// Fluent builder for creating <see cref="CacheKey"/> instances.
/// </summary>
public sealed class CacheKeyBuilder
{
    private readonly List<CacheKeySegment> _segments = [];

    /// <summary>
    /// Appends a literal string cache key segment. The value is emitted in double quotes.
    /// </summary>
    /// <param name="value">The literal key segment value.</param>
    /// <returns>The current builder.</returns>
    public CacheKeyBuilder Literal(string value)
    {
        _segments.Add(CacheKeySegment.Literal(value));
        return this;
    }

    /// <summary>
    /// Appends a variable reference as a literal string cache key segment.
    /// </summary>
    /// <param name="variable">The variable reference to include in the key.</param>
    /// <returns>The current builder.</returns>
    public CacheKeyBuilder Variable(VariableReference variable) => Literal(variable);

    /// <summary>
    /// Appends a file path cache key segment. Azure Pipelines hashes the referenced file when resolving the cache key.
    /// </summary>
    /// <param name="path">The absolute path or path relative to <c>$(System.DefaultWorkingDirectory)</c>.</param>
    /// <returns>The current builder.</returns>
    public CacheKeyBuilder File(string path)
    {
        _segments.Add(CacheKeySegment.File(path));
        return this;
    }

    /// <summary>
    /// Appends a file pattern cache key segment. Azure Pipelines hashes the files matching the pattern when resolving the cache key.
    /// </summary>
    /// <param name="pattern">The file pattern to hash.</param>
    /// <returns>The current builder.</returns>
    public CacheKeyBuilder FilePattern(string pattern)
    {
        _segments.Add(CacheKeySegment.FilePattern(pattern));
        return this;
    }

    /// <summary>
    /// Builds a <see cref="CacheKey"/> from the current segments.
    /// </summary>
    /// <returns>A cache key.</returns>
    public CacheKey Build() => CacheKey.FromSegments(_segments.ToArray());

    /// <summary>
    /// Converts a key builder to the built cache key.
    /// </summary>
    /// <param name="builder">The key builder.</param>
    public static implicit operator CacheKey(CacheKeyBuilder builder) => builder.Build();
}

/// <summary>
/// Defines the <c>Cache@2</c> Azure Pipelines task.
/// More details can be found in the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/cache-v2?view=azure-pipelines">official Azure DevOps pipelines documentation</see>,
/// the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/release/caching?view=azure-devops">official pipeline caching documentation</see>,
/// and the <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/CacheV2/task.json">official task specification</see>.
/// </summary>
/// <remarks>
/// The task restores a cache before subsequent steps run and automatically saves the cache in a post-job step when all previous
/// job steps have succeeded. The <see cref="CacheHitVariable"/> input names a variable that Azure Pipelines sets to <c>true</c>
/// for an exact primary-key hit, <c>inexact</c> for a restore-key hit, or <c>false</c> for a miss.
/// </remarks>
public record CacheTask : AzureDevOpsTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CacheTask"/> class.
    /// </summary>
    /// <param name="key">The cache key that uniquely identifies the cache.</param>
    /// <param name="path">The folder path to cache. Wildcards are not supported.</param>
    public CacheTask(CacheKey key, string path) : this(key.ToString(), path)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheTask"/> class.
    /// </summary>
    /// <param name="key">The cache key that uniquely identifies the cache.</param>
    /// <param name="path">The folder path to cache. Wildcards are not supported.</param>
    public CacheTask(CacheKey key, AdoExpression<string> path) : this(key.ToString(), path)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CacheTask"/> class.
    /// </summary>
    /// <param name="key">The cache key that uniquely identifies the cache.</param>
    /// <param name="path">The folder path to cache. Wildcards are not supported.</param>
    public CacheTask(AdoExpression<string> key, AdoExpression<string> path) : base("Cache@2")
    {
        Key = key;
        Path = path;
    }

    /// <summary>
    /// Gets or sets the key that uniquely identifies the cache. Use <c>|</c> to separate key segments.
    /// </summary>
    /// <remarks>
    /// File path and pattern segments are hashed by Azure Pipelines. Quote literal string segments, or use
    /// <see cref="CacheKeyBuilder.Literal(string)"/>, so strings containing dots are not interpreted as file paths.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<string>? Key
    {
        get => GetExpression<string>("key");
        init => SetRequiredStringProperty("key", value);
    }

    /// <summary>
    /// Gets or sets the folder path to cache. The value can be absolute or relative to <c>$(System.DefaultWorkingDirectory)</c>,
    /// and can contain variables. Wildcards are not supported.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Path
    {
        get => GetExpression<string>("path");
        init => SetRequiredStringProperty("path", value, disallowWildcards: true);
    }

    /// <summary>
    /// Gets or sets the variable name that receives cache hit status.
    /// </summary>
    /// <remarks>
    /// Azure Pipelines sets this variable to <c>true</c> for an exact primary-key hit, <c>inexact</c> for a restore-key hit,
    /// or <c>false</c> for a miss. Use the variable in later step conditions to skip work only on exact hits, or to distinguish
    /// restore-key hits from misses.
    /// </remarks>
    [YamlIgnore]
    public AdoExpression<string>? CacheHitVariable
    {
        get => GetExpression<string>("cacheHitVar");
        init => SetOptionalStringProperty("cacheHitVar", value);
    }

    /// <summary>
    /// Gets or sets additional restore key prefixes used if the primary <see cref="Key"/> misses.
    /// </summary>
    /// <remarks>
    /// Restore keys are serialized as the newline-delimited <c>restoreKeys</c> input expected by <c>Cache@2</c>. Put the most
    /// specific prefix first.
    /// </remarks>
    [YamlIgnore]
    public CacheKey[]? RestoreKeys
    {
        get => GetString("restoreKeys")?.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(key => CacheKey.Raw(key.TrimEnd('\r'))).ToArray();
        init => SetProperty("restoreKeys", value is null || value.Length == 0 ? null : string.Join("\n", value.Select(key => key.ToString())));
    }

    private static void ValidateRequiredStringProperty(string name, AdoExpression<string>? value, bool disallowWildcards = false)
    {
        if (value is null)
        {
            throw new ArgumentException($"'{name}' is required and cannot be null.", name);
        }

        if (value.Definition is string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                throw new ArgumentException($"'{name}' is required and cannot be empty or whitespace.", name);
            }

            if (disallowWildcards && (stringValue.Contains('*', StringComparison.Ordinal) || stringValue.Contains('?', StringComparison.Ordinal)))
            {
                throw new ArgumentException($"'{name}' cannot contain wildcards.", name);
            }
        }
    }

    private void SetRequiredStringProperty(string name, AdoExpression<string>? value, bool disallowWildcards = false)
    {
        ValidateRequiredStringProperty(name, value, disallowWildcards);
        SetProperty(name, value);
    }

    private void SetOptionalStringProperty(string name, AdoExpression<string>? value)
    {
        if (value?.Definition is string stringValue && string.IsNullOrWhiteSpace(stringValue))
        {
            Inputs.Remove(name);
        }
        else
        {
            SetProperty(name, value);
        }
    }
}
