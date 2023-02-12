using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&amp;tabs=schema%2Cparameter-schema#packages-resource">official Azure DevOps pipelines documentation</see>.
/// </summary>
public abstract record PackageResource
{
    /// <summary>
    /// Alias for the package resource
    /// </summary>
    [YamlMember(Alias = "package")]
    public string PackageAlias { get; }

    /// <summary>
    /// Type of the package.
    /// </summary>
    public abstract string PackageType { get; }

    /// <summary>
    /// Github service connection with the PAT type
    /// </summary>
    [DisallowNull]
    public Conditioned<string>? Connection { get; init; }

    /// <summary>
    /// &lt;Repository&gt;/&lt;Name of the package&gt;
    /// </summary>
    [DisallowNull]
    public string Name { get; init; }

    /// <summary>
    /// Version of the packge to consume
    /// Optional, defaults to latest
    /// </summary>
    [DisallowNull]
    [DefaultValue("latest")]
    public string? Version { get; init; } = "latest";

    /// <summary>
    /// To enable automated triggers (true/false)
    /// Optional, defaults to no triggers
    /// </summary>
    public bool Trigger { get; init; }

    public PackageResource(string packageAlias, string packageName)
    {
        PackageAlias = packageAlias ?? throw new System.ArgumentNullException(nameof(packageAlias));
        Name = packageName ?? throw new System.ArgumentNullException(nameof(packageName));
    }
}

public record NpmPackageResouce : PackageResource
{
    public override string PackageType => "npm";

    public NpmPackageResouce(string packageAlias, string packageName) : base(packageAlias, packageName)
    {
    }
}

public record NuGetPackageResouce : PackageResource
{
    public override string PackageType => "NuGet";

    public NuGetPackageResouce(string packageAlias, string packageName) : base(packageAlias, packageName)
    {
    }
}
