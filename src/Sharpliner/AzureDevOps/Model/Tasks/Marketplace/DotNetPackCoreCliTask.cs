using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Task represents the `dotnet pack` command.
/// </summary>
public record DotNetPackCoreCliTask : DotNetCoreCliTask
{

    /// <summary>
    /// Initializes a new instance of the <see cref="DotNetPackCoreCliTask"/> class.
    /// </summary>
    public DotNetPackCoreCliTask() : base("pack")
    {
    }

    /// <summary>
    /// Pattern to search for csproj or nuspec files to pack. You can separate multiple patterns with a semicolon,
    /// and you can make a pattern negative by prefixing it with !.
    /// Example: **/*.csproj;!**/*.Tests.csproj
    ///
    /// Argument aliases: searchPatternPack
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? PackagesToPack
    {
        get => GetConditioned<string>("packagesToPack");
        init => SetProperty("packagesToPack", value);
    }

    /// <summary>
    /// When using a csproj file this specifies the configuration to package.
    /// Argument aliases: configuration
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ConfigurationToPack
    {
        get => GetConditioned<string>("configurationToPack");
        init => SetProperty("configurationToPack", value);
    }

    /// <summary>
    /// Folder where packages will be created. If empty, packages will be created alongside the csproj file.
    ///
    /// Argument aliases: packDirectory
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? OutputDir
    {
        get => GetConditioned<string>("outputDir");
        init => SetProperty("outputDir", value);
    }

    /// <summary>
    /// Don't build the project before packing.
    /// Corresponds to the --no-build parameter of the `build` command.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? NoBuild
    {
        get => GetConditioned<bool>("nobuild");
        init => SetProperty("nobuild", value);
    }

    /// <summary>
    /// Additionally creates symbol NuGet packages.
    /// Corresponds to the --include-symbols command line parameter.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? IncludeSymbols
    {
        get => GetConditioned<bool>("includesymbols");
        init => SetProperty("includesymbols", value);
    }

    /// <summary>
    /// Includes source code in the package.
    /// Corresponds to the --include-source command line parameter.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? IncludeSource
    {
        get => GetConditioned<bool>("includesource");
        init => SetProperty("includesource", value);
    }

    /// <summary>
    /// Specifies a list of token = value pairs, separated by semicolons, where each occurrence of $token$ in the .nuspec file will be replaced with
    /// the given value.
    ///
    /// Values can be strings in quotation marks
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? BuildProperties
    {
        get => GetConditioned<string>("buildProperties");
        init => SetProperty("buildProperties", value);
    }

    /// <summary>
    /// You must select an environment variable and ensure it contains the version number you want to use.
    /// </summary>
    /// <param name="envVarName">Name of the env var where version is stored</param>
    public DotNetPackCoreCliTask VersionByEnvVar(string envVarName)
    {
        SetProperty("versioningScheme", "byEnvVar");
        SetProperty("versionEnvVar", envVarName);
        return this;
    }

    /// <summary>
    /// This will use the build number to version your package.
    /// Note: Under Options set the build number format to be '$(BuildDefinitionName)_$(Year:yyyy).$(Month).$(DayOfMonth)$(Rev:.r)'
    /// </summary>
    public DotNetPackCoreCliTask VersionByBuildNumber()
    {
        SetProperty("versioningScheme", "byBuildNumber");
        return this;
    }

    /// <summary>
    /// Set the version manually
    /// </summary>
    /// <param name="majorVersion">The 'X' in version X.Y.Z.</param>
    /// <param name="minorVersion">The 'Y' in version X.Y.Z.</param>
    /// <param name="patchVersion">The 'Z' in version X.Y.Z.</param>
    public DotNetPackCoreCliTask VersionManually(string majorVersion, string minorVersion, string patchVersion)
    {
        SetProperty("versioningScheme", "byPrereleaseNumber");
        SetProperty("majorVersion", majorVersion);
        SetProperty("minorVersion", minorVersion);
        SetProperty("patchVersion", patchVersion);
        return this;
    }
}
