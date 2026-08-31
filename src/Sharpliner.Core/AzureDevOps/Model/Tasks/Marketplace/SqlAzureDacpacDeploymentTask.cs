using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// Base type for the <c>SqlAzureDacpacDeployment@1</c> task.
/// See the <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/sql-azure-dacpac-deployment-v1">official Azure DevOps task reference</see>
/// and the <see href="https://raw.githubusercontent.com/microsoft/azure-pipelines-tasks/master/Tasks/SqlAzureDacpacDeploymentV1/task.json">task specification</see>.
/// </summary>
public abstract record SqlAzureDacpacDeploymentTask : AzureDevOpsTask
{
    /// <summary>Azure service connection type. The default is <see cref="SqlAzureServiceConnectionType.AzureResourceManager"/>.</summary>
    [YamlIgnore]
    public AdoExpression<SqlAzureServiceConnectionType>? AzureConnectionType { get => GetExpression<SqlAzureServiceConnectionType>("azureConnectionType"); init => SetProperty("azureConnectionType", value); }

    /// <summary>Azure Classic service connection. Use only with <see cref="SqlAzureServiceConnectionType.AzureClassic"/>.</summary>
    [YamlIgnore]
    public AdoExpression<string>? AzureClassicSubscription { get => GetExpression<string>("azureClassicSubscription"); init => SetProperty("azureClassicSubscription", value); }

    /// <summary>Azure Resource Manager service connection. This is the default connection type.</summary>
    [YamlIgnore]
    public AdoExpression<string>? AzureSubscription { get => GetExpression<string>("azureSubscription"); init => SetProperty("azureSubscription", value); }

    /// <summary>Authentication mode used to connect to the database.</summary>
    [YamlIgnore]
    public AdoExpression<SqlAzureAuthenticationType>? AuthenticationType { get => GetExpression<SqlAzureAuthenticationType>("AuthenticationType"); init => SetProperty("AuthenticationType", value); }

    /// <summary>Azure SQL Server name. Required for all authentication modes except <see cref="SqlAzureAuthenticationType.ConnectionString"/>.</summary>
    [YamlIgnore]
    public AdoExpression<string>? ServerName { get => GetExpression<string>("ServerName"); init => SetProperty("ServerName", value); }

    /// <summary>Azure SQL Database name. Required for all authentication modes except <see cref="SqlAzureAuthenticationType.ConnectionString"/>.</summary>
    [YamlIgnore]
    public AdoExpression<string>? DatabaseName { get => GetExpression<string>("DatabaseName"); init => SetProperty("DatabaseName", value); }

    /// <summary>SQL Server administrator login. Required with <see cref="SqlAzureAuthenticationType.Server"/>.</summary>
    [YamlIgnore]
    public AdoExpression<string>? SqlUsername { get => GetExpression<string>("SqlUsername"); init => SetProperty("SqlUsername", value); }

    /// <summary>SQL Server administrator password. Required with <see cref="SqlAzureAuthenticationType.Server"/>. Use a secret pipeline variable.</summary>
    [YamlIgnore]
    public AdoExpression<string>? SqlPassword { get => GetExpression<string>("SqlPassword"); init => SetProperty("SqlPassword", value); }

    /// <summary>Active Directory user name. Required with <see cref="SqlAzureAuthenticationType.AadAuthenticationPassword"/>.</summary>
    [YamlIgnore]
    public AdoExpression<string>? AadSqlUsername { get => GetExpression<string>("aadSqlUsername"); init => SetProperty("aadSqlUsername", value); }

    /// <summary>Active Directory password. Required with <see cref="SqlAzureAuthenticationType.AadAuthenticationPassword"/>. Use a secret pipeline variable.</summary>
    [YamlIgnore]
    public AdoExpression<string>? AadSqlPassword { get => GetExpression<string>("aadSqlPassword"); init => SetProperty("aadSqlPassword", value); }

