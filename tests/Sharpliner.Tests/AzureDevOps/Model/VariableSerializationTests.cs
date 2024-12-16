using Sharpliner.AzureDevOps;

namespace Sharpliner.Tests.AzureDevOps;

public class VariableSerializationTests
{
    private class VariablesPipeline : TestPipeline
    {
        private static readonly Variable s_configurationVariable = new("Configuration", "Release"); // We can create the objects and then reuse them for definition too

        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                s_configurationVariable,
                Variable("Framework", "net8.0"),     // Or we have this more YAML-like definition
                Group("PR keyvault variables"),

                If.Equal(variables.Build.Reason, "PullRequest")
                    .Variable("TargetBranch", variables.System.PullRequest.SourceBranch)
                    .Variable("IsPr", true),

                If.And(IsBranch("production"), NotEqual(s_configurationVariable, "Debug"))
                    .Variables(("PublishProfileFile", "Prod"), ("foo", "bar"))
                    .If.IsNotPullRequest
                        .Variable("AzureSubscription", "Int")
                        .Group("azure-int")
                    .EndIf
                    .If.IsPullRequest
                        .Variable("AzureSubscription", "Prod")
                        .Group("azure-prod"),
            }
        };
    }

    [Fact]
    public Task Serialize_Pipeline_Test()
    {
        VariablesPipeline pipeline = new();
        
        return Verify(pipeline.Serialize());
    }

    private class ContainsValueCondition_Test_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.ContainsValue(variables.Build.SourceBranch, "refs/heads/feature/", parameters["allowedTags"], variables["foo"], variables.Build.Reason)
                    .Variable("feature", "on"),
            }
        };
    }

    [Fact]
    public Task ContainsValue_Condition_Test()
    {
        var pipeline = new ContainsValueCondition_Test_Pipeline();

        return Verify(pipeline.Serialize());
    }

    private class GreaterThanCondition_Test_Pipeline : TestPipeline
    {
        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                If.Greater(variables.Build.BuildNumber, "100")
                    .Variable("high", true),
            }
        };
    }

    [Fact]
    public Task GreaterThan_Condition_Test()
    {
        var pipeline = new GreaterThanCondition_Test_Pipeline();

        return Verify(pipeline.Serialize());
    }
}
