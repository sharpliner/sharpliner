using System.Collections.Generic;

namespace Sharpliner.AzureDevOps
{
    /// <summary>
    /// This class is here only to override the Add() which is used for definition.
    /// </summary>
    public class ConditionedDefinitionList<T> : List<ConditionedDefinition<T>>
    {
        public new void Add(ConditionedDefinition<T> item)
        {
            item.IsList = true;

            // When we define a tree of conditional definitions, the expression returns
            // the leaf definition so we have to move up to the top-level definition
            while (item.Parent != null)
            {
                item = (T)item.Parent;
                item.IsList = true;
            }

            base.Add(item);
        }
    }
}
