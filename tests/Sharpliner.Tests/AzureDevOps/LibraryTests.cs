using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.Tests.AzureDevOps;

public class LibraryTests
{
    private class Job_Library : JobLibrary
    {
        public override List<AdoExpression<JobBase>> Jobs =>
        [
            new Job("Start")
            {
                Steps =
                {
                    Script.Inline("echo 'Hello World'")
                }
            },

            new Job("End")
            {
                Steps = new[]
                {
                    Script.Inline("echo 'Goodbye World'")
                }
            }
        ];
    }

    private class JobReferencingPipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("Init"),

                JobLibrary<Job_Library>()
            }
        };
    }

    [Fact]
    public Task Job_Library_Test()
    {
        var pipeline = new JobReferencingPipeline();

        return Verify(pipeline.Serialize());
    }

    private class DotNet_Step_Library : StepLibrary
    {
        public override List<AdoExpression<Step>> Steps =>
        [
            DotNet.Install.Sdk("6.0.100"),

            If.IsBranch("main")
                .Step(DotNet.Restore.Projects("src/MyProject.sln")),

            DotNet.Build("src/MyProject.sln"),
        ];
    }

    private class SimpleDotNetPipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Jobs =
            {
                new Job("Foo")
                {
                    Steps =
                    {
                        Script.Inline("echo 'Hello World'"),

                        StepLibrary<DotNet_Step_Library>(),

                        Script.Inline("echo 'Goodbye World'"),
                    }
                }
            }
        };
    }

    [Fact]
    public Task Step_Library_Test()
    {
        var pipeline = new SimpleDotNetPipeline();

        return Verify(pipeline.Serialize());
    }

    private class Variable_Library(string env) : VariableLibrary
    {
        public override List<AdoExpression<VariableBase>> Variables =>
        [
            Variable($"connection-string-{env}", $"{env}_123"),

            If.IsBranch(env)
                .Group("prod-kv")
        ];
    }

    private class ConditionalPipeline : SimpleTestPipeline
    {
        public override SingleStagePipeline Pipeline => new()
        {
            Variables =
            {
                Variable("test", true),

                If.NotIn("'$(Environment)'", "'prod'")
                    .VariableLibrary(new Variable_Library("dev"))
                    .VariableLibrary(new Variable_Library("staging"))
                .Else
                    .VariableLibrary(new Variable_Library("prod"))
            }
        };
    }

    [Fact]
    public Task Conditional_Library_Test()
    {
        var pipeline = new ConditionalPipeline();

        return Verify(pipeline.Serialize());
    }
}
