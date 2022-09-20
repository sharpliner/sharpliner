using Sharpliner.AzureDevOps.ConditionedExpressions.Interfaces;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

public class StaticVariableReference : VariableReference, ICompileTimeExpression
{
    internal StaticVariableReference(string variableName) : base(variableName)
    {
    }

    public string CompileTimeExpression => Condition.ExpressionStart + RuntimeExpression + Condition.ExpressionEnd;
}
