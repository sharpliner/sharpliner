﻿using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

public abstract record Conditioned : IYamlConvertible
{
    /// <summary>
    /// Evaluated textual representation of the condition, e.g. "ne('foo', 'bar')".
    /// </summary>
    internal string? Condition { get; set; }

    /// <summary>
    /// Pointer in case of nested conditional blocks.
    /// </summary>
    internal Conditioned? Parent { get; set; }

    /// <summary>
    /// In case we define multiple items inside one ${{ if }}, they are stored here.
    /// </summary>
    internal List<Conditioned> Definitions { get; } = new();

    /// <summary>
    /// When serializing, we need to distinguish whether serializing a list of items under a condition or just a value.
    /// </summary>
    internal bool IsList { get; set; } = false;

    protected Conditioned(string? condition)
    {
        Condition = condition;
    }

    public void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        => throw new NotImplementedException();

    public abstract void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer);

    /// <summary>
    /// This method is used for double-linking of the definition expression tree.
    /// </summary>
    /// <param name="condition">Parent condition</param>
    /// <param name="definition">Definition that was added below the condition</param>
    /// <returns>The conditioned definition coming out of the inputs</returns>
    internal static Conditioned<T> Link<T>(Condition condition, T definition)
    {
        var conditionedDefinition = new Conditioned<T>(definition, condition.ToString());
        condition.Parent?.Definitions.Add(conditionedDefinition);
        conditionedDefinition.Parent = condition.Parent;
        return conditionedDefinition;
    }

    /// <summary>
    /// This method is used for double-linking of the definition expression tree.
    /// </summary>
    /// <param name="condition">Parent condition</param>
    /// <param name="conditionedDefinition">Definition that was added below the condition</param>
    /// <returns>The conditioned definition coming out of the inputs</returns>
    internal static Conditioned<T> Link<T>(Condition condition, Conditioned<T> conditionedDefinition)
    {
        condition.Parent?.Definitions.Add(conditionedDefinition);
        conditionedDefinition.Parent = condition.Parent;
        return conditionedDefinition;
    }

    /// <summary>
    /// This method is used for double-linking of the definition expression tree.
    /// </summary>
    /// <param name="condition">Parent condition</param>
    /// <param name="template">Definition that was added below the condition</param>
    /// <returns>The conditioned definition coming out of the inputs</returns>
    internal static Conditioned<T> Link<T>(Condition condition, Template<T> template)
    {
        if (condition.Parent == null)
        {
            template.Condition = condition.ToString();
        }
        else
        {
            condition.Parent?.Definitions.Add(template);
            template.Parent = condition.Parent;
        }

        return template;
    }
}

/// <summary>
/// Represents an item that might or might have a condition.
/// Example of regular definition:
///     - task: publish
/// Example of conditioned definition:
///     - ${{ if eq(variables._RunAsInternal, True) }}:
///       name: NetCoreInternal-Pool
/// </summary>
public record Conditioned<T> : Conditioned
{
    // Make sure we can for example assign a string into ConditionedDefinition<string>
    public static implicit operator Conditioned<T>(T value) => new(definition: value);

    /// <summary>
    /// The actual definition (value).
    /// </summary>
    internal T? Definition { get; }

    internal Conditioned(T definition, string condition) : base(condition)
    {
        Definition = definition;
    }

    public Conditioned(T definition) : this()
    {
        Definition = definition;
    }

    protected Conditioned(string? condition) : base(condition)
    {
    }

    protected Conditioned() : base((string?)null)
    {
    }

    public ConditionBuilder<T> If => new(this);

    public Conditioned<T> EndIf
    {
        get
        {
            // If we're top-level, we create a fake new top with empty definition to collect all the definitions
            if (Parent == null)
            {
                Parent = new Conditioned<T>();
                Parent.Definitions.Add(this);
            }

            return Parent as Conditioned<T>
                ?? throw new InvalidOperationException("You have called EndIf on a top-level statement, EndIf can only be used to return from a nested definition");
        }
    }

    public Condition<T> Else
    {
        get
        {
            // If we're top-level, we create a fake new top with empty definition to collect all the definitions
            if (Parent == null)
            {
                Parent = new Conditioned<T>();
                Parent.Definitions.Add(this);
            }

            var notCondition = new NotCondition<T>(Condition ?? throw new InvalidOperationException("No condition to match Else against"))
            {
                Parent = Parent
            };

            return notCondition;
        }
    }

    public override void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        if (IsList)
        {
            WriteList(emitter, nestedObjectSerializer);
        }
        else
        {
            WriteValue(emitter, nestedObjectSerializer);
        }
    }

    /// <summary>
    /// This method's responsibility is to serialize a single item which might or might not have conditions underneath.
    /// Example #1 (no condition)
    ///   name: value1
    ///
    /// Example #2 (conditions)
    ///   name:
    ///     ${{ if eq(...) }}
    ///       name2: value1
    ///     ${{ if ne(...) }}
    ///       name2: value2
    /// </summary>
    private void WriteValue(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        if (!string.IsNullOrEmpty(Condition))
        {
            emitter.Emit(new Scalar("${{ if " + Condition + " }}"));
        }
        else if (Definitions.Count > 0)
        {
            emitter.Emit(new MappingStart());
        }

        // We are first serializing us. We can be
        //   - a condition-less definition (top level or leaf) => serialize value inside Definition
        //   - a template => serialize the special shape of template + parameters
        SerializeSelf(emitter, nestedObjectSerializer);

        // Otherwise, we expect a list of Definitions
        foreach (var childDefinition in Definitions)
        {
            nestedObjectSerializer(childDefinition);
        }

        if (string.IsNullOrEmpty(Condition) && Definitions.Count > 0)
        {
            emitter.Emit(new MappingEnd());
        }
    }

    /// <summary>
    /// This method's responsibility is to serialize a list of items which can share a common condition.
    /// </summary>
    private void WriteList(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        if (!string.IsNullOrEmpty(Condition))
        {
            emitter.Emit(new MappingStart());
            emitter.Emit(new Scalar("${{ if " + Condition + " }}"));
            emitter.Emit(new SequenceStart(AnchorName.Empty, TagName.Empty, true, SequenceStyle.Block));
        }

        // We are first serializing us. We can be
        //   - a condition-less definition (top level or leaf) => serialize value inside Definition
        //   - a template => serialize the special shape of template + parameters
        SerializeSelf(emitter, nestedObjectSerializer);

        // Otherwise, we expect a list of Definitions
        foreach (var childDefinition in Definitions)
        {
            nestedObjectSerializer(childDefinition);
        }

        if (!string.IsNullOrEmpty(Condition))
        {
            emitter.Emit(new SequenceEnd());
            emitter.Emit(new MappingEnd());
        }
    }

    protected virtual void SerializeSelf(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        if (Definition != null)
        {
            nestedObjectSerializer(Definition);
        }
    }

    internal IEnumerable<T> FlattenDefinitions()
    {
        var definitions = new List<T>();

        if (Definition is not null)
        {
            definitions.Add(Definition);
        }

        definitions.AddRange(
            Definitions
                .SelectMany(s => (s as Conditioned<T>)?.FlattenDefinitions()!)
                .Where(s => s is not null));

        return definitions;
    }
}
