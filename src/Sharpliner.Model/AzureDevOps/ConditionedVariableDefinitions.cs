namespace Sharpliner.Model.AzureDevOps
{
    /// <summary>
    /// Allows the Variable() and Group() syntax inside of the condition tree.
    /// </summary>
    public static class ConditionedVariableDefinitions
    {
        public static ConditionedDefinition<VariableBase> Variable(this Condition condition, string name, string value)
            => new(new Variable(name, value), condition.ToString());

        public static ConditionedDefinition<VariableBase> Variable(this Condition condition, string name, bool value)
            => new(new Variable(name, value), condition.ToString());

        public static ConditionedDefinition<VariableBase> Variable(this Condition condition, string name, int value)
            => new(new Variable(name, value), condition.ToString());

        public static ConditionedDefinition<VariableBase> Group(this Condition condition, string name)
            => new(new VariableGroup(name), condition.ToString());

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
    }
}
