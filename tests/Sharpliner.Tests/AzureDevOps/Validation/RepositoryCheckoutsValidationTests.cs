using System.Linq;
using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps.Validation;

public class RepositoryCheckoutsValidationTests
{
    private class MissingResourcePipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("job_1")
                {
                    Steps =
                    {
                        Checkout.Self,
                    }
                },
                new Job("job_2")
                {
                    Steps =
                    {
                        Checkout.None,
                        Checkout.Repository("self"),
                    }
                },
                new Job("job_3")
                {
                    Steps =
                    {
                        If.IsBranch("main")
                            .Step(Checkout.Repository("repo1"))
                        .Else
                            .Step(Checkout.Repository("repo2")),
                    }
                },
            },

            Resources = new Resources()
            {
                Repositories =
                {
                    new RepositoryResource("repo1"),
                    new RepositoryResource("repo3"),
                }
            }
        };
    }

    [Fact]
    public void MissingResource_Validation_Test()
    {
        var pipeline = new MissingResourcePipeline();

        var errors = pipeline.Validations.SelectMany(v => v.Validate()).ToList();

        Assert.Single(errors);
        Assert.Equal("Checked out repository `repo2` needs to be declared in pipeline resources", errors.Single().Message);
    }

    private class ResourcePipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Stages =
            {
                new Stage("Build")
                {
                    Jobs =
                    {
                        new Job("job_1")
                        {
                            Steps =
                            {
                                Checkout.Self,
                            }
                        },
                        new Job("job_2")
                        {
                            Steps =
                            {
                                Checkout.None,
                                Checkout.Repository("self"),
                            }
                        },
                        new Job("job_3")
                        {
                            Steps =
                            {
                                If.IsBranch("main")
                                    .Step(Checkout.Repository("repo1"))
                                .Else
                                    .Step(Checkout.Repository("repo2")),
                            }
                        },
                    }
                }
            },

            Resources = new Resources()
            {
                Repositories =
                {
                    new RepositoryResource("repo1"),
                    new RepositoryResource("repo2"),
                }
            }
        };
    }

    [Fact]
    public void Resource_Validation_Test()
    {
        var pipeline = new ResourcePipeline();
        var errors = pipeline.Validations.SelectMany(v => v.Validate()).ToList();
        errors.Should().BeEmpty();
    }
}
