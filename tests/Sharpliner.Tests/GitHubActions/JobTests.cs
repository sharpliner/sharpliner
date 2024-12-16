using FluentAssertions;
using Sharpliner.GitHubActions;

namespace Sharpliner.Tests.GitHubActions;

public class JobTests
{

    [Fact]
    public void Job_Null_Id()
    {
        // test in case users do not have nullable enabled
        Assert.Throws<ArgumentNullException>(() => new Job(null!));
    }

    [Fact]
    public void Job_Empty_Id()
    {
        Assert.Throws<ArgumentNullException>(() => new Job(string.Empty));
    }

    [Fact]
    public void Job_Defaults()
    {
        var j = new Job("concurrency")
        {
            Defaults =
            {
                Run =
                {
                    Shell = Shell.Pwsh
                }
            }
        };

        j.Defaults.Run.Shell.Should().Be(Shell.Pwsh);
        j.Defaults.Run.WorkingDirectory.Should().BeNull();
        j.Defaults.Run.CustomShell.Should().BeNull();
    }

    [Fact]
    public void Job_Defaults_Empty()
    {
        var j = new Job("concurrency");
        j.Defaults.Run.WorkingDirectory.Should().BeNull();
        j.Defaults.Run.CustomShell.Should().BeNull();
    }

    [Fact]
    public void Job_Container_No_Creds()
    {
        var j = new Job("container")
        {
            RunsOn = new("node:14.16")
            {
                Env =
                {
                    ["Database"] = "production",
                    ["Bot"] = "builder"
                },
                Ports = { 495, 500, 43 },
                Volumes = { "my_docker_volume:/volume_mount", "/data/my_data" }
            }
        };

        j.RunsOn.Image.Should().Be("node:14.16");
        j.RunsOn.Credentials.Should().BeNull();
        j.RunsOn.Ports.Should().Contain(43);
        j.RunsOn.Volumes.Should().Contain("/data/my_data");
    }

    [Fact]
    public void Job_Container_With_Creds_Construtor()
    {
        var j = new Job("container")
        {
            RunsOn = new("node:14.16", "mandel", "1234")
            {
                Env =
                {
                    ["Database"] = "production",
                    ["Bot"] = "builder"
                },
                Ports = { 495, 500, 43 },
                Volumes = { "my_docker_volume:/volume_mount", "/data/my_data" }
            }
        };
        j.RunsOn.Image.Should().Be("node:14.16");
        j.RunsOn.Credentials!.Username.Should().Be("mandel");
        j.RunsOn.Credentials.Password.Should().Be("1234");
        j.RunsOn.Ports.Should().Contain(43);
        j.RunsOn.Volumes.Should().Contain("/data/my_data");
    }

    [Fact]
    public void Job_Container_With_Creds()
    {
        var j = new Job("container")
        {
            RunsOn = new("node:14.16")
            {
                Credentials = new()
                {
                    Username = "mandel",
                    Password = "1234"
                },
                Env =
                {
                    ["Database"] = "production",
                    ["Bot"] = "builder"
                },
                Ports = { 495, 500, 43 },
                Volumes = { "my_docker_volume:/volume_mount", "/data/my_data" }
            }
        };

        j.RunsOn.Image.Should().Be("node:14.16");
        j.RunsOn.Credentials.Username.Should().Be("mandel");
        j.RunsOn.Credentials.Password.Should().Be("1234");
        j.RunsOn.Ports.Should().Contain(43);
        j.RunsOn.Volumes.Should().Contain("/data/my_data");
    }

