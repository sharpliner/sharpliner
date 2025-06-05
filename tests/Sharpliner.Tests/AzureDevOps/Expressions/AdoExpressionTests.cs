using FluentAssertions;
using Sharpliner.AzureDevOps;
using Sharpliner.AzureDevOps.Expressions;

namespace Sharpliner.Tests.AzureDevOps.Expressions;

public class AdoExpressionTests
{
    private class Equality_Test_Pipeline : SingleStagePipelineDefinition
    {
        public static VariableReference Variable1 => variables["foo"];
        public static VariableReference Variable2 => variables["bar"];

        public static ParameterReference Parameter1 => parameters["Xyz"];
        public static ParameterReference Parameter2 => parameters["Unn"];

        public static AdoExpression<string> Expression1 => "foo";
        public static AdoExpression<string> Expression2 => "bar";

        public override string TargetFile => throw new NotImplementedException();

        public override SingleStagePipeline Pipeline => throw new NotImplementedException();
    }
    
    [Fact]
    public void Equality_Test()
    {
        Equality_Test_Pipeline.Variable1.Should().Be(Equality_Test_Pipeline.Variable1);
        Equality_Test_Pipeline.Variable1.Should().NotBe(Equality_Test_Pipeline.Variable2);
        Equality_Test_Pipeline.Parameter1.Should().Be(Equality_Test_Pipeline.Parameter1);
        Equality_Test_Pipeline.Parameter1.Should().NotBe(Equality_Test_Pipeline.Parameter2);
        Equality_Test_Pipeline.Expression1.Should().Be(Equality_Test_Pipeline.Expression1);
        Equality_Test_Pipeline.Expression1.Should().NotBe(Equality_Test_Pipeline.Expression2);
    }
}
