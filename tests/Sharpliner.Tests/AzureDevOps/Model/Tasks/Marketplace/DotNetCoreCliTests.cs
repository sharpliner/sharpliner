using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class DotNetCoreCliTests
{
    private readonly DotNetTaskBuilder _builder = new();

    private class DotNet_Pipeline(Step step) : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("job")
                {
                    Steps = { step }
                }
            }
        };
    }

    [Fact]
    public Task Build_Command_Test()
    {
        var task = _builder.Build("project.csproj", true, "-c Release") with
        {
            WorkingDirectory = "/tmp"
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Install_Sdk_Command_Test()
    {
        var task = _builder.Install.Sdk("6.0.100-rc.2.21505.57", true) with
        {
            WorkingDirectory = "/tmp",
            InstallationPath = "/.dotnet",
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Install_Runtime_Command_Test()
    {
        var task = _builder.Install.Runtime("5.0.100") with
        {
            PerformMultiLevelLookup = true,
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Install_GlobalJson_Command_Test()
    {
        var task = _builder.Install.FromGlobalJson("/foo/global.json") with
        {
            WorkingDirectory = "/tmp",
            InstallationPath = "/.dotnet",
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Pack_Command_Test()
    {
        var task = _builder.Pack("src/*.csproj", "-c Release") with
        {
            NoBuild = true,
            ConfigurationToPack = "Release",
            IncludeSource = true,
            IncludeSymbols = true,
            OutputDir = "/tmp/staging/",
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Publish_Command_Test()
    {
        var task = _builder.Publish("src/*.csproj", true, "-c Release") with
        {
            ModifyOutputPath = true,
            ZipAfterPublish = true,
            Timeout = TimeSpan.FromMinutes(30),
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Push_Command_Test()
    {
        var task = _builder.Push(arguments: "-c Release") with
        {
            PublishPackageMetadata = true,
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Test_Command_Test()
    {
        var task = _builder.Test("*.sln", "/p:CollectCoverage=true /p:CoverletOutputFormat=cobertura") with
        {
            TestRunTitle = "main-test-results"
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Restore_Projects_Command_Test()
    {
        var task = _builder.Restore.Projects("src/*.csproj") with
        {
            NoCache = true,
            IncludeNuGetOrg = true,
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Restore_Projects_With_Config_Command_Test()
    {
        var task = _builder.Restore.Projects("src/*.csproj") with
        {
            NoCache = true,
            IncludeNuGetOrg = true,
            NuGetConfigPath = "src/nuget.config",
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Restore_FromFeed_Command_Test()
    {
        var task = _builder.Restore.FromFeed("dotnet-7-preview-feed", includeNuGetOrg: false) with
        {
            ExternalFeedCredentials = "feeds/dotnet-7",
            NoCache = true,
            RestoreDirectory = ".packages",
            VerbosityRestore = BuildVerbosity.Minimal
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public void Restore_FromFeed_WithNugetConfig_Command_Test()
    {
        Action action = () =>
        {
            _ = _builder.Restore.FromFeed("dotnet-7-preview-feed", includeNuGetOrg: false) with
            {
                ExternalFeedCredentials = "feeds/dotnet-7",
                NoCache = true,
                RestoreDirectory = ".packages",
                NuGetConfigPath = "this should cause an exception",
            };
        };

        action.Should().Throw<Exception>();
    }

    [Fact]
    public Task Restore_FromConfig_Command_Test()
    {
        var task = _builder.Restore.FromNuGetConfig("src/NuGet.config") with
        {
            Arguments = "foo"
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Run_Command_Test()
    {
        var task = _builder.Run with
        {
            Projects = "src/Component/Component.csproj",
            Arguments = "FailIfChanged=true"
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Custom_Command_Test()
    {
        var task = _builder.CustomCommand("--list-sdks") with
        {
            ContinueOnError = true,
        };

        return Verify(GetYaml(task));
    }

    private static string GetYaml(Step task) => new DotNet_Pipeline(task).Serialize();
}
