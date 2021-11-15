# Definition collections (generating definitions dynamically)

Sometimes you need to generate a large number of pipelines or templates which are very similar.
Since it is not possible to create dynamic implementations of definition classes, Sharpliner introduces collections.
Collections allow you to define a list of data objects that get turned into YAML files.

## Example

Say you want to generate many pipelines for your repository, each targeting a different platform.
By inheriting from [one of the `*Collection` classes](https://github.com/sharpliner/sharpliner/blob/main/src/Sharpliner/AzureDevOps/PublicDefinitions.cs), you can define many YAML pipelines at once.

Below we show a very minimal example but you can imagine that pipeline definitions will differ between OSes in more details than pool name.
It is much easier to accomodate for these inside C# than in YAML using if's and pipeline variables.

```csharp
class TestPipelines : SingleStagePipelineCollection
{
    private readonly string[] Platforms = new[]
    {
        "ubuntu-20.04",
        "windows-2019",
    };

    public override IEnumerable<PipelineDefinitionData<SingleStagePipeline>> Pipelines =>
        Platforms.Select(platform => new PipelineDefinitionData<SingleStagePipeline>(
            TargetFile: platform + ".yml",
            Pipeline: Define(platform),
            Header: new[] // Optional custom YAML file header
            {
                "This pipeline is not used in CI",
                "It has been generated from " + nameof(TestPipelines) + ".cs for E2E test purposes",
            }));

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
```

The code above would end up generating two YAML files `ubuntu-20.04.yml` and `windows-2019.yml`.
