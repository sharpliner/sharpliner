using System;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/archive-files-v2">official Azure DevOps pipelines documentation</see>
/// </summary>
public record ArchiveFilesTask : AzureDevOpsTask
{
    /// <summary>
    /// <para>
    /// Name of the root folder or the file path to files to add to the archive.
    /// </para>
    /// <para>
    /// For folders, everything in the named folder is added to the archive.
    /// </para>
    /// Defaults to <c>$(Build.BinariesDirectory)</c>
    /// </summary>
    [YamlIgnore]
    public string? RootFolderOrFile
    {
        get => GetString("rootFolderOrFile");
        init => SetProperty("rootFolderOrFile", value);
    }

    /// <summary>
    /// <para>
    /// Prepends the root folder name to file paths in the archive.
    /// </para>
    /// <para>
    /// Otherwise, all file paths will start one level lower.
    /// </para>
    /// Defaults to <c>true</c>
    /// </summary>
    [YamlIgnore]
    public bool IncludeRootFolder
    {
        get => GetBool("includeRootFolder", true);
        init => SetProperty("includeRootFolder", value);
    }

    /// <summary>
    /// <para>
    /// Specifies a compression format.
    /// </para>
    /// Defaults to <see cref="ArchiveType.Zip"/>
    /// </summary>
    [YamlIgnore]
    public ArchiveType ArchiveType
    {
        get => GetEnum("archiveType", ArchiveType.Zip);
        init => SetProperty("archiveType", value);
    }

    /// <summary>
    /// <para>
    /// Set compression level or None to create an uncompressed .7z file.
    /// </para>
    /// Defaults to <see cref="SevenZipCompressionLevel.Normal"/>
    /// </summary>
    [YamlIgnore]
    public SevenZipCompressionLevel SevenZipCompression
    {
        get => GetEnum("sevenZipCompression", SevenZipCompressionLevel.Normal);
        init => SetProperty("sevenZipCompression", value);
    }

    /// <summary>
    /// <para>
    /// Set a compression format or choose None to create an uncompressed .tar file.
    /// </para>
    /// Defaults to <see cref="TarCompressionType.Gz"/>
    /// </summary>
    [YamlIgnore]
    public TarCompressionType TarCompression
    {
        get => GetEnum("tarCompression", TarCompressionType.Gz);
        init => SetProperty("tarCompression", value);
    }

    /// <summary>
    /// <para>
    /// Specify the name of the archive file to create.
    /// </para>
    /// Defaults to <c>$(Build.ArtifactStagingDirectory)/$(Build.BuildId).zip</c>
    /// </summary>
    [YamlIgnore]
    public string? ArchiveFile
    {
        get => GetString("archiveFile");
        init => SetProperty("archiveFile", value);
    }

    /// <summary>
    /// <para>
    /// By default, overwrites an existing archive. Otherwise, when set to <c>false</c>, uncompressed tar files are added to the existing archive.
    /// </para>
    /// <para>
    /// Supported only for zip, 7z, tar (only compressed) and wim formats.
    /// </para>
    /// Defaults to <c>true</c>
    /// </summary>
    [YamlIgnore]
    public bool ReplaceExistingArchive
    {
        get => GetBool("replaceExistingArchive", true);
        init => SetProperty("replaceExistingArchive", value);
    }

    /// <summary>
    /// <para>
    /// If set to <c>true</c>, forces tools to use verbose output. Overrides the 'quiet' setting.
    /// </para>
    /// Defaults to <c>false</c>
    /// </summary>
    [YamlIgnore]
    public bool Verbose
    {
        get => GetBool("verbose", false);
        init => SetProperty("verbose", value);
    }

    /// <summary>
    /// <para>
    /// If set to <c>true</c>, forces tools to use quiet output. The verbose setting (or equivalent) can override this setting.
    /// </para>
    /// Defaults to <c>false</c>
    /// </summary>
    [YamlIgnore]
    public bool Quiet
    {
        get => GetBool("quiet", false);
        init => SetProperty("quiet", value);
    }

    /// <summary>
    /// Instantiates a new <see cref="ArchiveFilesTask"/> task with the specified parameters.
    /// </summary>
    /// <param name="rootFolderOrFile">The name of the root folder or the file path to files to add to the archive.</param>
    /// <param name="archiveType">The compression format.</param>
    /// <param name="archiveFile">The name of the archive file to create.</param>
    public ArchiveFilesTask(string rootFolderOrFile, ArchiveType archiveType, string archiveFile) : base("ArchiveFiles@2")
    {
        RootFolderOrFile = rootFolderOrFile;
        ArchiveType = archiveType;
        ArchiveFile = archiveFile;
    }
}

/// <summary>
/// A supported archive type
/// </summary>
public enum ArchiveType
{
    /// <summary>
    /// Default. Choose this format for all zip compatible types such as .zip, .jar, .war, .ear
    /// </summary>
    [YamlMember(Alias = "zip")]
    Zip,

    /// <summary>
    /// 7-Zip format, (.7z)
    /// </summary>
    [YamlMember(Alias = "7z")]
    _7z,

    /// <summary>
    /// tar format, use for compressed tars including .tar.gz, .tar.bz2, .tar.xz
    /// </summary>
    [YamlMember(Alias = "tar")]
    Tar,

    /// <summary>
    /// wim format, .wim
    /// </summary>
    [YamlMember(Alias = "wim")]
    Wim,
}

/// <summary>
/// <para>
/// 7z compression levels, see <see href="https://7-zip.opensource.jp/chm/cmdline/switches/method.htm#SevenZipX">-m (Set compression Method) switch</see> for more details.
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
    /// Copy mode - level of compression = 0 (no compression)
    /// </summary>
    [YamlMember(Alias = "none")]
    None,

    /// <summary>
    /// Fastest compressing, level 1
    /// </summary>
    [YamlMember(Alias = "fastest")]
    Fastest,

    /// <summary>
    /// Fast compressing, level 3
    /// </summary>
    [YamlMember(Alias = "fast")]
    Fast,

    /// <summary>
    /// Normal compression, level 5
    /// </summary>
    [YamlMember(Alias = "normal")]
    Normal,

    /// <summary>
    /// Maximum compression, level 7
    /// </summary>
    [YamlMember(Alias = "maximum")]
    Maximum,

    /// <summary>
    /// Ultra compression, level 9
    /// </summary>
    [YamlMember(Alias = "ultra")]
    Ultra,
}

/// <summary>
/// Tar compression format
/// </summary>
public enum TarCompressionType
{
    /// <summary>
    /// Default format for gzip compression (.tar.gz, .tar.tgz, .taz)
    /// </summary>
    [YamlMember(Alias = "gz")]
    Gz,

    /// <summary>
    /// bzip2 compression (.tar.bz2, .tz2, .tbz2)
    /// </summary>
    [YamlMember(Alias = "bz2")]
    Bz2,

    /// <summary>
    /// xz compression (.tar.xz, .txz)
    /// </summary>
    [YamlMember(Alias = "xz")]
    Xz,

    /// <summary>
    /// Create an uncompressed .tar file.
    /// </summary>
    [YamlMember(Alias = "none")]
    None,
}
