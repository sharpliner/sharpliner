using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks
{
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
            VersioningScheme = Require.NotNullAndNotEmpty(versioningScheme);
        }

        /// <summary>
        /// Gets or sets the pattern to search for csproj or nuspec files to pack.
        /// </summary>
        [YamlIgnore]
        public string? PackagesToPack
        {
            get => GetString("packagesToPack");
            init => SetProperty("packagesToPack", value);
        }

        /// <summary>
        /// Gets or sets the versioning scheme to use for the package version.
        /// </summary>
        [YamlIgnore]
        internal string VersioningScheme
        {
            get => GetString("versioningScheme")!;
            init => SetProperty("versioningScheme", value);
        }

        /// <summary>
        /// Specifies the configuration to package when using a csproj file.
        /// </summary>
        [YamlIgnore]
        public string? Configuration
        {
            get => GetString("configuration");
            init => SetProperty("configuration", value);
        }

        /// <summary>
        /// Specifies the folder where the task creates packages. If the value is empty, the task creates packages at the source root.
        /// </summary>
        [YamlIgnore]
        public string? PackDestination
        {
            get => GetString("packDestination");
            init => SetProperty("packDestination", value);
        }

        /// <summary>
        /// Specifies that the package contains sources and symbols. When used with a <c>.nuspec</c> file, this creates a regular NuGet package file and the corresponding symbols package.
        /// </summary>
        [YamlIgnore]
        public bool? IncludeSymbols
        {
            get => GetBool("includeSymbols", false);
            init => SetProperty("includeSymbols", value);
        }

        /// <summary>
        /// Determines if the output files of the project should be in the tool folder.
        /// </summary>
        [YamlIgnore]
        public bool? ToolPackage
        {
            get => GetBool("toolPackage", false);
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
            init => SetProperty("buildProperties", string.Join(";", value!.Select(x => $"{x.Key}={x.Value}")));
        }

        /// <summary>
        /// Specifies the amount of detail displayed in the output.
        /// </summary>
        [YamlIgnore]
        public PackVerbosity? VerbosityPack
        {
            get => GetString("verbosityPack") switch
            {
                "Quiet" => PackVerbosity.Quiet,
                "Normal" => PackVerbosity.Normal,
                "Detailed" => PackVerbosity.Detailed,
                _ => null
            };
            init => SetProperty("verbosityPack", SharplinerSerializer.Serialize(value!));
        }


        /// <summary>
        /// Specifies the base path of the files defined in the <c>nuspec</c> file.
        /// </summary>
        public string? BasePath
        {
            get => GetString("basePath");
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
        public bool IncludeReferencedProjects
        {
            get => GetBool("includeReferencedProjects", false);
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
        /// </summary>
        [YamlIgnore]
        public string MajorVersion
        {
            get => GetString("majorVersion")!;
            init => SetProperty("majorVersion", value);
        }

        /// <summary>
        /// The <c>Y</c> in version <see href="http://semver.org/spec/v1.0.0.html">X.Y.Z</see>.
        /// </summary>
        [YamlIgnore]
        public string MinorVersion
        {
            get => GetString("minorVersion")!;
            init => SetProperty("minorVersion", value);
        }

        /// <summary>
        /// The <c>Z</c> in version <see href="http://semver.org/spec/v1.0.0.html">X.Y.Z</see>.
        /// </summary>
        [YamlIgnore]
        public string PatchVersion
        {
            get => GetString("patchVersion")!;
            init => SetProperty("patchVersion", value);
        }

        /// <summary>
        /// Specifies the desired time zone used to produce the version of the package. Selecting <see cref="PackTimezoneType.UTC"/> is recommended if you're using hosted build agents, as their date and time might differ.
        /// </summary>
        [YamlIgnore]
        public PackTimezoneType? PackTimezone 
        {
            get => GetString("packTimezone") switch
            {
                "UTC" => PackTimezoneType.UTC,
                "Local" => PackTimezoneType.Local,
                _ => throw new ArgumentOutOfRangeException()
            };
            init => SetProperty("packTimezone", SharplinerSerializer.Serialize(value!));
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
        UTC,

        /// <summary>
        /// Local time zone.
        /// </summary>
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
        public NuGetPackCommandTaskByEnvVar(string versionEnvVar) : base("byEnvVar")
        {
            VersionEnvVar = Require.NotNullAndNotEmpty(versionEnvVar);
        }

        /// <summary>
        /// Specifies the variable name without <c>$</c>, <c>$env</c>, or <c>%</c>.
        /// </summary>
        [YamlIgnore]
        public string VersionEnvVar
        {
            get => GetString("versionEnvVar")!;
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
}
