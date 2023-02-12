using System.Diagnostics.CodeAnalysis;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// The webhooks resource enables you to integrate your pipeline with any external service and automate the workflow.
/// You can subscribe to any external events through its webhooks (GitHub, GitHub Enterprise, Nexus, Artifactory, and so on) and trigger your pipelines.
/// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/process/resources?view=azure-devops&amp;tabs=schema#define-a-builds-resource">official Azure DevOps pipelines documentation</see>.
/// </summary>
public record WebhookResource
{
    /// <summary>
    /// Identifier for the resource used in build resource variables
    /// </summary>
    [YamlMember(Alias = "webhook")]
    public string Identifier { get; }

    /// <summary>
    /// Service connection for your build service
    /// </summary>
    [DisallowNull]
    public Conditioned<string>? Connection { get; init; }

    /// <summary>
    /// Source definition of the build
    /// </summary>
    [DisallowNull]
    public ConditionedList<JsonParameterFilter> Filters { get; init; } = new();

    public WebhookResource(string identifier)
    {
        Identifier = identifier ?? throw new System.ArgumentNullException(nameof(identifier));
        Pipeline.ValidateName(Identifier);
    }
}

public record JsonParameterFilter
{
    /// <summary>
    /// JSON path in the payload
    /// </summary>
    public Conditioned<string> Path { get; }

    /// <summary>
    /// Expected value in the path provided
    /// </summary>
    public Conditioned<string> Value { get; init; }

    public JsonParameterFilter(string path, string value)
    {
        Path = path;
        Value = value;
    }
}
