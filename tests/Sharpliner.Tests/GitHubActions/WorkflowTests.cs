using FluentAssertions;
using Sharpliner.GitHubActions;

namespace Sharpliner.Tests.GitHubActions;

public class WorkflowTests
{
    private class TestWorkflow : WorkflowDefinition
    {
        public override string TargetFile => "foo.yml";

        public override Workflow Workflow => new()
        {
            On =
            {
                Manuals =
                {
                    WorkflowDispatch = new()
                    {
                        Inputs =
                        {
                            new("name")
                            {
                                Description = "Person to greet",
                                Default = "Mona the Octocat",
                                IsRequired = true,
                            }
                        }
                    }
                },

                Schedules =
                {
                    new("'*/30 5,17 * * *'")
                },

                Webhooks =
                {
                    new CheckRun
                    {
                        Activities =  { CheckRun.Activity.Completed, CheckRun.Activity.RequestedAction }
                    },
                    new PullRequest
                    {
                        Activities = { PullRequest.Activity.Assigned, PullRequest.Activity.Closed }
                    }
                }
            },


            Permissions = new Permissions
            {
                Read =
                {
                    GitHubPermissionScope.Actions,
                }
            }.All(GitHubPermission.Write),

            Jobs =
            {
                new Job("configuration")
                {
                    Name = "Configure Build",
                    Env =
                    {
                        ["Database"] = "production",
                        ["Bot"] = "builder"
                    }
                },
                new Job("tests")
                {
                    Name = "Run Tests",
                    Env =
                    {
                        ["Database"] = "production",
                        ["Bot"] = "builder"
                    }
                }
            }
        };
    }

    [Fact]
    public Task Workflow_Serialization_Test()
    {
        var workflow = new TestWorkflow();

        return Verify(workflow.Serialize());
    }
    
    [Fact]
    public void Workflow_Single_Manual_Workflow()
    {
        var w = new Workflow
        {
            On =
            {
                Manuals =
                {
                    WorkflowDispatch = new()
                    {
                        Inputs =
                        {
                            new("name")
                            {
                                Description = "Person to greet",
                                Default = "Mona the Octocat",
                                IsRequired = true,
                            }
                        }
                    }
                }
            }
        };

        w.Should().NotBeNull();
        w.On.Manuals.WorkflowDispatch.Should().NotBeNull();
        w.On.Manuals.RepositoryDispatch.Should().BeNull();
        w.On.Schedules.Should().BeEmpty();
        w.On.Webhooks.Should().BeEmpty();
    }

    [Fact]
    public void Workflow_Manual_RepositoryDispatch()
    {
        var w = new Workflow
        {
            On =
            {
                Manuals =
                {
                    RepositoryDispatch = new()
                    {
                        Activities = { "Opened", "MyActivity"}
                    }
                }
            }
        };

        w.Should().NotBeNull();
        w.On.Manuals.WorkflowDispatch.Should().BeNull();
        w.On.Manuals.RepositoryDispatch.Should().NotBeNull();
        w.On.Schedules.Should().BeEmpty();
        w.On.Webhooks.Should().BeEmpty();
    }

    [Fact]
    public void Workflow_Single_Schedule()
    {
        var w = new Workflow
        {
            On =
            {
                Schedules =
                {
                    new("'*/30 5,17 * * *'")
                }
            }
        };
        
        w.Should().NotBeNull();
        w.On.Manuals.WorkflowDispatch.Should().BeNull();
        w.On.Manuals.RepositoryDispatch.Should().BeNull();
        w.On.Schedules.Should().HaveCount(1);
        w.On.Webhooks.Should().BeEmpty();
    }

    [Fact]
    public void Workflow_Several_Sechedule()
    {
        var w = new Workflow
        {
            On =
            {
                Schedules =
                {
                    new("'*/30 5,17 * * *'"),
                    new("'*/30 1,17 * * *'")
                }
            }
        };
        
        w.Should().NotBeNull();
        w.On.Manuals.WorkflowDispatch.Should().BeNull();
        w.On.Manuals.RepositoryDispatch.Should().BeNull();
        w.On.Schedules.Should().HaveCount(2);
        w.On.Webhooks.Should().BeEmpty();
    }

