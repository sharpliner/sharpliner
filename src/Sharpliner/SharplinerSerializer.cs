using System;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Sharpliner;

/// <summary>
/// Serializer for Sharpliner objects.
/// </summary>
public static class SharplinerSerializer
{
    internal static ISerializer Serializer { get; } = InitializeSerializer();

    /// <summary>
    /// Serializes the given object to a YAML string. Can be used with <see cref="AzureDevOps.PipelineBase"/>
    /// </summary>
    /// <param name="data">The object to serialize</param>
    /// <param name="configuration">The optional configuration to use for serialization</param>
    /// <returns>The serialized YAML string</returns>
    public static string Serialize(object data, ISharplinerConfiguration? configuration = null)
    {
        var yaml = Serializer.Serialize(data);
        configuration ??= SharplinerConfiguration.Current;
        return configuration.Serialization.PrettifyYaml ? Prettify(yaml) : yaml;
    }

    private static ISerializer InitializeSerializer()
    {
        var serializerBuilder = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithTypeConverter(new YamlStringEnumConverter())
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitEmptyCollections)
            .WithEventEmitter(nextEmitter => new MultilineStringEmitter(nextEmitter));

        return serializerBuilder.Build();
    }

    internal static string Prettify(string yaml)
    {
        // Add empty new lines to make text more readable
        var newLineReplace = Environment.NewLine + "$1";
        yaml = s_sectionStartRegex.Replace(yaml, newLineReplace);
        yaml = s_mainItemStartRegex.Replace(yaml, newLineReplace);
        yaml = s_conditionedBlockStartRegex.Replace(yaml, newLineReplace);
        yaml = s_doubleNewLineStartRegex.Replace(yaml, ":" + Environment.NewLine);
        return yaml;
    }

    private static readonly Regex s_sectionStartRegex = new("((\r?\n)[a-zA-Z]+:)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex s_mainItemStartRegex = new("((\r?\n) {0,8}- ?[a-zA-Z]+@?[a-zA-Z\\.0-9]*:)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex s_conditionedBlockStartRegex = new("((\r?\n) {0,8}- ?\\${{ ?(if|else|each|parameters|variables|dependencies)[^\n]+\n)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex s_doubleNewLineStartRegex = new("(:\r?\n\r?\n)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
}
