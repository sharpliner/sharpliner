using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.CI;

/// <summary>
/// These pipelines are just a minimal E2E test of the collection generation mechanism.
/// Resulting pipelines are not used in the project (as opposed to the pr and publish pipelines).
/// </summary>
#region pipeline-collection
class TestPipelines : SingleStagePipelineCollection
{
    // Define your data
    private static readonly string[] s_platforms =
    [
        "ubuntu-20.04",
        "windows-2019",
    ];

    // Create a list of definitions, each is published in its own YAML file
    public override IEnumerable<PipelineDefinitionData<SingleStagePipeline>> Pipelines =>
        s_platforms.Select(platform => new PipelineDefinitionData<SingleStagePipeline>(
            TargetFile: $"tests/pipelines/{platform}.yml",
            Pipeline: Define(platform),
            // Optional custom YAML file header
            Header:
            [
                "This pipeline is not used in CI",
                "It has been generated from " + nameof(TestPipelines) + ".cs for E2E test purposes",
            ]));

    private static SingleStagePipeline Define(string platform) => new()
    {
        Jobs =
        {
            new Job("Build")
            {
                Pool = new HostedPool(name: platform),

                Steps =
                {
                    DotNet.Build("Sharpliner.slnx", includeNuGetOrg: true)
                          .DisplayAs("Build projects"),

                    DotNet.Test("Sharpliner.slnx")
                          .DisplayAs("Run unit tests")
                }
            }
        }
    };
}
#endregion


class SamplePipeline : SingleStagePipelineDefinition
{
    public override string TargetFile => "pipelines/sample-pipeline.yml";
    public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

    public override SingleStagePipeline Pipeline => new()
    {
        Trigger = new Trigger("main"),
        Pr = new PrTrigger("main"),
        Jobs =
        [
            new Job("Build", "Build and test")
            {
                Pool = new HostedPool("Azure Pipelines", "ubuntu-latest"),
                Steps =
                [
                    // Reference an example step template with strong-typed parameters
                    new BuildStepsTemplate(new()
                    {
                        SdkVersion = "8.0.x"
                    }),

                    DotNet.Test("**/*Tests.csproj") with
                    {
                        DisplayName = "Run tests"
                    },
                ]
            }
        ],
    };
}

/// <summary>
/// Sample step template demonstrating how to create a reusable YAML step template with typed parameters.
/// Step templates generate separate YAML files that can be referenced in pipelines.
/// Typed parameters provide IntelliSense support and type safety when using the template.
/// </summary>
class BuildStepsTemplate(BuildStepsParameters buildParameters)
    : StepTemplateDefinition<BuildStepsParameters>(buildParameters)
{
    private readonly BuildStepsParameters _parameters = buildParameters;

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
/// Typed parameters for the BuildStepsTemplate.
/// Properties are automatically converted to template parameters with appropriate types.
/// </summary>
record BuildStepsParameters
{
    /// <summary>
    /// The .NET SDK version to install and use for building.
    /// </summary>
    public string SdkVersion { get; init; } = "8.0.x";
}
