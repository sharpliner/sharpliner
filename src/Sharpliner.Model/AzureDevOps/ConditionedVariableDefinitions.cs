using System;

namespace Sharpliner.Model.AzureDevOps
{
    /// <summary>
    /// Allows the Variable() and Group() syntax inside of the condition tree.
    /// </summary>
    public static class ConditionedVariableDefinitions
    {
        public static ConditionedDefinition<VariableBase> Variable(this Condition condition, string name, string value)
            => new(new Variable(name, value), condition.ToString())
            {
                Parent = condition.Parent ?? (condition as Condition<VariableBase>)?.Parent
            };

        public static ConditionedDefinition<VariableBase> Variable(this Condition condition, string name, bool value)
            => new(new Variable(name, value), condition.ToString())
            {
                Parent = condition.Parent ?? (condition as Condition<VariableBase>)?.Parent
            };

        public static ConditionedDefinition<VariableBase> Variable(this Condition condition, string name, int value)
            => new(new Variable(name, value), condition.ToString())
            {
                Parent = condition.Parent ?? (condition as Condition<VariableBase>)?.Parent
            };

        public static ConditionedDefinition<VariableBase> Group(this Condition condition, string name)
            => new(new VariableGroup(name), condition.ToString())
            {
                Parent = condition.Parent ?? (condition as Condition<VariableBase>)?.Parent
            };

        public static ConditionedDefinition<VariableBase> Variable(
            this ConditionedDefinition<VariableBase> condition,
            string name,
            string value)
        {
            condition.Definitions.Add(new Variable(name, value));
            return condition;
        }

        public static ConditionedDefinition<VariableBase> Variable(
            this ConditionedDefinition<VariableBase> condition,
            string name,
            bool value)
        {
            condition.Definitions.Add(new Variable(name, value));
            return condition;
        }

        public static ConditionedDefinition<VariableBase> Variable(
            this ConditionedDefinition<VariableBase> condition,
            string name,
            int value)
        {
            condition.Definitions.Add(new Variable(name, value));
            return condition;
        }

        public static ConditionedDefinition<VariableBase> Group(
            this ConditionedDefinition<VariableBase> condition,
            string name)
        {
            condition.Definitions.Add(new VariableGroup(name));
            return condition;
        }

        public static ConditionedDefinition<VariableBase> EndIf(this ConditionedDefinition<VariableBase> condition)
            => condition.Parent as ConditionedDefinition<VariableBase>
            ?? throw new InvalidOperationException("You have called EndIf on a top level statement. EndIf should be only used to return from a nested definition.");
    }
}
