namespace Sharpliner.AzureDevOps;

/// <summary>
/// Definition of the extends keyword in Azure DevOps pipelines.
/// https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/extends?view=azure-pipelines
/// </summary>
public record Extends
{
    /// <summary>
    /// Path to the template that is being extended
    /// </summary>
    public string Template { get; }

    /// <summary>
    /// Values for template parameters
    /// </summary>
    public TemplateParameters? Parameters { get; init; }

    /// <summary>
    /// Definition of the extends keyword in Azure DevOps pipelines.
    /// https://learn.microsoft.com/en-us/azure/devops/pipelines/yaml-schema/extends?view=azure-pipelines
    /// </summary>
    /// <param name="template">Path to the template that is being extended</param>
    /// <param name="parameters">Values for template parameters</param>
    public Extends(string template, TemplateParameters? parameters = null)
    {
        Template = template;
        Parameters = parameters;
    }
}
