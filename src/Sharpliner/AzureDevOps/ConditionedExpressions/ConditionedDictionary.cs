using System.Collections.Generic;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

/// <summary>
/// A dictionary that can contain <see cref="Conditioned"/> items.
/// </summary>
public class ConditionedDictionary : Dictionary<string, object>
{
    /// <summary>
    /// Adds a new item to the dictionary.
    /// If the item is a <see cref="Conditioned"/> item, it will be marked as a single item.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="item">The value.</param>
    public new void Add(string key, object item) => base.Add(key, NormalizeValue(item));

    public new object this[string key]
    {
        get => base[key];
        set => base[key] = NormalizeValue(value);
    }

    private static object NormalizeValue(object item)
    {
        if (item is Conditioned conditioned)
        {
            // When we define a tree of conditional definitions, the expression returns
            // the leaf definition so we have to move up to the root item
            while (conditioned.Parent is Conditioned parent)
            {
                conditioned = parent;
            }

            conditioned.SetIsList(false);
            return conditioned;
        }

        if (item is Variable variable)
        {
            return new VariableReference(variable.Name);
        }

        if (item is Parameter parameter)
        {
            return new ParameterReference(parameter.Name);
        }

        return item;
    }
}
