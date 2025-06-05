using System;

namespace Sharpliner.AzureDevOps.Expressions;

internal static class NotConditionHelper
{
    public static string NegateCondition(string condition)
    {
        if (condition.StartsWith("eq("))
        {
            return string.Concat("ne", condition.AsSpan(2));
        }

        if (condition.StartsWith("ne("))
        {
            return string.Concat("eq", condition.AsSpan(2));
        }

        if (condition.StartsWith("in("))
        {
            return string.Concat("notIn", condition.AsSpan(2));
        }

        if (condition.StartsWith("notIn("))
        {
            return string.Concat("in", condition.AsSpan(5));
        }

        return $"not({condition})";
    }
}
