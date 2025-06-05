using System;
using System.Collections.Generic;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Defining dependencies between stages/jobs.
/// </summary>
public interface IDependsOn
{
    /// <summary>
    /// The name of the job / stage.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// List of names of other jobs / stages this job / stage depends on.
    /// </summary>
    DependsOn? DependsOn { get; }
}

/// <summary>
/// Represents dependencies between stages/jobs.
/// </summary>
/// <param name="values">The values to initialize the instance with.</param>
public class DependsOn(params string[] values) : ConditionedList<string>(values), IYamlConvertible
{
    /// <summary>
    /// Implicitly converts a string to a single dependency.
    /// </summary>
    /// <param name="value">The dependency name.</param>
    public static implicit operator DependsOn(string value) => [value];

    /// <summary>
    /// Implicitly converts a <see cref="ParameterReference"/> to a dependencies list.
    /// </summary>
    /// <param name="parameter">The parameter reference.</param>
    public static implicit operator DependsOn(ParameterReference parameter) => parameter.ToString();

    void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer) => throw new NotImplementedException();

    void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        if (Count is 1 && this[0].Definition is not null)
        {
            nestedObjectSerializer(this[0]);
        }
        else
        {
            // wr want to use the serialized version of the list
            nestedObjectSerializer(new List<AdoExpression<string>>(this));
        }
    }
}
