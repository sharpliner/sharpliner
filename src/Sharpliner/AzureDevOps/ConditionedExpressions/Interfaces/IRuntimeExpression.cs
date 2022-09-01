namespace Sharpliner.AzureDevOps.ConditionedExpressions.Interfaces;

/// <summary>
/// To be inherited by objects that support a Runtime expression.
/// E.g. A <see cref="ParameterReference"/> that can then be passed around and serialized as the runtime syntax parameters.ParameterName
/// </summary>
public interface IRuntimeExpression
{
    string RuntimeExpression { get; }
}
