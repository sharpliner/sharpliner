using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using Sharpliner.GraphTraversal;

namespace Sharpliner.Definition
{
    public static class SharplinerSerializer
    {
        public static ISerializer Serializer { get; } = InitializeSerializer();

        public static string Serialize(object data) => Serializer.Serialize(data);

        private static ISerializer InitializeSerializer()
        {
            var serializerBuilder = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithObjectGraphTraversalStrategyFactory((typeInspector, typeResolver, typeConverters, maximumRecursion) => new SharplinerGraphTraversalStrategy(typeInspector, typeResolver, maximumRecursion, CamelCaseNamingConvention.Instance))
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults);

            return serializerBuilder.Build();
        }
    }
}
