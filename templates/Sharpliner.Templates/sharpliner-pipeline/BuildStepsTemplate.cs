using Sharpliner.AzureDevOps;

namespace SharplinerPipelineProject;

/// <summary>
/// Sample step template collection demonstrating how to create reusable YAML step templates.
/// Step templates generate separate YAML files that can be referenced in pipelines.
/// </summary>
class BuildStepsTemplate : StepTemplateCollection
{
    public override List<TemplateDefinitionData<Step>> Templates =>
    [
        new(
            "pipelines/templates/build-steps.yml",
            [
                DotNet.Install.Sdk(parameters["sdkVersion"])
                    .DisplayAs("Install .NET SDK " + parameters["sdkVersion"]),

                DotNet.Restore.Projects("**/*.csproj"),

                DotNet.Build("**/*.csproj") with
                {
                    DisplayName = "Build project"
                },
            ],
            [StringParameter("sdkVersion")])
    ];
}
