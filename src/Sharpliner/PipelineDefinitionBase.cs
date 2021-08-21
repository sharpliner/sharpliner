using System;
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

        public void Publish()
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

            string fileContents = Serialize();

            string[]? header = Header;
            if (header?.Length > 0)
            {
                const string hash = "### ";
                fileContents = hash + string.Join(Environment.NewLine + hash, header) + Environment.NewLine + Environment.NewLine + fileContents;
            }

            File.WriteAllText(fileName, fileContents);
        }

        /// <summary>
        /// Gets the path where YAML of this definition should be published to.
        /// </summary>
        public string GetTargetPath()
        {
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

                    return Path.Combine(currentDir.FullName, TargetFile);

                case TargetPathType.RelativeToCurrentDir:
                    return TargetFile;

                case TargetPathType.RelativeToAssembly:
                    return Path.Combine(Assembly.GetExecutingAssembly().Location, TargetFile);

                case TargetPathType.Absolute:
                    return TargetFile;

                default:
                    throw new ArgumentOutOfRangeException(nameof(TargetPathType));
            }
        }

        /// <summary>
        /// Validates the pipeline for runtime errors (e.g. wrong dependsOn, artifact name typos..).
        /// </summary>
        public abstract void Validate();

        /// <summary>
        /// Serializes the pipeline into a YAML string.
        /// </summary>
        public abstract string Serialize();

        /// <summary>
        /// Allows the variables[""] notation for conditional definitions.
        /// </summary>
        protected readonly PipelineVariable variables = new();

        /// <summary>
        /// Header that will be shown at the top of the generated YAML file.
        /// Leave null or empty to omit file header.
        /// </summary>
        protected virtual string[]? Header => new[]
        {
            string.Empty,
            "DO NOT MODIFY THIS FILE!",
            string.Empty,
            $"This YAML was auto-generated from { GetType().Name }.cs",
            $"To make changes, change the C# definition and rebuild its project",
            string.Empty,
        };

        protected static Condition<T> And<T>(Condition condition1, Condition condition2) => new AndCondition<T>(condition1, condition2);

        protected static Condition Or<T>(Condition condition1, Condition condition2) => new OrCondition<T>(condition1, condition2);

        protected static Condition<T> Equal<T>(string expression1, string expression2) => new EqualityCondition<T>(expression1, expression2, true);

        protected static Condition<T> NotEqual<T>(string expression1, string expression2) => new EqualityCondition<T>(expression1, expression2, false);

        protected static Condition And(Condition condition1, Condition condition2) => new AndCondition(condition1, condition2);

        protected static Condition Or(Condition condition1, Condition condition2) => new OrCondition(condition1, condition2);

        protected static Condition Equal(string expression1, string expression2) => new EqualityCondition(expression1, expression2, true);

        protected static Condition NotEqual(string expression1, string expression2) => new EqualityCondition(expression1, expression2, false);

        protected static Condition IsBranch(string branchName) => new BranchCondition(branchName, true);

        protected static Condition IsNotBranch(string branchName) => new BranchCondition(branchName, false);

        protected static Condition IsPullRequest => new BuildReasonCondition("PullRequest", true);

        protected static Condition IsNotPullRequest => new BuildReasonCondition("PullRequest", false);
    }
}
