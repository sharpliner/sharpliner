using System.Collections.Generic;

namespace Sharpliner.Model.ConditionedDefinitions
{
    /// <summary>
    /// This class is here only to override the Add() which is used for definition.
    /// </summary>
    public class ConditionedDefinitionList<T> : List<T> where T : ConditionedDefinition
    {
        public new void Add(T item)
        {
            // When we define a tree of conditional definitions, the expression returns
            // the leaf definition so we have to move up to the top-level definition
            while (item.Parent != null)
            {
                item = (T)item.Parent;
            }

            base.Add(item);
        }
    }
}
