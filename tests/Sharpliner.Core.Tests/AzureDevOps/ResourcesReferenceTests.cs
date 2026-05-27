using Sharpliner.AzureDevOps;

namespace Sharpliner.Tests.AzureDevOps;

public class ResourcesReferenceTests
{
    private class ResourcesReferenceTest_Pipeline : SingleStagePipelineDefinition
    {
        public override string TargetFile => "pipeline.yml";

        public override SingleStagePipeline Pipeline => new()
        {
            Resources = new Resources()
            {
                Pipelines =
                {
                    new PipelineResource("source-pipeline")
                    {
                        Source = "PipelineTriggerSource",
                        Project = "FabrikamFiber",
                        Trigger = new PipelineTrigger()
                        {
                            Branches = new()
                            {
                                Include = [ "main" ]
                            }
                        }
                    },
                    new PipelineResource("other-project-pipeline")
                    {
                        Source = "PipelineTriggerFromOtherProject",
                        Project = "FabrikamRepo",
                        Trigger = new PipelineTrigger()
                    }
                }
            },
            Jobs =
            {
                new Job("TestJob")
                {
                    Pool = new HostedPool("ubuntu-latest"),
                    Steps =
                    {
                        Bash.Inline($"echo {resources.Pipeline["source-pipeline"].ProjectName}"),
                        Bash.Inline($"echo {resources.Pipeline["source-pipeline"].ProjectID}"),
                        Bash.Inline($"echo {resources.Pipeline["source-pipeline"].PipelineName}"),
                        Bash.Inline($"echo {resources.Pipeline["source-pipeline"].PipelineID}"),
                        Bash.Inline($"echo {resources.Pipeline["source-pipeline"].RunName}"),
                        Bash.Inline($"echo {resources.Pipeline["source-pipeline"].RunID}"),
                        Bash.Inline($"echo {resources.Pipeline["source-pipeline"].RunURI}"),
                        Bash.Inline($"echo {resources.Pipeline["source-pipeline"].SourceBranch}"),
                        Bash.Inline($"echo {resources.Pipeline["source-pipeline"].SourceCommit}"),
                        Bash.Inline($"echo {resources.Pipeline["source-pipeline"].SourceProvider}"),
                        Bash.Inline($"echo {resources.Pipeline["source-pipeline"].RequestedFor}"),
                        Bash.Inline($"echo {resources.Pipeline["source-pipeline"].RequestedForID}"),
                        Bash.Inline($"echo {resources.Pipeline["other-project-pipeline"].ProjectName}"),
                    }
                }
            }
        };
    }

    /// <summary>
    /// Tests that pipeline resource metadata can be accessed using the resources.Pipeline["alias"].PropertyName syntax.
    /// This verifies all 13 metadata properties are correctly serialized to YAML variables.
    /// </summary>
    [Fact]
    public Task PipelineResources_Serialization_Test()
    {
        var pipeline = new ResourcesReferenceTest_Pipeline();

        return Verify(pipeline.Serialize());
    }

    private class SimpleResourcesTest_Pipeline : SingleStagePipelineDefinition
    {
        public override string TargetFile => "simple-pipeline.yml";

        public override SingleStagePipeline Pipeline => new()
        {
            Resources = new Resources()
            {
                Pipelines =
                {
                    new PipelineResource("source-pipeline")
                    {
                        Source = "PipelineTriggerSource",
                        Project = "FabrikamFiber",
                        Trigger = new PipelineTrigger("main")
                    }
                }
            },
            Jobs =
            {
                new Job("EchoJob")
                {
                    Pool = new HostedPool("ubuntu-latest"),
                    Steps =
                    {
                        // Example from the issue description
                        Bash.Inline($"echo {resources.Pipeline["source-pipeline"].ProjectName}"),
                        Bash.Inline("printenv | sort")
                    }
                }
            }
        };
    }

    /// <summary>
    /// Tests the example scenario from the issue:
    /// Using resources.Pipeline["source-pipeline"].ProjectName to access pipeline resource metadata.
    /// </summary>
    [Fact]
    public Task SimpleResourcesTest_Serialization_Test()
    {
        var pipeline = new SimpleResourcesTest_Pipeline();

        return Verify(pipeline.Serialize());
    }
}
