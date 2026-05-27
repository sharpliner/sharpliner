using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.Expressions;

/// <summary>
/// A dictionary that can contain <see cref="AdoExpression"/> items.
/// </summary>
/// <remarks>
/// Template expression keys (those starting with <c>${{</c>, such as <c>${{ if ... }}</c> or
/// <c>${{ else }}</c>) are allowed to appear multiple times in the same mapping — this is required
/// to represent independent if/else groups inside a single inputs block.  Internally the class
/// makes duplicate keys unique by appending null-byte suffixes; when the YAML is emitted those
/// suffixes are stripped so the output contains the original key strings.
/// </remarks>
public class DictionaryExpression : Dictionary<string, object>, IYamlConvertible
{
    /// <summary>
    /// Initializes a new instance of the DictionaryExpression class.
    /// </summary>
    public DictionaryExpression()
    {
    }

    /// <summary>
    /// Initializes a new instance of the DictionaryExpression class using the specified dictionary as the source of
    /// key-value pairs.
    /// </summary>
    /// <param name="other">A dictionary containing the key-value pairs to initialize the expression with. Cannot be null.</param>
    public DictionaryExpression(Dictionary<string, object> other)
        : base(other)
    {
    }

    /// <summary>
    /// Adds a new item to the dictionary.
    /// If the item is a <see cref="AdoExpression"/> item, it will be marked as a single item.
    /// </summary>
    /// <remarks>
    /// When <paramref name="key"/> is a template expression key (starts with <c>${{</c>) and is
    /// already present, a unique variant of the key is stored internally so that both entries are
    /// preserved.  The original key is restored when the YAML is serialized.
    /// </remarks>
    /// <param name="key">The key.</param>
    /// <param name="item">The value.</param>
    public new void Add(string key, object item)
    {
        var processedValue = GetRootExpression(item);
        if (IsTemplateExpressionKey(key))
        {
            // Allow duplicate template-expression keys by appending null-byte suffixes to
            // make the internal dictionary key unique.  The suffix is stripped at serialization
            // time (see IYamlConvertible.Write), so the emitted YAML contains the original key.
            var uniqueKey = MakeUniqueKey(key);
            base.Add(uniqueKey, processedValue);
        }
        else
        {
            base.Add(key, processedValue);
        }
    }

    /// <summary>
    /// Gets or sets the item with the specified key.
    /// If the item is a <see cref="AdoExpression"/> item, it will be marked as a single item.
    /// </summary>
    /// <remarks>
    /// For template expression keys (those starting with <c>${{</c>) the setter always <b>appends</b>
    /// a new entry rather than replacing an existing one, so that multiple independent if/else groups
    /// inside the same mapping are all preserved.
    /// </remarks>
    /// <param name="key">The key.</param>
    /// <returns>The value</returns>
    public new object this[string key]
    {
        get => base[key];
        set
        {
            var processedValue = GetRootExpression(value);
            if (IsTemplateExpressionKey(key))
            {
                // For template-expression keys, append rather than replace so that independent
                // conditional groups (e.g. two separate if/else pairs) are all kept.
                var uniqueKey = MakeUniqueKey(key);
                base[uniqueKey] = processedValue;
            }
            else
            {
                base[key] = processedValue;
            }
        }
    }

    void IYamlConvertible.Read(IParser parser, System.Type expectedType, ObjectDeserializer nestedObjectDeserializer)
    {
        // Deserialization is not supported. Sharpliner is a code-first pipeline generation library;
        // DictionaryExpression objects are only ever written to YAML, never read back.
        throw new System.NotImplementedException("DictionaryExpression does not support YAML deserialization.");
    }

    /// <summary>
    /// Serializes all entries in insertion order, emitting the original (un-suffixed) key for
    /// every entry — including entries whose internal key has a uniquifying null-byte suffix.
    /// This allows duplicate template expression keys such as <c>${{ else }}</c> to appear
    /// multiple times in the output mapping, which is required by Azure Pipelines when a task
    /// has more than one independent if/else group inside its <c>inputs:</c> block.
    /// </summary>
    void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        emitter.Emit(new MappingStart(AnchorName.Empty, TagName.Empty, isImplicit: true, MappingStyle.Block));
        foreach (var kvp in this)
        {
            emitter.Emit(new Scalar(NormalizeKey(kvp.Key)));
            nestedObjectSerializer(kvp.Value, kvp.Value?.GetType() ?? typeof(object));
        }
        emitter.Emit(new MappingEnd());
    }

    // Returns true for Azure Pipelines template-expression keys such as "${{ if ... }}", "${{ else }}".
    private static bool IsTemplateExpressionKey(string key) => key.TrimStart().StartsWith("${{");

    // Strips the internal null-byte uniqueness suffix from a key, returning the original key string.
    private static string NormalizeKey(string key)
    {
        var idx = key.IndexOf('\x00');
        return idx >= 0 ? key[..idx] : key;
    }

    // Finds the shortest variant of key (key, key+\x00, key+\x00\x00, …) not yet present in the
    // underlying dictionary, and returns it.  This keeps all duplicate template-expression keys.
    private string MakeUniqueKey(string key)
    {
        var uniqueKey = key;
        while (base.ContainsKey(uniqueKey))
        {
            uniqueKey += '\x00';
        }
        return uniqueKey;
    }

    private static object GetRootExpression(object item)
    {
        if (item is AdoExpression conditioned)
        {
            // When we define a tree of conditional definitions, the expression returns
            // the leaf definition so we have to move up to the root item
            while (conditioned.Parent is AdoExpression parent)
            {
                conditioned = parent;
            }

            return conditioned;
        }

        return item;
    }
}
