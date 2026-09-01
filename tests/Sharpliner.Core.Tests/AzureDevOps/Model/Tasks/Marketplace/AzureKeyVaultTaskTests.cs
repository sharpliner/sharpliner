using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class AzureKeyVaultTaskTests
{
    [Fact]
    public Task Serialize_AzureKeyVaultV2_Task_With_Defaults_Test()
    {
        var task = new AzureKeyVaultTask("MyServiceConnection", "MyKeyVault");

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_AzureKeyVaultV2_Task_With_Custom_Inputs_Test()
    {
        var task = new AzureKeyVaultTask("MyServiceConnection", "MyKeyVault", "DbPassword,SigningKey", true);

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_AzureKeyVaultV1_Task_Test()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        var task = new AzureKeyVaultV1Task("LegacyServiceConnection", "LegacyVault", "LegacySecret", false);
#pragma warning restore CS0618 // Type or member is obsolete

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
