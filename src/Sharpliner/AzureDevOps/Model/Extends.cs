namespace Sharpliner.AzureDevOps.Model;

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
    /// <param name="temlate">Path to the template that is being extended</param>
    public Extends(string temlate, TemplateParameters? parameters = null)
    {
        Template = temlate;
        Parameters = parameters;
    }
}
