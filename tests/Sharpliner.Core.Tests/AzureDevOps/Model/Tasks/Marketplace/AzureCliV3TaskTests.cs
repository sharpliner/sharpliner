using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class AzureCliV3TaskTests
{
    [Fact]
    public Task Serialize_AzureResourceManager_Inline_Bash_Task()
    {
        var task = new InlineAzureCliV3Task(
            AzureCliV3ConnectionType.AzureResourceManager,
            "azure-resource-manager",
            ScriptType.Bash,
            "az group list")
        {
            ScriptArguments = "--output table",
            AddSpnToEnvironment = true,
            UseGlobalConfig = true,
            Cwd = "src",
            FailOnStandardError = true,
            VisibleAzLogin = false,
            AllowNoSubscriptions = true,
            KeepAzSessionActive = true,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_AzureDevOps_File_PowerShellCore_Task()
    {
        var task = new AzureCliV3FileTask(
            AzureCliV3ConnectionType.AzureDevOps,
            "azure-devops",
            ScriptType.Pscore,
            "scripts/deploy.ps1")
        {
            PowerShellErrorActionPreference = PowerShellErrorActionPreference.Continue,
            PowerShellIgnoreLASTEXITCODE = true,
        };

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
