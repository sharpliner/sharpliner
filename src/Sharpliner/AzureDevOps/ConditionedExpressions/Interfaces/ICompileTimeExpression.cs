namespace Sharpliner.AzureDevOps.ConditionedExpressions.Interfaces;

/// <summary>
/// <para>
/// To be inherited by objects that support a Compile-Time expression.
/// </para>
/// E.g. A <see cref="ParameterReference"/> that can then be passed around and serialized as the compile time syntax <c>${{ parameters.ParameterName }}</c>.
/// </summary>
public interface ICompileTimeExpression
{
    /// <summary>
    /// Gets the compile-time expression.
    /// </summary>
    string CompileTimeExpression { get; }
}
