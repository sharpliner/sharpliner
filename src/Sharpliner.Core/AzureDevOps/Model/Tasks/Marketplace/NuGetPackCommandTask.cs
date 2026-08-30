using System;
using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Represents the NuGetCommand@2 task for packing NuGet packages in Azure DevOps pipelines.
/// </summary>
/// <example>
/// <code>
/// var packTask = new NuGetPackCommandTask
/// {
///     PackagesToPack = "**/*.csproj",
///     Arguments = "-Properties Configuration=Release"
/// };
/// </code>
/// <para>The corresponding YAML will be:</para>
/// <code>
/// - task: NuGetCommand@2
///   inputs:
///     command: pack
///     packagesToPack: '**/*.csproj'
///     arguments: '-Properties Configuration=Release'
/// </code>
/// </example>
public abstract record NuGetPackCommandTask : NuGetCommandTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NuGetPackCommandTask"/> class.
    /// </summary>
    public NuGetPackCommandTask(string versioningScheme) : base("pack")
    {
        DisplayName = "NuGet pack";
        VersioningScheme = Require.NotNullAndNotEmpty(versioningScheme);
    }

    /// <summary>
    /// Gets or sets the pattern to search for <c>csproj</c> or <c>nuspec</c> files to pack.
    /// Multiple patterns can be separated with a semicolon and negative patterns can be prefixed with <c>!</c>,
    /// for example <c>**\*.csproj;!**\*.Tests.csproj</c>.
    /// The official input is <c>searchPatternPack</c>; <c>packagesToPack</c> is the YAML alias emitted for compatibility.
    /// Defaults to <c>**/*.csproj</c> when omitted.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PackagesToPack
    {
        get => GetExpression<string>("packagesToPack");
        init => SetProperty("packagesToPack", value);
    }

    /// <summary>
    /// Gets or sets the automatic package versioning scheme.
    /// Options are <c>off</c>, <c>byPrereleaseNumber</c>, <c>byEnvVar</c>, and <c>byBuildNumber</c>.
    /// Automatic package versioning cannot be used with <see cref="NuGetPackCommandTaskOff.IncludeReferencedProjects"/>.
    /// Defaults to <c>off</c>.
    /// </summary>
    [YamlIgnore]
    internal AdoExpression<string>? VersioningScheme
    {
        get => GetExpression<string>("versioningScheme");
        init => SetProperty("versioningScheme", value);
    }

    /// <summary>
    /// Specifies the configuration to package when using a <c>csproj</c> file.
    /// The official input is <c>configurationToPack</c>; <c>configuration</c> is the YAML alias emitted for compatibility.
    /// Defaults to <c>$(BuildConfiguration)</c> when omitted.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? Configuration
    {
        get => GetExpression<string>("configuration");
        init => SetProperty("configuration", value);
    }

    /// <summary>
    /// Specifies the folder where the task creates packages. If the value is empty, the task creates packages at the source root.
    /// The official input is <c>outputDir</c>; <c>packDestination</c> is the YAML alias emitted for compatibility.
    /// Defaults to <c>$(Build.ArtifactStagingDirectory)</c> when omitted.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PackDestination
    {
        get => GetExpression<string>("packDestination");
        init => SetProperty("packDestination", value);
    }

    /// <summary>
    /// Specifies that the package contains sources and symbols. When used with a <c>.nuspec</c> file, this creates a regular NuGet package file and the corresponding symbols package.
    /// Defaults to <c>false</c> when omitted.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? IncludeSymbols
    {
        get => GetExpression<bool>("includeSymbols");
        init => SetProperty("includeSymbols", value);
    }

    /// <summary>
    /// Determines if the output files of the project should be in the tool folder.
    /// Defaults to <c>false</c> when omitted.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? ToolPackage
    {
        get => GetExpression<bool>("toolPackage");
        init => SetProperty("toolPackage", value);
    }

    /// <summary>
    /// Specifies a list of token=value pairs, separated by semicolons, where each occurrence of <c>$token$</c> in the <c>.nuspec</c> file will be replaced with the given value. 
    /// Values can be strings in quotation marks.
    /// </summary>
    [YamlIgnore]
    public Dictionary<string, string>? BuildProperties
    {
        get => GetString("buildProperties")?.Split(';').ToDictionary(pair => pair.Split('=')[0], pair => pair.Split('=')[1]);
        init => SetProperty("buildProperties", string.Join(';', value!.Select(x => $"{x.Key}={x.Value}")));
    }

    /// <summary>
    /// Specifies the amount of detail displayed in the pack output.
    /// Options are <see cref="PackVerbosity.Quiet"/>, <see cref="PackVerbosity.Normal"/>, and <see cref="PackVerbosity.Detailed"/>.
    /// Defaults to <see cref="PackVerbosity.Detailed"/> when omitted.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<PackVerbosity>? VerbosityPack
    {
        get => GetExpression<PackVerbosity>("verbosityPack");
        init => SetProperty("verbosityPack", value);
    }


    /// <summary>
    /// Specifies the base path of the files defined in the <c>nuspec</c> file.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? BasePath
    {
        get => GetExpression<string>("basePath");
        init => SetProperty("basePath", value);
    }
}

