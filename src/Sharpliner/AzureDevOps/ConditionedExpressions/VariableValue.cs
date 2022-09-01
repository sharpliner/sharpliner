using Sharpliner.AzureDevOps.ConditionedExpressions.Interfaces;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

public class VariableValue : IRuntimeExpression, ICompileTimeExpression, IMacroExpression
{
    public string VariableName { get; }

    internal VariableValue(string variableName)
    {
        VariableName = variableName;
    }

    public string RuntimeExpression => $"variables['{VariableName}']";

    public string CompileTimeExpression => Condition.ExpressionStart + $"variables.{VariableName}" + Condition.ExpressionEnd;

    public string MacroExpression => $"$({VariableName})";

    public override string ToString() => MacroExpression;

    public static implicit operator string(VariableValue value) => value.ToString();
}
