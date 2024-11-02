namespace Sharpliner.AzureDevOps.ConditionedExpressions.Interfaces;

/// <summary>
/// <para>
/// To be inherited by objects that support a Runtime expression.
/// </para>
/// E.g. A <see cref="ParameterReference"/> that can then be passed around and serialized as the runtime syntax <c>parameters.ParameterName</c>.
/// </summary>
public interface IRuntimeExpression
{
    /// <summary>
    /// Gets the runtime expression.
    /// </summary>
    string RuntimeExpression { get; }
}
