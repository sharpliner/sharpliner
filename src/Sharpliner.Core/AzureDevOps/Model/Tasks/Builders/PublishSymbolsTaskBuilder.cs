using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Fluent builder for valid <c>PublishSymbols@2</c> mode-specific task configurations.
/// See <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/publish-symbols-v2">official Azure DevOps pipelines documentation</see>.
/// </summary>
public class PublishSymbolsTaskBuilder
{
    /// <summary>
    /// Gets builder methods for the <c>IndexSources=true</c> and <c>PublishSymbols=true</c> mode.
    /// </summary>
    public PublishSymbolsPublishingModeBuilder IndexAndPublish => new(indexSources: true);

    /// <summary>
    /// Gets builder methods for the <c>IndexSources=false</c> and <c>PublishSymbols=true</c> mode.
    /// </summary>
    public PublishSymbolsPublishingModeBuilder PublishOnly => new(indexSources: false);

    /// <summary>
    /// Creates the <c>IndexSources=true</c> and <c>PublishSymbols=false</c> mode.
    /// </summary>
    /// <param name="searchPattern">The glob pattern used to discover symbol files.</param>
    public PublishSymbolsIndexSourcesTask IndexOnly(AdoExpression<string> searchPattern) => new(searchPattern);
}

/// <summary>
/// Fluent mode builder for publish-enabled <c>PublishSymbols@2</c> configurations.
/// </summary>
public class PublishSymbolsPublishingModeBuilder
{
    private readonly bool _indexSources;

    internal PublishSymbolsPublishingModeBuilder(bool indexSources)
    {
        _indexSources = indexSources;
    }

    /// <summary>
    /// Publishes symbols to the Azure Artifacts symbol server in this organization/collection.
    /// </summary>
    /// <param name="searchPattern">The glob pattern used to discover symbol files.</param>
    public PublishSymbolsTeamServicesTask ToAzureArtifacts(AdoExpression<string> searchPattern) => new(searchPattern, _indexSources);

    /// <summary>
    /// Publishes symbols to a file share.
    /// </summary>
    /// <param name="searchPattern">The glob pattern used to discover symbol files.</param>
    /// <param name="symbolsPath">The symbol file share path (<c>/s</c> value passed to <c>symstore.exe add</c>).</param>
    public PublishSymbolsFileShareTask ToFileShare(AdoExpression<string> searchPattern, AdoExpression<string> symbolsPath)
        => new(searchPattern, symbolsPath, _indexSources);
}
