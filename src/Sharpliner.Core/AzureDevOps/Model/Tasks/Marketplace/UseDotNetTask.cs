using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// More details can be found in the
/// <see href="https://learn.microsoft.com/azure/devops/pipelines/tasks/reference/use-dotnet-v2">official Azure DevOps pipelines documentation</see>.
/// The task specification is maintained at
/// <see href="https://github.com/microsoft/azure-pipelines-tasks/blob/master/Tasks/UseDotNetV2/task.json">UseDotNetV2/task.json</see>.
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
        get => GetEnum("packageType", DotNetPackageType.Sdk);
        init => SetProperty("packageType", value);
    }

    /// <summary>
    /// Select this option to install all SDKs from <c>global.json</c> files.
    /// This input applies to SDK installs only.
    /// The files are searched from <c>$(System.DefaultWorkingDirectory)</c> unless
    /// <see cref="WorkingDirectory"/> provides another search root.
    /// Default value: false.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? UseGlobalJson
    {
        get => GetExpression<bool>("useGlobalJson");
        init => SetProperty("useGlobalJson", value);
    }

    /// <summary>
    /// Specifies the directory from which <c>global.json</c> files are searched when
    /// <see cref="UseGlobalJson"/> is true.
    /// Empty is the root of the repo (build) or artifacts (release), which is
    /// <c>$(System.DefaultWorkingDirectory)</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? WorkingDirectory
    {
        get => GetExpression<string>("workingDirectory");
        init => SetProperty("workingDirectory", value);
    }

    /// <summary>
    /// Specify version of .NET Core SDK or runtime to install.
    /// This input applies when <see cref="UseGlobalJson"/> is false, or when
    /// <see cref="PackageType"/> is <see cref="DotNetPackageType.Runtime"/>.
    /// Versions can be given in the following formats:
    /// <list type="bullet">
    /// <item><c>2.x</c>: install latest in major version.</item>
    /// <item><c>2.2.x</c>: install latest in major and minor version.</item>
    /// <item><c>2.2.104</c>: install exact version.</item>
    /// </list>
    /// Find the value of <c>version</c> for installing SDK/runtime from <c>releases.json</c>.
    /// The link to <c>releases.json</c> for a major/minor version can be found in the
    /// <see href="https://builds.dotnet.microsoft.com/dotnet/release-metadata/releases-index.json">releases-index file</see>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Version
    {
        get => GetExpression<string>("version");
        init => SetProperty("version", value);
    }

    /// <summary>
    /// Specify a compatible Visual Studio version for which the .NET Core SDK should be installed.
    /// Use a complete Visual Studio version containing major, minor, and patch numbers, such as <c>16.6.4</c>.
    /// Find compatible SDK/runtime version information in the
    /// <see href="https://builds.dotnet.microsoft.com/dotnet/release-metadata/releases-index.json">releases-index file</see>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? VsVersion
    {
        get => GetExpression<string>("vsVersion");
        init => SetProperty("vsVersion", value);
    }

    /// <summary>
    /// Select if you want to detect whether the specified version is already installed before attempting
    /// to download it. Use only when <see cref="InstallationPath"/> is empty or set to its default value.
    /// Default value: false.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? CheckForExistingVersion
    {
        get => GetExpression<bool>("checkForExistingVersion");
        init => SetProperty("checkForExistingVersion", value);
    }

    /// <summary>
    /// Specify where .NET Core SDK/runtime should be installed.
    /// Different paths can have the following impact on .NET's behavior:
    /// <list type="bullet">
    /// <item><c>$(Agent.ToolsDirectory)</c>: caches the version on the agent because this directory is not cleaned between pipelines.</item>
    /// <item><c>$(Agent.TempDirectory)</c>: avoids cached .NET versions because this folder is cleaned after each pipeline.</item>
    /// <item>Any other path: changes machine state and affects all processes that use that path.</item>
    /// </list>
    /// Note that <see cref="PerformMultiLevelLookup"/> can configure .NET host probing for a suitable version.
    /// Default value: <c>$(Agent.ToolsDirectory)/dotnet</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? InstallationPath
    {
        get => GetExpression<string>("installationPath");
        init => SetProperty("installationPath", value);
    }

    /// <summary>
    /// This input is only applicable to Windows based agents and configures the behavior of .NET host process for looking up a suitable shared framework.
    /// <list type="bullet">
    /// <item><c>false</c>: (default) only versions present in the folder specified in this task are looked up by the host process.</item>
    /// <item><c>true</c>: the host attempts to look in pre-defined global locations using multi-level lookup.</item>
    /// </list>
    /// The default global locations are:
    /// For Windows:
    ///     C:/Program Files/dotnet (64-bit processes)
    ///     C:/Program Files(x86)/dotnet(32-bit process)
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? PerformMultiLevelLookup
    {
        get => GetExpression<bool>("performMultiLevelLookup");
        init => SetProperty("performMultiLevelLookup", value);
    }

    /// <summary>
    /// Select if you want preview versions to be included while searching for latest versions, such as while searching <c>2.2.x</c>.
    /// This input applies when <see cref="UseGlobalJson"/> is false, or when
    /// <see cref="PackageType"/> is <see cref="DotNetPackageType.Runtime"/>.
    /// This setting is ignored if you specify an exact version, such as <c>3.0.100-preview3-010431</c>.
    /// Default value: false.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? IncludePreviewVersions
    {
        get => GetExpression<bool>("includePreviewVersions");
        init => SetProperty("includePreviewVersions", value);
    }

    /// <summary>
    /// Provide a timeout value, in milliseconds, for HTTP requests that the task makes to obtain the .NET package.
    /// Default value: 300000 milliseconds (5 minutes). Maximum value: 600000 milliseconds (10 minutes).
    /// </summary>
    [YamlIgnore]
    public AdoExpression<int>? RequestTimeout
    {
        get => GetExpression<int>("requestTimeout");
        init => SetProperty("requestTimeout", value);
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
    public UseDotNetTask(DotNetPackageType packageType, AdoExpression<string> version, AdoExpression<bool>? includePreviewVersions = null) : this()
    {
        PackageType = packageType;
        Version = version;
        IncludePreviewVersions = includePreviewVersions;
        DisplayName = packageType == DotNetPackageType.Runtime ? "Install .NET runtime" : "Install .NET SDK";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UseDotNetTask"/> class.
    /// </summary>
    public UseDotNetTask() : base("UseDotNet@2")
    {
        DisplayName = "Install .NET SDK";
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
    [YamlMember(Alias = "sdk")]
    Sdk,

    /// <summary>
    /// The dotnet runtime.
    /// </summary>
    [YamlMember(Alias = "runtime")]
    Runtime,
}
