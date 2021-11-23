using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// This class allows you to pass parameters to to templates.
/// To nest objects, insert another TemplateParameters value.
/// You can also specify a condition in the key and nest values conditionally.
/// </summary>
public class TemplateParameters : ConditionedDictionary
{
}
