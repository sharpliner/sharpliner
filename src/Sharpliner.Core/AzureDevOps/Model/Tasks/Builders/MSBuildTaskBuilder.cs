using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.Common.Model.Tasks;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Builder for creating an MSBuild task using the <c>MSBuild</c> keyword.
/// The generated YAML uses the <c>MSBuild@1</c> task as defined by the
/// <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/MSBuildV1/task.json">official MSBuildV1 task specification</see>.
/// </summary>
public class MSBuildTaskBuilder : TaskBuilderBase
{
    /// <summary>
    /// Creates an <c>MSBuild@1</c> task that builds the given project(s) or solution(s).
    /// Platform, configuration and additional MSBuild arguments can be freely combined as they are all optional.
    /// <para>For example:</para>
    /// <code lang="csharp">
    /// Steps =
    /// {
    ///     MSBuild.Build("**/*.sln", platform: "x64", configuration: "Release", msbuildArguments: "/t:Restore;Build")
    /// }
    /// </code>
    /// Will generate:
    /// <code lang="yaml">
    /// - task: MSBuild@1
    ///   inputs:
    ///     solution: '**/*.sln'
    ///     platform: x64
    ///     configuration: Release
    ///     msbuildArguments: /t:Restore;Build
    /// </code>
    /// </summary>
    /// <param name="solution">Relative path from repo root of the project(s) or solution(s) to run. Wildcards can be used.</param>
    /// <param name="platform">Platform to build, for example <c>x86</c>, <c>x64</c>, or <c>Any CPU</c>.</param>
    /// <param name="configuration">Configuration to build, for example <c>debug</c> or <c>release</c>.</param>
    /// <param name="msbuildArguments">Additional arguments passed to MSBuild (on Windows) and xbuild (on macOS).</param>
    /// <param name="displayName">Display name of the build step.</param>
    /// <returns>A new instance of <see cref="MSBuildTask"/> with the specified inputs.</returns>
    public MSBuildTask Build(
        AdoExpression<string> solution,
        AdoExpression<string>? platform = null,
        AdoExpression<string>? configuration = null,
        AdoExpression<string>? msbuildArguments = null,
        AdoExpression<string>? displayName = null)
        => new(solution)
        {
            Platform = platform,
            Configuration = configuration,
            MSBuildArguments = msbuildArguments,
            DisplayName = displayName!,
        };

    internal MSBuildTaskBuilder()
    {
    }
}
