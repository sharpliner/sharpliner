using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the <c>ArchiveFiles@2</c> Azure Pipelines task, which compresses files into <c>.7z</c>, <c>.tar.gz</c>, or <c>.zip</c> archives.
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/archive-files-v2">official Azure DevOps pipelines documentation</see>
/// and the
/// <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/ArchiveFilesV2/task.json">official ArchiveFilesV2 task specification</see>.
/// </summary>
public record ArchiveFilesTask : AzureDevOpsTask
{
    /// <summary>
    /// <para>
    /// Required <c>filePath</c> input. Name of the root folder or the file path to files to add to the archive.
    /// </para>
    /// <para>
    /// For folders, everything in the named folder is added to the archive.
    /// </para>
    /// Default value: <c>$(Build.BinariesDirectory)</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? RootFolderOrFile
    {
        get => GetExpression<string>("rootFolderOrFile");
        init => SetProperty("rootFolderOrFile", value);
    }

    /// <summary>
    /// <para>
    /// Required <c>boolean</c> input. Prepends the root folder name to file paths in the archive.
    /// </para>
    /// <para>
    /// Otherwise, all file paths will start one level lower.
    /// </para>
    /// Default value: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? IncludeRootFolder
    {
        get => GetExpression<bool>("includeRootFolder");
        init => SetProperty("includeRootFolder", value);
    }

