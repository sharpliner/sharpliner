# Azure Pipelines built-in task support matrix

This document is an audit of the complete catalog of Microsoft-maintained, **built-in** Azure Pipelines tasks
against Sharpliner's strongly typed task models and builders.
Its purpose is to make it obvious which tasks you can define with a dedicated C# API and which ones still
have to be defined through the generic `AzureDevOpsTask` escape hatch.

- [Sources](#sources)
- [Methodology](#methodology)
- [Sharpliner's strongly typed task inventory](#sharpliners-strongly-typed-task-inventory)
- [Support matrix](#support-matrix)
- [Summary](#summary)
- [Tasks we would like to see contributed](#tasks-we-would-like-to-see-contributed)
- [Refreshing this document](#refreshing-this-document)

## Sources

| Source | URL | Revision / date |
|---|---|---|
| Microsoft's official task reference index (the source that backs the [Microsoft Learn task reference](https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/)) | <https://raw.githubusercontent.com/MicrosoftDocs/azure-devops-yaml-schema/main/task-reference/index.md> | `main` branch, fetched **2026-08-30**; document's own `ms.date` front matter is **06/30/2026** |
| Microsoft's task implementations | <https://github.com/microsoft/azure-pipelines-tasks/tree/master/Tasks> | Used as a cross-check for task identities and versions, **2026-08-30** |
| Sharpliner's task models and builders | [`src/Sharpliner.Core/AzureDevOps/Model/Tasks`](../../src/Sharpliner.Core/AzureDevOps/Model/Tasks) | This repository, at the commit that introduced this document |

Only the `azure-pipelines` (Azure DevOps Services) moniker of the official index is audited, as it is a
superset of the tasks available on the on-premises Azure DevOps Server versions.

## Methodology

- Tasks are matched by their **canonical YAML identity** (for example `DotNetCoreCLI@2`), grouped into
  *task families* exactly as Microsoft's index groups them (one row per official specification, listing all majors).
- The generic [`AzureDevOpsTask`](../../src/Sharpliner.Core/AzureDevOps/Model/Tasks/AzureDevOpsTask.cs) escape hatch
  (`Task("Foo@1")`) is **not** counted as support - it can express every task and would make the audit meaningless.
- Sharpliner is considered to support a task family when it ships a dedicated record or builder that emits the
  task's current major version. Not modelling superseded majors (for example `DotNetCoreCLI@1`) is not counted
  against support.
- The pipeline keywords that are not tasks (`checkout`, `template`, `deployment` strategies, ...) are outside of the
  scope of this audit. Note that `script`, `bash`, `pwsh`, `powershell`, `publish` and `download` **are** shortcuts
  for real tasks (`CmdLine@2`, `Bash@3`, `PowerShell@2`, `PublishPipelineArtifact@1`, `DownloadPipelineArtifact@2`)
  and are therefore counted.
- Marketplace (non-Microsoft) tasks are out of the scope of this audit. They are supported through separate
  extension packages, see [Marketplace tasks](DefinitionReference.md#marketplace-tasks).

### Classification legend

| Classification | Meaning |
|---|---|
| ✅ Supported | A strongly typed Sharpliner record and/or builder emits the current major of the task |
| 🟡 Partial | Only some versions or some of the task's commands are modelled |
| ❌ Missing | No strongly typed API, the task can only be used through `Task("Name@version")` |
| ⚪ Out of scope | Deprecated, superseded, classic-release-only or third-party tasks that we intentionally do not model |

## Sharpliner's strongly typed task inventory

These are all the tasks Sharpliner emits from a dedicated API today:

| YAML identity | Sharpliner API |
|---|---|
| `ArchiveFiles@2` | `ArchiveFilesTask` |
| `AzureCLI@3`, `AzureCLI@2` | `AzureCliV3.Inline/File/FromFile/FromResourceFile` -> `InlineAzureCliV3Task`, `AzureCliV3FileTask`; `AzureCli.Inline/File/FromFile/FromResourceFile` -> `InlineAzureCliTask`, `AzureCliFileTask` |
| `AzureLoadTest@1` | `AzureLoadTestTask` |
| `AzureRmWebAppDeployment@5` | `AzureAppServiceDeploy.WebApp/WebAppLinux/Package/Container/PublishProfile` -> `AzureRmWebAppDeploymentPackageTask`, `AzureRmWebAppDeploymentContainerTask`, `AzureRmWebAppDeploymentPublishProfileTask` |
| `AzureCLI@2` | `AzureCli.Inline/File/FromFile/FromResourceFile` -> `InlineAzureCliTask`, `AzureCliFileTask` |
| `AzureKeyVault@2`, `AzureKeyVault@1` | `AzureKeyVault.DownloadSecrets` -> `AzureKeyVaultTask`, `AzureKeyVaultV1Task` |
| `AzurePowerShell@5`, `AzurePowerShell@4` | `AzurePowerShell.Inline/File/FromFile/FromResourceFile` -> `InlineAzurePowerShellTask`, `AzurePowerShellFileTask`, `InlineAzurePowerShellV4Task`, `AzurePowerShellV4FileTask` |
| `Bash@3` | `Bash.Inline/File/FromFile/FromResourceFile` -> `InlineBashTask`, `BashFileTask` (`bash` step shortcut) |
| `CMake@1` | `CMakeTask` |
| `CmdLine@2` | `Script.Inline/FromFile/FromResourceFile` -> `ScriptTask` (`script` step shortcut) |
| `CopyFiles@2` | `CopyFilesTask` |
| `DeleteFiles@1` | `DeleteFilesTask` |
| `Docker@2` | `Docker.Build/Push/BuildAndPush/Login/Logout/Start/Stop` -> `DockerTask` and its command specializations |
| `DockerCompose@1` | `DockerCompose.*` -> `DockerComposeTask` and its action-specific `Build`/`Push`/`Run`/`RunService`/`Lock`/`WriteImageDigests`/`CombineConfiguration`/`Command` specializations |
| `DotNetCoreCLI@2` | `DotNet.*` -> `DotNetCoreCliTask` and its `Build`/`Test`/`Pack`/`Publish`/`Push`/`Restore` specializations |
| `DownloadPipelineArtifact@2` | `Download.Current/FromPipelineResource/SpecificBuild/LatestFromBranch/None` -> `DownloadTask` (`download` step shortcut) |
| `ExtractFiles@1` | `ExtractFilesTask` |
| `HelmDeploy@1` | `Helm.*` -> `HelmDeployTask` and its `Install`/`Upgrade`/`Package`/`Push`/`Init`/`Login`/`Logout`/`Create`/`Ls`/`Get`/`Expose`/`Delete`/`Uninstall`/`Rollback` specializations |
| `npmAuthenticate@0` | `Npm.Authenticate` -> `NpmAuthenticateTask` |
| `AdvancedSecurity-Codeql-Init@1` | `AdvancedSecurity.Codeql.Init/InitWithoutBuild/InitWithAutomaticInstall` -> `AdvancedSecurityCodeqlInitTask` |
| `NuGetAuthenticate@1` | `NuGet.Authenticate` -> `NuGetAuthenticateTask` |
| `NuGetCommand@2` | `NuGet.*` -> `NuGetCommandTask` and its `Restore`/`Pack`/`Push`/`Custom` specializations |
| `PowerShell@2` | `Powershell.*`, `Pwsh.*` -> `InlinePowershellTask`, `PowershellFileTask`, `InlinePwshTask` |
| `PublishCodeCoverageResults@2` | `PublishCodeCoverageResultsTask` |
| `PublishPipelineArtifact@1` | `Publish.Pipeline`, `Publish.FileShare` -> `PublishTask` (`publish` step shortcut) |
| `PublishSymbols@2` | `Publish.Symbols.*` -> `PublishSymbolsTask` mode-specific specializations |
| `PublishTestResults@2` | `PublishTestResultsTask` |
| `UniversalPackages@1` | `Download.UniversalPackage(...)`, `Publish.UniversalPackage(...)` -> `UniversalPackagesV1DownloadTask`, `UniversalPackagesV1PublishTask` |
| `UniversalPackages@0` | `UniversalPackagesDownloadTask`, `UniversalPackagesPublishTask` |
| `UseDotNet@2` | `DotNet.Install.*` -> `UseDotNetTask` |

## Support matrix

### Build tasks

| Task | YAML identity (all majors) | Classification | Sharpliner API / rationale |
|---|---|---|---|
| .NET Core | `DotNetCoreCLI@2`, `DotNetCoreCLI@1`, `DotNetCoreCLI@0` | ✅ Supported | `DotNet.*` builder + `DotNetCoreCliTask` (`Build`/`Test`/`Pack`/`Publish`/`Push`/`Restore`/`Run`/`Custom`). Only the current `@2` major is modelled. |
| Advanced Security Initialize CodeQL | `AdvancedSecurity-Codeql-Init@1` | ✅ Supported | `AdvancedSecurity.Codeql.Init/InitWithoutBuild/InitWithAutomaticInstall` -> `AdvancedSecurityCodeqlInitTask`. |
| Advanced Security Perform CodeQL analysis | `AdvancedSecurity-Codeql-Analyze@1` | ❌ Missing | No strongly typed model or builder. |
| Advanced Security Publish Results | `AdvancedSecurity-Publish@1` | ❌ Missing | No strongly typed model or builder. |
| Android Build | `AndroidBuild@1` | ⚪ Out of scope | Deprecated by Microsoft; the docs recommend the `Gradle` task instead. |
| Android Signing | `AndroidSigning@3`, `AndroidSigning@2`, `AndroidSigning@1` | ❌ Missing | No strongly typed model or builder. |
| Ant | `Ant@1` | ❌ Missing | No strongly typed model or builder. |
| Azure IoT Edge | `AzureIoTEdge@2` | ❌ Missing | No strongly typed model or builder. |
| CMake | `CMake@1` | ✅ Supported | `CMakeTask`. |
| Container Build | `ContainerBuild@0` | ❌ Missing | No strongly typed model or builder. |
| Docker | `Docker@2`, `Docker@1`, `Docker@0` | ✅ Supported | `Docker.Build/Push/BuildAndPush/Login/Logout/Start/Stop` -> `DockerTask` and its command specializations model the current `Docker@2` major. Superseded `Docker@1`/`Docker@0` majors are not modelled. |
| Docker Compose | `DockerCompose@1`, `DockerCompose@0` | ❌ Missing | No strongly typed model or builder. |
| Docker | `Docker@2`, `Docker@1`, `Docker@0` | ❌ Missing | No strongly typed model or builder. |
| Docker Compose | `DockerCompose@1`, `DockerCompose@0` | ✅ Supported | `DockerCompose.*` builder + `DockerComposeTask` with action-specific `Build`/`Push`/`Run`/`RunService`/`Lock`/`WriteImageDigests`/`CombineConfiguration`/`Command` records. Only the current `@1` major is modelled. |
| Download GitHub NuGet | `DownloadGitHubNugetPackage@1` | ⚪ Out of scope | Deprecated by Microsoft; superseded by `NuGetCommand@2`/`DotNetCoreCLI@2` with a GitHub service connection, both of which are supported. |
| Go | `Go@0` | ❌ Missing | No strongly typed model or builder. |
| Gradle | `Gradle@4`, `Gradle@3`, `Gradle@2`, `Gradle@1` | ❌ Missing | No strongly typed model or builder. |
| Grunt | `Grunt@0` | ❌ Missing | No strongly typed model or builder. |
| gulp | `gulp@1`, `gulp@0` | ❌ Missing | No strongly typed model or builder. |
| Index sources and publish symbols | `PublishSymbols@2`, `PublishSymbols@1` | ✅ Supported | `Publish.Symbols.*` builder + `PublishSymbolsTask` mode-specific specializations. Only current major `@2` is modelled. |
| Jenkins queue job | `JenkinsQueueJob@2`, `JenkinsQueueJob@1` | ❌ Missing | No strongly typed model or builder. |
| Maven | `Maven@4`, `Maven@3`, `Maven@2`, `Maven@1` | ❌ Missing | No strongly typed model or builder. |
| MSBuild | `MSBuild@1` | ❌ Missing | No strongly typed model or builder. |
| Prepare Analysis Configuration | `SonarQubePrepare@8`, `SonarQubePrepare@7`, `SonarQubePrepare@6`, `SonarQubePrepare@5`, `SonarQubePrepare@4` | ⚪ Out of scope | SonarQube tasks ship in a third-party (SonarSource) extension. Sharpliner models such tasks in separate extension packages, see [Marketplace tasks](DefinitionReference.md#marketplace-tasks). |
| Publish Quality Gate Result | `SonarQubePublish@8`, `SonarQubePublish@7`, `SonarQubePublish@6`, `SonarQubePublish@5`, `SonarQubePublish@4` | ⚪ Out of scope | Third-party SonarSource extension, see `SonarQubePrepare`. |
| Run Code Analysis | `SonarQubeAnalyze@8`, `SonarQubeAnalyze@7`, `SonarQubeAnalyze@6`, `SonarQubeAnalyze@5`, `SonarQubeAnalyze@4` | ⚪ Out of scope | Third-party SonarSource extension, see `SonarQubePrepare`. |
| Visual Studio build | `VSBuild@1` | ✅ Supported | `VSBuildTask` and `VSBuildTaskBuilder` provide a strongly typed model and fluent builder for `VSBuild@1`. |
| Xcode | `Xcode@5`, `Xcode@4` | ❌ Missing | No strongly typed model or builder. |
| Xcode Build | `Xcode@3`, `Xcode@2` | ⚪ Out of scope | Superseded majors of the `Xcode` task; only `Xcode@5` would be modelled. |
| Xcode Package iOS | `XcodePackageiOS@0` | ⚪ Out of scope | Deprecated by Microsoft (Xcode 7 and below). |

### Deploy tasks

| Task | YAML identity (all majors) | Classification | Sharpliner API / rationale |
|---|---|---|---|
| App Center distribute | `AppCenterDistribute@3`, `AppCenterDistribute@2`, `AppCenterDistribute@1`, `AppCenterDistribute@0` | ❌ Missing | No strongly typed model or builder. |
| ARM template deployment | `AzureResourceManagerTemplateDeployment@3` | ❌ Missing | No strongly typed model or builder. |
| Azure App Configuration Export | `AzureAppConfigurationExport@10` | ❌ Missing | No strongly typed model or builder. |
| Azure App Configuration Import | `AzureAppConfigurationImport@10` | ❌ Missing | No strongly typed model or builder. |
| Azure App Configuration Snapshot | `AzureAppConfigurationSnapshot@1` | ❌ Missing | No strongly typed model or builder. |
| Azure App Service Classic (Deprecated) | `AzureWebPowerShellDeployment@1` | ⚪ Out of scope | Deprecated by Microsoft; superseded by `AzureRmWebAppDeployment`. |
| Azure App Service deploy | `AzureRmWebAppDeployment@5`, `AzureRmWebAppDeployment@4`, `AzureRmWebAppDeployment@3`, `AzureRmWebAppDeployment@2` | ✅ Supported | `AzureAppServiceDeploy.*` builder + `AzureRmWebAppDeploymentPackageTask`, `AzureRmWebAppDeploymentContainerTask` and `AzureRmWebAppDeploymentPublishProfileTask`. Only the current `@5` major is modelled; the older majors take the same inputs apart from the Linux deployment options. |
| Azure App Service manage | `AzureAppServiceManage@0` | ❌ Missing | No strongly typed model or builder. |
| Azure App Service Settings | `AzureAppServiceSettings@1` | ❌ Missing | No strongly typed model or builder. |
| Azure CLI | `AzureCLI@3`, `AzureCLI@2`, `AzureCLI@1` | ✅ Supported | `AzureCliV3.Inline/File/FromFile/FromResourceFile` builders emit the current `AzureCLI@3` major; `AzureCli` retains the `AzureCLI@2` API. |
| Azure CLI Preview | `AzureCLI@0` | ⚪ Out of scope | Deprecated `AzureCLI@0` preview version. |
| Azure Cloud Service deployment | `AzureCloudPowerShellDeployment@2`, `AzureCloudPowerShellDeployment@1` | ❌ Missing | No strongly typed model or builder. |
| Azure Container Apps Deploy | `AzureContainerApps@1`, `AzureContainerApps@0` | ❌ Missing | No strongly typed model or builder. |
| Azure Database for MySQL deployment | `AzureMysqlDeployment@2`, `AzureMysqlDeployment@1` | ❌ Missing | No strongly typed model or builder. |
| Azure file copy | `AzureFileCopy@6`, `AzureFileCopy@5`, `AzureFileCopy@4`, `AzureFileCopy@3`, `AzureFileCopy@2`, `AzureFileCopy@1` | ❌ Missing | No strongly typed model or builder. |
| Azure Function on Kubernetes | `AzureFunctionOnKubernetes@1`, `AzureFunctionOnKubernetes@0` | ❌ Missing | No strongly typed model or builder. |
| Azure Functions Deploy | `AzureFunctionApp@2`, `AzureFunctionApp@1` | ❌ Missing | No strongly typed model or builder. |
| Azure Functions for container | `AzureFunctionAppContainer@1` | ❌ Missing | No strongly typed model or builder. |
| Azure Key Vault | `AzureKeyVault@2`, `AzureKeyVault@1` | ✅ Supported | `AzureKeyVault.DownloadSecrets(...)` -> `AzureKeyVaultTask` (`@2`). `AzureKeyVaultV1Task` and `AzureKeyVault.DownloadSecretsV1(...)` cover the deprecated `@1` major. |
| Azure Functions Deploy | `AzureFunctionApp@2`, `AzureFunctionApp@1` | ✅ Supported | `AzureFunctionApp.Windows`/`Linux`/`FlexConsumption` builders and `AzureFunctionAppV2Task`/`AzureFunctionAppV1Task` models. |
| Azure Functions for container | `AzureFunctionAppContainer@1` | ✅ Supported | `AzureFunctionApp.Container` builder and `AzureFunctionAppContainerV1Task` model. |
| Azure Key Vault | `AzureKeyVault@2`, `AzureKeyVault@1` | ❌ Missing | No strongly typed model or builder. |
| Azure Monitor alerts (Deprecated) | `AzureMonitorAlerts@0` | ⚪ Out of scope | Deprecated by Microsoft (classic Azure Monitor alerts). |
| Azure PowerShell | `AzurePowerShell@5`, `AzurePowerShell@4`, `AzurePowerShell@3`, `AzurePowerShell@2`, `AzurePowerShell@1` | ✅ Supported | `AzurePowerShell.Inline/File/FromFile/FromResourceFile` builders emit `AzurePowerShell@5`; `InlineAzurePowerShellV4Task`/`AzurePowerShellV4FileTask` emit `AzurePowerShell@4`. The `@3`, `@2` and `@1` majors are deprecated by Microsoft and are not modelled. |
| Azure resource group deployment | `AzureResourceGroupDeployment@2`, `AzureResourceGroupDeployment@1` | ❌ Missing | No strongly typed model or builder. |
| Azure Spring Apps | `AzureSpringCloud@0` | ❌ Missing | No strongly typed model or builder. |
| Azure SQL Database deployment | `SqlAzureDacpacDeployment@1` | ❌ Missing | No strongly typed model or builder. |
| Azure VM scale set deployment | `AzureVmssDeployment@1`, `AzureVmssDeployment@0` | ❌ Missing | No strongly typed model or builder. |
| Azure Web App | `AzureWebApp@1` | ❌ Missing | No strongly typed model or builder. |
| Azure Web App for Containers | `AzureWebAppContainer@1` | ❌ Missing | No strongly typed model or builder. |
| Bicep Deploy | `BicepDeploy@0` | ❌ Missing | No strongly typed model or builder. |
| Build machine image | `PackerBuild@1`, `PackerBuild@0` | ❌ Missing | No strongly typed model or builder. |
| Check Azure Policy compliance | `AzurePolicyCheckGate@0` | ❌ Missing | No strongly typed model or builder. |
| Chef | `Chef@1` | ❌ Missing | No strongly typed model or builder. |
| Chef Knife | `ChefKnife@1` | ❌ Missing | No strongly typed model or builder. |
| Copy files over SSH | `CopyFilesOverSSH@0` | ❌ Missing | No strongly typed model or builder. |
| Deploy to Kubernetes | `KubernetesManifest@1`, `KubernetesManifest@0` | ❌ Missing | No strongly typed model or builder. |
| IIS web app deploy | `IISWebAppDeploymentOnMachineGroup@0` | ❌ Missing | No strongly typed model or builder. |
| IIS Web App deployment (Deprecated) | `IISWebAppDeployment@1` | ⚪ Out of scope | Deprecated by Microsoft; superseded by `IISWebAppDeploymentOnMachineGroup@0`. |
| IIS web app manage | `IISWebAppManagementOnMachineGroup@0` | ❌ Missing | No strongly typed model or builder. |
| Invoke REST API | `InvokeRESTAPI@1`, `InvokeRESTAPI@0` | ❌ Missing | No strongly typed model or builder. |
| Kubectl | `Kubernetes@1`, `Kubernetes@0` | 🟡 Partial | `Kubernetes.ServiceConnection/AzureResourceManager/None` builders emit `Kubernetes@1`; `KubernetesV0Task` supports existing deprecated pipelines. |
| Manual intervention | `ManualIntervention@8` | ⚪ Out of scope | Classic release pipelines only, it cannot be used in YAML pipelines which are the only thing Sharpliner generates. |
| Manual validation | `ManualValidation@1`, `ManualValidation@0` | ❌ Missing | No strongly typed model or builder. |
| MySQL database deploy | `MysqlDeploymentOnMachineGroup@1` | ❌ Missing | No strongly typed model or builder. |
| Package and deploy Helm charts | `HelmDeploy@1`, `HelmDeploy@0` | ✅ Supported | `Helm.*` builder -> `HelmDeployTask` and its per-command specializations (`HelmDeploy@1`). The legacy `HelmDeploy@0` major is not modelled. |
| PowerShell on target machines | `PowerShellOnTargetMachines@3`, `PowerShellOnTargetMachines@2`, `PowerShellOnTargetMachines@1` | ❌ Missing | No strongly typed model or builder. |
| Service Fabric application deployment | `ServiceFabricDeploy@1` | ❌ Missing | No strongly typed model or builder. |
| Service Fabric Compose deploy | `ServiceFabricComposeDeploy@0` | ❌ Missing | No strongly typed model or builder. |
| SQL Server database deploy | `SqlDacpacDeploymentOnMachineGroup@0` | ❌ Missing | No strongly typed model or builder. |
| SQL Server database deploy (Deprecated) | `SqlServerDacpacDeployment@1` | ⚪ Out of scope | Deprecated by Microsoft; superseded by `SqlDacpacDeploymentOnMachineGroup@0`. |
| SSH | `SSH@0` | ❌ Missing | No strongly typed model or builder. |
| Windows machine file copy | `WindowsMachineFileCopy@2`, `WindowsMachineFileCopy@1` | ❌ Missing | No strongly typed model or builder. |

### Package tasks

| Task | YAML identity (all majors) | Classification | Sharpliner API / rationale |
|---|---|---|---|
| Cargo authenticate (for task runners) | `CargoAuthenticate@0` | ❌ Missing | No strongly typed model or builder. |
| CocoaPods | `CocoaPods@0` | ❌ Missing | No strongly typed model or builder. |
| Conda environment | `CondaEnvironment@1`, `CondaEnvironment@0` | ⚪ Out of scope | Deprecated by Microsoft; the docs recommend calling `conda` from a script step. |
| Download Github Npm Package | `DownloadGithubNpmPackage@1` | ⚪ Out of scope | Deprecated by Microsoft; superseded by `Npm@1` with a GitHub service connection. |
| Gradle Authenticate | `GradleAuthenticate@0` | ❌ Missing | No strongly typed model or builder. |
| Maven Authenticate | `MavenAuthenticate@0` | ❌ Missing | No strongly typed model or builder. |
| npm | `Npm@1`, `Npm@0` | ✅ Supported | `Npm.Install(...)`, `Npm.InstallFromFeed(...)`, `Npm.Ci(...)`, `Npm.CiFromFeed(...)`, `Npm.Custom(...)`, `Npm.CustomFromFeed(...)`, `Npm.PublishToExternalRegistry(...)`, `Npm.PublishToFeed(...)` -> typed `Npm@1` task models. |
| npm authenticate (for task runners) | `npmAuthenticate@0` | ✅ Supported | `Npm.Authenticate(...)` -> `NpmAuthenticateTask`. |
| NuGet | `NuGetCommand@2`, `NuGet@0` | ✅ Supported | `NuGet.*` builder (`Restore`, `Pack`, `Push`, `Custom`) -> `NuGetCommandTask` (`NuGetCommand@2`). The legacy `NuGet@0` major is not modelled. |
| NuGet authenticate | `NuGetAuthenticate@1`, `NuGetAuthenticate@0` | ✅ Supported | `NuGet.Authenticate(...)` -> `NuGetAuthenticateTask`. |
| NuGet Installer | `NuGetInstaller@0` | ⚪ Out of scope | Legacy task; superseded by `NuGetCommand@2` + `NuGetAuthenticate`, both supported. |
| NuGet packager | `NuGetPackager@0` | ⚪ Out of scope | Deprecated by Microsoft; superseded by `NuGetCommand@2` which is supported. |
| NuGet publisher | `NuGetPublisher@0` | ⚪ Out of scope | Deprecated by Microsoft; superseded by `NuGetCommand@2` which is supported. |
| NuGet Restore | `NuGetRestore@1` | ⚪ Out of scope | Legacy task; superseded by `NuGetCommand@2` which is supported. |
| PyPI publisher | `PyPIPublisher@0` | ❌ Missing | No strongly typed model or builder. |
| Python pip authenticate | `PipAuthenticate@1`, `PipAuthenticate@0` | ❌ Missing | No strongly typed model or builder. |
| Python twine upload authenticate | `TwineAuthenticate@1`, `TwineAuthenticate@0` | ❌ Missing | No strongly typed model or builder. |
| Universal packages | `UniversalPackages@1`, `UniversalPackages@0` | ✅ Supported | `Download.UniversalPackage(...)`/`Publish.UniversalPackage(...)` -> `UniversalPackagesV1DownloadTask`/`UniversalPackagesV1PublishTask` (`UniversalPackages@1`), plus `UniversalPackagesDownloadTask`/`UniversalPackagesPublishTask` (`UniversalPackages@0`) for backward compatibility. |

### Test tasks

| Task | YAML identity (all majors) | Classification | Sharpliner API / rationale |
|---|---|---|---|
| App Center test | `AppCenterTest@1` | ❌ Missing | No strongly typed model or builder. |
| Azure Load Testing | `AzureLoadTest@1` | ✅ Supported | `AzureLoadTestTask` (`AzureLoadTest@1`). |
| Azure Test Plan | `AzureTestPlan@0` | ❌ Missing | No strongly typed model or builder. |
| Container Structure Test | `ContainerStructureTest@0` | ❌ Missing | No strongly typed model or builder. |
| Mobile Center Test | `VSMobileCenterTest@0` | ⚪ Out of scope | Deprecated/renamed by Microsoft; superseded by `AppCenterTest@1`. |
| Publish code coverage results | `PublishCodeCoverageResults@2`, `PublishCodeCoverageResults@1` | ✅ Supported | `PublishCodeCoverageResultsTask` (`PublishCodeCoverageResults@2`). |
| Publish Test Results | `PublishTestResults@2`, `PublishTestResults@1` | ✅ Supported | `PublishTestResultsTask` (`PublishTestResults@2`). |
| Run functional tests | `RunVisualStudioTestsusingTestAgent@1` | ⚪ Out of scope | Deprecated by Microsoft; superseded by `VSTest`. |
| Visual Studio Test | `VSTest@3`, `VSTest@2`, `VSTest@1` | ❌ Missing | No strongly typed model or builder. |
| Visual Studio test agent deployment | `DeployVisualStudioTestAgent@2`, `DeployVisualStudioTestAgent@1` | ⚪ Out of scope | Deprecated by Microsoft; superseded by `VSTest`. |

### Tool tasks

| Task | YAML identity (all majors) | Classification | Sharpliner API / rationale |
|---|---|---|---|
| .NET Core SDK/runtime installer | `DotNetCoreInstaller@1`, `DotNetCoreInstaller@0` | ⚪ Out of scope | Superseded by `UseDotNet@2` which is supported through `DotNet.Install`. |
| Docker CLI installer | `DockerInstaller@0` | ❌ Missing | No strongly typed model or builder. |
| Duffle tool installer | `DuffleInstaller@0` | ❌ Missing | No strongly typed model or builder. |
| Go tool installer | `GoTool@0` | ❌ Missing | No strongly typed model or builder. |
| Helm tool installer | `HelmInstaller@1`, `HelmInstaller@0` | ❌ Missing | No strongly typed model or builder. |
| Install Azure Func Core Tools | `FuncToolsInstaller@0` | ❌ Missing | No strongly typed model or builder. |
| Java tool installer | `JavaToolInstaller@1`, `JavaToolInstaller@0` | ❌ Missing | No strongly typed model or builder. |
| Kubectl tool installer | `KubectlInstaller@0` | ❌ Missing | No strongly typed model or builder. |
| Kubelogin tool installer | `KubeloginInstaller@0` | ❌ Missing | No strongly typed model or builder. |
| NuGet tool installer | `NuGetToolInstaller@1`, `NuGetToolInstaller@0` | ❌ Missing | No strongly typed model or builder. |
| Use .NET Core | `UseDotNet@2` | ✅ Supported | `DotNet.Install.Sdk/Runtime/FromGlobalJson` -> `UseDotNetTask`. |
| Use Node.js ecosystem | `UseNode@1`, `NodeTool@0` | ❌ Missing | No strongly typed model or builder. |
| Use Python version | `UsePythonVersion@0` | ❌ Missing | No strongly typed model or builder. |
| Use Ruby version | `UseRubyVersion@0` | ❌ Missing | No strongly typed model or builder. |
| Visual Studio test platform installer | `VisualStudioTestPlatformInstaller@1` | ❌ Missing | No strongly typed model or builder. |

### Utility tasks

| Task | YAML identity (all majors) | Classification | Sharpliner API / rationale |
|---|---|---|---|
| Advanced Security Dependency Scanning | `AdvancedSecurity-Dependency-Scanning@1` | ❌ Missing | No strongly typed model or builder. |
| Archive files | `ArchiveFiles@2`, `ArchiveFiles@1` | ✅ Supported | `ArchiveFilesTask` (`ArchiveFiles@2`). |
| Azure Network Load Balancer | `AzureNLBManagement@1` | ❌ Missing | No strongly typed model or builder. |
| Bash | `Bash@3` | ✅ Supported | `Bash.Inline/File/FromFile/FromResourceFile` -> `InlineBashTask`/`BashFileTask` and the `bash` step shortcut. |
| Batch script | `BatchScript@1` | ❌ Missing | No strongly typed model or builder. |
| Cache | `Cache@2` | ❌ Missing | No strongly typed model or builder. |
| Cache (Beta) | `CacheBeta@1`, `CacheBeta@0` | ⚪ Out of scope | Beta predecessor of `Cache@2`. |
| Command Line | `CmdLine@2`, `CmdLine@1` | ✅ Supported | `Script.Inline/FromFile/FromResourceFile` -> `ScriptTask`, which serializes to the `script` step shortcut for `CmdLine@2`. |
| Copy and Publish Build Artifacts | `CopyPublishBuildArtifacts@1` | ⚪ Out of scope | Deprecated by Microsoft; superseded by `CopyFiles@2` + `PublishBuildArtifacts@1`. |
| Copy files | `CopyFiles@2`, `CopyFiles@1` | ✅ Supported | `CopyFilesTask` (`CopyFiles@2`). |
| cURL Upload Files | `cURLUploader@2`, `cURLUploader@1` | ❌ Missing | No strongly typed model or builder. |
| Decrypt file (OpenSSL) | `DecryptFile@1` | ❌ Missing | No strongly typed model or builder. |
| Delay | `Delay@1` | ❌ Missing | No strongly typed model or builder. |
| Delete files | `DeleteFiles@1` | ✅ Supported | `DeleteFilesTask` (`DeleteFiles@1`). |
| Deploy Azure Static Web App | `AzureStaticWebApp@0` | ❌ Missing | No strongly typed model or builder. |
| Download artifacts from file share | `DownloadFileshareArtifacts@1` | ❌ Missing | No strongly typed model or builder. |
| Download build artifacts | `DownloadBuildArtifacts@1`, `DownloadBuildArtifacts@0` | ❌ Missing | No strongly typed model or builder. |
| Download GitHub Release | `DownloadGitHubRelease@0` | ❌ Missing | No strongly typed model or builder. |
| Download package | `DownloadPackage@1`, `DownloadPackage@0` | ❌ Missing | No strongly typed model or builder. |
| Download Pipeline Artifacts | `DownloadPipelineArtifact@2`, `DownloadPipelineArtifact@1`, `DownloadPipelineArtifact@0` | ✅ Supported | `Download.Current/FromPipelineResource/SpecificBuild/LatestFromBranch/None` -> `DownloadTask` and the `download` step shortcut. |
| Download secure file | `DownloadSecureFile@1` | ✅ Supported | `Download.SecureFile` -> `DownloadSecureFileTask` (`DownloadSecureFile@1`). |
| Extract files | `ExtractFiles@1` | ✅ Supported | `ExtractFilesTask` (`ExtractFiles@1`). |
| File transform | `FileTransform@2`, `FileTransform@1` | ❌ Missing | No strongly typed model or builder. |
| FTP upload | `FtpUpload@2`, `FtpUpload@1` | ❌ Missing | No strongly typed model or builder. |
| GitHub Comment | `GitHubComment@0` | ❌ Missing | No strongly typed model or builder. |
| GitHub Release | `GitHubRelease@1`, `GitHubRelease@0` | ❌ Missing | No strongly typed model or builder. |
| Install Apple certificate | `InstallAppleCertificate@2`, `InstallAppleCertificate@1`, `InstallAppleCertificate@0` | ❌ Missing | No strongly typed model or builder. |
| Install Apple provisioning profile | `InstallAppleProvisioningProfile@1`, `InstallAppleProvisioningProfile@0` | ❌ Missing | No strongly typed model or builder. |
| Install SSH key | `InstallSSHKey@0` | ❌ Missing | No strongly typed model or builder. |
| Invoke Azure Function | `AzureFunction@2`, `AzureFunction@1`, `AzureFunction@0` | ❌ Missing | No strongly typed model or builder. |
| Jenkins download artifacts | `JenkinsDownloadArtifacts@2`, `JenkinsDownloadArtifacts@1` | ❌ Missing | No strongly typed model or builder. |
| Node.js tasks runner installer | `NodeTaskRunnerInstaller@0` | ❌ Missing | No strongly typed model or builder. |
| Notation | `Notation@0` | ❌ Missing | No strongly typed model or builder. |
| PowerShell | `PowerShell@2`, `PowerShell@1` | ✅ Supported | `Powershell.*`/`Pwsh.*` builders -> `InlinePowershellTask`/`PowershellFileTask`/`InlinePwshTask` and the `powershell`/`pwsh` step shortcuts. |
| Publish build artifacts | `PublishBuildArtifacts@1` | ❌ Missing | Only the newer `PublishPipelineArtifact@1` is modelled through the `publish` shortcut. |
| Publish Pipeline Artifacts | `PublishPipelineArtifact@1`, `PublishPipelineArtifact@0` | ✅ Supported | `Publish.Pipeline`/`Publish.FileShare` -> `PublishTask`, the `publish` step shortcut. |
| Publish Pipeline Metadata | `PublishPipelineMetadata@0` | ❌ Missing | No strongly typed model or builder. |
| Publish To Azure Service Bus | `PublishToAzureServiceBus@2`, `PublishToAzureServiceBus@1`, `PublishToAzureServiceBus@0` | ❌ Missing | No strongly typed model or builder. |
| Python script | `PythonScript@0` | ❌ Missing | No strongly typed model or builder. |
| Query Azure Monitor alerts | `AzureMonitor@1` | ❌ Missing | No strongly typed model or builder. |
| Query Classic Azure Monitor alerts | `AzureMonitor@0` | ⚪ Out of scope | Classic Azure Monitor alerts, superseded by `AzureMonitor@1`. |
| Query work items | `queryWorkItems@0` | ❌ Missing | No strongly typed model or builder. |
| Review App | `ReviewApp@0` | ❌ Missing | No strongly typed model or builder. |
| Service Fabric PowerShell | `ServiceFabricPowerShell@1` | ❌ Missing | No strongly typed model or builder. |
| Shell script | `ShellScript@2` | ❌ Missing | No strongly typed model or builder. |
| Update Service Fabric App Versions | `ServiceFabricUpdateAppVersions@1` | ❌ Missing | No strongly typed model or builder. |
| Update Service Fabric manifests | `ServiceFabricUpdateManifests@2` | ❌ Missing | No strongly typed model or builder. |

## Summary

| Category | Total | ✅ Supported | 🟡 Partial | ❌ Missing | ⚪ Out of scope |
|---|---|---|---|---|---|
| Build | 28 | 1 | 0 | 20 | 7 |
| Deploy | 50 | 1 | 1 | 42 | 6 |
| Build | 28 | 2 | 0 | 19 | 7 |
| Deploy | 50 | 0 | 1 | 43 | 6 |
| Package | 18 | 3 | 1 | 8 | 6 |
| Test | 10 | 3 | 0 | 4 | 3 |
| Tool | 15 | 1 | 0 | 13 | 1 |
| Utility | 47 | 10 | 0 | 34 | 3 |
| **Total** | **168** | **19** | **2** | **121** | **26** |

Sharpliner covers **21 of the 168** official built-in task families (19 fully, 2 partially).
Most of the covered tasks are the ones needed for .NET, NuGet and artifact workflows, which is where the
library grew from. The **121 missing** families are dominated by deploy tasks (Azure resources, Kubernetes,
| Utility | 47 | 9 | 0 | 35 | 3 |
| **Total** | **168** | **17** | **2** | **123** | **26** |

Sharpliner covers **19 of the 168** official built-in task families (17 fully, 2 partially).
Most of the covered tasks are the ones needed for .NET, NuGet and artifact workflows, which is where the
| Utility | 47 | 9 | 0 | 35 | 3 |
| **Total** | **168** | **17** | **2** | **123** | **26** |

Sharpliner covers **19 of the 168** official built-in task families (17 fully, 2 partially).
Most of the covered tasks are the ones needed for .NET, NuGet and artifact workflows, which is where the
| Utility | 47 | 9 | 0 | 35 | 3 |
| **Total** | **168** | **17** | **2** | **123** | **26** |

Sharpliner covers **19 of the 168** official built-in task families (17 fully, 2 partially).
Most of the covered tasks are the ones needed for .NET, NuGet and artifact workflows, which is where the
library grew from. The **123 missing** families are dominated by deploy tasks (Azure resources, Kubernetes,
Service Fabric) and by tool installers.

## Tasks we would like to see contributed

The list below groups the missing (and partially covered) tasks per category so that each item can be turned
into an individually actionable issue. Only the current major of each family is listed - that is the version a
new model should target.

When picking one up, a contribution is expected to contain:

- a `record` deriving from `AzureDevOpsTask` (or from an abstract base when the task has mutually exclusive
  command modes, like `NuGetCommandTask` does), placed in
  [`src/Sharpliner.Core/AzureDevOps/Model/Tasks/Marketplace`](../../src/Sharpliner.Core/AzureDevOps/Model/Tasks/Marketplace),
- optionally a builder in [`Tasks/Builders`](../../src/Sharpliner.Core/AzureDevOps/Model/Tasks/Builders)
  when the task has several distinct modes of operation,
- XML documentation on every public member, linking to the official task specification,
- serialization tests in `tests/Sharpliner.Core.Tests/AzureDevOps/Model/Tasks`,
- an updated public API export file (see [Generating the Public API](../../README.md#generating-the-public-api)),
- an update of this document and of the [definition reference](DefinitionReference.md).

Good candidates to start with, as they are the most commonly used ones in .NET pipelines, are `Cache@2`,
`Docker@2`, `PublishBuildArtifacts@1`, `DownloadBuildArtifacts@1`, `AzurePowerShell@5`,
`Docker@2`, `PublishBuildArtifacts@1`, `DownloadBuildArtifacts@1`, `AzureKeyVault@2`,
`PublishBuildArtifacts@1`, `DownloadBuildArtifacts@1`, `AzureKeyVault@2`, `AzurePowerShell@5`,
`Npm@1` and `VSTest@3`.

### Missing build tasks

- `AdvancedSecurity-Codeql-Analyze@1` – Advanced Security Perform CodeQL analysis
- `AdvancedSecurity-Publish@1` – Advanced Security Publish Results
- `AndroidSigning@3` – Android Signing
- `Ant@1` – Ant
- `AzureIoTEdge@2` – Azure IoT Edge
- `CMake@1` – CMake
- `ContainerBuild@0` – Container Build
- `DockerCompose@1` – Docker Compose
- `Go@0` – Go
- `Gradle@4` – Gradle
- `Grunt@0` – Grunt
- `gulp@1` – gulp
- `JenkinsQueueJob@2` – Jenkins queue job
- `Maven@4` – Maven
- `MSBuild@1` – MSBuild
- `Xcode@5` – Xcode

### Missing deploy tasks

- `AppCenterDistribute@3` – App Center distribute
- `AzureResourceManagerTemplateDeployment@3` – ARM template deployment
- `AzureAppConfigurationExport@10` – Azure App Configuration Export
- `AzureAppConfigurationImport@10` – Azure App Configuration Import
- `AzureAppConfigurationSnapshot@1` – Azure App Configuration Snapshot
- `AzureAppServiceManage@0` – Azure App Service manage
- `AzureAppServiceSettings@1` – Azure App Service Settings
- `AzureCloudPowerShellDeployment@2` – Azure Cloud Service deployment
- `AzureContainerApps@1` – Azure Container Apps Deploy
- `AzureMysqlDeployment@2` – Azure Database for MySQL deployment
- `AzureFileCopy@6` – Azure file copy
- `AzureFunctionOnKubernetes@1` – Azure Function on Kubernetes
- `AzureFunctionApp@2` – Azure Functions Deploy
- `AzureFunctionAppContainer@1` – Azure Functions for container
- `AzurePowerShell@5` – Azure PowerShell
- `AzureKeyVault@2` – Azure Key Vault
- `AzureResourceGroupDeployment@2` – Azure resource group deployment
- `AzureSpringCloud@0` – Azure Spring Apps
- `SqlAzureDacpacDeployment@1` – Azure SQL Database deployment
- `AzureVmssDeployment@1` – Azure VM scale set deployment
- `AzureWebApp@1` – Azure Web App
- `AzureWebAppContainer@1` – Azure Web App for Containers
- `BicepDeploy@0` – Bicep Deploy
- `PackerBuild@1` – Build machine image
- `AzurePolicyCheckGate@0` – Check Azure Policy compliance
- `Chef@1` – Chef
- `ChefKnife@1` – Chef Knife
- `CopyFilesOverSSH@0` – Copy files over SSH
- `KubernetesManifest@1` – Deploy to Kubernetes
- `IISWebAppDeploymentOnMachineGroup@0` – IIS web app deploy
- `IISWebAppManagementOnMachineGroup@0` – IIS web app manage
- `InvokeRESTAPI@1` – Invoke REST API
- `Kubernetes@1` – Kubectl
- `ManualValidation@1` – Manual validation
- `MysqlDeploymentOnMachineGroup@1` – MySQL database deploy
- `PowerShellOnTargetMachines@3` – PowerShell on target machines
- `ServiceFabricDeploy@1` – Service Fabric application deployment
- `ServiceFabricComposeDeploy@0` – Service Fabric Compose deploy
- `SqlDacpacDeploymentOnMachineGroup@0` – SQL Server database deploy
- `SSH@0` – SSH
- `WindowsMachineFileCopy@2` – Windows machine file copy

### Missing package tasks

- `CargoAuthenticate@0` – Cargo authenticate (for task runners)
- `CocoaPods@0` – CocoaPods
- `GradleAuthenticate@0` – Gradle Authenticate
- `MavenAuthenticate@0` – Maven Authenticate
- `Npm@1` – npm
- `PyPIPublisher@0` – PyPI publisher
- `PipAuthenticate@1` – Python pip authenticate
- `TwineAuthenticate@1` – Python twine upload authenticate

### Missing test tasks

- `AppCenterTest@1` – App Center test
- `AzureTestPlan@0` – Azure Test Plan
- `ContainerStructureTest@0` – Container Structure Test
- `VSTest@3` – Visual Studio Test

### Missing tool tasks

- `DockerInstaller@0` – Docker CLI installer
- `DuffleInstaller@0` – Duffle tool installer
- `GoTool@0` – Go tool installer
- `HelmInstaller@1` – Helm tool installer
- `FuncToolsInstaller@0` – Install Azure Func Core Tools
- `JavaToolInstaller@1` – Java tool installer
- `KubectlInstaller@0` – Kubectl tool installer
- `KubeloginInstaller@0` – Kubelogin tool installer
- `NuGetToolInstaller@1` – NuGet tool installer
- `UseNode@1` – Use Node.js ecosystem
- `UsePythonVersion@0` – Use Python version
- `UseRubyVersion@0` – Use Ruby version
- `VisualStudioTestPlatformInstaller@1` – Visual Studio test platform installer

### Missing utility tasks

- `AdvancedSecurity-Dependency-Scanning@1` – Advanced Security Dependency Scanning
- `AzureNLBManagement@1` – Azure Network Load Balancer
- `BatchScript@1` – Batch script
- `Cache@2` – Cache
- `cURLUploader@2` – cURL Upload Files
- `DecryptFile@1` – Decrypt file (OpenSSL)
- `Delay@1` – Delay
- `AzureStaticWebApp@0` – Deploy Azure Static Web App
- `DownloadFileshareArtifacts@1` – Download artifacts from file share
- `DownloadBuildArtifacts@1` – Download build artifacts
- `DownloadGitHubRelease@0` – Download GitHub Release
- `DownloadPackage@1` – Download package
- `FileTransform@2` – File transform
- `FtpUpload@2` – FTP upload
- `GitHubComment@0` – GitHub Comment
- `GitHubRelease@1` – GitHub Release
- `InstallAppleCertificate@2` – Install Apple certificate
- `InstallAppleProvisioningProfile@1` – Install Apple provisioning profile
- `InstallSSHKey@0` – Install SSH key
- `AzureFunction@2` – Invoke Azure Function
- `JenkinsDownloadArtifacts@2` – Jenkins download artifacts
- `NodeTaskRunnerInstaller@0` – Node.js tasks runner installer
- `Notation@0` – Notation
- `PublishBuildArtifacts@1` – Publish build artifacts
- `PublishPipelineMetadata@0` – Publish Pipeline Metadata
- `PublishToAzureServiceBus@2` – Publish To Azure Service Bus
- `PythonScript@0` – Python script
- `AzureMonitor@1` – Query Azure Monitor alerts
- `queryWorkItems@0` – Query work items
- `ReviewApp@0` – Review App
- `ServiceFabricPowerShell@1` – Service Fabric PowerShell
- `ShellScript@2` – Shell script
- `ServiceFabricUpdateAppVersions@1` – Update Service Fabric App Versions
- `ServiceFabricUpdateManifests@2` – Update Service Fabric manifests

## Refreshing this document

The audit is a point-in-time snapshot. To refresh it:

1. Download the current task index from
   <https://raw.githubusercontent.com/MicrosoftDocs/azure-devops-yaml-schema/main/task-reference/index.md>
   and use the `:::moniker range="=azure-pipelines"` section (it lists every task family with all its majors).
2. List the task identities Sharpliner emits, for example:

   ```bash
   grep -rhoE '"[A-Za-z][A-Za-z0-9._-]*@[0-9]+"' --include='*.cs' src/ | tr -d '"' | sort -u
   ```

3. Update the [inventory](#sharpliners-strongly-typed-task-inventory), the
   [support matrix](#support-matrix) and the [summary](#summary), and record the new fetch date in
   [Sources](#sources).
