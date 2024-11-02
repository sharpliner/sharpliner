namespace Sharpliner.AzureDevOps.ConditionedExpressions.Interfaces;

/// <summary>
/// <para>
/// To be inherited by objects that support a Macro expression.
/// </para>
/// E.g. A <see cref="VariableReference"/> that can then be passed around and serialized as the macro syntax <c>$(VariableName)</c>.
/// </summary>
public interface IMacroExpression
{
    /// <summary>
    /// Gets the macro expression.
    /// </summary>
    string MacroExpression { get; }
}
