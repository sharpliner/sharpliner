using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class TaskBuilderTests
{
    private abstract class TestPipeline : SingleStagePipelineDefinition
    {
        public override string TargetFile => "azure-pipelines.yml";

        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;
    }

    private class BashTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("test")
                {
                    Steps =
                    {
                        Bash.FromResourceFile("Sharpliner.Tests.AzureDevOps.Resources.test-script.sh"),
                        Bash.FromResourceFile("test-script.sh"),
                        Bash.Inline("cat /etc/passwd", "rm -rf tests.xml"),
                        Bash.File("foo.sh")
                            .DisplayAs("Test task"),
                        Bash.File("some/script.sh") with
                        {
                            Arguments = "foo bar",
                            ContinueOnError = true,
                            FailOnStderr = true,
                            BashEnv = "~/.bash_profile",
                            DisplayName = "Test task"
                        },
                        Bash.FromFile( "AzureDevops/Resources/test-script.sh"),
                    }
                }
            }
        };
    }

    [Fact]
    public Task Serialize_Bash_Builders_Test()
    {
        BashTaskPipeline pipeline = new();

        return Verify(pipeline.Serialize());
    }

    private class PowershellTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("test")
                {
                    Steps =
                    {
                        Powershell.FromResourceFile("Sharpliner.Tests.AzureDevOps.Resources.Test-Script.ps1"),
                        Powershell.FromResourceFile("Test-Script.ps1"),
                        Powershell.Inline("$Files = Get-ChildItem *.sln", "Remove-Item $Files"),
                        Powershell.File("foo.ps1"),
                        Powershell.FromFile("AzureDevops/Resources/Test-Script.ps1"),
                    }
                }
            }
        };
    }

    [Fact]
    public Task Serialize_Powershell_Builders_Test()
    {
        PowershellTaskPipeline pipeline = new();
        
        return Verify(pipeline.Serialize());
    }

    private class PwshTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
                {
                    new Job("test")
                    {
                        Steps =
                        {
                            Pwsh.FromResourceFile("Sharpliner.Tests.AzureDevOps.Resources.Test-Script.ps1"),
                            Pwsh.FromResourceFile("Test-Script.ps1", "A display name"),
                            Pwsh.Inline("$Files = Get-ChildItem *.sln", "Remove-Item $Files"),
                            Pwsh.File("foo.ps1"),
                            Pwsh.FromFile("AzureDevops/Resources/Test-Script.ps1"),
                        }
                    }
                }
        };
    }

    [Fact]
    public Task Serialize_Pwsh_Builders_Test()
    {
        PwshTaskPipeline pipeline = new();
        
        return Verify(pipeline.Serialize());
    }

    private class ScriptTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("test")
                {
                    Steps =
                    {
                        Script.FromResourceFile("Sharpliner.Tests.AzureDevOps.Resources.test-script"),
                        Script.FromResourceFile("test-script", "A display name"),
                        Script.Inline("echo 'Hello, world!'", "echo 'Goodbye, world!'") with { DisplayName = "A display name" },
                        Script.FromFile("AzureDevOps/Resources/test-script"),
                    }
                }
            }
        };
    }

    [Fact]
    public Task Serialize_Script_Builders_Test()
    {
        ScriptTaskPipeline pipeline = new();
        
        return Verify(pipeline.Serialize());
    }

    private class PublishTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("test")
                {
                    Steps =
                    {
                        Publish.Pipeline("Binary", "bin/Debug/net8.0/") with
                        {
                            DisplayName = "Publish artifact",
                            ContinueOnError = false
                        },

                        Publish.FileShare("additional-binary", "bin/Debug/netstandard2.0/", $"{variables.Build.ArtifactStagingDirectory}/additional-binary") with
                        {
                            Parallel = true
                        },

                        Publish.Pipeline("artifactName", "some/file/path.txt"),
                    }
                }
            }
        };
    }

    [Fact]
    public Task Serialize_Publish_Builder_Test()
    {
        PublishTaskPipeline pipeline = new();
        
        return Verify(pipeline.Serialize());
    }

    private class CheckoutTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("test")
                {
                    Steps =
                    {
                        Checkout.None,
                        Checkout.Self with
                        {
                            Submodules = SubmoduleCheckout.None,
                            Path = "$(Build.SourcesDirectory)/local",
                            PersistCredentials = true,
                            Lfs = true,
                        },
                        Checkout.Self with
                        {
                            DisplayName = "Checkout shallow self",
                            Submodules = SubmoduleCheckout.SingleLevel,
                            Path = "$(Build.SourcesDirectory)/local-shallow",
                        },
                        Checkout.Repository("https://github.com/sharpliner/sharpliner.git") with
                        {
                            Submodules = SubmoduleCheckout.Recursive,
                            Clean = true,
                            FetchDepth = 0,
                            FetchFilter = "tree:0",
                            FetchTags = true,
                        }
                    }
                }
            }
        };
    }

    [Fact]
    public Task Serialize_Checkout_Builder_Test()
    {
        CheckoutTaskPipeline pipeline = new();
        
        return Verify(pipeline.Serialize());
    }

    private class DownloadTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("test")
                {
                    Steps =
                    {
                        Download.None,
                        Download.Current with
                        {
                            Tags =
                            [
                                "release",
                                "nightly",
                            ],
                            Artifact = "Frontend",
                            Patterns =
                            [
                                "frontend/**/*",
                                "frontend.config",
                            ]
                        },
                        Download.SpecificBuild("public", 56, 1745, "MyProject.CLI", patterns: [ "**/*.dll", "**/*.exe" ]) with
                        {
                            AllowPartiallySucceededBuilds = true,
                            RetryDownloadCount = 3,
                            Tags = ["non-release", "preview"],
                        },
                        Download.LatestFromBranch("internal", 23, "refs/heads/develop", path: variables.Build.ArtifactStagingDirectory) with
                        {
                            AllowFailedBuilds = true,
                            CheckDownloadedFiles = true,
                            PreferTriggeringPipeline = true,
                            Artifact = "Another.CLI",
                        }
                    }
                }
            }
        };
    }

    [Fact]
    public Task Serialize_Download_Builder_Test()
    {
        DownloadTaskPipeline pipeline = new();
        
        return Verify(pipeline.Serialize());
    }

    private class TaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("test")
                {
                    Steps =
                    {
                        Task("VSBuild@1", "Build") with
                        {
                            Timeout = TimeSpan.FromHours(2),
                            Inputs = new()
                            {
                                { "solution", "**/*.sln" }
                            },
                            RetryCountOnTaskFailure = 2
                        }
                    }
                }
            }
        };
    }

    [Fact]
    public Task Serialize_Task_Builder_Test()
    {
        TaskPipeline pipeline = new();
        
        return Verify(pipeline.Serialize());
    }

    private class NuGetTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("test")
                {
                    Steps =
                    {
                        NuGet.Authenticate(["MyServiceConnection"], true),
                        NuGet.Restore.FromFeed("my-project/my-project-scoped-feed") with
                        {
                            RestoreSolution = "**/*.sln",
                            IncludeNuGetOrg = false,
                        },
                        NuGet.Restore.FromFeed("my-organization-scoped-feed") with
                        {
                            RestoreSolution = "**/*.sln",
                        },
                        NuGet.Restore.FromNuGetConfig("./nuget.config") with
                        {
                            RestoreSolution = "**/*.sln",
                            ExternalFeedCredentials = "MyExternalFeedCredentials",
                            NoCache = true,
                            ContinueOnError = true
                        },
                        NuGet.Pack.WithoutPackageVersioning with
                        {
                            PackagesToPack = "**/*.csproj",
                            IncludeSymbols = true,
                            ToolPackage = true,
                            BuildProperties = new()
                            {
                                ["prop1"] = "value1",
                                ["prop2"] = "value2"
                            }

                        },
                        NuGet.Pack.ByPrereleaseNumber("3", "1", "4"),
                        NuGet.Pack.ByEnvVar("VERSION"),
                        NuGet.Pack.ByBuildNumber with
                        {
                            PackagesToPack = "**/*.csproj",
                            Configuration = "Release",
                            PackDestination = "artifacts/packages"
                        },
                        NuGet.Push.ToInternalFeed("MyInternalFeed"),
                        NuGet.Push.ToExternalFeed("MyExternalFeedCredentials"),
                        NuGet.Custom(@"config -Set repositoryPath=c:\packages -configfile c:\my.config")
                    }
                }
            }
        };
    }

    [Fact]
    public Task Serialize_NuGet_Builders_Test()
    {
        NuGetTaskPipeline pipeline = new();
        
        return Verify(pipeline.Serialize());
    }
}
