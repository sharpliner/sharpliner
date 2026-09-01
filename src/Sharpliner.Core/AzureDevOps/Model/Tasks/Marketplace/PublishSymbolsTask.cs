using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the common <c>PublishSymbols@2</c> task inputs shared across indexing/publishing modes.
/// Modelled from the official Azure Pipelines
/// <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/PublishSymbolsV2/task.json">PublishSymbolsV2 task.json</see>
/// task version 2.279.1.
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/publish-symbols-v2?view=azure-pipelines">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract record PublishSymbolsTask : AzureDevOpsTask
{
    /// <summary>
    /// Name of the Azure Resource Manager service connection used by the symbol upload tool.
    /// Supported authentication type is workload identity federation.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ConnectedServiceName
    {
        get => GetExpression<string>("ConnectedServiceName");
        init => SetProperty("ConnectedServiceName", value);
    }

    /// <summary>
    /// Path to the folder searched for symbol files.
    /// Defaults to <c>$(Build.SourcesDirectory)</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SymbolsFolder
    {
        get => GetExpression<string>("SymbolsFolder");
        init => SetProperty("SymbolsFolder", value);
    }

    /// <summary>
    /// Pattern used to discover symbol files to process.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SearchPattern
    {
        get => GetExpression<string>("SearchPattern");
        init => SetProperty("SearchPattern", value);
    }

    /// <summary>
    /// Path to a manifest file containing additional symbol client keys to publish.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Manifest
    {
        get => GetExpression<string>("Manifest");
        init => SetProperty("Manifest", value);
    }

    /// <summary>
    /// Enables verbose logging.
    /// Default value is <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? DetailedLog
    {
        get => GetExpression<bool>("DetailedLog");
        init => SetProperty("DetailedLog", value);
    }

    /// <summary>
    /// Warns when sources cannot be indexed for a PDB file instead of logging as normal output.
    /// Default value is <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? TreatNotIndexedAsWarning
    {
        get => GetExpression<bool>("TreatNotIndexedAsWarning");
        init => SetProperty("TreatNotIndexedAsWarning", value);
    }

    /// <summary>
    /// Uses the .NET-based symbol upload client tool.
    /// This option only matters on Windows agents.
    /// Default value is <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? UseNetCoreClientTool
    {
        get => GetExpression<bool>("UseNetCoreClientTool");
        init => SetProperty("UseNetCoreClientTool", value);
    }

    /// <summary>
    /// Maximum number of minutes to wait before failing the task.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<int>? SymbolsMaximumWaitTime
    {
        get => GetExpression<int>("SymbolsMaximumWaitTime");
        init => SetProperty("SymbolsMaximumWaitTime", value);
    }

    /// <summary>
    /// Product value passed to <c>symstore.exe</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SymbolsProduct
    {
        get => GetExpression<string>("SymbolsProduct");
        init => SetProperty("SymbolsProduct", value);
    }

    /// <summary>
    /// Version value passed to <c>symstore.exe</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SymbolsVersion
    {
        get => GetExpression<string>("SymbolsVersion");
        init => SetProperty("SymbolsVersion", value);
    }

    /// <summary>
    /// Artifact name used for the symbols artifact.
    /// Default value is <c>Symbols_$(BuildConfiguration)</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SymbolsArtifactName
    {
        get => GetExpression<string>("SymbolsArtifactName");
        init => SetProperty("SymbolsArtifactName", value);
    }

    /// <summary>
    /// Initializes a mode-specific <see cref="PublishSymbolsTask"/>.
    /// </summary>
    /// <param name="searchPattern">Pattern used to discover symbol files.</param>
    /// <param name="indexSources">Whether to inject source server information into PDB files.</param>
    /// <param name="publishSymbols">Whether to publish symbol files.</param>
    protected PublishSymbolsTask(AdoExpression<string> searchPattern, bool indexSources, bool publishSymbols)
        : base("PublishSymbols@2")
    {
        DisplayName = "Index sources and publish symbols";
        SearchPattern = searchPattern;
        SetProperty("IndexSources", indexSources);
        SetProperty("PublishSymbols", publishSymbols);
    }
}

/// <summary>
/// Represents publish-enabled <c>PublishSymbols@2</c> modes where <c>PublishSymbols=true</c>.
/// </summary>
public abstract record PublishSymbolsPublishTask : PublishSymbolsTask
{
    /// <summary>
    /// Initializes a publish-enabled <see cref="PublishSymbolsTask"/>.
    /// </summary>
    /// <param name="searchPattern">Pattern used to discover symbol files.</param>
    /// <param name="indexSources">Whether to inject source server information into PDB files.</param>
    /// <param name="symbolServerType">Destination symbol server type.</param>
    protected PublishSymbolsPublishTask(AdoExpression<string> searchPattern, bool indexSources, SymbolServerType symbolServerType)
        : base(searchPattern, indexSources, publishSymbols: true)
    {
        SetProperty("SymbolServerType", symbolServerType);
    }
}

