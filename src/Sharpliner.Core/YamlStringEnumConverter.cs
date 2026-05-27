using System;
using System.Linq;
using System.Reflection;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner;

internal class YamlStringEnumConverter : IYamlTypeConverter
{
    public bool Accepts(Type type) => type.IsEnum;

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer) => throw new NotImplementedException();

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer) => emitter.Emit(new Scalar(GetEnumValue(type, value)!));

    internal static string? GetEnumValue(Type type, object? value)
    {
        if (value is null)
        {
            return null;
        }

        var enumMember = type.GetMember(value.ToString()!).FirstOrDefault();
        return enumMember?
                   .GetCustomAttributes<YamlMemberAttribute>(true)
                   .Select(ema => ema.Alias)
                   .FirstOrDefault()
               ?? value.ToString()!;
    }
}
