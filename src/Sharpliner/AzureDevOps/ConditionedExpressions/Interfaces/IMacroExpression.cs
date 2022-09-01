namespace Sharpliner.AzureDevOps.ConditionedExpressions.Interfaces;

/// <summary>
/// To be inherited by objects that support a Macro expression.
/// E.g. A <see cref="VariableReference"/> that can then be passed around and serialized as the macro syntax $(VariableName)
/// </summary>
public interface IMacroExpression
{
    string MacroExpression { get; }
}
