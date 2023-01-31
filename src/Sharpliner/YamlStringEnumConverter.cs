using System;
using System.Linq;
using System.Reflection;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner;

public class YamlStringEnumConverter : IYamlTypeConverter
{
    public bool Accepts(Type type) => type.IsEnum;

    public object ReadYaml(IParser parser, Type type) => throw new NotImplementedException();

    public void WriteYaml(IEmitter emitter, object? value, Type type)
    {
        if (value is null)
        {
            emitter.Emit(new Scalar(null!));
            return;
        }

        var enumMember = type.GetMember(value.ToString()!).FirstOrDefault();
        var yamlValue = enumMember?
                            .GetCustomAttributes<YamlMemberAttribute>(true)
                            .Select(ema => ema.Alias)
                            .FirstOrDefault()
                        ?? value.ToString();
        
        emitter.Emit(new Scalar(yamlValue!));
    }
}
