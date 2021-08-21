namespace Sharpliner.AzureDevOps
{
    /// <summary>
    /// Allows better syntax inside of the condition tree.
    /// </summary>
    public static class ConditionedDefinitions
    {
        public static ConditionedDefinition<Stage> Stage(this Condition<Stage> condition, Stage stage)
            => ConditionedDefinition.Link(condition, stage);

        public static ConditionedDefinition<Stage> Stage(this Condition<Stage> condition, ConditionedDefinition<Stage> stage)
            => ConditionedDefinition.Link(condition, stage);

        public static ConditionedDefinition<Stage> Stage(this ConditionedDefinition<Stage> condition, Stage stage)
        {
            condition.Definitions.Add(stage);
            return condition;
        }

        public static ConditionedDefinition<Step> Step(this Condition<Step> condition, Step step)
            => ConditionedDefinition.Link(condition, step);

        public static ConditionedDefinition<Step> Step(this Condition<Step> condition, ConditionedDefinition<Step> step)
            => ConditionedDefinition.Link(condition, step);

        public static ConditionedDefinition<Step> Step(this ConditionedDefinition<Step> condition, Step step)
        {
            condition.Definitions.Add(step);
            return condition;
        }

        public static ConditionedDefinition<Job> Job(this Condition<Job> condition, Job job)
            => ConditionedDefinition.Link(condition, job);

        public static ConditionedDefinition<Job> Job(this Condition<Job> condition, ConditionedDefinition<Job> job)
            => ConditionedDefinition.Link(condition, job);

        public static ConditionedDefinition<Job> Job(this ConditionedDefinition<Job> condition, Job job)
        {
            condition.Definitions.Add(job);
            return condition;
        }
    }
}
