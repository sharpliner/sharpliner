using System.Collections.Generic;
using System.Linq;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

public class InlineCondition
{
    private readonly string _condition;

    public InlineCondition(string condition)
    {
        _condition = condition;
    }

    public InlineCondition(string keyword, IEnumerable<StringOrVariableOrParameter> expressions)
    {
        _condition = $"{keyword}({string.Join(", ", expressions.Select(SerializeExpression).Select(Condition.WrapQuotes).Select(Condition.RemoveTags))})";
    }

    private string SerializeExpression(StringOrVariableOrParameter expression)
    {
        return expression.Match(
            str => str,
            variable => variable.RuntimeExpression,
            parameter => parameter.CompileTimeExpression
        );
    }

    public static implicit operator InlineCondition(Condition condition)
    {
        if (!string.IsNullOrEmpty(condition.Keyword))
        {
            return new InlineCondition(condition.Keyword, condition.Expressions);
        }

        return new InlineCondition(condition.ConditionString);

    }

    public override string ToString() => _condition;
}
