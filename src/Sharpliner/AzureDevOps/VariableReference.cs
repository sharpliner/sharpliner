namespace Sharpliner.AzureDevOps;

public sealed class VariableReference
{
    internal static string GetReference(string prefix, string variableName) => $"variables['{prefix}{variableName}']";

    public string this[string variableName] => $"variables['{variableName}']";

    /// <summary>
    /// Variables connected to the agent running the current build (e.g. Agent.HomeDirectory)
    /// Read more at https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables?view=azure-devops&tabs=yaml#agent-variables-devops-services
    /// </summary>
    public AgentVariableReference Agent { get; } = new();

    /// <summary>
    /// Variables connected to the current build (e.g. build number)
    /// Read more at https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables?view=azure-devops&tabs=yaml#build-variables-devops-services
    /// </summary>
    public BuildVariableReference Build { get; } = new();
}


public sealed class AgentVariableReference
{
    public string this[string variableName] => GetReference(variableName);

    /// <summary>
    /// The local path on the agent where all folders for a given build pipeline are created.
    /// This variable has the same value as Pipeline.Workspace.
    /// For example: /home/vsts/work/1
    /// </summary>
    public string BuildDirectory => GetReference("BuildDirectory");

    /// <summary>
    /// A mapping from container resource names in YAML to their Docker IDs at runtime.
    /// For example:
    /// {
    ///   "one_container": {
    ///     "id": "bdbb357d73a0bd3550a1a5b778b62a4c88ed2051c7802a0659f1ff6e76910190"
    ///   },
    ///   "another_container": {
    ///     "id": "82652975109ec494876a8ccbb875459c945982952e0a72ad74c91216707162bb"
    ///   }
    /// }
    /// </summary>
    public string ContainerMapping => GetReference("ContainerMapping");

    /// <summary>
    /// The directory the agent is installed into. This contains the agent software.
    /// For example: c:\agent.
    /// </summary>
    public string HomeDirectory => GetReference("HomeDirectory");

    /// <summary>
    /// The ID of the agent.
    /// </summary>
    public string Id => GetReference("Id");

    /// <summary>
    /// The name of the running job.
    /// This will usually be "Job" or "__default", but in multi-config scenarios, will be the configuration.
    /// </summary>
    public string JobName => GetReference("JobName");

    /// <summary>
    /// The status of the build.
    ///   - Canceled
    ///   - Failed
    ///   - Succeeded
    ///   - SucceededWithIssues (partially successful) 
    /// </summary>
    public string JobStatus => GetReference("JobStatus");

    /// <summary>
    /// The name of the machine on which the agent is installed.
    /// </summary>
    public string MachineName => GetReference("MachineName");

    /// <summary>
    /// The name of the agent that is registered with the pool.
    /// If you are using a self-hosted agent, then this name is specified by you. See agents.
    /// </summary>
    public string Name => GetReference("Name");

    /// <summary>
    /// The operating system of the agent host.
    /// Valid values are:
    ///   - Windows_NT
    ///   - Darwin
    ///   - Linux
    /// </summary>
    public string OS => GetReference("OS");

    /// <summary>
    /// The operating system processor architecture of the agent host.
    /// Valid values are:
    ///   - X86
    ///   - X64
    ///   - ARM
    /// </summary>
    public string OSArchitecture => GetReference("OSArchitecture");

    /// <summary>
    /// A temporary folder that is cleaned after each pipeline job.
    /// This directory is used by tasks such as .NET Core CLI task to hold temporary items like test results before they are published.
    /// For example: /home/vsts/work/_temp for Ubuntu
    /// </summary>
    public string TempDirectory => GetReference("TempDirectory");

    /// <summary>
    /// The directory used by tasks such as Node Tool Installer and Use Python Version to switch between multiple versions of a tool.
    /// These tasks will add tools from this directory to PATH so that subsequent build steps can use them.
    /// Learn about managing this directory on a self-hosted agent.
    /// </summary>
    public string ToolsDirectory => GetReference("ToolsDirectory");

    /// <summary>
    /// The working directory for this agent.
    /// For example: c:\agent_work
    /// Note: This directory is not guaranteed to be writable by pipeline tasks (eg. when mapped into a container)
    /// </summary>
    public string WorkFolder => GetReference("WorkFolder");

    private static string GetReference(string name) => VariableReference.GetReference("Agent.", name);
}


public sealed class BuildVariableReference
{
    public string this[string variableName] => GetReference(variableName);

