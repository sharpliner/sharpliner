using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Sharpliner;

public static class SharplinerSerializer
{
    public static ISerializer Serializer { get; } = InitializeSerializer();

    public static string Serialize(object data) => Serializer.Serialize(data);

    private static ISerializer InitializeSerializer()
    {
        var serializerBuilder = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitEmptyCollections);

        return serializerBuilder.Build();
    }
}
