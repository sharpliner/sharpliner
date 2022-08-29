using System.Linq;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

public class StringCondition : Condition
{
    protected StringCondition(string keyword, bool requireTwoPlus, params string[] expressions) : base(keyword, requireTwoPlus, expressions.Select(WrapQuotes))
    {
    }
}

public class StringCondition<T> : Condition<T>
{
    protected StringCondition(string keyword, bool requireTwoPlus, params string[] expressions) : base(keyword, requireTwoPlus, expressions.Select(WrapQuotes))
    {
    }
}
