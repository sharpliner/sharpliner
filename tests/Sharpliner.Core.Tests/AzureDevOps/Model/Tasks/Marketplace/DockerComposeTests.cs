using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class DockerComposeTests
{
    private readonly DockerComposeTaskBuilder _builder = new();

    private class DockerCompose_Pipeline(Step step) : SimpleTestPipeline
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
        var task = _builder.Build("src/docker-compose.yml") with
        {
            ContainerRegistryType = DockerComposeContainerRegistryType.AzureContainerRegistry,
            AzureSubscription = "azure-rm",
            AzureContainerRegistry = "myacr.azurecr.io",
            AdditionalImageTags = "latest\n$(Build.BuildNumber)",
            IncludeSourceTags = true,
            IncludeLatestTag = true,
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Push_Command_Test()
    {
        var task = _builder.Push("deploy/docker-compose.yml") with
        {
            ContainerRegistryType = DockerComposeContainerRegistryType.ContainerRegistry,
            DockerRegistryEndpoint = "external-registry",
            AdditionalImageTags = "release\n$(Build.SourceVersion)",
            IncludeSourceTags = false,
            IncludeLatestTag = true,
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Run_Command_Test()
    {
        var task = _builder.Run("docker-compose.yml") with
        {
            ProjectName = "integration-tests",
            BuildImages = false,
            Detached = false,
            AbortOnContainerExit = true,
            Arguments = "--remove-orphans",
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Run_Service_Command_Test()
    {
        var task = _builder.RunService("web") with
        {
            DockerComposeFile = "src/docker-compose.yml",
            ContainerName = "web-container",
            Ports = "8080:80\n8443:443",
            WorkingDirectory = "/workspace/app",
            EntryPoint = "/bin/sh",
            ContainerCommand = "-c \"dotnet MyApp.dll\"",
            Detached = false,
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Lock_Command_Test()
    {
        var task = _builder.Lock("docker-compose.yml") with
        {
            RemoveBuildOptions = true,
            BaseResolveDirectory = "src/containers",
            OutputDockerComposeFile = "$(Build.StagingDirectory)/docker-compose.lock.yml",
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Write_Image_Digests_Command_Test()
    {
        var task = _builder.WriteImageDigests("docker-compose.yml") with
        {
            ImageDigestComposeFile = "$(Build.StagingDirectory)/docker-compose.images.yml",
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Combine_Configuration_Command_Test()
    {
        var task = _builder.CombineConfiguration("docker-compose.yml") with
        {
            RemoveBuildOptions = true,
            BaseResolveDirectory = "src/containers",
            OutputDockerComposeFile = "$(Build.StagingDirectory)/docker-compose.combined.yml",
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public Task Command_Command_Test()
    {
        var task = _builder.Command("config") with
        {
            DockerComposeFile = "src/docker-compose.yml",
            DockerComposeFileArgs = "TAG=$(Build.BuildNumber)\nCONFIG=Release",
            Arguments = "--quiet",
            CurrentWorkingDirectory = "/workspace",
            DockerComposePath = "/usr/local/bin/docker-compose",
        };

        return Verify(GetYaml(task));
    }

    [Fact]
    public void Run_Service_Requires_Service_Name_Test()
    {
        Action action = () => _ = _builder.RunService(string.Empty);

        action.Should().Throw<ArgumentException>();
    }

    private static string GetYaml(Step task) => new DockerCompose_Pipeline(task).Serialize();
}
