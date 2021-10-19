using Sharpliner.AzureDevOps.ConditionedExpressions;

namespace Sharpliner.AzureDevOps
{
    public static class ConditionExtensions
    {
        public static Conditioned<VariableBase> Variable(this Condition condition, string name, string value)
            => Conditioned.Link<VariableBase>(condition, new Variable(name, value));

        public static Conditioned<VariableBase> Variable(this Condition condition, string name, bool value)
            => Conditioned.Link<VariableBase>(condition, new Variable(name, value));

        public static Conditioned<VariableBase> Variable(this Condition condition, string name, int value)
            => Conditioned.Link<VariableBase>(condition, new Variable(name, value));

        public static Conditioned<VariableBase> Group(this Condition condition, string name)
            => Conditioned.Link<VariableBase>(condition, new VariableGroup(name));

        public static Conditioned<Stage> Stage(this Condition condition, Stage stage)
            => Conditioned.Link(condition, stage);

        public static Conditioned<Stage> Stage(this Condition condition, Conditioned<Stage> stage)
            => Conditioned.Link(condition, stage);

        public static Conditioned<JobBase> Job(this Condition condition, JobBase job)
            => Conditioned.Link(condition, job);

        public static Conditioned<JobBase> Job(this Condition condition, Conditioned<JobBase> job)
            => Conditioned.Link(condition, job);

        public static Conditioned<JobBase> DeploymentJob(this Condition condition, JobBase job)
            => Conditioned.Link(condition, job);

        public static Conditioned<JobBase> DeploymentJob(this Condition condition, Conditioned<JobBase> job)
            => Conditioned.Link(condition, job);

        public static Conditioned<Step> Step(this Condition condition, Step step)
            => Conditioned.Link(condition, step);

        public static Conditioned<Step> Step(this Condition condition, Conditioned<Step> step)
            => Conditioned.Link(condition, step);

        public static Conditioned<Pool> Pool(this Condition condition, Pool pool)
            => Conditioned.Link(condition, pool);

        public static Conditioned<Pool> Pool(this Condition condition, Conditioned<Pool> pool)
            => Conditioned.Link(condition, pool);

        public static Conditioned<T> Template<T>(this Condition condition, string path, TemplateParameters? parameters = null)
            => Conditioned.Link(condition, new Template<T>(condition: condition.ToString(), path: path, parameters ?? new TemplateParameters()));

        public static Conditioned<Strategy> Strategy(this Condition condition, Strategy strategy)
            => Conditioned.Link(condition, strategy);
    }
}