    /// <summary>
    /// The local path on the agent where any artifacts are copied to before being pushed to their destination.
    /// For example: c:\agent_work\1\a
    /// 
    /// A typical way to use this folder is to publish your build artifacts with the Copy files and Publish build artifacts tasks.
    /// 
    /// Note: Build.ArtifactStagingDirectory and Build.StagingDirectory are interchangeable. This directory is purged before each new build, so you don't have to clean it up yourself.
    /// 
    /// See Artifacts in Azure Pipelines.
    /// 
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </summary>
    public string ArtifactStagingDirectory => GetReference("ArtifactStagingDirectory");

    /// <summary>
    /// The local path on the agent where any artifacts are copied to before being pushed to their destination.
    /// For example: c:\agent_work\1\a
    /// A typical way to use this folder is to publish your build artifacts with the Copy files and Publish build artifacts tasks.
    /// Note: Build.ArtifactStagingDirectory and Build.StagingDirectory are interchangeable. This directory is purged before each new build, so you don't have to clean it up yourself.
    /// See Artifacts in Azure Pipelines.
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </summary>
    public string StagingDirectory => GetReference("StagingDirectory");

    /// <summary>
    /// The ID of the record for the completed build.
    /// </summary>
    public string BuildId => GetReference("BuildId");

    /// <summary>
    /// The name of the completed build, also known as the run number.You can specify what is included in this value.
    /// A typical use of this variable is to make it part of the label format, which you specify on the repository tab.
    /// 
    /// Note: This value can contain whitespace or other invalid label characters. In these cases, the label format will fail.
    /// 
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </summary>
    public string BuildNumber => GetReference("BuildNumber");

    /// <summary>
    /// The URI for the build.
    /// For example: vstfs:///Build/Build/1430
    ///
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag. 
    /// </summary>
    public string BuildUri => GetReference("BuildUri");

    /// <summary>
    /// The local path on the agent you can use as an output folder for compiled binaries.
    /// By default, new build pipelines are not set up to clean this directory. You can define your build to clean it up on the Repository tab.
    /// For example: c:\agent_work\1\b
    ///
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag. 
    /// </summary>
    public string BinariesDirectory => GetReference("BinariesDirectory");

    /// <summary>
    /// The ID of the container for your artifact.
    /// When you upload an artifact in your pipeline, it is added to a container that is specific for that particular artifact.
    /// </summary>
    public string ContainerId => GetReference("ContainerId");

    /// <summary>
    /// The name of the build pipeline.
    /// Note: This value can contain whitespace or other invalid label characters. In these cases, the label format will fail.
    /// </summary>
    public string DefinitionName => GetReference("DefinitionName");

    /// <summary>
    /// The version of the build pipeline.
    /// </summary>
    public string DefinitionVersion => GetReference("DefinitionVersion");

    /// <summary>
    /// See "How are the identity variables set?".
    /// Note: This value can contain whitespace or other invalid label characters. In these cases, the label format will fail.
    /// </summary>
    public string QueuedBy => GetReference("QueuedBy");

    /// <summary>
    /// See "How are the identity variables set?"
    /// </summary>
    public string QueuedById => GetReference("QueuedById");

    /// <summary>
    /// The event that caused the build to run.
    ///   - Manual: A user manually queued the build.
    ///   - IndividualCI: Continuous integration (CI) triggered by a Git push or a TFVC check-in.
    ///   - BatchedCI: Continuous integration (CI) triggered by a Git push or a TFVC check-in, and the Batch changes was selected.
    ///   - Schedule: Scheduled trigger.
    ///   - ValidateShelveset: A user manually queued the build of a specific TFVC shelveset.
    ///   - CheckInShelveset: Gated check-in trigger.
    ///   - PullRequest: The build was triggered by a Git branch policy that requires a build.
    ///   - ResourceTrigger: The build was triggered by a resource trigger or it was triggered by another build.
    /// </summary>
    public string Reason => GetReference("Reason");

    /// <summary>
    /// Variables connected to repository information
    /// </summary>
    public RepositoryVariableReference Repository => new();

    /// <summary>
    /// See "How are the identity variables set?"
    /// Note: This value can contain whitespace or other invalid label characters. In these cases, the label format will fail.
    /// </summary>
    public string RequestedFor => GetReference("RequestedFor");

    /// <summary>
    /// See "How are the identity variables set?"
    /// </summary>
    public string RequestedForEmail => GetReference("RequestedForEmail");

