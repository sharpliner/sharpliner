using System;
using System.Linq;
using Sharpliner.GitHubActions;
using Xunit;

namespace Sharpliner.Tests.GitHub
{
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
        public void Job_Valid_Id()
        {
            var j = new Job("configure");
            Assert.Equal("configure", j.Id);
        }

        [Fact]
        public void Job_Enviroment()
        {
            var j = new Job("configure") {Environment = new("Name")};
            Assert.Equal("Name", j.Environment.Name);
            Assert.Null(j.Environment.Url);
        }

        [Fact]
        public void Job_Concurrency()
        {
            var j = new Job("concurrency")
            {
                Concurrency = new ("build", true)
            };

            Assert.NotNull(j.Concurrency);
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
            Assert.NotEmpty(j.Outputs.Keys);
        }

        [Fact]
        public void Job_No_Outputs()
        {

            var j = new Job("concurrency");

            Assert.Empty(j.Outputs.Keys);
        }

        [Fact]
        public void Job_Env_Empty()
        {
            var j = new Job("concurrency");

            Assert.Empty(j.Env.Keys);
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

            Assert.NotEmpty(j.Env.Keys);
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

            Assert.Equal(Shell.Pwsh, j.Defaults.Run.Shell);
            Assert.Null(j.Defaults.Run.WorkingDirectory);
            Assert.Null(j.Defaults.Run.CustomShell);
        }

        [Fact]
        public void Job_Defaults_Empty()
        {
            var j = new Job("concurrency");
            Assert.Null(j.Defaults.Run.WorkingDirectory);
            Assert.Null(j.Defaults.Run.CustomShell);
        }

        [Fact]
        public void Job_Container_No_Creds()
        {
            var j = new Job("container")
            {
                RunsOn = new ("node:14.16")
                {
                    Env =
                    {
                        ["Database"] = "production",
                        ["Bot"] = "builder"
                    },
                    Ports = {495, 500, 43},
                    Volumes = {"my_docker_volume:/volume_mount", "/data/my_data"}
                }
            };

            Assert.Equal("node:14.16", j.RunsOn.Image);
            Assert.Null(j.RunsOn.Credentials);
            Assert.Contains(43, j.RunsOn.Ports);
            Assert.Contains("/data/my_data", j.RunsOn.Volumes);
        }

        [Fact]
        public void Job_Container_With_Creds_Construtor()
        {
            var j = new Job("container")
            {
                RunsOn = new ("node:14.16", "mandel", "1234")
                {
                    Env =
                    {
                        ["Database"] = "production",
                        ["Bot"] = "builder"
                    },
                    Ports = {495, 500, 43},
                    Volumes = {"my_docker_volume:/volume_mount", "/data/my_data"}
                }
            };
            Assert.Equal("node:14.16", j.RunsOn.Image);
            Assert.Equal("mandel", j.RunsOn?.Credentials?.Username);
            Assert.Equal("1234", j.RunsOn?.Credentials?.Password);
            Assert.Contains(43, j.RunsOn?.Ports);
            Assert.Contains("/data/my_data", j.RunsOn?.Volumes);
        }


        [Fact]
        public void Job_Container_Wit_Creds()
        {
            var j = new Job("container")
            {
                RunsOn = new ("node:14.16")
                {
                    Credentials = new ()
                    {
                        Username = "mandel",
                        Password = "1234"
                    },
                    Env =
                    {
                        ["Database"] = "production",
                        ["Bot"] = "builder"
                    },
                    Ports = {495, 500, 43},
                    Volumes = {"my_docker_volume:/volume_mount", "/data/my_data"}
                }
            };

            Assert.Equal("node:14.16", j.RunsOn.Image);
            Assert.Equal("mandel", j.RunsOn.Credentials.Username);
            Assert.Equal("1234", j.RunsOn.Credentials.Password);
            Assert.Contains(43, j.RunsOn.Ports);
            Assert.Contains("/data/my_data", j.RunsOn.Volumes);
        }

        [Fact]
        public void Job_Default_Matrix()
        {
            var j = new Job("matrix")
            {
                Strategy = new ()
                {
                    Configuration = new ()
                    {
                        {"Foo", new () {1,2,3}},
                        {"Bar", new (){"ubuntu", "windows"}},
                    }
                }
            };

            // validate that we do have the values and can access them
            Assert.NotNull(j.Strategy);
            Assert.True(j.Strategy.Configuration?.Keys.Contains("Foo"));
            Assert.True(j.Strategy.Configuration?.Keys.Contains("Bar"));
            Assert.True(j.Strategy.FailFast);
            Assert.Equal(int.MaxValue, j.Strategy.MaxParallel);
        }

        [Fact]
        public void Job_Matrix_Fast_Fail ()
        {
            var j = new Job("matrix")
            {
                Strategy = new()
                {
                    Configuration = new ()
                    {
                        {"Foo", new(){1,2,3}},
                        {"Bar", new(){"ubuntu", "windows" }},
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
                Strategy = new ()
                {
                    Configuration = new ()
                    {
                        {"Foo", new (){1,2,3}},
                        {"Bar", new (){"ubuntu", "windows" }},
                    },
                    MaxParallel = 2,
                },
            };
            Assert.Equal(2, j.Strategy.MaxParallel);
        }

        [Fact]
        public void Job_Matrix_Include_Vars()
        {
            var j = new Job("matrix")
            {
                Strategy = new ()
                {
                    Configuration = new ()
                    {
                        {"Fo" , new () {1,2,3}},
                        {"Br" , new () {"ubuntu", "windows" }},
                    },
                    Include =
                    {
                        new ()
                        {
                            Configuration = new ()
                            {
                                {"Foo", 4},
                                {"Bar", "macOS"},
                            },
                            Variables = new ()
                            {
                                {"ENV",  "DEBUG"}
                            }
                        }
                    }
                },
            };

            Assert.True(j.Strategy.Include[0].Configuration?.ContainsKey("Foo"));
            Assert.True(j.Strategy.Include[0].Configuration?.ContainsKey("Bar"));
            Assert.True(j.Strategy.Include[0].Variables?.ContainsKey("ENV"));
        }

        [Fact]
        public void Job_Matrix_Exclude_Vars()
        {
            var j = new Job("matrix")
            {
                Strategy = new ()
                {
                    Configuration = new()
                    {
                        {"Foo", new (){1,2,3}},
                        {"Bar", new (){"ubuntu", "windows" }},
                    },
                    Exclude =
                    {
                        new ()
                        {
                            Configuration = new()
                            {
                                {"Foo", 4},
                                {"Bar", "macOS"},
                            },
                            Variables = new()
                            {
                                {"ENV", "DEBUG"}
                            }
                        }
                    }
                },
            };

            Assert.True(j.Strategy.Exclude[0].Configuration?.Keys.Contains("Foo"));
            Assert.True(j.Strategy.Exclude[0].Configuration?.Keys.Contains("Bar"));
            Assert.True(j.Strategy.Exclude[0].Variables?.Keys.Contains("ENV"));
        }
    }
}
