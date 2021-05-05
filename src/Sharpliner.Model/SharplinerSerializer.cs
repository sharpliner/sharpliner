using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace Sharpliner.Model.Definition
{
    public static class SharplinerSerializer
    {
        private static readonly ISerializer s_serializer = InitializeSerializer();

        private static ISerializer InitializeSerializer()
        {
            var serializerBuilder = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults);

            return serializerBuilder.Build();
        }

        public static ISerializer Serializer => s_serializer;

        public static string Serialize(object data) => s_serializer.Serialize(data);
    }
}
