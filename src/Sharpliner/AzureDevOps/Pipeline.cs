using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps
{
    public abstract record PipelineBase
    {
        protected static readonly Regex s_nameRegex = new("^[A-Za-z0-9_]+$", RegexOptions.Compiled);

        private Conditioned<Trigger>? _trigger;
        private Conditioned<PrTrigger>? _pr;
        private Conditioned<Resources>? _resources;

        /// <summary>
        /// Name of the pipline in the build numbering format
        /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/process/run-number?view=azure-devops&amp;tabs=yaml">official Azure DevOps pipelines documentation</see>.
        /// </summary>
        [YamlMember(Order = 100)]
        [DisallowNull]
        public string? Name { get; init; }

        /// <summary>
        /// Specifies when the pipeline is supposed to run
        /// </summary>
        [YamlMember(Order = 200)]
        [DisallowNull]
        public Conditioned<Trigger>? Trigger { get => _trigger; init => _trigger = value?.GetRoot(); }

        /// <summary>
        /// A pull request trigger specifies which branches cause a pull request build to run.
        /// If you specify no pull request trigger, pull requests to any branch trigger a build.
        /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/build/triggers?tabs=yaml&amp;view=azure-devops#pr-triggers">official Azure DevOps pipelines documentation</see>.
        /// </summary>
        [YamlMember(Order = 300)]
        [DisallowNull]
        public Conditioned<PrTrigger>? Pr { get => _pr; init => _pr = value?.GetRoot(); }

        /// <summary>
        /// A resource is any external service that is consumed as part of your pipeline
        /// More details can be found in <see href="https://docs.microsoft.com/en-us/azure/devops/pipelines/process/resources?view=azure-devops&amp;tabs=schema">official Azure DevOps pipelines documentation</see>.
        /// </summary>
        [YamlMember(Order = 400)]
        [DisallowNull]
        public Conditioned<Resources>? Resources { get => _resources; init => _resources = value?.GetRoot(); }

        /// <summary>
        /// Specifies variables at the pipeline level
        /// You can add hard-coded values directly, reference variable groups, or insert via variable templates.
        /// </summary>
        [YamlMember(Order = 500)]
        public ConditionedList<VariableBase> Variables { get; } = new();

        public abstract void Validate();

        protected static void ValidateDependsOn<T>(ConditionedList<T> definitions) where T : IDependsOn
        {
            var allDefs = definitions.SelectMany(s => s.FlattenDefinitions());

            var duplicate = allDefs.FirstOrDefault(d => allDefs.Count(o => o.Name == d.Name) > 1);
            if (duplicate is not null)
            {
                throw new Exception($"Found duplicate {typeof(T).Name.ToLower()} name '{duplicate.Name}'");
            }

            var invalidName = allDefs.FirstOrDefault(d => !s_nameRegex.IsMatch(d.Name));
            if (invalidName is not null)
            {
                throw new Exception($"Invalid character found in {typeof(T).Name.ToLower()} name '{invalidName.Name}', only A-Z, a-z, 0-9, and underscore are allowed");
            }

            foreach (var definition in allDefs)
            {
                if (definition.DependsOn is EmptyDependsOn)
                {
                    continue;
                }

                foreach (var dependsOn in definition.DependsOn)
                {
                    if (dependsOn == definition.Name)
                    {
                        throw new Exception($"{typeof(T).Name} `{definition.Name}` depends on itself");
                    }

                    if (!allDefs.Any(d => d.Name == dependsOn))
                    {
                        throw new Exception($"{typeof(T).Name} `{definition.Name}` depends on stage `{dependsOn}` which was not found");
                    }
                }
            }

            // TODO: Validate circular dependencies?
        }
    }

    public record Pipeline : PipelineBase
    {
        [YamlMember(Order = 600)]
        public ConditionedList<Stage> Stages { get; init; } = new();

        public override void Validate()
        {
            ValidateDependsOn(Stages);

            foreach (var stage in Stages.SelectMany(s => s.FlattenDefinitions()))
            {
                ValidateDependsOn(stage.Jobs);
            }
        }

        internal static void ValidateName(string name)
        {
            if (!s_nameRegex.IsMatch(name))
            {
                throw new FormatException($"Invalid identifier '{name}'! Only A-Z, a-z, 0-9, and underscore are allowed.");
            }
        }
    }

    public record SingleStagePipeline : PipelineBase
    {
        [YamlMember(Order = 600)]
        public ConditionedList<Job> Jobs { get; init; } = new();

        public override void Validate()
        {
            ValidateDependsOn(Jobs);
        }
    }
}
