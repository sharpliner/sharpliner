using System;
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

    }
}