    [Fact]
    public void Workflow_Single_Webhook()
    {
        var w = new Workflow()
        {
            On =
            {
                Webhooks =
                {
                    new CheckRun
                    {
                        Activities =  { CheckRun.Activity.Completed, CheckRun.Activity.RequestedAction }
                    }
                }
            }
        };
        
        w.Should().NotBeNull();
        w.On.Manuals.WorkflowDispatch.Should().BeNull();
        w.On.Manuals.RepositoryDispatch.Should().BeNull();
        w.On.Schedules.Should().BeEmpty();
        w.On.Webhooks.Should().ContainSingle();
    }

    [Fact]
    public void Workflow_Several_webhook()
    {
        var w = new Workflow
        {
            On =
            {
                Webhooks =
                {
                    new CheckRun
                    {
                        Activities =  { CheckRun.Activity.Completed, CheckRun.Activity.RequestedAction }
                    },
                    new PullRequest
                    {
                        Activities = { PullRequest.Activity.Assigned, PullRequest.Activity.Closed }
                    }
                }
            }
        };

        w.Should().NotBeNull();
        w.On.Manuals.WorkflowDispatch.Should().BeNull();
        w.On.Manuals.RepositoryDispatch.Should().BeNull();
        w.On.Schedules.Should().BeEmpty();
        w.On.Webhooks.Should().HaveCount(2);
    }

    [Fact]
    public void Workflow_ReadAll_Permissions()
    {
        var w = new Workflow
        {
            On =
            {
                Webhooks =
                {
                    new PullRequest
                    {
                        Activities = { PullRequest.Activity.Assigned, PullRequest.Activity.Closed }
                    }
                }
            },
            Permissions = new Permissions().All(GitHubPermission.Read)
        };
        
        foreach (var scope in Enum.GetValues<GitHubPermissionScope>())
        {
            w.Permissions.Read.Should().Contain(scope);
        }
    }

    [Fact]
    public void Workflow_WriteAll_Permissions()
    {

        var w = new Workflow
        {
            On =
            {
                Webhooks =
                {
                    new PullRequest
                    {
                        Activities = { PullRequest.Activity.Assigned, PullRequest.Activity.Closed }
                    }
                }
            },
            Permissions = new Permissions().All(GitHubPermission.Write)
        };

        foreach (var scope in Enum.GetValues<GitHubPermissionScope>())
        {
            w.Permissions.Write.Should().Contain(scope);
        }
    }

    [Fact]
    public void Workflow_OneRead_WriteAll_Permissions()
    {

        var w = new Workflow
        {
            On =
            {
                Webhooks =
                {
                    new PullRequest
                    {
                        Activities = { PullRequest.Activity.Assigned, PullRequest.Activity.Closed }
                    }
                }
            },
            Permissions = new Permissions
            {
                Read =
                    {
                        GitHubPermissionScope.Actions,
                    }
            }.All(GitHubPermission.Write)
        };

        w.Permissions.Read.Should().Contain(GitHubPermissionScope.Actions);
        
        foreach (var scope in Enum.GetValues<GitHubPermissionScope>())
        {
            w.Permissions.Write.Should().Contain(scope);
        }
    }

    [Fact]
    public void Workflow_OneWrite_ReadAll_Permissions()
    {
        var w = new Workflow
        {
            On =
            {
                Webhooks =
                {
                    new PullRequest
                    {
                        Activities = { PullRequest.Activity.Assigned, PullRequest.Activity.Closed }
                    }
                }
            },
            Permissions = new Permissions
            {
                Write =
                    {
                        GitHubPermissionScope.Actions,
                    }
            }.All(GitHubPermission.Read)
        };

        w.Permissions.Write.Should().Contain(GitHubPermissionScope.Actions);
        
        foreach (var scope in Enum.GetValues<GitHubPermissionScope>())
        {
            w.Permissions.Read.Should().Contain(scope);
        }
    }

    [Fact]
    public void Workflow_Detailed_Permissions()
    {
        var w = new Workflow
        {
            On =
            {
                Webhooks =
                {
                    new PullRequest
                    {
                        Activities = { PullRequest.Activity.Assigned, PullRequest.Activity.Closed }
                    }
                }
            },
            Permissions =
            {
                Write =
                {
                    GitHubPermissionScope.Actions,
                },
                Read = {
                    GitHubPermissionScope.Actions,
                    GitHubPermissionScope.Contents
                }
            }
        };

        w.Permissions.Write.Should().Contain(GitHubPermissionScope.Actions);
        w.Permissions.Read.Should().Contain(GitHubPermissionScope.Actions);
        w.Permissions.Read.Should().Contain(GitHubPermissionScope.Contents);
    }

