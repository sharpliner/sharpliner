using Sharpliner.Model.AzureDevOps;

namespace Sharpliner.Serialization.Tests
{
    internal class MockPipeline : AzureDevOpsPipelineDefinition
    {
        public override string TargetFile => "azure-pipelines.yml";

        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

        public override AzureDevOpsPipeline Pipeline => new()
        {
            Name = "$(Date:yyyMMdd).$(Rev:rr)",

            Trigger = new DetailedTrigger
            {
                Batched = false,
                Branches = new()
                {
                    Include =
                    {
                        "main",
                        "release/*",
                    }
                }
            },

            Pr = new BranchPrTrigger("main", "release/*"),

            Variables =
            {
                new Variable("Configuration", "Release"), // We can create the objects and then resue them for definition too
                Variable("Configuration", "Release"),     // Or we have this more YAML-like definition
                Group("PR keyvault variables"),

                If.Equal(variables["Build.Reason"], "PullRequest")
                    .Variable("TargetBranch", "$(System.PullRequest.SourceBranch)")
                    .Variable("IsPr", true),

                If.And(Equal(variables["Build.SourceBranch"], "refs/heads/production"), NotEqual("Configuration", "Debug"))
                    .Variable("PublishProfileFile", "Prod")
                    .If.NotEqual(variables["Build.Reason"], "PullRequest")
                        .Variable("AzureSubscription", "Int")
                        .Group("azure-int")
                    .EndIf
                    .If.Equal(variables["Build.Reason"], "PullRequest")
                        .Variable("AzureSubscription", "Prod")
                        .Group("azure-prod"),
            }
        };
    }
}
