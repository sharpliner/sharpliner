using System;
using System.Collections.Generic;
using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating <c>VSBuild@1</c> tasks.
/// See the official <see href="https://learn.microsoft.com/azure/devops/pipelines/tasks/build/visual-studio-build">VSBuild@1 task reference</see>.
/// </summary>
public class VSBuildTaskBuilder
{
    internal VSBuildTaskBuilder()
    {
    }

    /// <summary>
    /// Starts a fluent VSBuild configuration for a solution or MSBuild project path.
    /// </summary>
    /// <param name="solution">Relative path from repo root of the solution(s) or MSBuild project to run. Wildcards are supported, for example <c>**\*.sln</c>.</param>
    /// <returns>A fluent builder that can configure platform, configuration, and packaging options.</returns>
    public VSBuildFluentBuilder Solution(AdoExpression<string> solution) => new(solution);

    /// <summary>
    /// Creates a basic <see cref="VSBuildTask"/> for a solution or MSBuild project path.
    /// </summary>
    /// <param name="solution">Relative path from repo root of the solution(s) or MSBuild project to run.</param>
    /// <returns>A <see cref="VSBuildTask"/> instance.</returns>
    public VSBuildTask Task(AdoExpression<string> solution) => new(solution);
}

/// <summary>
/// Fluent builder for constructing <see cref="VSBuildTask"/> instances.
/// </summary>
public class VSBuildFluentBuilder
{
    private VSBuildTask _task;

    internal VSBuildFluentBuilder(AdoExpression<string> solution)
    {
        _task = new(solution);
    }

    /// <summary>
    /// Sets the VSBuild <c>platform</c> input.
    /// </summary>
    /// <param name="platform">Build platform, for example <c>Any CPU</c>, <c>x86</c>, or <c>x64</c>.</param>
    /// <returns>The current builder instance.</returns>
    public VSBuildFluentBuilder Platform(AdoExpression<string> platform)
    {
        _task = _task with { Platform = platform };
        return this;
    }

    /// <summary>
    /// Sets the VSBuild <c>configuration</c> input.
    /// </summary>
    /// <param name="configuration">Build configuration, for example <c>Debug</c> or <c>Release</c>.</param>
    /// <returns>The current builder instance.</returns>
    public VSBuildFluentBuilder Configuration(AdoExpression<string> configuration)
    {
        _task = _task with { Configuration = configuration };
        return this;
    }

    /// <summary>
    /// Sets both <c>platform</c> and <c>configuration</c> inputs.
    /// </summary>
    /// <param name="platform">Build platform.</param>
    /// <param name="configuration">Build configuration.</param>
    /// <returns>The current builder instance.</returns>
    public VSBuildFluentBuilder PlatformAndConfiguration(AdoExpression<string> platform, AdoExpression<string> configuration)
        => Platform(platform).Configuration(configuration);

    /// <summary>
    /// Sets the VSBuild <c>msbuildArgs</c> input directly.
    /// </summary>
    /// <param name="arguments">Additional arguments passed to MSBuild.</param>
    /// <returns>The current builder instance.</returns>
    public VSBuildFluentBuilder MSBuildArguments(AdoExpression<string> arguments)
    {
        _task = _task with { MSBuildArgs = arguments };
        return this;
    }

    /// <summary>
    /// Adds standard web packaging properties to <c>msbuildArgs</c>, including <c>DeployOnBuild</c>, <c>WebPublishMethod</c>,
    /// <c>PackageAsSingleFile</c>, and <c>PackageLocation</c>. Existing <c>msbuildArgs</c> (if any) are preserved and prefixed.
    /// </summary>
    /// <param name="packageLocation">Package output location, for example <c>$(Build.ArtifactStagingDirectory)\WebApp.zip</c>.</param>
    /// <param name="packageAsSingleFile">Value for <c>PackageAsSingleFile</c>. Default: <c>true</c>.</param>
    /// <param name="skipInvalidConfigurations">When set, also adds <c>SkipInvalidConfigurations</c>.</param>
    /// <returns>The current builder instance.</returns>
    public VSBuildFluentBuilder WebPackage(
        string packageLocation,
        bool? packageAsSingleFile = null,
        bool? skipInvalidConfigurations = null)
    {
        List<string> arguments = [];

        if (_task.MSBuildArgs is not null)
        {
            arguments.Add(_task.MSBuildArgs.ToString());
        }

        arguments.Add("/p:DeployOnBuild=true");
        arguments.Add("/p:WebPublishMethod=Package");
        arguments.Add($"/p:PackageAsSingleFile={ToBooleanArgument(packageAsSingleFile, true)}");
        arguments.Add($"/p:PackageLocation=\"{packageLocation}\"");

        if (skipInvalidConfigurations is not null)
        {
            arguments.Add($"/p:SkipInvalidConfigurations={ToBooleanArgument(skipInvalidConfigurations, true)}");
        }

        _task = _task with
        {
            MSBuildArgs = string.Join(" ", arguments)
        };

        return this;
    }

    /// <summary>
    /// Builds and returns the configured <see cref="VSBuildTask"/>.
    /// </summary>
    public VSBuildTask Build() => _task;

    /// <summary>
    /// Implicitly converts the fluent builder into a <see cref="VSBuildTask"/>.
    /// </summary>
    /// <param name="builder">The fluent builder instance.</param>
    public static implicit operator VSBuildTask(VSBuildFluentBuilder builder) => builder.Build();

    private static string ToBooleanArgument(bool? value, bool defaultValue)
        => (value ?? defaultValue).ToString().ToLowerInvariant();
}
