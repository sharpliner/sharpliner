using FluentAssertions;
using Sharpliner.AzureDevOps;

namespace Sharpliner.Tests.AzureDevOps;

public class PipelineResourceSerializationTests
{
    private class ResourcePipeline : SingleStagePipelineDefinition
    {
        public override string TargetFile => "foo.yaml";

        public override SingleStagePipeline Pipeline => new()
        {
            Resources = new Resources()
            {
                Pipelines =
                {
                    new PipelineResource("source-pipeline")
                    {
                        Source = "TriggeringPipeline",
                        Trigger = new()
                        {
                            Branches = new()
                            {
                                Include =
                                [
                                    "main",
                                    "develop",
                                    "features/*"
                                ],
                                Exclude =
                                [
                                    "features/experimental/*"
                                ]
                            }
                        }
                    }
                },
                Builds =
                {
                    new BuildResource("Spaceworkz")
                    {
                        Type = "Jenkins",
                        Connection = "MyJenkinsServer",
                        Source = "SpaceworkzProj",
                        Trigger = true
                    }
                },
                Repositories =
                {
                    new RepositoryResource("sharpliner")
                    {
                        Type = RepositoryType.Git,
                        Endpoint = "https://github.com/sharpliner.sharpliner",
                    },
                    new RepositoryResource("1ESPipelineTemplates")
                    {
                        Type = RepositoryType.Git,
                        Name = "1ESPipelineTemplates/1ESPipelineTemplates",
                        Ref = "refs/tags/release"
                    },
                    // See https://github.com/microsoft/vscode-dts/blob/33d4a59963409f3b2b051e1a567e46a0e270684f/ci.yml#L10-L15
                    new RepositoryResource("templates")
                    {
                        Type = RepositoryType.GitHub,
                        Name = "microsoft/vscode-engineering",
                        Ref = "main",
                        Endpoint = "Monaco"
                    }
                },
                Containers =
                {
                    // See https://github.com/microsoft/LightGBM/blob/480600b3afaf2a0a6f32cf417edf9567f625b2c3/.vsts-ci.yml#L28-L43
                    new ContainerResource("linux-artifact-builder")
                    {
                        Image = "lightgbm/vsts-agent:manylinux_2_28_x86_64",
                        MountReadOnly = new()
                        {
                            Work = false,
                            Externals = true,
                            Tools = true,
                            Tasks = true
                        },
                    },
                    new ContainerResource("ubuntu-latest")
                    {
                        Image = "ubuntu:22:04",
                        Options = "--name ci-container -v /usr/bin/docker:/tmp/docker:ro",
                        MountReadOnly = new()
                        {
                            Work = false,
                            Externals = true,
                            Tools = true,
                            Tasks = true
                        }
                    },
                    // See https://github.com/speediedan/finetuning-scheduler/blob/fb75fe996fb2bdefa65acdf6836eddfffb7373d6/.azure-pipelines/gpu-tests.yml#L58-L63
                    new ContainerResource(variables["image"])
                    {
                        MapDockerSocket = false,
                        Volumes = [ "/var/run/user/998/docker.sock:/var/run/docker.sock" ],
                        Options = "--gpus all --shm-size=512m"
                    }
                },
                Packages =
                {
                    new NpmPackageResource("contoso", "yourname/contoso")
                    {
                        Connection = "pat-contoso",
                        Version = "7.130.88",
                        Trigger = true
                    },
                    new NuGetPackageResource("newtonsoftjson", "newtonsoft.json")
                    {
                        Connection = "pat-new",
                        Version = "13.0.3",
                    }
                },
                Webhooks =
                {
                    new WebhookResource("WebHook")
                    {
                        Connection = "IncomingWH",
                        Filters =
                        {
                            new JsonParameterFilter("repositoryName", "maven-releases"),
                            new JsonParameterFilter("action", "CREATED")
                        }
                    }
                }
            }
        };
    }

    [Fact]
    public Task ResourcePipeline_Serialization_Test()
    {
        var pipeline = new ResourcePipeline();

        return Verify(pipeline.Serialize());
    }
}
