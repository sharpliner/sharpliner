using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Sharpliner.AzureDevOps.Serialization;
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
        yaml = DecodeEscapedCodePoints(yaml);
        configuration ??= SharplinerConfiguration.Current;
        return configuration.Serialization.PrettifyYaml ? Prettify(yaml) : yaml;
    }

    private static ISerializer InitializeSerializer()
    {
        var serializerBuilder = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithTypeConverter(new YamlStringEnumConverter())
            .WithTypeInspector(inner => new ConditionalScalarTypeInspector(inner))
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults | DefaultValuesHandling.OmitEmptyCollections)
            .WithEventEmitter(nextEmitter => new MultilineStringEmitter(nextEmitter))
            .DisableAliases();

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
    private static readonly Regex s_conditionedBlockStartRegex = new("((\r?\n) {0,8}- ?\\${{ ?(if|else|each|parameters|variables|dependencies|stageDependencies)[^\n]+\n)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex s_doubleNewLineStartRegex = new("(:\r?\n\r?\n)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex s_escapedCodePointRegex = new(@"\\U([0-9a-fA-F]{8})", RegexOptions.Compiled);

    private static string DecodeEscapedCodePoints(string yaml) =>
        s_escapedCodePointRegex.Replace(yaml, match =>
        {
            if (!IsInsideDoubleQuotedScalar(yaml, match.Index))
            {
                return match.Value;
            }

            var precedingBackslashes = 0;
            for (var i = match.Index - 1; i >= 0 && yaml[i] == '\\'; i--)
            {
                precedingBackslashes++;
            }

            // Respect escaped backslashes. For example:
            // "\\U0001F6E0" should remain text and not be decoded to an emoji.
            if (precedingBackslashes % 2 != 0)
            {
                return match.Value;
            }

            if (!int.TryParse(match.Groups[1].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var codePoint))
            {
                return match.Value;
            }

            if (codePoint is > 0x10FFFF or (>= 0xD800 and <= 0xDFFF))
            {
                return match.Value;
            }

            return char.ConvertFromUtf32(codePoint);
        });

    private static bool IsInsideDoubleQuotedScalar(string yaml, int index)
    {
        var lineStart = yaml.LastIndexOf('\n', index);
        lineStart = lineStart == -1 ? 0 : lineStart + 1;

        var inQuotes = false;
        for (var i = lineStart; i < index; i++)
        {
            if (yaml[i] == '"' && (i == 0 || yaml[i - 1] != '\\'))
            {
                inQuotes = !inQuotes;
            }
        }

        return inQuotes;
    }
}