/// <summary>
/// Specifies the amount of detail displayed in the output for the pack command.
/// </summary>
public enum PackVerbosity
{
    /// <summary>
    /// Quiet verbosity.
    /// </summary>
    Quiet,

    /// <summary>
    /// Normal verbosity.
    /// </summary>
    Normal,

    /// <summary>
    /// Detailed verbosity.
    /// This is the default used by NuGetCommand@2 when verbosity is omitted.
    /// </summary>
    Detailed,
}

/// <summary>
/// Represents the NuGetCommand@2 task for packing NuGet packages in Azure DevOps pipelines with <c>versioningScheme></c> set to <c>off</c>.
/// </summary>
public record NuGetPackCommandTaskOff : NuGetPackCommandTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NuGetPackCommandTaskOff"/> class.
    /// </summary>
    public NuGetPackCommandTaskOff() : base("off") { }

    /// <summary>
    /// Includes referenced projects either as dependencies or as part of the package. 
    /// Cannot be used with automatic package versioning. 
    /// If a referenced project has a corresponding <c>nuspec</c> file that has the same name as the project, then that referenced project is added as a dependency. 
    /// Otherwise, the referenced project is added as part of the package. 
    /// Learn more about <see href="https://learn.microsoft.com/en-us/nuget/tools/cli-ref-pack">using the pack command for NuGet CLI to create NuGet packages</see>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? IncludeReferencedProjects
    {
        get => GetExpression<bool>("includeReferencedProjects");
        init => SetProperty("includeReferencedProjects", value);
    } 
}