    /// <summary>
    /// See "How are the identity variables set?"
    /// </summary>
    public string RequestedForId => GetReference("RequestedForId");

    /// <summary>
    /// The branch of the triggering repo the build was queued for.
    /// Some examples:
    ///   - Git repo branch: refs/heads/master
    ///   - Git repo pull request: refs/pull/1/merge
    ///   - TFVC repo branch: $/teamproject/main
    ///   - TFVC repo gated check-in: Gated_2016-06-06_05.20.51.4369;username @live.com
    ///   - TFVC repo shelveset build: myshelveset; username @live.com
    ///   - When your pipeline is triggered by a tag: refs/tags/your-tag-name
    ///   
    /// When you use this variable in your build number format, the forward slash characters (/) are replaced with underscore characters _).
    /// Note: In TFVC, if you are running a gated check-in build or manually building a shelveset, you cannot use this variable in your build number format.
    /// </summary>
    public string SourceBranch => GetReference("SourceBranch");

    /// <summary>
    /// The name of the branch in the triggering repo the build was queued for.
    ///   - Git repo branch or pull request: The last path segment in the ref. For example, in refs/heads/master this value is master.In refs/heads/feature/tools this value is tools.
    ///   - TFVC repo branch: The last path segment in the root server path for the workspace. For example, in $/teamproject/main this value is main.
    ///   - TFVC repo gated check-in or shelveset build is the name of the shelveset.For example, Gated_2016-06-06_05.20.51.4369; username @live.com or myshelveset; username @live.com.
    ///
    /// Note: In TFVC, if you are running a gated check-in build or manually building a shelveset, you cannot use this variable in your build number format. 	Yes
    /// </summary>
    public string SourceBranchName => GetReference("SourceBranchName");

    /// <summary>
    /// The local path on the agent where your source code files are downloaded. For example: c:\agent_work\1\s
    /// By default, new build pipelines update only the changed files.
    /// 
    /// Important note: If you check out only one Git repository, this path will be the exact path to the code. If you check out multiple repositories, it will revert to its default value, which is $(Pipeline.Workspace)/s, even if the self (primary) repository is checked out to a custom path different from its multi-checkout default path $(Pipeline.Workspace)/s/<RepoName> (in this respect, the variable differs from the behavior of the Build.Repository.LocalPath variable).
    /// 
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </summary>
    public string SourcesDirectory => GetReference("SourcesDirectory");

    /// <summary>
    /// The latest version control change of the triggering repo that is included in this build.
    ///   - Git: The commit ID
    ///   - TFVC: the changeset
    /// </summary>
    public string SourceVersion => GetReference("SourceVersion");

    /// <summary>
    /// The comment of the commit or changeset for the triggering repo. We truncate the message to the first line or 200 characters, whichever is shorter.
    /// The Build.SourceVersionMessage corresponds to the message on Build.SourceVersion commit. The Build.SourceVersion commit for a PR build is the merge commit (not the commit on the source branch).
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag. Also, this variable is only available on the step level and is neither available in the job nor stage levels (i.e. the message is not extracted until the job had started and checked out the code).
    /// Note: This variable is available in TFS 2015.4.
    /// </summary>
    public string SourceVersionMessage => GetReference("SourceVersionMessage");

    /// <summary>
    /// Defined if your repository is Team Foundation Version Control.
    /// If you are running a gated build or a shelveset build, this is set to the name of the shelveset you are building.
    /// Note: This variable yields a value that is invalid for build use in a build number format.
    /// </summary>
    public string SourceTfvcShelveset => GetReference("SourceTfvcShelveset");

    /// <summary>
    /// The local path on the agent where the test results are created.
    /// For example: c:\agent_work\1\TestResults
    ///
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </summary>
    public string CommonTestResultsDirectory => GetReference("Common.TestResultsDirectory");

    /// <summary>
    /// Variables connected to why the build was created.
    /// </summary>
    public TriggeredByVariableReference TriggeredBy => new();

    private static string GetReference(string name) => VariableReference.GetReference("Build.", name);
}

public sealed class RepositoryVariableReference
{
    public string this[string variableName] => GetReference(variableName);

    /// <summary>
    /// The value you've selected for Clean in the source repository settings.
    /// 
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </summary>
    public string Clean => GetReference("Clean");