    /// <summary>Connection string. Required with <see cref="SqlAzureAuthenticationType.ConnectionString"/>.</summary>
    [YamlIgnore]
    public AdoExpression<string>? ConnectionString { get => GetExpression<string>("ConnectionString"); init => SetProperty("ConnectionString", value); }

    /// <summary>Firewall rule configuration. Default is <see cref="SqlAzureIpDetectionMethod.AutoDetect"/>.</summary>
    [YamlIgnore]
    public AdoExpression<SqlAzureIpDetectionMethod>? IpDetectionMethod { get => GetExpression<SqlAzureIpDetectionMethod>("IpDetectionMethod"); init => SetProperty("IpDetectionMethod", value); }

    /// <summary>Start of the allowed IPv4 range. Required when <see cref="IpDetectionMethod"/> is <see cref="SqlAzureIpDetectionMethod.IpAddressRange"/>.</summary>
    [YamlIgnore]
    public AdoExpression<string>? StartIpAddress { get => GetExpression<string>("StartIpAddress"); init => SetProperty("StartIpAddress", value); }

    /// <summary>End of the allowed IPv4 range. Required when <see cref="IpDetectionMethod"/> is <see cref="SqlAzureIpDetectionMethod.IpAddressRange"/>.</summary>
    [YamlIgnore]
    public AdoExpression<string>? EndIpAddress { get => GetExpression<string>("EndIpAddress"); init => SetProperty("EndIpAddress", value); }

    /// <summary>Whether to delete the firewall rule after the task completes. Default is <c>true</c>.</summary>
    [YamlIgnore]
    public AdoExpression<bool>? DeleteFirewallRule { get => GetExpression<bool>("DeleteFirewallRule"); init => SetProperty("DeleteFirewallRule", value); }

    /// <summary>Configures SQL Server authentication, including its required server, database, login, and password inputs.</summary>
    public SqlAzureDacpacDeploymentTask WithSqlServerAuthentication(AdoExpression<string> serverName, AdoExpression<string> databaseName, AdoExpression<string> sqlUsername, AdoExpression<string> sqlPassword)
        => this with { AuthenticationType = SqlAzureAuthenticationType.Server, ServerName = serverName, DatabaseName = databaseName, SqlUsername = sqlUsername, SqlPassword = sqlPassword };

    /// <summary>Configures Active Directory password authentication, including its required server, database, login, and password inputs.</summary>
    public SqlAzureDacpacDeploymentTask WithActiveDirectoryPasswordAuthentication(AdoExpression<string> serverName, AdoExpression<string> databaseName, AdoExpression<string> username, AdoExpression<string> password)
        => this with { AuthenticationType = SqlAzureAuthenticationType.AadAuthenticationPassword, ServerName = serverName, DatabaseName = databaseName, AadSqlUsername = username, AadSqlPassword = password };

    /// <summary>Configures Active Directory integrated authentication, including its required server and database inputs.</summary>
    public SqlAzureDacpacDeploymentTask WithActiveDirectoryIntegratedAuthentication(AdoExpression<string> serverName, AdoExpression<string> databaseName)
        => this with { AuthenticationType = SqlAzureAuthenticationType.AadAuthenticationIntegrated, ServerName = serverName, DatabaseName = databaseName };

    /// <summary>Configures connection string authentication.</summary>
    public SqlAzureDacpacDeploymentTask WithConnectionStringAuthentication(AdoExpression<string> connectionString)
        => this with { AuthenticationType = SqlAzureAuthenticationType.ConnectionString, ConnectionString = connectionString };

    /// <summary>Configures an Azure Resource Manager service connection.</summary>
    public SqlAzureDacpacDeploymentTask WithAzureResourceManagerServiceConnection(AdoExpression<string> azureSubscription)
        => this with { AzureConnectionType = SqlAzureServiceConnectionType.AzureResourceManager, AzureSubscription = azureSubscription };