    [Fact]
    public void Job_Services_With_Containers()
    {
        var j = new Job("services")
        {
            Services =
            {
                new ("service_with_container_image")
                {
                    Container = new ("nginx")
                },
                new ("service_with_container_ports_and_volumes")
                {
                    Container = new ("redis")
                    {
                        Ports = {495, 500, 43},
                        Volumes = {"my_docker_volume:/volume_mount", "/data/my_data"}
                    }
                },
                new ("service_with_container_creds")
                {
                    Container = new ("node:14.16")
                    {
                        Credentials = new ()
                        {
                            Username = "hope",
                            Password = "1234"
                        },
                        Ports = {495, 500, 43},
                        Volumes = {"my_docker_volume:/volume_mount", "/data/my_data"}
                    }
                }
            }
        };

        j.Services[0].Container!.Image.Should().Be("nginx");
        j.Services[1].Container!.Ports.Should().Contain(495);
        j.Services[1].Container!.Volumes.Should().Contain("my_docker_volume:/volume_mount");
        j.Services[2].Container!.Credentials!.Username.Should().Be("hope");
        j.Services[2].Container!.Credentials!.Password.Should().Be("1234");
    }

    [Fact]
    public void Job_Default_Matrix()
    {
        var j = new Job("matrix")
        {
            Strategy = new()
            {
                Configuration = new()
                {
                    { "Foo", new() { 1, 2, 3 } },
                    { "Bar", new() { "ubuntu", "windows" } },
                }
            }
        };

        // validate that we do have the values and can access them
        j.Strategy.Should().NotBeNull();
        j.Strategy.Configuration.Should().ContainKey("Foo");
        j.Strategy.Configuration.Should().ContainKey("Bar");
        j.Strategy.FailFast.Should().BeTrue();
        j.Strategy.MaxParallel.Should().Be(int.MaxValue);
    }

    [Fact]
    public void Job_Matrix_Fast_Fail()
    {
        var j = new Job("matrix")
        {
            Strategy = new()
            {
                Configuration = new()
                {
                    { "Foo", new() { 1, 2, 3 } },
                    { "Bar", new() { "ubuntu", "windows" } },
                },
                FailFast = false,
            }
        };
        Assert.False(j.Strategy.FailFast);
    }

    [Fact]
    public void Job_Matrix_Max_Parallel()
    {
        var j = new Job("matrix")
        {
            Strategy = new()
            {
                Configuration = new()
                {
                    { "Foo", new() { 1, 2, 3 } },
                    { "Bar", new() { "ubuntu", "windows" } },
                },
                MaxParallel = 2,
            },
        };
        
        j.Strategy.MaxParallel.Should().Be(2);
    }

    [Fact]
    public void Job_Matrix_Include_Vars()
    {
        var j = new Job("matrix")
        {
            Strategy = new()
            {
                Configuration = new()
                {
                    { "Fo", new() { 1, 2, 3 } },
                    { "Br", new() { "ubuntu", "windows" } },
                },
                Include =
                    {
                        new ()
                        {
                            Configuration = new ()
                            {
                                { "Foo", 4 },
                                { "Bar", "macOS" },
                            },
                            Variables = new ()
                            {
                                { "ENV",  "DEBUG" }
                            }
                        }
                    }
            },
        };

        j.Strategy.Include[0].Configuration.Should().ContainKey("Foo");
        j.Strategy.Include[0].Configuration.Should().ContainKey("Bar");
        j.Strategy.Include[0].Variables.Should().ContainKey("ENV");
    }

    [Fact]
    public void Job_Matrix_Exclude_Vars()
    {
        var j = new Job("matrix")
        {
            Strategy = new()
            {
                Configuration = new()
                {
                    { "Foo", new() { 1, 2, 3 } },
                    { "Bar", new() { "ubuntu", "windows" } },
                },
                Exclude =
                    {
                        new ()
                        {
                            Configuration = new()
                            {
                                { "Foo", 4 },
                                { "Bar", "macOS" },
                            },
                            Variables = new()
                            {
                                { "ENV", "DEBUG" }
                            }
                        }
                    }
            },
        };

        j.Strategy.Exclude[0].Configuration.Should().ContainKey("Foo");
        j.Strategy.Exclude[0].Configuration.Should().ContainKey("Bar");
        j.Strategy.Exclude[0].Variables.Should().ContainKey("ENV");
    }
}
