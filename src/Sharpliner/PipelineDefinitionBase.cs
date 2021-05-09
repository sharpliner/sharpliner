using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Sharpliner.AzureDevOps;

namespace Sharpliner.Definition
{
    /// <summary>
    /// To define a pipeline, inherit from this class and implement the missing fields.
    /// The type will be located via reflection and the pipeline will be compiled into a YAML file.
    /// Define one pipeline child class per resulting file.
    /// </summary>
    public abstract class PipelineDefinitionBase
    {
        /// <summary>
        /// Path to the YAML file where this pipeline will be exported to.
        /// When you build the project, the pipeline will be saved into a file on this path.
        /// Example: "/pipelines/ci.yaml"
        /// </summary>
        public abstract string TargetFile { get; }

        /// <summary>
        /// Override this to define where the resulting YAML should be stored (together with TargetFile).
        /// Default is RelativeToCurrentDir.
        /// </summary>
        public virtual TargetPathType TargetPathType => TargetPathType.RelativeToCurrentDir;

        public virtual void Publish()
        {
            string fileName;
            switch (TargetPathType)
            {
                case TargetPathType.RelativeToGitRoot:
                    var currentDir = new DirectoryInfo(Directory.GetCurrentDirectory());
                    while (!Directory.Exists(Path.Combine(currentDir.FullName, ".git")))
                    {
                        currentDir = currentDir.Parent;

                        if (currentDir == null)
                        {
                            throw new Exception($"Failed to find git repository in {Directory.GetParent(Assembly.GetExecutingAssembly().Location)?.FullName}");
                        }
                    }

                    fileName = Path.Combine(currentDir.FullName, TargetFile);
                    break;

                case TargetPathType.RelativeToCurrentDir:
                    fileName = TargetFile;
                    break;

                case TargetPathType.RelativeToAssembly:
                    fileName = Path.Combine(Assembly.GetExecutingAssembly().Location, TargetFile);
                    break;

                case TargetPathType.Absolute:
                    if (!Path.IsPathRooted(TargetFile))
                    {
                        throw new Exception($"Failed to publish pipeline to {TargetFile}, path is not absolute.");
                    }

                    fileName = TargetFile;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(TargetPathType));
            }

            File.WriteAllText(fileName, Serialize());
        }

        /// <summary>
        /// Serializes the pipeline into a YAML string.
        /// </summary>
        protected abstract string Serialize();

        /// <summary>
        /// Allows the variables[""] notation for conditional definitions.
        /// </summary>
        protected readonly PipelineVariable variables = new();

        protected static Condition<T> And<T>(Condition condition1, Condition condition2) => new AndCondition<T>(condition1, condition2);

        protected static Condition Or<T>(Condition condition1, Condition condition2) => new OrCondition<T>(condition1, condition2);

        protected static Condition<T> Equal<T>(string expression1, string expression2) => new EqualityCondition<T>(expression1, expression2, true);

        protected static Condition<T> NotEqual<T>(string expression1, string expression2) => new EqualityCondition<T>(expression1, expression2, false);

        protected static Condition And(Condition condition1, Condition condition2) => new AndCondition(condition1, condition2);

        protected static Condition Or(Condition condition1, Condition condition2) => new OrCondition(condition1, condition2);

        protected static Condition Equal(string expression1, string expression2) => new EqualityDefinitionCondition(expression1, expression2, true);

        protected static Condition NotEqual(string expression1, string expression2) => new EqualityDefinitionCondition(expression1, expression2, false);
    }
}