    /// <summary>Configures an Azure Classic service connection.</summary>
    public SqlAzureDacpacDeploymentTask WithAzureClassicServiceConnection(AdoExpression<string> azureClassicSubscription)
        => this with { AzureConnectionType = SqlAzureServiceConnectionType.AzureClassic, AzureClassicSubscription = azureClassicSubscription };

    /// <summary>Configures service principal authentication, including its required server and database inputs.</summary>
    public SqlAzureDacpacDeploymentTask WithServicePrincipalAuthentication(AdoExpression<string> serverName, AdoExpression<string> databaseName)
        => this with { AuthenticationType = SqlAzureAuthenticationType.ServicePrincipal, ServerName = serverName, DatabaseName = databaseName };

    /// <summary>Configures an explicit firewall IP range.</summary>
    public SqlAzureDacpacDeploymentTask WithFirewallIpRange(AdoExpression<string> startIpAddress, AdoExpression<string> endIpAddress)
        => this with { IpDetectionMethod = SqlAzureIpDetectionMethod.IpAddressRange, StartIpAddress = startIpAddress, EndIpAddress = endIpAddress };

    /// <summary>Initializes a SQL Azure deployment task with its package type and action.</summary>
    protected SqlAzureDacpacDeploymentTask(SqlAzureTaskNameSelector taskNameSelector, SqlAzureDeploymentAction deploymentAction)
        : base("SqlAzureDacpacDeployment@1")
    {
        SetProperty("TaskNameSelector", taskNameSelector);
        SetProperty("DeploymentAction", deploymentAction);
    }
}

/// <summary>Base type for DACPAC deployment actions.</summary>
public abstract record SqlAzureDacpacTask : SqlAzureDacpacDeploymentTask
{
    /// <summary>DACPAC file path. Required for Publish, Script, and DeployReport actions.</summary>
    [YamlIgnore]
    public AdoExpression<string>? DacpacFile { get => GetExpression<string>("DacpacFile"); init => SetProperty("DacpacFile", value); }

    /// <summary>BACPAC file path. Required for the Import action.</summary>
    [YamlIgnore]
    public AdoExpression<string>? BacpacFile { get => GetExpression<string>("BacpacFile"); init => SetProperty("BacpacFile", value); }

    /// <summary>Optional publish profile XML file path.</summary>
    [YamlIgnore]
    public AdoExpression<string>? PublishProfile { get => GetExpression<string>("PublishProfile"); init => SetProperty("PublishProfile", value); }

    /// <summary>Optional additional SqlPackage.exe arguments.</summary>
    [YamlIgnore]
    public AdoExpression<string>? AdditionalArguments { get => GetExpression<string>("AdditionalArguments"); init => SetProperty("AdditionalArguments", value); }

    /// <summary>Initializes a DACPAC task with its deployment action.</summary>
    protected SqlAzureDacpacTask(SqlAzureDeploymentAction deploymentAction) : base(SqlAzureTaskNameSelector.DacpacTask, deploymentAction) { }
}

/// <summary>Publishes a DACPAC to an Azure SQL Database.</summary>
public record SqlAzureDacpacPublishTask : SqlAzureDacpacTask
{
    /// <summary>Initializes a Publish task with the DACPAC file to publish.</summary>
    public SqlAzureDacpacPublishTask(AdoExpression<string> dacpacFile) : base(SqlAzureDeploymentAction.Publish) => DacpacFile = dacpacFile;
}

/// <summary>Extracts an Azure SQL Database to a DACPAC.</summary>
public record SqlAzureDacpacExtractTask() : SqlAzureDacpacTask(SqlAzureDeploymentAction.Extract);

/// <summary>Exports an Azure SQL Database to a BACPAC.</summary>
public record SqlAzureDacpacExportTask() : SqlAzureDacpacTask(SqlAzureDeploymentAction.Export);

