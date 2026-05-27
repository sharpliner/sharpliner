using System;

namespace Sharpliner.AzureDevOps.Expressions;

/// <summary>
/// Helpers for converting a scalar <see cref="AdoExpression{TSrc}"/> tree (including
/// If/ElseIf/Else branches) into a mirror <see cref="AdoExpression{TDst}"/> tree by
/// applying a converter function to each leaf value.
///
/// <para>
/// This is used by computed properties such as <c>JobBase.TimeoutInMinutes</c> that
/// need to expose a converted view (TimeSpan → int minutes) while preserving any
/// conditional <c>${{ if ... }}</c> branches authored on the source property.
/// </para>
/// </summary>
internal static class AdoExpressionConversion
{
    /// <summary>
    /// Mirrors the conditional structure of <paramref name="source"/> as a new
    /// <see cref="AdoExpression{TDst}"/> tree, applying <paramref name="convert"/>
    /// to each leaf value. Returns the leaf node equivalent to <paramref name="source"/>.
    /// </summary>
    public static AdoExpression<TDst>? ConvertScalar<TSrc, TDst>(
        AdoExpression<TSrc>? source,
        Func<TSrc, TDst> convert)
    {
        if (source is null)
        {
            return null;
        }

        // Walk to the topmost expression.
        AdoExpression sourceRoot = source;
        while (sourceRoot.Parent is not null)
        {
            sourceRoot = sourceRoot.Parent;
        }

        // No If/Else chain — single leaf.
        if (sourceRoot.Condition is not null || sourceRoot.Definitions.Count == 0)
        {
            var value = source.Definition;
            if (value is null)
            {
                return source.Condition is null
                    ? null
                    : new AdoExpression<TDst>(default!, source.Condition);
            }
            return source.Condition is null
                ? new AdoExpression<TDst>(convert(value))
                : new AdoExpression<TDst>(convert(value), source.Condition);
        }

        // Synthetic root (If/ElseIf/Else chain) — mirror all branches.
        var dstRoot = AdoExpression<TDst>.CreateEmpty();
        AdoExpression<TDst>? leaf = null;
        foreach (var srcBranch in sourceRoot.Definitions)
        {
            if (srcBranch is not AdoExpression<TSrc> typed || typed.Condition is null)
            {
                continue;
            }

            TDst dstValue = typed.Definition is null ? default! : convert(typed.Definition);
            var dstBranch = new AdoExpression<TDst>(dstValue, typed.Condition)
            {
                Parent = dstRoot
            };
            dstRoot.Definitions.Add(dstBranch);
            leaf = dstBranch;
        }

        return leaf;
    }
}