    [Fact]
    public void Workflow_No_Environment_Variables()
    {
        var w = new Workflow
        {
            On =
            {
                Webhooks =
                {
                    new PullRequest
                    {
                        Activities = { PullRequest.Activity.Assigned, PullRequest.Activity.Closed }
                    }
                }
            },
        };
        w.Env.Keys.Should().BeEmpty();
    }

    [Fact]
    public void Workflow_Environment_Variables()
    {
        var w = new Workflow
        {
            On =
            {
                Webhooks =
                {
                    new PullRequest
                    {
                        Activities = { PullRequest.Activity.Assigned, PullRequest.Activity.Closed }
                    }
                }
            },
            Env =
            {
                ["Database"] = "production",
                ["Bot"] = "builder"
            }
        };
        w.Env.Keys.Should().NotBeEmpty();
    }

    [Fact]
    public void Workflow_With_Concurrency()
    {
        var w = new Workflow
        {
            On =
            {
                Webhooks =
                {
                    new PullRequest
                    {
                        Activities = { PullRequest.Activity.Assigned, PullRequest.Activity.Closed }
                    }
                }
            },
            Concurrency = new("build", true)
        };
        w.Concurrency.Should().NotBeNull();
    }

    [Fact]
    public void Workflow_Defaults_Defaults()
    {
        var w = new Workflow
        {
            On =
            {
                Webhooks =
                {
                    new PullRequest
                    {
                        Activities = { PullRequest.Activity.Assigned, PullRequest.Activity.Closed }
                    }
                }
            },
        };
        w.Defaults.Run.Shell.Should().Be(Shell.Default);
        w.Defaults.Run.WorkingDirectory.Should().BeNull();
        w.Defaults.Run.CustomShell.Should().BeNull();
    }

    [Fact]
    public void Workflow_Defaults_Shell()
    {
        var w = new Workflow
        {
            On =
            {
                Webhooks =
                {
                    new PullRequest
                    {
                        Activities = { PullRequest.Activity.Assigned, PullRequest.Activity.Closed }
                    }
                }
            },
            Defaults =
            {
                Run =
                {
                    Shell = Shell.Pwsh
                }
            }
        };
        
        w.Defaults.Run.Shell.Should().Be(Shell.Pwsh);
        w.Defaults.Run.WorkingDirectory.Should().BeNull();
        w.Defaults.Run.CustomShell.Should().BeNull();
    }

    [Fact]
    public void Workflow_Defaults_WorkingDir()
    {
        var w = new Workflow
        {
            On =
            {
                Webhooks =
                {
                    new PullRequest
                    {
                        Activities = { PullRequest.Activity.Assigned, PullRequest.Activity.Closed }
                    }
                }
            },
            Defaults =
            {
                Run =
                {
                    WorkingDirectory = "scripts"
                }
            }
        };

        w.Defaults.Run.Shell.Should().Be(Shell.Default);
        w.Defaults.Run.WorkingDirectory.Should().NotBeNull();
        w.Defaults.Run.CustomShell.Should().BeNull();
    }

    [Fact]
    public void Workflow_Defaults_CustomShell()
    {
        var w = new Workflow
        {
            On =
            {
                Webhooks =
                {
                    new PullRequest
                    {
                        Activities = { PullRequest.Activity.Assigned, PullRequest.Activity.Closed }
                    }
                }
            },
            Defaults =
            {
                Run =
                {
                    CustomShell=  "perl {0}"
                }
            }
        };

        w.Defaults.Run.Shell.Should().Be(Shell.Default);
        w.Defaults.Run.WorkingDirectory.Should().BeNull();
        w.Defaults.Run.CustomShell.Should().NotBeNull();
        w.Defaults.Run.CustomShell.Should().Be("perl {0}");
    }

    [Fact]
    public void Workflow_With_Job()
    {
        var w = new Workflow
        {
            On =
            {
                Webhooks =
                {
                    new PullRequest
                    {
                        Activities = { PullRequest.Activity.Assigned, PullRequest.Activity.Closed }
                    }
                }
            },
            Jobs =
            {
                new Job("configuration")
                {
                    Name = "Configure Build",
                    Env =
                    {
                        ["Database"] = "production",
                        ["Bot"] = "builder"
                    }
                },
                new Job("tests")
                {
                    Name = "Run Tests",
                    Env =
                    {
                        ["Database"] = "production",
                        ["Bot"] = "builder"
                    }
                }
            }
        };
        
        w.Jobs.Should().HaveCount(2);
        w.Jobs[0].Id.Should().Be("configuration");
        w.Jobs[1].Id.Should().Be("tests");
    }
}