/// <summary>Imports a BACPAC into an Azure SQL Database.</summary>
public record SqlAzureDacpacImportTask : SqlAzureDacpacTask
{
    /// <summary>Initializes an Import task with the BACPAC file to import.</summary>
    public SqlAzureDacpacImportTask(AdoExpression<string> bacpacFile) : base(SqlAzureDeploymentAction.Import) => BacpacFile = bacpacFile;
}

/// <summary>Generates a deployment script from a DACPAC.</summary>
public record SqlAzureDacpacScriptTask : SqlAzureDacpacTask
{
    /// <summary>Initializes a Script task with the DACPAC file from which to generate the script.</summary>
    public SqlAzureDacpacScriptTask(AdoExpression<string> dacpacFile) : base(SqlAzureDeploymentAction.Script) => DacpacFile = dacpacFile;
}

/// <summary>Generates a drift report for an Azure SQL Database.</summary>
public record SqlAzureDacpacDriftReportTask() : SqlAzureDacpacTask(SqlAzureDeploymentAction.DriftReport);

/// <summary>Generates a deployment report from a DACPAC.</summary>
public record SqlAzureDacpacDeployReportTask : SqlAzureDacpacTask
{
    /// <summary>Initializes a Deploy Report task with the DACPAC file to compare.</summary>
    public SqlAzureDacpacDeployReportTask(AdoExpression<string> dacpacFile) : base(SqlAzureDeploymentAction.DeployReport) => DacpacFile = dacpacFile;
}

/// <summary>Runs a SQL script file against an Azure SQL Database.</summary>
public record SqlAzureSqlScriptTask : SqlAzureDacpacDeploymentTask
{
    /// <summary>SQL script file path.</summary>
    [YamlIgnore]
    public AdoExpression<string>? SqlFile { get => GetExpression<string>("SqlFile"); init => SetProperty("SqlFile", value); }

    /// <summary>Optional additional Invoke-Sqlcmd arguments.</summary>
    [YamlIgnore]
    public AdoExpression<string>? SqlAdditionalArguments { get => GetExpression<string>("SqlAdditionalArguments"); init => SetProperty("SqlAdditionalArguments", value); }

    /// <summary>Initializes a task with the SQL script file to run.</summary>
    public SqlAzureSqlScriptTask(AdoExpression<string> sqlFile) : base(SqlAzureTaskNameSelector.SqlTask, SqlAzureDeploymentAction.Publish) => SqlFile = sqlFile;
}

/// <summary>Runs an inline SQL script against an Azure SQL Database.</summary>
public record SqlAzureInlineSqlTask : SqlAzureDacpacDeploymentTask
{
    /// <summary>Inline SQL script content.</summary>
    [YamlIgnore]
    public AdoExpression<string>? SqlInline { get => GetExpression<string>("SqlInline"); init => SetProperty("SqlInline", value); }

    /// <summary>Optional additional Invoke-Sqlcmd arguments.</summary>
    [YamlIgnore]
    public AdoExpression<string>? InlineAdditionalArguments { get => GetExpression<string>("InlineAdditionalArguments"); init => SetProperty("InlineAdditionalArguments", value); }

    /// <summary>Initializes a task with the inline SQL to run.</summary>
    public SqlAzureInlineSqlTask(AdoExpression<string> sqlInline) : base(SqlAzureTaskNameSelector.InlineSqlTask, SqlAzureDeploymentAction.Publish) => SqlInline = sqlInline;
}

/// <summary>Fluent entry point for creating valid SQL Azure deployment package types and actions.</summary>
public class SqlAzureDacpacDeploymentTaskBuilder
{
    /// <summary>Gets the DACPAC action builder.</summary>
    public SqlAzureDacpacBuilder Dacpac => new();

    /// <summary>Creates a task that runs a SQL script file. The task always uses the Publish action.</summary>
    public SqlAzureSqlScriptTask SqlScript(AdoExpression<string> sqlFile) => new(sqlFile);

