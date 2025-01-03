using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

/// <summary>
/// This class is here only to override the Add() which is used for definition.
/// </summary>
public class ConditionedList<T> : List<Conditioned<T>>
{
    /// <summary>
    /// Creates a new instance of <see cref="ConditionedList{T}"/>.
    /// </summary>
    public ConditionedList()
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="ConditionedList{T}"/> with the specified values.
    /// </summary>
    protected ConditionedList(IEnumerable<T> values)
        : base(values.Select(v => new Conditioned<T>(v)))
    {
    }

    /// <summary>
    /// Adds a new item to the list.
    /// If the item is a <see cref="Conditioned{T}"/> item, it will be marked as a list.
    /// </summary>
    public new void Add(Conditioned<T> item)
    {
        base.Add(GetRootConditioned(item));
    }

    /// <summary>
    /// Gets or sets the item at the specified index.
    /// If the item is a <see cref="Conditioned{T}"/> item, it will be marked as a list.
    /// </summary>
    public new Conditioned<T> this[int index]
    {
        get => base[index];
        set => base[index] = GetRootConditioned(value);
    }

    private static Conditioned<T> GetRootConditioned(Conditioned<T> item)
    {
        // When we define a tree of conditional definitions, the expression returns
        // the leaf definition so we have to move up to the top-level definition
        while (item.Parent is Conditioned<T> parent)
        {
            item = parent;
        }

        item.SetIsList(true);
        return item;
    }

    // Make sure we can for example assign common collection types into ConditionedList

    /// <summary>
    /// Implicitly converts a <see cref="List{T}"/> to a <see cref="ConditionedList{T}"/>.
    /// </summary>
    /// <param name="list">The list to convert.</param>
    public static implicit operator ConditionedList<T>(List<T> list) => new(list);

    /// <summary>
    /// Implicitly converts an array to a <see cref="ConditionedList{T}"/>.
    /// </summary>
    /// <param name="values">The array to convert.</param>
    public static implicit operator ConditionedList<T>(T[] values) => new(values);

    /// <summary>
    /// Implicitly converts a <see cref="ReadOnlyCollection{T}"/> to a <see cref="ConditionedList{T}"/>.
    /// </summary>
    /// <param name="values">The collection to convert.</param>
    public static implicit operator ConditionedList<T>(ReadOnlyCollection<T> values) => new(values);

    /// <summary>
    /// Implicitly converts a single value to a <see cref="ConditionedList{T}"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator ConditionedList<T>(T value) => new([value]);
}
