using Sharpliner.AzureDevOps;

namespace SharplinerPipelineProject.Pipelines;

/// <summary>
/// Sample step library demonstrating how to create reusable step collections.
/// Step libraries allow you to define common step sequences that can be used across multiple pipelines.
/// </summary>
class BuildSteps : StepLibrary
{
    public override List<Step> Steps =>
    [
        DotNet.Install.Sdk("8.0.x"),

        DotNet.Restore.Projects("**/*.csproj"),

        DotNet.Build("**/*.csproj") with
        {
            DisplayName = "Build project"
        },
    ];
}