    /// <summary>
    /// <para>
    /// Required <c>pickList</c> input. Specifies a compression format.
    /// </para>
    /// <para>
    /// Allowed values: <c>zip</c>, <c>7z</c>, <c>tar</c>, and <c>wim</c>.
    /// </para>
    /// Default value: <see cref="ArchiveType.Zip"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<ArchiveType>? ArchiveType
    {
        get => GetExpression<ArchiveType>("archiveType");
        init => SetProperty("archiveType", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>pickList</c> input. Set a compression level, or choose <see cref="SevenZipCompressionLevel.None"/> to create an uncompressed 7z file.
    /// </para>
    /// <para>
    /// Use when <see cref="ArchiveType"/> is <see cref="Tasks.ArchiveType._7z"/>.
    /// Allowed values: <c>ultra</c>, <c>maximum</c>, <c>normal</c>, <c>fast</c>, <c>fastest</c>, and <c>none</c>.
    /// </para>
    /// Default value: <see cref="SevenZipCompressionLevel.Normal"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<SevenZipCompressionLevel>? SevenZipCompression
    {
        get => GetExpression<SevenZipCompressionLevel>("sevenZipCompression");
        init => SetProperty("sevenZipCompression", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>pickList</c> input. Set a compression scheme, or choose <see cref="TarCompressionType.None"/> to create an uncompressed tar file.
    /// </para>
    /// <para>
    /// Use when <see cref="ArchiveType"/> is <see cref="Tasks.ArchiveType.Tar"/>.
    /// Allowed values: <c>gz</c>, <c>bz2</c>, <c>xz</c>, and <c>none</c>.
    /// </para>
    /// Default value: <see cref="TarCompressionType.Gz"/>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<TarCompressionType>? TarCompression
    {
        get => GetExpression<TarCompressionType>("tarCompression");
        init => SetProperty("tarCompression", value);
    }

    /// <summary>
    /// <para>
    /// Required <c>filePath</c> input. Specify the name of the archive file to create.
    /// </para>
    /// <para>
    /// For example, to create <c>foo.tgz</c>, select the <see cref="Tasks.ArchiveType.Tar"/> archive type and <see cref="TarCompressionType.Gz"/> tar compression.
    /// </para>
    /// Default value: <c>$(Build.ArtifactStagingDirectory)/$(Build.BuildId).zip</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ArchiveFile
    {
        get => GetExpression<string>("archiveFile");
        init => SetProperty("archiveFile", value);
    }

    /// <summary>
    /// <para>
    /// Required <c>boolean</c> input. Specifies whether to overwrite an existing archive.
    /// </para>
    /// <para>
    /// When set to <c>false</c>, files are added to the existing archive.
    /// </para>
    /// <para>
    /// This append behavior is supported for zip, 7z, compressed tar, and wim archives.
    /// </para>
    /// Default value: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? ReplaceExistingArchive
    {
        get => GetExpression<bool>("replaceExistingArchive");
        init => SetProperty("replaceExistingArchive", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>boolean</c> input. If set to <c>true</c>, forces tools to use verbose output and overrides the <see cref="Quiet"/> setting.
    /// </para>
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? Verbose
    {
        get => GetExpression<bool>("verbose");
        init => SetProperty("verbose", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>boolean</c> input. If set to <c>true</c>, forces tools to use quiet output and can be overridden by the <see cref="Verbose"/> setting.
    /// </para>
    /// Default value: <c>false</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? Quiet
    {
        get => GetExpression<bool>("quiet");
        init => SetProperty("quiet", value);
    }

    /// <summary>
    /// Instantiates a new <see cref="ArchiveFilesTask"/> task with the specified parameters.
    /// </summary>
    /// <param name="rootFolderOrFile">The name of the root folder or the file path to files to add to the archive.</param>
    /// <param name="archiveType">The compression format.</param>
    /// <param name="archiveFile">The name of the archive file to create.</param>
    public ArchiveFilesTask(string rootFolderOrFile, AdoExpression<ArchiveType> archiveType, AdoExpression<string> archiveFile)
        : base("ArchiveFiles@2")
    {
        RootFolderOrFile = rootFolderOrFile;
        ArchiveType = archiveType;
        ArchiveFile = archiveFile;
    }
}

/// <summary>
/// Supported values for the <c>archiveType</c> input of <see cref="ArchiveFilesTask"/>.
/// </summary>
public enum ArchiveType
{
    /// <summary>
    /// Default. Choose this format for all zip-compatible types such as <c>.zip</c>, <c>.jar</c>, <c>.war</c>, and <c>.ear</c>.
    /// </summary>
    [YamlMember(Alias = "zip")]
    Zip,

    /// <summary>
    /// 7-Zip format (<c>.7z</c>).
    /// </summary>
    [YamlMember(Alias = "7z")]
    _7z,

    /// <summary>
    /// Tar format. Use for all tar files, including compressed tars such as <c>.tar.gz</c>, <c>.tar.bz2</c>, and <c>.tar.xz</c>.
    /// </summary>
    [YamlMember(Alias = "tar")]
    Tar,

    /// <summary>
    /// Windows Imaging format (<c>.wim</c>).
    /// </summary>
    [YamlMember(Alias = "wim")]
    Wim,
}

/// <summary>
/// <para>
/// Supported values for the <c>sevenZipCompression</c> input of <see cref="ArchiveFilesTask"/>.
/// See the <see href="https://7-zip.opensource.jp/chm/cmdline/switches/method.htm#SevenZipX">-m (Set compression Method) switch</see> for more details.
/// </para>
/// <code>
/// Level	Method	Dictionary	FastBytes	MatchFinder	Filter	Description
/// 0	Copy					                        No compression.
/// 1	LZMA2	64 KB   	32  	        HC4	        BCJ	Fastest compressing
/// 3	LZMA2	1 MB    	32  	        HC4	        BCJ	Fast compressing
/// 5	LZMA2	16 MB   	32  	        BT4	        BCJ	Normal compressing
/// 7	LZMA2	32 MB   	64  	        BT4	        BCJ	Maximum compressing
/// 9	LZMA2	64 MB   	64  	        BT4	        BCJ2	Ultra compressing
/// </code>
/// </summary>
public enum SevenZipCompressionLevel
{
    /// <summary>
    /// Copy mode, level 0 (no compression).
    /// </summary>
    [YamlMember(Alias = "none")]
    None,

    /// <summary>
    /// Fastest compression, level 1.
    /// </summary>
    [YamlMember(Alias = "fastest")]
    Fastest,

    /// <summary>
    /// Fast compression, level 3.
    /// </summary>
    [YamlMember(Alias = "fast")]
    Fast,

    /// <summary>
    /// Normal compression, level 5. This is the default value.
    /// </summary>
    [YamlMember(Alias = "normal")]
    Normal,

    /// <summary>
    /// Maximum compression, level 7.
    /// </summary>
    [YamlMember(Alias = "maximum")]
    Maximum,

    /// <summary>
    /// Ultra compression, level 9.
    /// </summary>
    [YamlMember(Alias = "ultra")]
    Ultra,
}

/// <summary>
/// Supported values for the <c>tarCompression</c> input of <see cref="ArchiveFilesTask"/>.
/// </summary>
public enum TarCompressionType
{
    /// <summary>
    /// Default gzip compression (<c>.tar.gz</c>, <c>.tar.tgz</c>, <c>.taz</c>).
    /// </summary>
    [YamlMember(Alias = "gz")]
    Gz,

    /// <summary>
    /// bzip2 compression (<c>.tar.bz2</c>, <c>.tz2</c>, <c>.tbz2</c>).
    /// </summary>
    [YamlMember(Alias = "bz2")]
    Bz2,

    /// <summary>
    /// xz compression (<c>.tar.xz</c>, <c>.txz</c>).
    /// </summary>
    [YamlMember(Alias = "xz")]
    Xz,

    /// <summary>
    /// Create an uncompressed <c>.tar</c> file.
    /// </summary>
    [YamlMember(Alias = "none")]
    None,
}