    /// <summary>
    /// The local path on the agent where your source code files are downloaded.
    /// For example: c:\agent_work\1\s
    ///
    /// By default, new build pipelines update only the changed files.You can modify how files are downloaded on the Repository tab.
    /// Important note: If you check out only one Git repository, this path will be the exact path to the code.If you check out multiple repositories, the behavior is as follows(and might differ from the value of the Build.SourcesDirectory variable) :
    ///   - If the checkout step for the self(primary) repository has no custom checkout path defined, or the checkout path is the multi-checkout default path $(Pipeline.Workspace)/s/<RepoName> for the self repository, the value of this variable will revert to its default value, which is $(Pipeline.Workspace)/s.
    ///   - If the checkout step for the self (primary) repository does have a custom checkout path defined (and it's not its multi-checkout default path), this variable will contain the exact path to the self repository.
    /// 
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </summary>
    public string LocalPath => GetReference("LocalPath");

    /// <summary>
    /// The unique identifier of the repository.
    /// This won't change, even if the name of the repository does.
    /// 
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </summary>
    public string ID => GetReference("ID");

    /// <summary>
    /// The name of the triggering repository.
    /// 
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </summary>
    public string Name => GetReference("Name");

    /// <summary>
    /// The type of the triggering repository.
    ///   - TfsGit: TFS Git repository
    ///   - TfsVersionControl: Team Foundation Version Control
    ///   - Git: Git repository hosted on an external server
    ///   - GitHub
    ///   - Svn: Subversion
    /// 
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </summary>
    public string Provider => GetReference("Provider");

    /// <summary>
    /// Defined if your repository is Team Foundation Version Control. The name of the TFVC workspace used by the build agent.
    /// For example, if the Agent.BuildDirectory is c:\agent_work\12 and the Agent.Id is 8, the workspace name could be: ws_12_8
    /// 
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </summary>
    public string TfvcWorkspace => GetReference("Tfvc.Workspace");

    /// <summary>
    /// The URL for the triggering repository. For example:
    ///   - Git: https://dev.azure.com/fabrikamfiber/_git/Scripts
    ///   - TFVC: https://dev.azure.com/fabrikamfiber/ 
    /// 
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </summary>
    public string Uri => GetReference("Uri");

    /// <summary>
    /// The value you've selected for Checkout submodules on the repository tab. With multiple repos checked out, this value tracks the triggering repository's setting.
    /// 
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// </summary>
    public string GitSubmoduleCheckout => GetReference("Git.SubmoduleCheckout");

    private static string GetReference(string name) => VariableReference.GetReference("Build.Repository.", name);
}

public sealed class TriggeredByVariableReference
{
    public string this[string variableName] => GetReference(variableName);

    /// <summary>
    /// If the build was triggered by another build, then this variable is set to the BuildID of the triggering build. In Classic pipelines, this variable is triggered by a build completion trigger.
    ///
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// If you are triggering a YAML pipeline using resources, you should use the resources variables instead.
    /// </summary>
    public string BuildId => GetReference("BuildId");

    /// <summary>
    /// If the build was triggered by another build, then this variable is set to the DefinitionID of the triggering build. In Classic pipelines, this variable is triggered by a build completion trigger.
    ///
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// If you are triggering a YAML pipeline using resources, you should use the resources variables instead.
    /// </summary>
    public string DefinitionId => GetReference("DefinitionId");

    /// <summary>
    /// If the build was triggered by another build, then this variable is set to the name of the triggering build pipeline. In Classic pipelines, this variable is triggered by a build completion trigger.
    ///
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// If you are triggering a YAML pipeline using resources, you should use the resources variables instead.
    /// </summary>
    public string DefinitionName => GetReference("DefinitionName");

    /// <summary>
    /// If the build was triggered by another build, then this variable is set to the number of the triggering build. In Classic pipelines, this variable is triggered by a build completion trigger.
    ///
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// If you are triggering a YAML pipeline using resources, you should use the resources variables instead.
    /// </summary>
    public string BuildNumber => GetReference("BuildNumber");

    /// <summary>
    /// If the build was triggered by another build, then this variable is set to ID of the project that contains the triggering build. In Classic pipelines, this variable is triggered by a build completion trigger.
    ///
    /// This variable is agent-scoped, and can be used as an environment variable in a script and as a parameter in a build task, but not as part of the build number or as a version control tag.
    /// If you are triggering a YAML pipeline using resources, you should use the resources variables instead.
    /// </summary>
    public string ProjectID => GetReference("ProjectID");

    private static string GetReference(string name) => VariableReference.GetReference("Build.TriggeredBy.", name);
}
