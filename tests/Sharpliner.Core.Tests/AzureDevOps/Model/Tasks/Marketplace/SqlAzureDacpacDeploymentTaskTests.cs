using Sharpliner.AzureDevOps.Tasks;

namespace Sharpliner.Tests.AzureDevOps;

public class SqlAzureDacpacDeploymentTaskTests
{
    [Fact]
    public Task Serialize_Dacpac_Publish_Task_With_Sql_Authentication_And_Firewall_Range_Test()
    {
        SqlAzureDacpacDeploymentTask task = new SqlAzureDacpacDeploymentTaskBuilder().Dacpac.Publish("database.dacpac")
            with
            {
                AdditionalArguments = "/p:BlockOnPossibleDataLoss=False",
                DeleteFirewallRule = false,
            };
        task = task
            .WithAzureResourceManagerServiceConnection("azure-service-connection")
            .WithSqlServerAuthentication("server.database.windows.net", "database", "administrator", "$(sqlPassword)")
            .WithFirewallIpRange("196.21.30.50", "196.21.30.65");

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Sql_Script_Task_With_Active_Directory_Authentication_Test()
    {
        SqlAzureDacpacDeploymentTask task = new SqlAzureDacpacDeploymentTaskBuilder().SqlScript("scripts/deploy.sql")
            with
            {
                SqlAdditionalArguments = "-ConnectionTimeout 100",
            };
        task = task.WithActiveDirectoryPasswordAuthentication("server.database.windows.net", "database", "user@contoso.com", "$(aadPassword)");

        return Verify(SharplinerSerializer.Serialize(task));
    }

    [Fact]
    public Task Serialize_Inline_Sql_Task_With_Connection_String_Authentication_Test()
    {
        SqlAzureDacpacDeploymentTask task = new SqlAzureDacpacDeploymentTaskBuilder().InlineSql("SELECT 1")
            with
            {
                InlineAdditionalArguments = "-OutputSqlErrors",
            };
        task = task.WithConnectionStringAuthentication("Server=server.database.windows.net;Database=database;");

        return Verify(SharplinerSerializer.Serialize(task));
    }
}
