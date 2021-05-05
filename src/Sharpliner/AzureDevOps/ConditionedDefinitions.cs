namespace Sharpliner.AzureDevOps
{
    /// <summary>
    /// Allows better syntax inside of the condition tree.
    /// </summary>
    public static class ConditionedDefinitions
    {
        public static ConditionedDefinition<Stage> Stage(this Condition<Stage> condition, Stage stage)
            => ConditionedDefinition.Link(condition, stage);

        public static ConditionedDefinition<Stage> Stage(this ConditionedDefinition<Stage> condition, Stage stage)
        {
            condition.Definitions.Add(stage);
            return condition;
        }

        public static ConditionedDefinition<Step> Step(this Condition<Step> condition, Step step)
            => ConditionedDefinition.Link(condition, step);

        public static ConditionedDefinition<Step> Step(this ConditionedDefinition<Step> condition, Step step)
        {
            condition.Definitions.Add(step);
            return condition;
        }
    }
}
