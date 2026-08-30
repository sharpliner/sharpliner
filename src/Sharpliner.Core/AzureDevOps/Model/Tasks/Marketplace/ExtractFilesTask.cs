using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Extracts archive and compression files such as <c>.7z</c>, <c>.rar</c>, <c>.tar.gz</c>, and <c>.zip</c>.
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/extract-files-v1?view=azure-pipelines">official Azure DevOps pipelines documentation</see>
/// and the <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/ExtractFilesV1/task.json">official task specification</see>.
/// </summary>
public record ExtractFilesTask : AzureDevOpsTask
{
    private const string DefaultArchiveFilePatterns = "**/*.zip";

    /// <summary>
    /// Specifies the file paths or patterns of the archive files to extract. Supports multiple lines of minimatch patterns.
    /// Defaults to <c>**/*.zip</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ArchiveFilePatterns
    {
        get => GetExpression<string>("archiveFilePatterns", DefaultArchiveFilePatterns);
        init => SetProperty("archiveFilePatterns", value);
    }

    /// <summary>
    /// Specifies the destination folder into which archive files should be extracted. Use variables when extracting files
    /// outside the repository, for example <c>$(Agent.BuildDirectory)</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? DestinationFolder
    {
        get => GetExpression<string>("destinationFolder");
        init => SetProperty("destinationFolder", value);
    }

    /// <summary>
    /// Specifies whether to delete the entire content of the destination directory before archive contents are extracted into it.
    /// Defaults to <code>true</code>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? CleanDestinationFolder
    {
        get => GetExpression<bool>("cleanDestinationFolder", true);
        init => SetProperty("cleanDestinationFolder", value);
    }

    /// <summary>
    /// Specifies the option to overwrite existing files in the destination directory if they already exist. When set to
    /// <c>false</c>, the task does not pass overwrite options to the extraction tools.
    /// Defaults to <code>false</code>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? OverwriteExistingFiles
    {
        get => GetExpression<bool>("overwriteExistingFiles", false);
        init => SetProperty("overwriteExistingFiles", value);
    }

    /// <summary>
    /// Specifies the custom path to the 7z utility. For example, <c>C:\7z\7z.exe</c> on Windows and
    /// <c>/usr/local/bin/7z</c> on macOS/Ubuntu. If it is not specified on Windows, the default 7z version
    /// supplied with the task is used.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PathToSevenZipTool
    {
        get => GetExpression<string>("pathToSevenZipTool");
        init => SetProperty("pathToSevenZipTool", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtractFilesTask"/> class with explicit archive file patterns and
    /// destination folder.
    /// </summary>
    /// <param name="archiveFilePatterns">The file paths or patterns of the archive files to extract.</param>
    /// <param name="destinationFolder">The destination folder into which archive files should be extracted.</param>
    public ExtractFilesTask(AdoExpression<string> archiveFilePatterns, AdoExpression<string> destinationFolder)
        : base("ExtractFiles@1")
    {
        ArchiveFilePatterns = archiveFilePatterns;
        DestinationFolder = destinationFolder;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtractFilesTask"/> class using the official
    /// <c>archiveFilePatterns</c> default value, <c>**/*.zip</c>.
    /// </summary>
    /// <param name="destinationFolder">The destination folder into which archive files should be extracted.</param>
    public ExtractFilesTask(AdoExpression<string> destinationFolder)
        : this(DefaultArchiveFilePatterns, destinationFolder)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtractFilesTask"/> class using the official
    /// <c>archiveFilePatterns</c> default value, <c>**/*.zip</c>.
    /// </summary>
    /// <param name="destinationFolder">The destination folder into which archive files should be extracted.</param>
    public ExtractFilesTask(string destinationFolder)
        : this((AdoExpression<string>)destinationFolder)
    {
    }
}
