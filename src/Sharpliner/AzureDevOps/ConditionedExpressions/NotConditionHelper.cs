using System;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

internal static class NotConditionHelper
{
    public static string NegateCondition(string condition)
    {
        if (condition.StartsWith("eq("))
        {
            return string.Concat("ne", condition.Substring(2));
        }

        if (condition.StartsWith("ne("))
        {
            return string.Concat("eq", condition.Substring(2));
        }

        if (condition.StartsWith("in("))
        {
            return string.Concat("notIn", condition.Substring(2));
        }

        if (condition.StartsWith("notIn("))
        {
            return string.Concat("in", condition.Substring(5));
        }

        return $"not({condition})";
    }
}
