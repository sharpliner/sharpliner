using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Sharpliner;

internal static class EnumUtils<TEnum> where TEnum : struct, Enum
{
    private static readonly Dictionary<string, TEnum> s_enumValues = Enum.GetValues<TEnum>()
        .ToDictionary(GetEnumValueName, x => x);

    public static TEnum Parse(string value) 
        => s_enumValues.TryGetValue(value, out var result) ? result : throw new ArgumentException($"Unknown value '{value}' for enum '{typeof(TEnum).Name}'");

    private static string GetEnumValueName(TEnum value)
    {
        var field = typeof(TEnum).GetField(value.ToString())!;
        return field.GetCustomAttribute<YamlMemberAttribute>(false)
            ?.Alias ?? CamelCaseNamingConvention.Instance.Apply(field.Name);
    }
}
