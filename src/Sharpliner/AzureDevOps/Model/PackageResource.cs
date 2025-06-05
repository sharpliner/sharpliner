using System.Diagnostics.CodeAnalysis;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// <para>
/// You can consume NuGet and npm GitHub packages as a resource in YAML pipelines. Use <see cref="NuGetPackageResource"/> and <see cref="NpmPackageResource"/>.
/// </para>
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
    public AdoExpression<string>? Connection { get; init; }

    /// <summary>
    /// &lt;Repository&gt;/&lt;Name of the package&gt;
    /// </summary>
    [DisallowNull]
    public AdoExpression<string> Name { get; init; }

    /// <summary>
    /// Version of the packge to consume
    /// Optional, defaults to latest
    /// </summary>
    [DisallowNull]
    public AdoExpression<string>? Version { get; init; }

    /// <summary>
    /// To enable automated triggers (true/false)
    /// Optional, defaults to no triggers
    /// </summary>
    public AdoExpression<bool>? Trigger { get; init; }

    /// <summary>
    /// Creates a new instance of the <see cref="PackageResource"/> class.
    /// </summary>
    /// <param name="packageAlias">The alias for the package resource.</param>
    /// <param name="packageName">The name of the package.</param>
    /// <exception cref="System.ArgumentNullException">If any of the parameters is null.</exception>
    public PackageResource(string packageAlias, string packageName)
    {
        PackageAlias = packageAlias ?? throw new System.ArgumentNullException(nameof(packageAlias));
        Name = packageName ?? throw new System.ArgumentNullException(nameof(packageName));
    }
}

/// <summary>
/// Used to consume npm GitHub packages as a resource in pipelines.
/// </summary>
public record NpmPackageResource : PackageResource
{
    /// <inheritdoc/>
    public override string PackageType => "npm";

    /// <summary>
    /// Creates a new instance of the <see cref="NpmPackageResource"/> class.
    /// </summary>
    /// <param name="packageAlias">The alias for the package resource.</param>
    /// <param name="packageName">The name of the package.</param>
    public NpmPackageResource(string packageAlias, string packageName) : base(packageAlias, packageName)
    {
    }
}

/// <summary>
/// Used to consume NuGet packages as a resource in pipelines.
/// </summary>
public record NuGetPackageResource : PackageResource
{
    /// <inheritdoc/>
    public override string PackageType => "NuGet";

    /// <summary>
    /// Creates a new instance of the <see cref="NuGetPackageResource"/> class.
    /// </summary>
    /// <param name="packageAlias">The alias for the package resource.</param>
    /// <param name="packageName">The name of the package.</param>
    public NuGetPackageResource(string packageAlias, string packageName) : base(packageAlias, packageName)
    {
    }
}
