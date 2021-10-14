using System;
using System.Linq;
using Sharpliner.GitHubActions;
using Xunit;

using static Xunit.Assert;

namespace Sharpliner.Tests.GitHub
{
    public class JobTests
    {

        [Fact]
        public void Job_Null_Id()
        {
            // test in case users do not have nullable enabled
            Throws<ArgumentNullException>(() => new Job(null!));
        }

        [Fact]
        public void Job_Empty_Id()
        {
            Throws<ArgumentNullException>(() => new Job(string.Empty));
        }

        [Fact]
        public void Job_Valid_Id()
        {
            var j = new Job("configure");
            Equal("configure", j.Id);
        }

        [Fact]
        public void Job_Enviroment()
        {
            var j = new Job("configure") {Environment = new("Name")};
            Equal("Name", j.Environment.Name);
            Null(j.Environment.Url);
        }

        [Fact]
        public void Job_Concurrency()
        {
            var j = new Job("concurrency")
            {
                Concurrency = new("build", true)
            };

            NotNull(j.Concurrency);
        }

        [Fact]
        public void Job_Outputs()
        {
            var j = new Job("concurrency")
            {
                Outputs =
                {
                    ["name"] = "expression",
                    ["second"] = "expression"
                }
            };
            NotEmpty(j.Outputs.Keys);
        }

        [Fact]
        public void Job_No_Outputs()
        {

            var j = new Job("concurrency");

            Empty(j.Outputs.Keys);
        }

        [Fact]
        public void Job_Env_Empty()
        {
            var j = new Job("concurrency");

            Empty(j.Env.Keys);
        }

        [Fact]
        public void Job_Env()
        {
            var j = new Job("concurrency")
            {
                Env =
                {
                    ["Database"] = "production",
                    ["Bot"] = "builder"
                }
            };

            NotEmpty(j.Env.Keys);
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

            Equal(Shell.Pwsh, j.Defaults.Run.Shell);
            Null(j.Defaults.Run.WorkingDirectory);
            Null(j.Defaults.Run.CustomShell);
        }

        [Fact]
        public void Job_Defaults_Empty()
        {
            var j = new Job("concurrency");
            Null(j.Defaults.Run.WorkingDirectory);
            Null(j.Defaults.Run.CustomShell);
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

            Equal("node:14.16", j.RunsOn.Image);
            Null(j.RunsOn.Credentials);
            Contains(43, j.RunsOn.Ports);
            Contains("/data/my_data", j.RunsOn.Volumes);
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
            Equal("node:14.16", j.RunsOn.Image);
            Equal("mandel", j.RunsOn.Credentials?.Username);
            Equal("1234", j.RunsOn.Credentials?.Password);
            Contains(43, j.RunsOn.Ports);
            Contains("/data/my_data", j.RunsOn.Volumes);
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

            Equal("node:14.16", j.RunsOn.Image);
            Equal("mandel", j.RunsOn.Credentials.Username);
            Equal("1234", j.RunsOn.Credentials.Password);
            Contains(43, j.RunsOn.Ports);
            Contains("/data/my_data", j.RunsOn.Volumes);
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

            Assert.Equal("nginx", j.Services[0].Container.Image);

            Assert.Contains(495, j.Services[1].Container.Ports);
            Assert.Contains("my_docker_volume:/volume_mount", j.Services[1].Container.Volumes);

            Assert.Equal("hope", j.Services[2].Container.Credentials?.Username);
            Assert.Equal("1234", j.Services[2].Container.Credentials?.Password);
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
            NotNull(j.Strategy);
            Contains("Foo", j.Strategy.Configuration.Keys);
            Contains("Bar", j.Strategy.Configuration.Keys);
            True(j.Strategy.FailFast);
            Equal(int.MaxValue, j.Strategy.MaxParallel);
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
            False(j.Strategy.FailFast);
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
            Equal(2, j.Strategy.MaxParallel);
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

            True(j.Strategy.Include[0].Configuration?.ContainsKey("Foo"));
            True(j.Strategy.Include[0].Configuration?.ContainsKey("Bar"));
            True(j.Strategy.Include[0].Variables?.ContainsKey("ENV"));
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

            Contains("Foo", j.Strategy?.Exclude[0].Configuration?.Keys);
            Contains("Bar", j.Strategy?.Exclude[0].Configuration?.Keys);
            Contains("ENV", j.Strategy?.Exclude[0].Variables?.Keys);
        }

        [Fact]
        public void Job_With_Simple_Step()
        {
            var j = new Job("Steps")
            {
                Steps =
                {
                    new InlineStep ("bash")
                    {
                       Name= "Bash example",
                       Contents = {
                           "rm *.js",
                           "npm ci",
                           "npm run build",
                           "echo $TEST"
                       },
                       WorkingDirectory = "~/test",
                       Shell = Shell.Bash,
                       ContinueOnError = true,
                       TimeoutMinutes = null,
                       Variables = new () {
                           {"TEST", "Manuel"},
                       },
                    },
                    new InlineStep("pwsh") {
                        Name = "Pwsh example",
                        Contents = {"echo ${env:PATH}"},
                        WorkingDirectory = "~/pwsh",
                        Shell = Shell.Pwsh,
                        ContinueOnError = false,
                        TimeoutMinutes = 800,
                    },
                    new InlineStep("python")
                    {
                        Name ="Python example",
                        Contents =
                        {
                            "import os",
                            "print(os.environ['PATH'])",
                        },
                        WorkingDirectory = "~/python",
                        Shell = Shell.Python,
                        CustomShell = null,
                        ContinueOnError = false,
                        TimeoutMinutes = 100,
                        Variables = null
                    }
                }
            };
            Equal(3, j.Steps.Count);
            Equal("bash", j.Steps[0].Id);
            Equal(Shell.Bash, j.Steps[0].Shell);
            Equal("pwsh", j.Steps[1].Id);
            Equal(Shell.Pwsh, j.Steps[1].Shell);
            Equal("python", j.Steps[2].Id);
            Equal(Shell.Python, j.Steps[2].Shell);
        }

        [Fact]
        public void Job_Wit_Custom_Shell()
        {
            var j = new Job("Steps")
            {
                Steps =
                {
                    new InlineStep ("perl")
                    {
                       Name= "Perl example",
                       Contents = {
                           "print %ENV"
                       },
                       WorkingDirectory = "~/test",
                       Shell = Shell.Custom,
                       CustomShell = "perl {0}",
                       ContinueOnError = true,
                       TimeoutMinutes = null,
                       Variables = new () {
                           {"TEST", "Manuel"},
                       },
                    },
                }
            };

            Single(j.Steps);
            Equal("perl", j.Steps[0].Id);
            Equal(Shell.Custom, j.Steps[0].Shell);
            Equal("perln  {0}", j.Steps[0].CustomShell);
        }
    }
}
