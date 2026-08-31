using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Tasks;

/// <summary>
/// <para>
/// Represents the <c>AzureLoadTest@1</c> task which automates performance regression testing with
/// <see href="https://learn.microsoft.com/en-us/azure/load-testing/">Azure Load Testing</see>.
/// </para>
/// <para>
/// The task runs an Apache JMeter script through an existing Azure Load Testing resource and succeeds when the load test finishes
/// and all <see href="https://learn.microsoft.com/en-us/azure/load-testing/how-to-define-test-criteria">test criteria</see> pass.
/// It is part of the
/// <see href="https://marketplace.visualstudio.com/items?itemName=AzloadTest.AzloadTesting">Azure Load Testing marketplace extension</see>,
/// which has to be installed in the organization before the task can be used.
/// </para>
/// <para>
/// More details can be found in the
/// <see href="https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/azure-load-test-v1?view=azure-pipelines">official Azure DevOps pipelines documentation</see>.
/// </para>
/// </summary>
/// <example>
/// <code lang="csharp">
/// new AzureLoadTestTask("my-azure-subscription", "loadtest.yaml", "my-resource-group", "my-load-test-resource")
/// {
///     EnvironmentVariables = [new("MYAPP_URL", "$(myAppUrl)")],
/// }
/// </code>
/// <para>Generated YAML:</para>
/// <code lang="yaml">
/// - task: AzureLoadTest@1
///   inputs:
///     azureSubscription: my-azure-subscription
///     loadTestConfigFile: loadtest.yaml
///     resourceGroup: my-resource-group
///     loadTestResource: my-load-test-resource
///     env: |-
///       [
///         {
///           "name": "MYAPP_URL",
///           "value": "$(myAppUrl)"
///         }
///       ]
/// </code>
/// </example>
public record AzureLoadTestTask : AzureDevOpsTask
{
    private readonly IReadOnlyCollection<AzureLoadTestVariable>? _environmentVariables;
    private readonly IReadOnlyCollection<AzureLoadTestVariable>? _secrets;

    /// <summary>
    /// Required <c>string</c> input (YAML alias <c>connectedServiceNameARM</c>).
    /// The Azure Resource Manager service connection of the subscription that hosts the load test resource.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? AzureSubscription
    {
        get => GetExpression<string>("azureSubscription");
        init => SetProperty("azureSubscription", value);
    }

    /// <summary>
    /// <para>
    /// Required <c>string</c> input. The path to the load test YAML configuration file, fully qualified or relative to the
    /// default working directory.
    /// </para>
    /// See the
    /// <see href="https://learn.microsoft.com/en-us/azure/load-testing/reference-test-config-yaml">test configuration YAML reference</see>
    /// for the supported contents of the file.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? LoadTestConfigFile
    {
        get => GetExpression<string>("loadTestConfigFile");
        init => SetProperty("loadTestConfigFile", value);
    }

    /// <summary>
    /// Required <c>string</c> input. The Azure resource group that contains the load test resource.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? ResourceGroup
    {
        get => GetExpression<string>("resourceGroup");
        init => SetProperty("resourceGroup", value);
    }

    /// <summary>
    /// Required <c>string</c> input. The name of an existing Azure Load Testing resource that runs the test.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? LoadTestResource
    {
        get => GetExpression<string>("loadTestResource");
        init => SetProperty("loadTestResource", value);
    }

    /// <summary>
    /// Optional <c>string</c> input. A custom name for the load test run.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? LoadTestRunName
    {
        get => GetExpression<string>("loadTestRunName");
        init => SetProperty("loadTestRunName", value);
    }

    /// <summary>
    /// Optional <c>string</c> input. A custom description for the load test run.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? LoadTestRunDescription
    {
        get => GetExpression<string>("loadTestRunDescription");
        init => SetProperty("loadTestRunDescription", value);
    }

