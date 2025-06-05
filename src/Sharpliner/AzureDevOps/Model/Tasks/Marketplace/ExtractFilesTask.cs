using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/extract-files-v1?view=azure-pipelines">official Azure DevOps pipelines documentation</see>
/// </summary>
public record ExtractFilesTask : AzureDevOpsTask
{
    /// <summary>
    /// Specifies the file paths or patterns of the archive files to extract. Supports multiple lines of minimatch patterns.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ArchiveFilePatterns
    {
        get => GetConditioned<string>("archiveFilePatterns");
        init => SetProperty("archiveFilePatterns", value);
    }

    /// <summary>
    /// Specifies the destination folder into which archive files should be extracted.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? DestinationFolder
    {
        get => GetConditioned<string>("destinationFolder");
        init => SetProperty("destinationFolder", value);
    }

    /// <summary>
    /// Specifies the option to delete the entire content of the destination directory (clean) before archive contents are extracted into it.
    /// Defaults to <code>true</code>
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? CleanDestinationFolder
    {
        get => GetConditioned<bool>("cleanDestinationFolder", true);
        init => SetProperty("cleanDestinationFolder", value);
    }

    /// <summary>
    /// Specifies the option to overwrite existing files in the destination directory if they already exist.
    /// If the option is false, the script prompts on existing files, asking whether you want to overwrite them.
    /// Defaults to <code>false</code>
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? OverwriteExistingFiles
    {
        get => GetConditioned<bool>("overwriteExistingFiles", false);
        init => SetProperty("overwriteExistingFiles", value);
    }

    /// <summary>
    /// Specifies the custom path to 7z utility. For example, C:\7z\7z.exe on Windows and /usr/local/bin/7z on MacOS/Ubuntu.
    /// If it's not specified on Windows, the default 7zip version supplied with a task will be used.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PathToSevenZipTool
    {
        get => GetConditioned<string>("pathToSevenZipTool");
        init => SetProperty("pathToSevenZipTool", value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CopyFilesTask"/> class with required properties.
    /// </summary>
    /// <param name="destinationFolder">The destination folder into which archive files should be extracted..</param>
    public ExtractFilesTask(string destinationFolder)
        : base("ExtractFiles@1")
    {
        DestinationFolder = destinationFolder;
    }
}

