using Sharpliner.AzureDevOps;

namespace SharplinerPipelineProject;

/// <summary>
/// Sample step template demonstrating how to create a reusable YAML step template.
/// Step templates generate separate YAML files that can be referenced in pipelines.
/// </summary>
class BuildStepsTemplate : StepTemplateDefinition
{
    public override string TargetFile => "pipelines/templates/build-steps.yml";

    protected Parameter sdkVersion = StringParameter("sdkVersion");

    public override List<Parameter> Parameters => [sdkVersion];

    public override ConditionedList<Step> Definition =>
    [
        DotNet.Install.Sdk(sdkVersion)
            .DisplayAs("Install .NET SDK " + sdkVersion),

        DotNet.Restore.Projects("**/*.csproj"),

        DotNet.Build("**/*.csproj") with
        {
            DisplayName = "Build project"
        },
    ];
}
