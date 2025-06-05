using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Sharpliner.AzureDevOps.ConditionedExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Sharpliner.AzureDevOps;

internal static class TypedTemplateUtils<TParameters> where TParameters : class, new()
{
    internal static List<Parameter> ToParameters()
    {
        var result = new List<Parameter>();
        var defaultParameters = new TParameters();
        foreach (var property in typeof(TParameters).GetProperties())
        {
            var name = property.GetCustomAttribute<YamlMemberAttribute>()?.Alias ?? CamelCaseNamingConvention.Instance.Apply(property.Name);
            var defaultValue = property.GetValue(defaultParameters);
            var allowedValues = property.GetCustomAttribute<AllowedValuesAttribute>()?.Values;
            Parameter parameter = property.PropertyType switch
            {
                { } type when type.IsEnum => (Parameter)Activator.CreateInstance(typeof(EnumParameter<>).MakeGenericType(type), name, null, defaultValue)!,
                { } type when type == typeof(string) => new StringParameter(name, defaultValue: defaultValue as string, allowedValues: allowedValues?.Cast<string>()),
                { } type when type == typeof(bool?) || type == typeof(bool) => new BooleanParameter(name, defaultValue: defaultValue as bool?),
                { } type when type == typeof(int?) || type == typeof(int) => new NumberParameter(name, defaultValue: defaultValue as int?, allowedValues: allowedValues?.Cast<int?>()),
                { } type when type == typeof(Step) => new StepParameter(name, defaultValue: defaultValue as Step),
                { } type when type == typeof(AdoExpressionList<Step>) => new StepListParameter(name, defaultValue: defaultValue as AdoExpressionList<Step>),
                { } type when type.IsAssignableFrom(typeof(JobBase)) => new JobParameter(name, defaultValue: defaultValue as JobBase),
                { } type when type == typeof(AdoExpressionList<JobBase>) => new JobListParameter(name, defaultValue: defaultValue as AdoExpressionList<JobBase>),
                { } type when type == typeof(DeploymentJob) => new DeploymentParameter(name, defaultValue: defaultValue as DeploymentJob),
                { } type when type == typeof(AdoExpressionList<DeploymentJob>) => new DeploymentListParameter(name, defaultValue: defaultValue as AdoExpressionList<DeploymentJob>),
                { } type when type == typeof(Stage) => new StageParameter(name, defaultValue: defaultValue as Stage),
                { } type when type == typeof(AdoExpressionList<Stage>) => new StageListParameter(name, defaultValue: defaultValue as AdoExpressionList<Stage>),
                _ => new ObjectParameter(name),
            };

            parameter =  parameter with { DisplayName = property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName };
            result.Add(parameter);
        }

        return result;
    }

    internal static TemplateParameters ToTemplateParameters(TParameters parameters)
    {
        var result = new TemplateParameters();
        var defaultParameters = new TParameters();

        foreach (var property in typeof(TParameters).GetProperties())
        {
            var value = property.GetValue(parameters);
            var defaultValue = property.GetValue(defaultParameters);
            if (value is not null && !SharplinerSerializer.Serialize(value!).Equals(SharplinerSerializer.Serialize(defaultValue!)))
            {
                var name = property.GetCustomAttribute<YamlMemberAttribute>()?.Alias;
                name ??= CamelCaseNamingConvention.Instance.Apply(property.Name);

                result.Add(name, value);
            }
        }

        return result;
    }
}
