using System;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/tool/dotnet-core-tool-installer?view=azure-devops">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record UseDotNetTask : AzureDevOpsTask
{
    /// <summary>
    /// Please select whether to install only runtime or SDK
    /// Default value: sdk
    /// </summary>
    [YamlIgnore]
    public DotNetPackageType PackageType
    {
        get => GetString("packageType", "sdk") switch
        {
            "sdk" => DotNetPackageType.Sdk,
            "runtime" => DotNetPackageType.Runtime,
            _ => throw new NotImplementedException(),
        };
        init => SetProperty("packageType", value switch
        {
            DotNetPackageType.Sdk => "sdk",
            DotNetPackageType.Runtime => "runtime",
            _ => throw new NotImplementedException()
        });
    }

    /// <summary>
    /// Select this option to install all SDKs from global.json files.
    /// These files are searched from system.DefaultWorkingDirectory.
    /// You can change the search root path by setting working directory input
    /// </summary>
    [YamlIgnore]
    public bool UseGlobalJson
    {
        get => GetBool("useGlobalJson", false);
        init => SetProperty("useGlobalJson", value ? "true" : "false");
    }

    /// <summary>
    /// Current working directory where the script is run.
    /// Empty is the root of the repo (build) or artifacts (release), which is $(System.DefaultWorkingDirectory)
    /// </summary>
    [YamlIgnore]
    public string? WorkingDirectory
    {
        get => GetString("workingDirectory");
        init => SetProperty("workingDirectory", value);
    }

    /// <summary>
    /// Specify version of .NET Core SDK or runtime to install.
    /// Versions can be given in the following formats
    /// 2.x => Install latest in major version.
    /// 3.1.x => Install latest in major and minor version
    /// 3.1.402 => Install exact version
    /// Find the value of version for installing SDK/Runtime, from the releases.json.The link to releases.json of that major.minor version can be found in releases-index file.. Like link to releases.json for 3.1 version is https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/3.1/releases.json
    /// </summary>
    [YamlIgnore]
    public string? Version
    {
        get => GetString("version");
        init => SetProperty("version", value);
    }

    /// <summary>
    /// Specify where .NET Core SDK/Runtime should be installed.
    /// Different paths can have the following impact on .NET's behavior.
    ///   - $(Agent.ToolsDirectory): This makes the version to be cached on the agent since this directory is not cleanup up across pipelines.All pipelines running on the agent, would have access to the versions installed previously using the agent.
    ///   - $(Agent.TempDirectory): This can ensure that a pipeline doesn't use any cached version of .NET core since this folder is cleaned up after each pipeline.
    ///   - Any other path: You can configure any other path given the agent process has access to the path.This will change the state of the machine and impact all processes running on it.
    /// Note that you can also configure Multi-Level Lookup setting which can configure.NET host's probing for a suitable version.
    /// Default value: $(Agent.ToolsDirectory)/dotnet
    /// </summary>
    [YamlIgnore]
    public string? InstallationPath
    {
        get => GetString("installationPath");
        init => SetProperty("installationPath", value);
    }

    /// <summary>
    /// This input is only applicable to Windows based agents and configures the behavior of .NET host process for looking up a suitable shared framework.
    ///   - false: (default) Only versions present in the folder specified in this task would be looked by the host process.
    ///   - true: The host will attempt to look in pre-defined global locations using multi-level lookup.
    /// The default global locations are:
    /// For Windows:
    ///     C:/Program Files/dotnet (64-bit processes)
    ///     C:/Program Files(x86)/dotnet(32-bit process)
    /// </summary>
    [YamlIgnore]
    public bool PerformMultiLevelLookup
    {
        get => GetBool("performMultiLevelLookup", false);
        init => SetProperty("performMultiLevelLookup", value ? "true" : "false");
    }

    /// <summary>
    /// Select if you want preview versions to be included while searching for latest versions, such as while searching 3.1.x.
    /// This setting is ignored if you specify an exact version, such as: 3.0.100-preview3-010431
    /// Default value: false
    /// </summary>
    [YamlIgnore]
    public bool IncludePreviewVersions
    {
        get => GetBool("includePreviewVersions", false);
        init => SetProperty("includePreviewVersions", value ? "true" : "false");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UseDotNetTask"/> class with the specified arguments.
    /// </summary>
    /// <param name="packageType">
    /// Please select whether to install only runtime or SDK
    /// Default value: sdk
    /// </param>
    /// <param name="version">
    /// Specify version of .NET Core SDK or runtime to install.
    /// Versions can be given in the following formats
    /// <code>
    /// 2.x => Install latest in major version.
    /// 3.1.x => Install latest in major and minor version
    /// 3.1.402 => Install exact version
    /// </code>
    /// Find the value of version for installing SDK, from the <c>releases.json</c> for example <see href="https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/3.1/releases.json">releases.json for 3.1</see>
    /// </param>
    /// <param name="includePreviewVersions">
    /// <para>
    /// Select if you want preview versions to be included while searching for latest versions, for example <c>3.1.x</c>.
    /// </para>
    /// <para>
    /// This setting is ignored if you specify an exact version, such as: 3.0.100-preview3-010431
    /// </para>
    /// </param>
    public UseDotNetTask(DotNetPackageType packageType, string version, bool includePreviewVersions = false) : this()
    {
        PackageType = packageType;
        Version = version;

        if (includePreviewVersions)
        {
            IncludePreviewVersions = includePreviewVersions;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UseDotNetTask"/> class.
    /// </summary>
    public UseDotNetTask() : base("UseDotNet@2")
    {
    }
}

/// <summary>
/// Package type to install
/// </summary>
public enum DotNetPackageType
{
    /// <summary>
    /// The dotnet SDK (contains runtime)
    /// </summary>
    Sdk,

    /// <summary>
    /// The dotnet runtime.
    /// </summary>
    Runtime,
}
