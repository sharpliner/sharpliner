using System;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/archive-files-v2">official Azure DevOps pipelines documentation</see>
/// </summary>
public record ArchiveFilesTask : AzureDevOpsTask
{
    /// <summary>
    /// Name of the root folder or the file path to files to add to the archive.
    /// For folders, everything in the named folder is added to the archive.
    /// Defaults to <code>$(Build.BinariesDirectory)</code>
    /// </summary>
    [YamlIgnore]
    public string? RootFolderOrFile
    {
        get => GetString("rootFolderOrFile");
        init => SetProperty("rootFolderOrFile", value);
    }

    /// <summary>
    /// Prepends the root folder name to file paths in the archive.
    /// Otherwise, all file paths will start one level lower.
    /// Defaults to <code>true</code>
    /// </summary>
    [YamlIgnore]
    public bool IncludeRootFolder
    {
        get => GetBool("includeRootFolder", true);
        init => SetProperty("includeRootFolder", value);
    }

    /// <summary>
    /// Specifies a compression format.
    /// zip, 7z, tar, wim
    /// Defaults to <code>zip</code>
    /// </summary>
    [YamlIgnore]
    public ArchiveType ArchiveType
    {
        get => GetString("archiveType") switch
        {
            "zip" => ArchiveType.Zip,
            "7z" => ArchiveType._7z,
            "tar" => ArchiveType.Tar,
            "wim" => ArchiveType.Wim,
            _ => throw new NotImplementedException()
        };
        init => SetProperty("archiveType", value switch
        {
            ArchiveType.Zip => "zip",
            ArchiveType._7z => "7z",
            ArchiveType.Tar => "tar",
            ArchiveType.Wim => "wim",
            _ => throw new NotImplementedException()
        });
    }

    /// <summary>
    /// Set compression level or None to create an uncompressed .7z file.
    /// Defaults to <code>normal</code>
    /// </summary>
    [YamlIgnore]
    public SevenZipCompressionLevel SevenZipCompression
    {
        get => GetString("sevenZipCompression") switch
        {
            "none" => SevenZipCompressionLevel.None,
            "fastest" => SevenZipCompressionLevel.Fastest,
            "fast" => SevenZipCompressionLevel.Fast,
            "normal" => SevenZipCompressionLevel.Normal,
            "maximum" => SevenZipCompressionLevel.Maximum,
            "ultra" => SevenZipCompressionLevel.Ultra,
            _ => throw new NotImplementedException()
        };
        init => SetProperty("sevenZipCompression", value switch
        {
            SevenZipCompressionLevel.None => "none",
            SevenZipCompressionLevel.Fastest => "fastest",
            SevenZipCompressionLevel.Fast => "fast",
            SevenZipCompressionLevel.Normal => "normal",
            SevenZipCompressionLevel.Maximum => "maximum",
            SevenZipCompressionLevel.Ultra => "ultra",
            _ => throw new NotImplementedException()
        });
    }

    /// <summary>
    /// Set a compression format or choose None to create an uncompressed .tar file.
    /// Defaults to <code>gz</code>
    /// </summary>
    [YamlIgnore]
    public TarCompressionType TarCompression
    {
        get => GetString("tarCompression") switch
        {
            "gz" => TarCompressionType.Gz,
            "bz2" => TarCompressionType.Bz2,
            "xz" => TarCompressionType.Xz,
            "none" => TarCompressionType.None,
            _ => throw new NotImplementedException()
        };
        init => SetProperty("tarCompression", value switch
        {
            TarCompressionType.Gz => "gz",
            TarCompressionType.Bz2 => "bz2",
            TarCompressionType.Xz => "xz",
            TarCompressionType.None => "none",
            _ => throw new NotImplementedException()
        });
    }

    /// <summary>
    /// Specify the name of the archive file to create.
    /// Defaults to <code>$(Build.ArtifactStagingDirectory)/$(Build.BuildId).zip</code>
    /// </summary>
    [YamlIgnore]
    public string? ArchiveFile
    {
        get => GetString("archiveFile");
        init => SetProperty("archiveFile", value);
    }

    /// <summary>
    /// By default, overwrites an existing archive. Otherwise, when set to false, uncompressed tar files are added to the existing archive.
    /// Supported only for zip, 7z, tar (only compressed) and wim formats.
    /// Defaults to <code>true</code>
    /// </summary>
    [YamlIgnore]
    public bool ReplaceExistingArchive
    {
        get => GetBool("replaceExistingArchive", true);
        init => SetProperty("replaceExistingArchive", value);
    }

    /// <summary>
    /// If set to true, forces tools to use verbose output. Overrides the 'quiet' setting.
    /// Defaults to <code>false</code>
    /// </summary>
    [YamlIgnore]
    public bool Verbose
    {
        get => GetBool("verbose", false);
        init => SetProperty("verbose", value);
    }

    /// <summary>
    /// If set to true, forces tools to use quiet output. The verbose setting (or equivalent) can override this setting.
    /// Defaults to <code>false</code>
    /// </summary>
    [YamlIgnore]
    public bool Quiet
    {
        get => GetBool("quiet", false);
        init => SetProperty("quiet", value);
    }

    public ArchiveFilesTask(string rootFolderOrFile, ArchiveType archiveType, string archiveFile) : base("ArchiveFiles@2")
    {
        RootFolderOrFile = rootFolderOrFile;
        ArchiveType = archiveType;
        ArchiveFile = archiveFile;
    }
}

public enum ArchiveType
{
    Zip,
    _7z,
    Tar,
    Wim,
}

public enum SevenZipCompressionLevel
{
    None,
    Fastest,
    Fast,
    Normal,
    Maximum,
    Ultra,
}

public enum TarCompressionType
{
    Gz,
    Bz2,
    Xz,
    None,
}
