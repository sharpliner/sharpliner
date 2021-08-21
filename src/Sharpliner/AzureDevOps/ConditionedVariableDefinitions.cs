using System;

namespace Sharpliner.AzureDevOps
{
    /// <summary>
    /// Allows the Variable() and Group() syntax inside of the condition tree.
    /// </summary>
    public static class ConditionedVariableDefinitions
    {
        public static ConditionedDefinition<VariableBase> Variable(this Condition condition, string name, string value)
            => ConditionedDefinition.Link<VariableBase>(condition, new Variable(name, value));

        public static ConditionedDefinition<VariableBase> Variable(this Condition condition, string name, bool value)
            => ConditionedDefinition.Link<VariableBase>(condition, new Variable(name, value));

        public static ConditionedDefinition<VariableBase> Variable(this Condition condition, string name, int value)
            => ConditionedDefinition.Link<VariableBase>(condition, new Variable(name, value));

        public static ConditionedDefinition<VariableBase> Group(this Condition condition, string name)
            => ConditionedDefinition.Link<VariableBase>(condition, new VariableGroup(name));

        public static ConditionedDefinition<Stage> Stage(this Condition condition, Stage stage)
            => ConditionedDefinition.Link(condition, stage);

        public static ConditionedDefinition<Job> Job(this Condition condition, Job job)
            => ConditionedDefinition.Link(condition, job);

        public static ConditionedDefinition<Step> Step(this Condition condition, Step step)
            => ConditionedDefinition.Link(condition, step);

        public static ConditionedDefinition<Stage> Stage(this Condition condition, ConditionedDefinition<Stage> stage)
            => ConditionedDefinition.Link(condition, stage);

        public static ConditionedDefinition<Job> Job(this Condition condition, ConditionedDefinition<Job> job)
            => ConditionedDefinition.Link(condition, job);

        public static ConditionedDefinition<Step> Step(this Condition condition, ConditionedDefinition<Step> step)
            => ConditionedDefinition.Link(condition, step);

        public static ConditionedDefinition<T> Template<T>(this Condition condition, string path, TemplateParameters parameters)
            => ConditionedDefinition.Link(condition, new Template<T>(condition: condition.ToString(), path: path, parameters));

        public static ConditionedDefinition<VariableBase> Variable(
            this ConditionedDefinition<VariableBase> conditionedDefinition,
            string name,
            string value)
        {
            conditionedDefinition.Definitions.Add(new Variable(name, value));
            return conditionedDefinition;
        }

        public static ConditionedDefinition<VariableBase> Variable(
            this ConditionedDefinition<VariableBase> conditionedDefinition,
            string name,
            bool value)
        {
            conditionedDefinition.Definitions.Add(new Variable(name, value));
            return conditionedDefinition;
        }

        public static ConditionedDefinition<VariableBase> Variable(
            this ConditionedDefinition<VariableBase> conditionedDefinition,
            string name,
            int value)
        {
            conditionedDefinition.Definitions.Add(new Variable(name, value));
            return conditionedDefinition;
        }

        public static ConditionedDefinition<VariableBase> Group(
            this ConditionedDefinition<VariableBase> conditionedDefinition,
            string name)
        {
            conditionedDefinition.Definitions.Add(new VariableGroup(name));
            return conditionedDefinition;
        }
    }
}
