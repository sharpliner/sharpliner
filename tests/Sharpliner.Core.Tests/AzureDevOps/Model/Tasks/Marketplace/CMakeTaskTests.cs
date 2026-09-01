using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Expressions;
using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class CMakeTaskTests
{
    [Fact]
    public Task Serialize_Task_With_Defaults_Test()
    {
        var task = new CMakeTask();

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public void Defaults_Match_Task_Specification_Test()
    {
        var task = new CMakeTask();

        Assert.Equal((AdoExpression<string>)"build", task.WorkingDirectory);
        Assert.Equal((AdoExpression<string>)string.Empty, task.Arguments);
        Assert.Equal((AdoExpression<bool>)false, task.RunInsideShell);
        Assert.Empty(task.Inputs);
    }

    [Fact]
    public Task Serialize_Task_With_All_Inputs_Test()
    {
        var task = new CMakeTask
        {
            WorkingDirectory = "$(Build.ArtifactStagingDirectory)/cmake-build",
            Arguments = "-S $(Build.SourcesDirectory) -G Ninja -DCMAKE_BUILD_TYPE=Release",
            RunInsideShell = true,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Task_With_Expressions_Test()
    {
        var task = new CMakeTask
        {
            WorkingDirectory = new ParameterReference("buildDirectory"),
            Arguments = new VariableReference("cmakeArguments"),
            RunInsideShell = new ParameterReference("runInsideShell"),
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
