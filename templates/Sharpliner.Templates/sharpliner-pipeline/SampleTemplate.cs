using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Expressions;

namespace SharplinerPipelineProject;

/// <summary>
/// Sample step template demonstrating how to create a reusable YAML step template with typed parameters.
/// Step templates generate separate YAML files that can be referenced in pipelines.
/// Typed parameters provide IntelliSense support and type safety when using the template.
/// </summary>
class SampleTemplate(SampleTemplateParameters buildParameters)
    : StepTemplateDefinition<SampleTemplateParameters>(buildParameters)
{
    private readonly SampleTemplateParameters _parameters = buildParameters;

    public override string TargetFile => "pipelines/templates/build-steps.yml";

    public override AdoExpressionList<Step> Definition =>
    [
        DotNet.Install.Sdk(_parameters.SdkVersion)
            .DisplayAs("Install .NET SDK " + _parameters.SdkVersion),

        DotNet.Restore.Projects("**/*.csproj"),

        DotNet.Build("**/*.csproj") with
        {
            DisplayName = "Build project"
        },
    ];
}

/// <summary>
/// Typed parameters for the SampleTemplate.
/// Properties are automatically converted to template parameters with appropriate types.
/// </summary>
record SampleTemplateParameters
{
    /// <summary>
    /// The .NET SDK version to install and use for building.
    /// </summary>
    public string SdkVersion { get; init; } = "8.0.x";
}
