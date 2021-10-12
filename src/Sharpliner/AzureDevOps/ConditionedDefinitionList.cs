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
            // When we define a tree of conditional definitions, the expression returns
            // the leaf definition so we have to move up to the top-level definition
            while (item.Parent is ConditionedDefinition<T> parent)
            {
                item = parent;
            }

            SetIsListForAll(item);
            base.Add(item);
        }

        private static void SetIsListForAll(ConditionedDefinition? item)
        {
            if (item == null)
            {
                return;
            }

            item.IsList = true;

            if (item is ConditionedDefinition<T> temp && temp.Definition is ConditionedDefinition temp2)
            {
                SetIsListForAll(temp2);
            }

            foreach (var child in item.Definitions)
            {
                SetIsListForAll(child);
            }
        }
    }
}
