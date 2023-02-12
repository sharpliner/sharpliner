using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps.ConditionedExpressions;

public class ConditionedTests
{
    private class Equality_Test_Pipeline : SingleStagePipelineDefinition
    {
        public static VariableReference Variable1 => variables["foo"];
        public static VariableReference Variable2 => variables["bar"];

        public static ParameterReference Parameter1 => parameters["Xyz"];
        public static ParameterReference Parameter2 => parameters["Unn"];

        public static Conditioned<string> Conditioned1 => "foo";
        public static Conditioned<string> Conditioned2 => "bar";

        public override string TargetFile => throw new System.NotImplementedException();

        public override SingleStagePipeline Pipeline => throw new System.NotImplementedException();
    }
    
    [Fact]
    public void Equality_Test()
    {
        Equality_Test_Pipeline.Variable1.Should().Be(Equality_Test_Pipeline.Variable1);
        Equality_Test_Pipeline.Variable1.Should().NotBe(Equality_Test_Pipeline.Variable2);
        Equality_Test_Pipeline.Parameter1.Should().Be(Equality_Test_Pipeline.Parameter1);
        Equality_Test_Pipeline.Parameter1.Should().NotBe(Equality_Test_Pipeline.Parameter2);
        Equality_Test_Pipeline.Conditioned1.Should().Be(Equality_Test_Pipeline.Conditioned1);
        Equality_Test_Pipeline.Conditioned1.Should().NotBe(Equality_Test_Pipeline.Conditioned2);
    }
}
