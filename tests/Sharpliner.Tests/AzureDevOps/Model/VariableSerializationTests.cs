using FluentAssertions;
using Sharpliner.AzureDevOps;
using Xunit;

namespace Sharpliner.Tests.AzureDevOps;

public class VariableSerializationTests
{
    private class VariablesPipeline : TestPipeline
    {
        private static readonly Variable s_ConfigurationVariable = new Variable("Configuration", "Release"); // We can create the objects and then reuse them for definition too

        public override Pipeline Pipeline => new()
        {
            Variables =
            {
                s_ConfigurationVariable,
                Variable("Framework", "net8.0"),     // Or we have this more YAML-like definition
                Group("PR keyvault variables"),

                If.Equal(variables.Build.Reason, "PullRequest")
                    .Variable("TargetBranch", variables.System.PullRequest.SourceBranch)
                    .Variable("IsPr", true),

                If.And(IsBranch("production"), NotEqual(s_ConfigurationVariable, "Debug"))
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
    public void Serialize_Pipeline_Test()
    {
        VariablesPipeline pipeline = new();
        string yaml = pipeline.Serialize();
        yaml.Trim().Should().Be(
            """
            variables:
            - name: Configuration
              value: Release

            - name: Framework
              value: net8.0

            - group: PR keyvault variables

            - ${{ if eq(variables['Build.Reason'], 'PullRequest') }}:
              - name: TargetBranch
                value: $(System.PullRequest.SourceBranch)

              - name: IsPr
                value: true

            - ${{ if and(eq(variables['Build.SourceBranch'], 'refs/heads/production'), ne(variables['Configuration'], 'Debug')) }}:
              - name: PublishProfileFile
                value: Prod

              - name: foo
                value: bar

              - ${{ if ne(variables['Build.Reason'], 'PullRequest') }}:
                - name: AzureSubscription
                  value: Int

                - group: azure-int

              - ${{ if eq(variables['Build.Reason'], 'PullRequest') }}:
                - name: AzureSubscription
                  value: Prod

                - group: azure-prod
            """);
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
    public void ContainsValue_Condition_Test()
    {
        var pipeline = new ContainsValueCondition_Test_Pipeline();

        var yaml = pipeline.Serialize();

        yaml.Trim().Should().Be(
            """
            variables:
            - ${{ if containsValue('refs/heads/feature/', parameters.allowedTags, variables['foo'], variables['Build.Reason'], variables['Build.SourceBranch']) }}:
              - name: feature
                value: on
            """
        );
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
    public void GreaterThan_Condition_Test()
    {
        var pipeline = new GreaterThanCondition_Test_Pipeline();

        var yaml = pipeline.Serialize();

        yaml.Trim().Should().Be(
            """
            variables:
            - ${{ if gt(variables['Build.BuildNumber'], 100) }}:
              - name: high
                value: true
            """
        );
    }
}
