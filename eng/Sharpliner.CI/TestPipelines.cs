using System.Collections.Generic;
using System.Linq;
using Sharpliner.AzureDevOps;

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
                    DotNet.Build("Sharpliner.sln", includeNuGetOrg: true)
                          .DisplayAs("Build projects"),

                    DotNet.Test("Sharpliner.sln")
                          .DisplayAs("Run unit tests")
                }
            }
        }
    };
}
#endregion
