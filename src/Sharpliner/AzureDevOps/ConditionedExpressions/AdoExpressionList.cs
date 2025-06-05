using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

/// <summary>
/// This class is here only to override the Add() which is used for definition.
/// </summary>
public class AdoExpressionList<T> : List<AdoExpression<T>>
{
    /// <summary>
    /// Creates a new instance of <see cref="AdoExpressionList{T}"/>.
    /// </summary>
    public AdoExpressionList()
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="AdoExpressionList{T}"/> with the specified values.
    /// </summary>
    protected AdoExpressionList(IEnumerable<T> values)
        : base(values.Select(v => new AdoExpression<T>(v)))
    {
    }

    /// <summary>
    /// Adds a new item to the list.
    /// If the item is a <see cref="AdoExpression{T}"/> item, it will be marked as a list.
    /// </summary>
    public new void Add(AdoExpression<T> item)
    {
        base.Add(GetRootConditioned(item));
    }

    /// <summary>
    /// Gets or sets the item at the specified index.
    /// If the item is a <see cref="AdoExpression{T}"/> item, it will be marked as a list.
    /// </summary>
    public new AdoExpression<T> this[int index]
    {
        get => base[index];
        set => base[index] = GetRootConditioned(value);
    }

    private static AdoExpression<T> GetRootConditioned(AdoExpression<T> item)
    {
        // When we define a tree of conditional definitions, the expression returns
        // the leaf definition so we have to move up to the top-level definition
        while (item.Parent is AdoExpression<T> parent)
        {
            item = parent;
        }

        item.SetIsList(true);
        return item;
    }

    // Make sure we can for example assign common collection types into ConditionedList

    /// <summary>
    /// Implicitly converts a <see cref="List{T}"/> to a <see cref="AdoExpressionList{T}"/>.
    /// </summary>
    /// <param name="list">The list to convert.</param>
    public static implicit operator AdoExpressionList<T>(List<T> list) => [.. list];

    /// <summary>
    /// Implicitly converts an array to a <see cref="AdoExpressionList{T}"/>.
    /// </summary>
    /// <param name="values">The array to convert.</param>
    public static implicit operator AdoExpressionList<T>(T[] values) => [.. values];

    /// <summary>
    /// Implicitly converts a <see cref="ReadOnlyCollection{T}"/> to a <see cref="AdoExpressionList{T}"/>.
    /// </summary>
    /// <param name="values">The collection to convert.</param>
    public static implicit operator AdoExpressionList<T>(ReadOnlyCollection<T> values) => [.. values];

    /// <summary>
    /// Implicitly converts a single value to a <see cref="AdoExpressionList{T}"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator AdoExpressionList<T>(T value) => new([value]);
}
