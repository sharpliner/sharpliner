using Sharpliner.Model.AzureDevOps;
using Sharpliner.Model.AzureDevOps.Tasks;

namespace Sharpliner.Serialization.Tests
{
    internal class MockPipeline : AzureDevOpsPipelineDefinition
    {
        public override string TargetFile => "azure-pipelines.yml";

        public override TargetPathType TargetPathType => TargetPathType.RelativeToGitRoot;

        public override AzureDevOpsPipeline Pipeline => new()
        {
            /*Name = "$(Date:yyyMMdd).$(Rev:rr)",

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
            */
            Variables =
            {
                Variable("Configuration", "Release"),     // Or we have this more YAML-like definition
                Group("PR keyvault variables"),

                If.Equal(variables["Build.Reason"], "PullRequest")
                    .Variable("TargetBranch", "$(System.PullRequest.SourceBranch)")
                    .Variable("IsPr", true),

                //If.And(Equal(variables["Build.SourceBranch"], "refs/heads/production"), NotEqual("Configuration", "Debug"))
                //    .Variable("PublishProfileFile", "Prod")
                //    .If.NotEqual(variables["Build.Reason"], "PullRequest")
                //        .Variable("AzureSubscription", "Int")
                //        .Group("azure-int")
                //    .EndIf()
                //    .If.Equal(variables["Build.Reason"], "PullRequest")
                //        .Variable("AzureSubscription", "Prod")
                //        .Group("azure-prod"),
            },
            /*
            Stages =
            {
                new("Build", "Build the project")
                {
                    Jobs =
                    {
                        new("Build_API", "Build API")
                        {
                            Variables =
                            {
                                If.Equal("variables['IsPr']", "true")
                                    .Variable("DotnetVersion", "6.0-preview-4"),

                                If.NotEqual("variables['IsPr']", "true")
                                    .Variable("DotnetVersion", "6.0"),
                            },

                            Steps =
                            {
                                new InlineBashTask("Restore .NET",
                                    "curl -L https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh",
                                    "chmod +x dotnet-install.sh",
                                    "./dotnet-install.sh --channel $(DotnetVersion) --install-dir ./.dotnet"),

                                new InlineBashTask("Build .sln",
                                    "./.dotnet/dotnet build src/MySolution.sln -c $(Configuration)"),

                                new InlineBashTask("Run unit tests",
                                    "./.dotnet/dotnet test src/MySolution.sln")
                                {
                                    ContinueOnError = true,
                                    Condition = "eq(variables['Build.Reason'], \"PullRequest\")",
                                },

                                new InlineBashTask("Upload tests results",
                                    "./upload.sh ./** /TestResult.xml")
                                {
                                    Condition = "eq(variables['Build.Reason'], \"PullRequest\")",
                                },
                            }
                        }
                    }
                },

                new("Test", "Run E2E tests")
                {
                    DependsOn = { "Build" },
                    Jobs =
                    {
                        // ...
                    }
                }
            }*/
        };
    }
}