    /// <summary>Creates a task that runs inline SQL. The task always uses the Publish action.</summary>
    public SqlAzureInlineSqlTask InlineSql(AdoExpression<string> sqlInline) => new(sqlInline);
}

/// <summary>Creates DACPAC tasks for the actions supported by <c>SqlAzureDacpacDeployment@1</c>.</summary>
public class SqlAzureDacpacBuilder
{
    /// <summary>Creates a Publish task.</summary>
    public SqlAzureDacpacPublishTask Publish(AdoExpression<string> dacpacFile) => new(dacpacFile);

    /// <summary>Creates an Extract task.</summary>
    public SqlAzureDacpacExtractTask Extract() => new();

    /// <summary>Creates an Export task.</summary>
    public SqlAzureDacpacExportTask Export() => new();

    /// <summary>Creates an Import task.</summary>
    public SqlAzureDacpacImportTask Import(AdoExpression<string> bacpacFile) => new(bacpacFile);

    /// <summary>Creates a Script task.</summary>
    public SqlAzureDacpacScriptTask Script(AdoExpression<string> dacpacFile) => new(dacpacFile);

    /// <summary>Creates a Drift Report task.</summary>
    public SqlAzureDacpacDriftReportTask DriftReport() => new();

    /// <summary>Creates a Deploy Report task.</summary>
    public SqlAzureDacpacDeployReportTask DeployReport(AdoExpression<string> dacpacFile) => new(dacpacFile);
}

/// <summary>Azure service connection types supported by the task.</summary>
public enum SqlAzureServiceConnectionType
{
    /// <summary>Azure Classic service connection.</summary>
    [YamlMember(Alias = "ConnectedServiceName")]
    AzureClassic,
    /// <summary>Azure Resource Manager service connection.</summary>
    [YamlMember(Alias = "ConnectedServiceNameARM")]
    AzureResourceManager,
}

/// <summary>Authentication modes supported by the task.</summary>
public enum SqlAzureAuthenticationType
{
    /// <summary>SQL Server administrator credentials.</summary>
    [YamlMember(Alias = "server")]
    Server,
    /// <summary>Active Directory user name and password.</summary>
    [YamlMember(Alias = "aadAuthenticationPassword")]
    AadAuthenticationPassword,
    /// <summary>Active Directory integrated authentication.</summary>
    [YamlMember(Alias = "aadAuthenticationIntegrated")]
    AadAuthenticationIntegrated,
    /// <summary>SQL Server connection string.</summary>
    [YamlMember(Alias = "connectionString")]
    ConnectionString,
    /// <summary>Service principal authentication.</summary>
    [YamlMember(Alias = "servicePrincipal")]
    ServicePrincipal,
}

/// <summary>Deployment actions supported for DACPAC tasks.</summary>
public enum SqlAzureDeploymentAction
{
    /// <summary>Publishes a DACPAC.</summary>
    Publish,
    /// <summary>Extracts a DACPAC.</summary>
    Extract,
    /// <summary>Exports a BACPAC.</summary>
    Export,
    /// <summary>Imports a BACPAC.</summary>
    Import,
    /// <summary>Generates a script.</summary>
    Script,
    /// <summary>Generates a drift report.</summary>
    DriftReport,
    /// <summary>Generates a deployment report.</summary>
    DeployReport,
}

/// <summary>Deployment package types supported by the task.</summary>
public enum SqlAzureTaskNameSelector
{
    /// <summary>DACPAC package.</summary>
    DacpacTask,
    /// <summary>SQL script file.</summary>
    SqlTask,
    /// <summary>Inline SQL script.</summary>
    InlineSqlTask,
}

/// <summary>Firewall rule configuration modes.</summary>
public enum SqlAzureIpDetectionMethod
{
    /// <summary>Automatically detects the agent IP range.</summary>
    AutoDetect,
    /// <summary>Uses an explicit IP address range.</summary>
    [YamlMember(Alias = "IPAddressRange")]
    IpAddressRange,
}
