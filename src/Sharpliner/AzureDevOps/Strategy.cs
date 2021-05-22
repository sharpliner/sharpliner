using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#strategies
    /// </summary>
    public abstract record Strategy
    {
        /// <summary>
        /// The maximum number of simultaneous matrix legs to run at once.
        /// If unspecified or set to 0, no limit is applied.
        /// </summary>
        public int MaxParallel { get; init; } = 0;
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#matrix
    /// </summary>
    public record MatrixStrategy : Strategy, IYamlConvertible
    {
        /// <summary>
        /// Defines copies of a job, each with different input.
        /// </summary>
        public Dictionary<string, (string, string)[]> Matrix { get; init; } = new();

        public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer) => throw new NotImplementedException();
        public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        {
            emitter.Emit(new MappingStart());
            emitter.Emit(new Scalar("matrix"));
            emitter.Emit(new MappingStart());

            foreach (KeyValuePair<string, (string, string)[]> pair in Matrix)
            {
                emitter.Emit(new Scalar(pair.Key));
                emitter.Emit(new MappingStart());

                foreach (var variable in pair.Value)
                {
                    emitter.Emit(new Scalar(variable.Item1));
                    emitter.Emit(new Scalar(variable.Item2));
                }

                emitter.Emit(new MappingEnd());
            }

            emitter.Emit(new MappingEnd());

            if (MaxParallel != 0)
            {
                emitter.Emit(new Scalar("maxParallel"));
                emitter.Emit(new Scalar(MaxParallel.ToString()));
            }

            emitter.Emit(new MappingEnd());
        }
    }

    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/devops/pipelines/yaml-schema?view=azure-devops&tabs=schema%2Cparameter-schema#parallel
    /// </summary>
    public record ParallelStrategy : Strategy
    {
        /// <summary>
        /// Specifies how many duplicates of a job should run
        /// </summary>
        public int Parallel { get; init; }
    }
}