    /// <summary>
    /// <para>
    /// Optional <c>string</c> input serialized into the <c>env</c> input. The environment variables passed to the test run,
    /// whose names have to match the variable names used in the Apache JMeter test script.
    /// </para>
    /// The values are emitted as the JSON array of <c>name</c>/<c>value</c> objects that the task expects.
    /// </summary>
    [YamlIgnore]
    public IReadOnlyCollection<AzureLoadTestVariable>? EnvironmentVariables
    {
        get => _environmentVariables;
        init
        {
            _environmentVariables = value;
            SetProperty("env", SerializeVariables(value));
        }
    }

    /// <summary>
    /// <para>
    /// Optional <c>string</c> input serialized into the <c>secrets</c> input. The secrets passed to the test run,
    /// whose names have to match the secret names used in the Apache JMeter test script.
    /// </para>
    /// <para>
    /// The values are emitted as the JSON array of <c>name</c>/<c>value</c> objects that the task expects.
    /// Always reference secret pipeline variables (for example <c>$(mySecret)</c>) instead of hardcoding secret values.
    /// </para>
    /// </summary>
    [YamlIgnore]
    public IReadOnlyCollection<AzureLoadTestVariable>? Secrets
    {
        get => _secrets;
        init
        {
            _secrets = value;
            SetProperty("secrets", SerializeVariables(value));
        }
    }

    /// <summary>
    /// <para>
    /// Optional <c>string</c> input. Overrides parameters of the load test YAML configuration file using the following JSON format:
    /// </para>
    /// <code lang="json">
    /// {
    ///   "testId": "testId",
    ///   "displayName": "displayName",
    ///   "description": "description",
    ///   "engineInstances": 1,
    ///   "autoStop": {
    ///     "errorPercentage": 90,
    ///     "timeWindow": 10
    ///   }
    /// }
    /// </code>
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? OverrideParameters
    {
        get => GetExpression<string>("overrideParameters");
        init => SetProperty("overrideParameters", value);
    }

    /// <summary>
    /// Optional <c>string</c> input. The name of the output variable that stores the test run ID for use in subsequent tasks.
    /// Default value: <c>ALTOutputVar</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<string>? OutputVariableName
    {
        get => GetExpression<string>("outputVariableName");
        init => SetProperty("outputVariableName", value);
    }

    /// <summary>
    /// Optional <c>boolean</c> input. Indicates whether to wait for the load test run to complete before proceeding.
    /// Default value: <c>true</c>.
    /// </summary>
    [YamlIgnore]
    public AdoExpression<bool>? WaitForCompletion
    {
        get => GetExpression<bool>("waitForCompletion");
        init => SetProperty("waitForCompletion", value);
    }

    /// <summary>
    /// Instantiates a new <see cref="AzureLoadTestTask"/> with the required inputs.
    /// </summary>
    /// <param name="azureSubscription">The Azure Resource Manager service connection used to run the load test.</param>
    /// <param name="loadTestConfigFile">The path to the load test YAML configuration file.</param>
    /// <param name="resourceGroup">The Azure resource group that contains the load test resource.</param>
    /// <param name="loadTestResource">The name of an existing Azure Load Testing resource.</param>
    public AzureLoadTestTask(
        AdoExpression<string> azureSubscription,
        AdoExpression<string> loadTestConfigFile,
        AdoExpression<string> resourceGroup,
        AdoExpression<string> loadTestResource)
        : base("AzureLoadTest@1")
    {
        AzureSubscription = azureSubscription;
        LoadTestConfigFile = loadTestConfigFile;
        ResourceGroup = resourceGroup;
        LoadTestResource = loadTestResource;
    }

    private static string? SerializeVariables(IReadOnlyCollection<AzureLoadTestVariable>? variables)
    {
        if (variables is null)
        {
            return null;
        }

        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
        {
            writer.WriteStartArray();

            foreach (var variable in variables)
            {
                writer.WriteStartObject();
                writer.WriteString("name", variable.Name);
                writer.WriteString("value", variable.Value);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }
}

/// <summary>
/// Represents a single environment variable or secret passed to the <see cref="AzureLoadTestTask"/>.
/// </summary>
/// <param name="Name">Name of the variable, has to match the name used in the Apache JMeter test script.</param>
/// <param name="Value">Value of the variable, can reference a pipeline variable such as <c>$(myVariable)</c>.</param>
public record AzureLoadTestVariable(string Name, string Value);