/// <summary>
/// Represents the NuGetCommand@2 task for packing NuGet packages in Azure DevOps pipelines with <c>versioningScheme></c> set to <c>byPrereleaseNumber</c>.
/// </summary>
public record NuGetPackCommandTaskByPrereleaseNumber : NuGetPackCommandTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NuGetPackCommandTaskByPrereleaseNumber"/> class.
    /// </summary>
    /// <param name="majorVersion">The <c>X</c> in version <see href="http://semver.org/spec/v1.0.0.html">X.Y.Z</see>.</param>
    /// <param name="minorVersion">The <c>Y</c> in version <see href="http://semver.org/spec/v1.0.0.html">X.Y.Z</see>.</param>
    /// <param name="patchVersion">The <c>Z</c> in version <see href="http://semver.org/spec/v1.0.0.html">X.Y.Z</see>.</param>
    public NuGetPackCommandTaskByPrereleaseNumber(string majorVersion, string minorVersion, string patchVersion) : base("byPrereleaseNumber")
    {
        MajorVersion = Require.NotNullAndNotEmpty(majorVersion);
        MinorVersion = Require.NotNullAndNotEmpty(minorVersion);
        PatchVersion = Require.NotNullAndNotEmpty(patchVersion);
    }

    /// <summary>
    /// The <c>X</c> in version <see href="http://semver.org/spec/v1.0.0.html">X.Y.Z</see>.
    /// The official input is <c>requestedMajorVersion</c>; <c>majorVersion</c> is the YAML alias emitted for compatibility.
    /// Defaults to <c>1</c> in the Azure Pipelines task UI.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? MajorVersion
    {
        get => GetExpression<string>("majorVersion");
        init => SetProperty("majorVersion", value);
    }

    /// <summary>
    /// The <c>Y</c> in version <see href="http://semver.org/spec/v1.0.0.html">X.Y.Z</see>.
    /// The official input is <c>requestedMinorVersion</c>; <c>minorVersion</c> is the YAML alias emitted for compatibility.
    /// Defaults to <c>0</c> in the Azure Pipelines task UI.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? MinorVersion
    {
        get => GetExpression<string>("minorVersion");
        init => SetProperty("minorVersion", value);
    }

    /// <summary>
    /// The <c>Z</c> in version <see href="http://semver.org/spec/v1.0.0.html">X.Y.Z</see>.
    /// The official input is <c>requestedPatchVersion</c>; <c>patchVersion</c> is the YAML alias emitted for compatibility.
    /// Defaults to <c>0</c> in the Azure Pipelines task UI.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PatchVersion
    {
        get => GetExpression<string>("patchVersion");
        init => SetProperty("patchVersion", value);
    }

    /// <summary>
    /// Specifies the desired time zone used to produce the version of the package.
    /// Selecting <see cref="PackTimezoneType.UTC"/> is recommended if you're using hosted build agents, as their date and time might differ.
    /// Applies only when <c>versioningScheme</c> is <c>byPrereleaseNumber</c>.
    /// Defaults to <c>utc</c> when omitted.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<PackTimezoneType>? PackTimezone 
    {
        get => GetExpression<PackTimezoneType>("packTimezone");
        init => SetProperty("packTimezone", value);
    }
}

/// <summary>
/// Specifies the desired time zone used to produce the version of the package.
/// </summary>
public enum PackTimezoneType
{
    /// <summary>
    /// UTC time zone.
    /// </summary>
    [YamlMember(Alias = "utc")]
    UTC,

    /// <summary>
    /// Local time zone.
    /// </summary>
    [YamlMember(Alias = "local")]
    Local
}

/// <summary>
/// Represents the NuGetCommand@2 task for packing NuGet packages in Azure DevOps pipelines with <c>versioningScheme></c> set to <c>byEnvVar</c>.
/// </summary>
public record NuGetPackCommandTaskByEnvVar : NuGetPackCommandTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NuGetPackCommandTaskByEnvVar"/> class.
    /// </summary>
    /// <param name="versionEnvVar">The variable name without <c>$</c>, <c>$env</c>, or <c>%</c>.</param>
    public NuGetPackCommandTaskByEnvVar(AdoExpression<string> versionEnvVar) : base("byEnvVar")
    {
        VersionEnvVar = versionEnvVar ?? throw new ArgumentNullException(nameof(versionEnvVar));
    }

    /// <summary>
    /// Specifies the variable name without <c>$</c>, <c>$env</c>, or <c>%</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? VersionEnvVar
    {
        get => GetExpression<string>("versionEnvVar");
        init => SetProperty("versionEnvVar", value);
    }
}

/// <summary>
/// Represents the NuGetCommand@2 task for packing NuGet packages in Azure DevOps pipelines with <c>versioningScheme></c> set to <c>byBuildNumber</c>.
/// </summary>
public record NuGetPackCommandTaskByBuildNumber : NuGetPackCommandTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NuGetPackCommandTaskByBuildNumber"/> class.
    /// </summary>
    public NuGetPackCommandTaskByBuildNumber() : base("byBuildNumber") { }
}
