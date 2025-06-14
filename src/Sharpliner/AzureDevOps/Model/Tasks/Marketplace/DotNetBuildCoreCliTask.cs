using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Task represents the <c>>dotnet build</c> command.
/// </summary>
public record DotNetBuildCoreCliTask : DotNetCoreCliTask
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DotNetBuildCoreCliTask"/> class.
    /// </summary>
    public DotNetBuildCoreCliTask() : base("build")
    {
    }

    /// <summary>
    /// Include NuGet.org in the generated NuGet.config
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? IncludeNuGetOrg
    {
        get => GetExpression<bool>("includeNuGetOrg");
        init => SetProperty("includeNuGetOrg", value);
    }
}
