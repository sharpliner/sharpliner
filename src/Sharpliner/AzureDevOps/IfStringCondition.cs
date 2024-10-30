using Sharpliner.AzureDevOps.ConditionedExpressions;
using Sharpliner.AzureDevOps.ConditionedExpressions.Arguments;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Base class for string conditions.
/// </summary>
public abstract class IfStringCondition : IfCondition
{
    private readonly string _keyword;
    private readonly string _one;
    private readonly string _two;

    /// <summary>
    /// Creates a new instance of <see cref="IfStringCondition"/> with the specified keyword and expressions.
    /// </summary>
    /// <param name="keyword">The function keyword.</param>
    /// <param name="one">The first array expression.</param>
    /// <param name="two">The second expression.</param>
    protected IfStringCondition(string keyword, IfArrayExpression one, IfExpression two)
        : this(keyword, Serialize(one), two)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="IfStringCondition"/> with the specified keyword and expressions.
    /// </summary>
    /// <param name="keyword">The function keyword.</param>
    /// <param name="one">The first expression.</param>
    /// <param name="two">The second array expression.</param>
    protected IfStringCondition(string keyword, IfExpression one, IfArrayExpression two)
        : this(keyword, one, Serialize(two))
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="IfStringCondition"/> with the specified keyword and expressions.
    /// </summary>
    /// <param name="keyword">The function keyword.</param>
    /// <param name="one">The first expression.</param>
    /// <param name="two">The second expression.</param>
    protected IfStringCondition(string keyword, IfExpression one, IfExpression two)
    {
        _keyword = keyword;
        _one = Serialize(one);
        _two = Serialize(two);
    }

    internal override string Serialize() => WrapTag($"{_keyword}({WrapQuotes(_one)}, {WrapQuotes(_two)})");
}

public abstract class IfStringCondition<T> : IfCondition<T>
{
    private readonly string _keyword;
    private readonly string _one;
    private readonly string _two;

    protected IfStringCondition(string keyword, IfArrayExpression one, IfExpression two, Conditioned<T>? parent = null)
        : this(keyword, Serialize(one), two, parent)
    {
    }

    protected IfStringCondition(string keyword, IfExpression one, IfArrayExpression two, Conditioned<T>? parent = null)
        : this(keyword, one, Serialize(two), parent)
    {
    }

    protected IfStringCondition(string keyword, IfExpression one, IfExpression two, Conditioned<T>? parent = null)
    {
        _keyword = keyword;
        _one = Serialize(one);
        _two = Serialize(two);
        Parent = parent;
    }

    internal override string Serialize() => WrapTag($"{_keyword}({WrapQuotes(_one)}, {WrapQuotes(_two)})");
}
