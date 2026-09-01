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
                        Bash.Inline("cat /etc/passwd", "rm -rf tests.xml") with
                        {
                            WorkingDirectory = "src",
                            FailOnStderr = If.Equal("variables['Build.Reason']", "'PullRequest'")
                                .Value(true)
                                .Else
                                .Value(false),
                            Target = "host",
                            RetryCountOnTaskFailure = 2,
                        },
                        Bash.File("foo.sh")
                            .DisplayAs("Test task"),
                        Bash.File("some/script.sh") with
                        {
                            Arguments = "foo bar",
                            ContinueOnError = true,
                            FailOnStderr = true,
                            BashEnv = "~/.bash_profile",
                            DisplayName = "Test task",
                            WorkingDirectory = "src",
                            Target = new StepTarget
                            {
                                Container = "node",
                                Commands = StepTargetCommands.Restricted,
                                SettableVariables = StepTargetSettableVariables.Allowed("sauce"),
                            },
                            RetryCountOnTaskFailure = 3,
                        },
                        Bash.FromFile( "AzureDevOps/Resources/test-script.sh"),
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

    [Fact]
    public void StepTargetSettableVariables_Allowed_Requires_AtLeast_One_Variable()
    {
        Assert.Throws<ArgumentException>(() => StepTargetSettableVariables.Allowed());
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
                        Powershell.FromFile("AzureDevOps/Resources/Test-Script.ps1"),
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
                            Pwsh.FromFile("AzureDevOps/Resources/Test-Script.ps1"),
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
                            Parallel = true,
                            ParallelCount = 16,
                        },

                        Publish.PipelineArtifact("Packages", "artifacts/packages") with
                        {
                            DisplayName = "Publish packages",
                            Properties = """{"user-type":"packages"}""",
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

    private class PublishSymbolsTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("test")
                {
                    Steps =
                    {
                        Publish.Symbols.IndexAndPublish.ToAzureArtifacts("**/bin/**/*.pdb") with
                        {
                            IndexableFileFormats = IndexableFileFormats.SourceMap,
                        },
                        Publish.Symbols.PublishOnly.ToFileShare("**/bin/**/*.pdb", @"\\my-share\symbols") with
                        {
                            CompressSymbols = true,
                        },
                        Publish.Symbols.IndexOnly("**/bin/**/*.pdb"),
                    }
                }
            }
        };
    }

    [Fact]
    public Task Serialize_PublishSymbols_Builder_Test()
    {
        PublishSymbolsTaskPipeline pipeline = new();

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
                        Checkout.Self with
                        {
                            DisplayName = "Checkout sparse self",
                            Path = "$(Build.SourcesDirectory)/local-sparse",
                            SparseCheckoutDirectories = "src/Sharpliner",
                            WorkspaceRepo = true
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
                        Download.SecureFile("ca.pem", retryCount: 5, socketTimeout: 30000) with
                        {
                            Name = "caFile",
                        },
                        Download.Current with
                        {
                            Artifact = "Frontend",
                            Patterns =
                            [
                                "frontend/**/*",
                                "frontend.config",
                            ]
                        },
                        Download.CurrentBuild() with
                        {
                            ArtifactName = "CurrentTaskArtifactAlias",
                            DownloadPath = "$(Pipeline.Workspace)/current-task",
                            ItemPattern = [ "**/*.nupkg" ],
                        },
                        Download.FromPipelineResource("myPipelineResource", "ResourceArtifact", [ "**/*.dll" ]),
                        Download.SpecificBuild("public", 56, 1745, "MyProject.CLI", patterns: [ "**/*.dll", "**/*.exe" ]) with
                        {
                            TargetPath = "$(Pipeline.Workspace)/specific",
                        },
                        Download.Latest("public", 56, "Latest.CLI") with
                        {
                            AllowFailedBuilds = true,
                            AllowPartiallySucceededBuilds = true,
                            PreferTriggeringPipeline = true,
                            Tags = ["non-release", "preview"],
                        },
                        Download.LatestFromBranch("internal", 23, "refs/heads/develop", path: variables.Build.ArtifactStagingDirectory) with
                        {
                            CheckDownloadedFiles = true,
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

    private class AzureCliTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("test")
                {
                    Steps =
                    {
                        AzureCli.File("connectedServiceNameARM", ScriptType.Ps, "foo.ps1"),
                        AzureCli.FromFile("connectedServiceNameARM", ScriptType.Ps, "AzureDevOps/Resources/Test-Script.ps1"),
                        AzureCli.FromResourceFile("connectedServiceNameARM", ScriptType.Ps, "Test-Script.ps1"),
                        AzureCli.FromResourceFile("connectedServiceNameARM", ScriptType.Ps, "Sharpliner.Tests.AzureDevOps.Resources.Test-Script.ps1"),
                        AzureCli.Inline("connectedServiceNameARM", ScriptType.Ps, displayName: null, "Write-Host \"test\"")
                    }
                }
            }
        };
    }

    [Fact]
    public Task Serialize_AzureCli_Builder_Test()
    {
        AzureCliTaskPipeline pipeline = new();

        return Verify(pipeline.Serialize());
    }

    private class AdvancedSecurityTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("test")
                {
                    Steps =
                    {
                        AdvancedSecurity.Codeql.Init(CodeqlLanguage.CSharp),
                        AdvancedSecurity.Codeql.InitWithoutBuild(CodeqlLanguage.Cpp, CodeqlLanguage.Python),
                        AdvancedSecurity.Codeql.InitWithAutomaticInstall([CodeqlLanguage.Java], cleanupOldAutomaticInstalls: true)
                    }
                }
            }
        };
    }

    [Fact]
    public Task Serialize_AdvancedSecurity_Builder_Test()
    {
        AdvancedSecurityTaskPipeline pipeline = new();

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
                        NuGet.Authenticate(),
                        NuGet.Authenticate([" MyServiceConnection ", "", " AnotherServiceConnection "], true),
                        NuGet.Authenticate("MyAzureDevOpsServiceConnection", "https://pkgs.dev.azure.com/my-org/my-project/_packaging/my-feed/nuget/v3/index.json"),
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
                            DisableParallelProcessing = true,
                            RestoreDirectory = "packages",
                            VerbosityRestore = NuGetVerbosity.Normal,
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
                        NuGet.Pack.ByPrereleaseNumber("3", "1", "4") with
                        {
                            PackTimezone = PackTimezoneType.Local,
                        },
                        NuGet.Pack.ByEnvVar("VERSION"),
                        NuGet.Pack.ByBuildNumber with
                        {
                            PackagesToPack = "**/*.csproj",
                            Configuration = "Release",
                            PackDestination = "artifacts/packages",
                            BasePath = "src",
                            VerbosityPack = PackVerbosity.Quiet,
                        },
                        NuGet.Push.ToInternalFeed("MyInternalFeed") with
                        {
                            PackagesToPush = ["$(Build.ArtifactStagingDirectory)/**/*.nupkg", "!$(Build.ArtifactStagingDirectory)/**/*.symbols.nupkg"],
                            PublishPackageMetadata = false,
                            AllowPackageConflicts = true,
                            RequestTimeout = 300,
                            VerbosityPush = NuGetVerbosity.Quiet,
                        },
                        NuGet.Push.ToExternalFeed("MyExternalFeedCredentials") with
                        {
                            RequestTimeout = 120,
                            VerbosityPush = NuGetVerbosity.Normal,
                        },
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

    private class KubernetesManifestTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("test")
                {
                    Steps =
                    {
                        KubernetesManifest.Deploy.WithKubernetesServiceConnection("aks-connection", "k8s/deployment.yml\nk8s/service.yml") with
                        {
                            Namespace = "production",
                            Containers = "sample/web:2.0.0",
                            ImagePullSecrets = "acr-secret",
                        },
                        KubernetesManifest.Bake.Helm("charts/web") with
                        {
                            ReleaseName = "webapp",
                            Overrides = "image.tag=2.0.0",
                        },
                        KubernetesManifest.Patch.NamedWithKubernetesServiceConnection("aks-connection", KubernetesManifestKind.Deployment, "webapp", "{\"spec\":{\"replicas\":3}}") with
                        {
                            MergeStrategy = KubernetesManifestMergeStrategy.Strategic,
                        },
                        KubernetesManifest.Scale.WithKubernetesServiceConnection("aks-connection", KubernetesManifestKind.Deployment, "webapp", "3"),
                        KubernetesManifest.CreateSecret.DockerRegistryWithKubernetesServiceConnection("aks-connection", "acr-secret", "my-acr-service-connection"),
                    }
                }
            }
        };
    }

    [Fact]
    public Task Serialize_KubernetesManifest_Builders_Test()
    {
        KubernetesManifestTaskPipeline pipeline = new();

        return Verify(pipeline.Serialize());
    }

    private class NpmTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("test")
                {
                    Steps =
                    {
                        Npm.Authenticate(".npmrc"),
                        Npm.Authenticate("packages/mypackage/.npmrc", ["MyServiceConnection", "AnotherServiceConnection"]),
                        Npm.Authenticate(".npmrc", "MyAzureDevOpsServiceConnection", "https://pkgs.dev.azure.com/my-org/my-project/_packaging/my-feed/npm/registry/"),
                        Npm.Authenticate("empty/.npmrc", []),
                        Npm.Authenticate("whitespace/.npmrc", [" MyServiceConnection ", "", " AnotherServiceConnection "]),
                        new NpmAuthenticateTask("null/.npmrc") with { CustomEndpoints = null },
                        Npm.Install([" MyServiceConnection ", "", " AnotherServiceConnection "]) with
                        {
                            WorkingDirectory = "src/web",
                            Verbose = true,
                        },
                        Npm.InstallFromFeed("MyProject/MyFeed"),
                        Npm.Ci() with
                        {
                            WorkingDirectory = "src/web",
                        },
                        Npm.CiFromFeed("MyProject/MyFeed") with
                        {
                            Verbose = false,
                        },
                        Npm.Custom("dist-tag ls mypackage", ["ExternalNpmRegistry"]) with
                        {
                            WorkingDirectory = "src/web",
                        },
                        Npm.CustomFromFeed("dist-tag ls mypackage", "MyProject/MyFeed"),
                        Npm.PublishToExternalRegistry("MyExternalPublishServiceConnection") with
                        {
                            WorkingDirectory = "src/web",
                            Verbose = true,
                        },
                        Npm.PublishToFeed("MyProject/MyFeed") with
                        {
                            PublishPackageMetadata = false,
                        },
                    }
                }
            }
        };
    }

    [Fact]
    public Task Serialize_Npm_Builders_Test()
    {
        NpmTaskPipeline pipeline = new();

        return Verify(pipeline.Serialize());
    }

    private class VSBuildTaskPipeline : TestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("test")
                {
                    Steps =
                    {
                        VSBuild.Solution("src/MyApp.sln")
                            .PlatformAndConfiguration("Any CPU", "Release")
                            .WebPackage(@"$(Build.ArtifactStagingDirectory)\MyApp.zip", skipInvalidConfigurations: true)
                            .Build() with
                        {
                            VsVersion = VSBuildVisualStudioVersion.VisualStudio2022,
                            MaximumCpuCount = true,
                        }
                    }
                }
            }
        };
    }

    [Fact]
    public Task Serialize_VSBuild_Builder_Test()
    {
        VSBuildTaskPipeline pipeline = new();

        return Verify(pipeline.Serialize());
    }
}
