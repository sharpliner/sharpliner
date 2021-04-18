using Sharpliner.Model.AzureDevOps;
using Sharpliner.Model.Definition;

namespace Sharpliner.Serialization.Tests
{
    internal class MockPipeline : PipelineDefinitionBase
    {
        public override string TargetFile => "azure-pipelines.yml";

        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

        public override Pipeline Pipeline => new()
        {
            Name = "$(Date:yyyMMdd).$(Rev:rr)",

            Variables =
            {
                Variable("Configuration", "Release"),
                Group("PR keyvault variables"),

                If.Equal("variables['Build.Reason']", "PullRequest")
                    .Variable("TargetBranch", "$(System.PullRequest.SourceBranch)")
                    .Variable("IsPr", true),

                If.And(Equal("variables['Build.SourceBranch']", "refs/heads/production"), NotEqual("Configuration", "Debug"))
                    .Variable("PublishProfileFile", "Prod")
                    .If.NotEqual("variables['Build.Reason']", "PullRequest")
                        .Variable("AzureSubscription", "Int")
                        .Group("azure-int")
                    .EndIf
                    .If.Equal("variables['Build.Reason']", "PullRequest")
                        .Variable("AzureSubscription", "Prod")
                        .Group("azure-prod"),
            }
        };
    }
}
