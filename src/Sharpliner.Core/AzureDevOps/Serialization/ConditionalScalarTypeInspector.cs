using System;
using System.Collections.Generic;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.TypeInspectors;

namespace Sharpliner.AzureDevOps.Serialization;

/// <summary>
/// A YamlDotNet <see cref="ITypeInspector"/> decorator that handles conditional
/// <see cref="AdoExpression{T}"/> properties where <c>T</c> is a scalar (primitive,
/// enum, string, <see cref="TimeSpan"/>, or <see cref="decimal"/>).
///
/// <para>
/// YamlDotNet emits the property key before invoking the value's serializer, and the
/// value-position serializer cannot inject sibling keys into the parent mapping. For
/// mapping-typed properties (e.g. <c>Pool</c>) this is fine because the value can be
/// replaced by a <c>${{ if ... }}: { ... }</c> mapping. For scalar properties this is
/// impossible — emitting <c>timeoutInMinutes:\n  ${{ if ... }}: 60</c> is invalid
/// Azure Pipelines syntax. So we must lift the conditional up to the parent mapping
/// as a sibling key: <c>${{ if ... }}: { timeoutInMinutes: 60 }</c>.
/// </para>
/// </summary>
internal sealed class ConditionalScalarTypeInspector : TypeInspectorSkeleton
{
    private readonly ITypeInspector _inner;

    public ConditionalScalarTypeInspector(ITypeInspector inner)
    {
        _inner = inner;
    }

    public override string GetEnumName(Type enumType, string name) => _inner.GetEnumName(enumType, name);

    public override string GetEnumValue(object enumValue) => _inner.GetEnumValue(enumValue);

    public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object? container)
    {
        var properties = _inner.GetProperties(type, container);

        if (container is null)
        {
            return properties;
        }

        List<IPropertyDescriptor>? rewritten = null;
        var originals = new List<IPropertyDescriptor>();

        foreach (var prop in properties)
        {
            originals.Add(prop);
            if (TryExpandConditional(prop, container, out var expanded))
            {
                if (rewritten is null)
                {
                    // Copy any properties we've already seen
                    rewritten = new List<IPropertyDescriptor>(originals.Count - 1);
                    for (int i = 0; i < originals.Count - 1; i++)
                    {
                        rewritten.Add(originals[i]);
                    }
                }

                foreach (var d in expanded!)
                {
                    rewritten.Add(d);
                }
            }
            else
            {
                rewritten?.Add(prop);
            }
        }

        return rewritten ?? (IEnumerable<IPropertyDescriptor>)originals;
    }

    private static bool TryExpandConditional(
        IPropertyDescriptor prop,
        object container,
        out List<IPropertyDescriptor>? expanded)
    {
        expanded = null;

        if (!IsScalarAdoExpression(prop.Type))
        {
            return false;
        }

        IObjectDescriptor descriptor;
        try
        {
            descriptor = prop.Read(container);
        }
        catch
        {
            return false;
        }

        if (descriptor.Value is not AdoExpression expression)
        {
            return false;
        }

        if (expression.Condition is null && expression.Parent is null)
        {
            // Plain value with no condition — let YamlDotNet handle it normally.
            return false;
        }

        var innerType = prop.Type.GetGenericArguments()[0];

        // Walk up to the root. For If/ElseIf/Else chains, the root is a synthetic
        // AdoExpression created by `Else` / `ElseIf` getters: it has a null Condition
        // and holds each branch in `Definitions`. For a simple `If.X.Value(...)`,
        // there is no parent and the expression itself is the only branch.
        var root = expression;
        while (root.Parent is not null)
        {
            root = root.Parent;
        }

        var branches = new List<AdoExpression>();
        if (root.Condition is null)
        {
            // Synthetic root — collect each conditional branch from Definitions.
            foreach (var def in root.Definitions)
            {
                if (def is AdoExpression branch && branch.Condition is not null)
                {
                    branches.Add(branch);
                }
            }
        }
        else
        {
            branches.Add(root);
        }

        if (branches.Count == 0)
        {
            return false;
        }

        expanded = new List<IPropertyDescriptor>(branches.Count);
        foreach (var branch in branches)
        {
            expanded.Add(new ConditionalKeyPropertyDescriptor(
                key: branch.Condition!.ToString()!,
                order: prop.Order,
                innerName: prop.Name,
                innerType: innerType,
                innerValue: branch.GetDefinitionValue()));
        }

        return true;
    }

    private static bool IsScalarAdoExpression(Type type)
    {
        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(AdoExpression<>))
        {
            return false;
        }

        var inner = type.GetGenericArguments()[0];
        var nonNullable = Nullable.GetUnderlyingType(inner) ?? inner;
        return nonNullable.IsPrimitive
            || nonNullable.IsEnum
            || nonNullable == typeof(string)
            || nonNullable == typeof(TimeSpan)
            || nonNullable == typeof(decimal);
    }

    /// <summary>
    /// A virtual <see cref="IPropertyDescriptor"/> whose name is a
    /// <c>${{ if ... }}</c> expression and whose value is a single-entry mapping
    /// containing the original property name and value.
    /// </summary>
    private sealed class ConditionalKeyPropertyDescriptor : IPropertyDescriptor
    {
        private readonly string _innerName;
        private readonly Type _innerType;
        private readonly object? _innerValue;

        public ConditionalKeyPropertyDescriptor(
            string key,
            int order,
            string innerName,
            Type innerType,
            object? innerValue)
        {
            Name = key;
            Order = order;
            _innerName = innerName;
            _innerType = innerType;
            _innerValue = innerValue;
        }

        public string Name { get; }
        public int Order { get; set; }
        public Type Type => typeof(InnerMappingValue);
        public Type? TypeOverride { get; set; }
        public Type? ConverterType => null;
        public ScalarStyle ScalarStyle { get; set; } = ScalarStyle.Plain;
        public bool CanWrite => false;
        public bool AllowNulls => true;
        public bool Required => false;

        public T? GetCustomAttribute<T>() where T : System.Attribute => null;

        public IObjectDescriptor Read(object target)
            => new ObjectDescriptor(
                new InnerMappingValue(_innerName, _innerType, _innerValue),
                typeof(InnerMappingValue),
                typeof(InnerMappingValue));

        public void Write(object target, object? value)
            => throw new NotSupportedException();
    }

    /// <summary>
    /// Emits a block-style mapping with a single entry: <c>innerName: innerValue</c>.
    /// </summary>
    private sealed class InnerMappingValue : IYamlConvertible
    {
        private readonly string _innerName;
        private readonly Type _innerType;
        private readonly object? _innerValue;

        public InnerMappingValue(string innerName, Type innerType, object? innerValue)
        {
            _innerName = innerName;
            _innerType = innerType;
            _innerValue = innerValue;
        }

        public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
            => throw new NotSupportedException();

        public void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
        {
            emitter.Emit(new MappingStart(AnchorName.Empty, TagName.Empty, isImplicit: true, MappingStyle.Block));
            emitter.Emit(new Scalar(_innerName));
            nestedObjectSerializer(_innerValue, _innerType);
            emitter.Emit(new MappingEnd());
        }
    }
}
