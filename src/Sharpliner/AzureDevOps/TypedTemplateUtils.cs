using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Sharpliner.AzureDevOps.Expressions;
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
                { } type when type == typeof(IEnumerable<string>) => new ArrayParameter<string>(name, defaultValue: defaultValue != null
                    ? new(((IEnumerable<string>)defaultValue).ToArray())
                    : null),
                { } type when type == typeof(List<string>) => new ArrayParameter<string>(name, defaultValue: defaultValue != null
                    ? new(((List<string>)defaultValue).ToArray())
                    : null),
                { } type when type == typeof(bool?) || type == typeof(bool) => new BooleanParameter(name, defaultValue: defaultValue as bool?),
                { } type when type == typeof(int?) || type == typeof(int) => new NumberParameter(name, defaultValue: defaultValue as int?, allowedValues: allowedValues?.Cast<int?>()),
                { } type when type == typeof(Step) => new StepParameter(name, defaultValue: defaultValue as Step),
                { } type when type == typeof(AdoExpressionList<Step>) => new StepListParameter(name, defaultValue: defaultValue as AdoExpressionList<Step>),
                { } type when type == typeof(DeploymentJob) => new DeploymentParameter(name, defaultValue: defaultValue as DeploymentJob),
                { } type when type.IsAssignableFrom(typeof(JobBase)) && type != typeof(object) => new JobParameter(name, defaultValue: defaultValue as JobBase),
                { } type when type == typeof(AdoExpressionList<JobBase>) => new JobListParameter(name, defaultValue: defaultValue as AdoExpressionList<JobBase>),
                { } type when type == typeof(AdoExpressionList<DeploymentJob>) => new DeploymentListParameter(name, defaultValue: defaultValue as AdoExpressionList<DeploymentJob>),
                { } type when type == typeof(Stage) => new StageParameter(name, defaultValue: defaultValue as Stage),
                { } type when type == typeof(AdoExpressionList<Stage>) => new StageListParameter(name, defaultValue: defaultValue as AdoExpressionList<Stage>),
                { } type when type.IsArray => ParseDefaultArrayParameter(name, defaultValue as Array),
                _ => ParseDefaultObjectParameter(name, defaultValue),
            };

            parameter = parameter with { DisplayName = property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName };
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

    private static ObjectParameter ParseDefaultObjectParameter(string name, object? defaultValue)
        => new(name, defaultValue: defaultValue != null
            ? new(ToDictionary(defaultValue))
            : null);

    // Helper method to convert an object's public properties to a dictionary
    private static Dictionary<string, object> ToDictionary(object obj)
    {
        var dict = new Dictionary<string, object>();
        foreach (var prop in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            dict[prop.Name] = prop.GetValue(obj)!;
        }
        return dict;
    }

    private static ArrayParameter<object?> ParseDefaultArrayParameter(string name, Array? defaultValue)
        => new(name, defaultValue: defaultValue != null
            ? new(defaultValue.Cast<object?>())
            : null);
}
