using System;
using System.Diagnostics.CodeAnalysis;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps
{
    /// <summary>
    /// If your pipeline has templates in another repository, or if you want to use multi-repo checkout with a repository that requires a service connection, you must let the system know about that repository.
    /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/process/resources?view=azure-devops&amp;tabs=schema#define-a-repositories-resource">official Azure DevOps pipelines documentation</see>.
    /// </summary>
    public record RepositoryResource
    {
        /// <summary>
        /// Identifier for the resource used in pipeline resource variables (A-Z, a-z, 0-9, and underscore)
        /// </summary>
        [YamlMember(Alias = "pipeline")]
        public string Identifier { get; }

        /// <summary>
        /// Project for the source
        /// Optional for current project
        /// </summary>
        [YamlIgnore]
        [DisallowNull]
        public RepositoryType Type { get; init; } = RepositoryType.Git;

        [YamlMember(Alias = "type")]
        public string RepoType => Type switch
        {
            RepositoryType.Git => "git",
            RepositoryType.GitHub => "github",
            RepositoryType.GitHubEnterprise => "githubenterprise",
            RepositoryType.BitBucket => "bitbucket",
            _ => throw new NotImplementedException(),
        };

        /// <summary>
        /// Repository name (format depends on `Type`)
        /// </summary>
        [DisallowNull]
        public string? Name { get; init; }

        /// <summary>
        /// Ref name to use
        /// Defaults to 'refs/heads/main'
        /// </summary>
        [DisallowNull]
        public string? Ref { get; init; }

        /// <summary>
        /// Name of the service connection to use (for types that aren't Azure Repos)
        /// </summary>
        [DisallowNull]
        public string? Endpoint { get; init; }

        /// <summary>
        /// Triggers are not enabled by default unless you add trigger section to the resource
        /// </summary>
        [DisallowNull]
        public PipelineTrigger? Trigger { get; init; }

        public RepositoryResource(string identifier)
        {
            Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            Pipeline.ValidateName(Identifier);
        }
    }

    public enum RepositoryType
    {
        /// <summary>
        /// The name value refers to another repository in the same project.
        /// Example name: otherRepo
        /// To refer to a repository in another project within the same organization, prefix the name with that project's name.
        /// Example name: OtherProject/otherRepo.
        /// </summary>
        Git,

        /// <summary>
        /// The name value is the full name of the GitHub repository and includes the user or organization.
        /// Example name: Microsoft/vscode
        /// </summary>
        GitHub,

        /// <summary>
        /// The name value is the full name of the GitHub Enterprise repository and includes the user or organization.
        /// Example name: Microsoft/vscode
        /// </summary>
        GitHubEnterprise,

        /// <summary>
        /// The name value is the full name of the Bitbucket Cloud repository and includes the user or organization.
        /// Example name: MyBitbucket/vscode
        /// </summary>
        BitBucket,
    }
}
