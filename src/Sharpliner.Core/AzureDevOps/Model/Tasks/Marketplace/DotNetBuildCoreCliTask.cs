using System;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Task represents the <c>dotnet build</c> command.
/// </summary>
public record DotNetBuildCoreCliTask : DotNetCoreCliTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DotNetBuildCoreCliTask"/> class.
    /// </summary>
    public DotNetBuildCoreCliTask() : base("build")
    {
        DisplayName = "dotnet build";
    }

    /// <summary>
    /// DotNetCoreCLI@2 does not define <c>includeNuGetOrg</c> for the build command.
    /// This property is retained for source compatibility with older Sharpliner versions.
    /// </summary>
    [Obsolete("DotNetCoreCLI@2 does not define includeNuGetOrg for the build command.")]
    [YamlIgnore]
    public AdoExpression<bool>? IncludeNuGetOrg
    {
        get => GetExpression<bool>("includeNuGetOrg");
        init => SetProperty("includeNuGetOrg", value);
    }
}
