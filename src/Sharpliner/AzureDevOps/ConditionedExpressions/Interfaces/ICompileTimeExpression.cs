namespace Sharpliner.AzureDevOps.ConditionedExpressions.Interfaces;

/// <summary>
/// To be inherited by objects that support a Compile-Time expression.
/// E.g. A <see cref="ParameterReference"/> that can then be passed around and serialized as the compile time syntax ${{ parameters.ParameterName }}
/// </summary>
public interface ICompileTimeExpression
{
    string CompileTimeExpression { get; }
}
