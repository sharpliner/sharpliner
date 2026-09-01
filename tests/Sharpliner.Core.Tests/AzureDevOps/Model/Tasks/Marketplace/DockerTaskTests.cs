using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class DockerTaskTests
{
    private readonly DockerTaskBuilder _builder = new();

    [Fact]
    public Task Build_Command_Test()
    {
        var task = _builder.Build(
            "Dockerfile",
            repository: "contoso/my-app",
            tags: ["$(Build.BuildId)", "latest"],
            buildContext: ".",
            arguments: "--build-arg HTTP_PROXY=http://10.20.30.2:1234 --quiet") with
        {
            ContainerRegistry = "my-registry-service-connection",
            AddPipelineData = false,
            AddBaseImageData = false,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Build_Command_With_Defaults_Test()
    {
        var task = _builder.Build("Dockerfile");

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Push_Command_Test()
    {
        var task = _builder.Push(
            "contoso/my-app",
            tags: ["$(Build.BuildId)", "latest"],
            arguments: "--disable-content-trust") with
        {
            ContainerRegistry = "my-registry-service-connection",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task BuildAndPush_Command_Test()
    {
        var task = _builder.BuildAndPush(
            "Dockerfile",
            "contoso/my-app",
            tags: ["$(Build.BuildId)"],
            buildContext: ".") with
        {
            ContainerRegistry = "my-registry-service-connection",
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Login_Command_Test()
    {
        var task = _builder.Login("my-registry-service-connection");

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Logout_Command_Test()
    {
        var task = _builder.Logout("my-registry-service-connection");

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Logout_Command_Without_Registry_Test()
    {
        var task = _builder.Logout();

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Start_Command_Test()
    {
        var task = _builder.Start("my-container", "-e ASPNETCORE_ENVIRONMENT=Development");

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Stop_Command_Test()
    {
        var task = _builder.Stop("my-container");

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Push_Single_Tag_Command_Test()
    {
        var task = _builder.Push("contoso/my-app", tags: ["$(Build.BuildId)"]);

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