/// <summary>
/// Represents the mode that publishes symbols to Azure Artifacts symbol server in this organization/collection.
/// This mode maps to <c>SymbolServerType=TeamServices</c>.
/// </summary>
public record PublishSymbolsTeamServicesTask : PublishSymbolsPublishTask
{
    /// <summary>
    /// Number of days to retain uploaded symbols.
    /// Default value is <c>36530</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<int>? SymbolExpirationInDays
    {
        get => GetExpression<int>("SymbolExpirationInDays");
        init => SetProperty("SymbolExpirationInDays", value);
    }

    /// <summary>
    /// Symbol file formats that should be uploaded to Azure Artifacts symbol server.
    /// Default value is <see cref="IndexableFileFormats.Default"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<IndexableFileFormats>? IndexableFileFormats
    {
        get => GetExpression<IndexableFileFormats>("IndexableFileFormats");
        init => SetProperty("IndexableFileFormats", value);
    }

    /// <summary>
    /// Initializes the Azure Artifacts publish mode.
    /// </summary>
    /// <param name="searchPattern">Pattern used to discover symbol files.</param>
    /// <param name="indexSources">Whether to inject source server information into PDB files.</param>
    public PublishSymbolsTeamServicesTask(AdoExpression<string> searchPattern, bool indexSources = true)
        : base(searchPattern, indexSources, SymbolServerType.TeamServices)
    {
    }
}

/// <summary>
/// Represents the mode that publishes symbols to a file share.
/// This mode maps to <c>SymbolServerType=FileShare</c>.
/// </summary>
public record PublishSymbolsFileShareTask : PublishSymbolsPublishTask
{
    /// <summary>
    /// File share path used as <c>/s</c> argument for <c>symstore.exe add</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? SymbolsPath
    {
        get => GetExpression<string>("SymbolsPath");
        init => SetProperty("SymbolsPath", value);
    }

    /// <summary>
    /// Compresses symbols when publishing to a file share.
    /// Default value is <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? CompressSymbols
    {
        get => GetExpression<bool>("CompressSymbols");
        init => SetProperty("CompressSymbols", value);
    }

    /// <summary>
    /// Initializes the file-share publish mode.
    /// </summary>
    /// <param name="searchPattern">Pattern used to discover symbol files.</param>
    /// <param name="symbolsPath">File share path that hosts symbols.</param>
    /// <param name="indexSources">Whether to inject source server information into PDB files.</param>
    public PublishSymbolsFileShareTask(AdoExpression<string> searchPattern, AdoExpression<string> symbolsPath, bool indexSources = true)
        : base(searchPattern, indexSources, SymbolServerType.FileShare)
    {
        SymbolsPath = symbolsPath;
    }
}

/// <summary>
/// Represents the indexing-only mode where symbols are indexed but not published.
/// This mode maps to <c>IndexSources=true</c> and <c>PublishSymbols=false</c>.
/// </summary>
public record PublishSymbolsIndexSourcesTask : PublishSymbolsTask
{
    /// <summary>
    /// Initializes the indexing-only mode.
    /// </summary>
    /// <param name="searchPattern">Pattern used to discover symbol files.</param>
    public PublishSymbolsIndexSourcesTask(AdoExpression<string> searchPattern)
        : base(searchPattern, indexSources: true, publishSymbols: false)
    {
    }
}

/// <summary>
/// Destination symbol server type used by <c>PublishSymbols@2</c> when symbol publishing is enabled.
/// </summary>
public enum SymbolServerType
{
    /// <summary>
    /// Symbol Server in this organization/collection (Azure Artifacts).
    /// </summary>
    TeamServices,

    /// <summary>
    /// File share.
    /// </summary>
    FileShare,
}

/// <summary>
/// Symbol file format filter for <c>PublishSymbols@2</c> when publishing to Azure Artifacts.
/// </summary>
public enum IndexableFileFormats
{
    /// <summary>
    /// The default set of symbols to upload.
    /// </summary>
    Default,

    /// <summary>
    /// Only PDB symbols (Windows PDB and portable managed PDB).
    /// </summary>
    Pdb,

    /// <summary>
    /// Only JavaScript source map symbols (<c>*.js.map</c>).
    /// </summary>
    SourceMap,

    /// <summary>
    /// All supported symbol formats.
    /// </summary>
    All,
}
