using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps.ConditionedExpressions;

/// <summary>
/// Represents an item that might or might not have a condition.
/// Example of regular definition:
/// <code lang="csharp">
/// Task("publish")
/// </code>
/// will output:
/// <code lang="yaml">
/// - task: publish
/// </code>
/// Example of conditioned definition:
/// <code lang="csharp">
/// If.Equal(variables["_RunAsInternal"], "true")
///     .Variable("NetCoreInternal-Pool", true)
/// .EndIf
/// </code>
/// will output:
/// <code lang="yaml">
/// - ${{ if eq(variables._RunAsInternal, True) }}:
///   - name: NetCoreInternal-Pool
///     value: true
/// </code>
/// </summary>
public abstract record Conditioned : IYamlConvertible
{
    private class ValueEqualityList : List<Conditioned>
    {
        public override bool Equals(object? obj)
        {
            if (obj is ValueEqualityList other && Count == 0 && other.Count == 0)
            {
                return true;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    /// <summary>
    /// Evaluated textual representation of the condition, e.g. <c>ne('foo', 'bar')</c>.
    /// </summary>
    internal IfCondition? Condition { get; set; }

    /// <summary>
    /// Pointer in case of nested conditional blocks.
    /// </summary>
    internal Conditioned? Parent { get; set; }

    /// <summary>
    /// In case we define multiple items inside one <c>${{ if }}</c>, they are stored here.
    /// </summary>
    internal List<Conditioned> Definitions { get; } = new ValueEqualityList();

    /// <summary>
    /// When serializing, we need to distinguish whether serializing a list of items under a condition or just a value.
    /// </summary>
    internal bool IsList { get; set; } = false;

    /// <summary>
    /// Creates a new instance of <see cref="Conditioned"/> with the given condition.
    /// </summary>
    /// <param name="condition">The condition.</param>
    protected Conditioned(IfCondition? condition)
    {
        Condition = condition;
    }

    void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
        => throw new NotImplementedException();

    void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer) => WriteInternal(emitter, nestedObjectSerializer);

    internal abstract void WriteInternal(IEmitter emitter, ObjectSerializer nestedObjectSerializer);

    internal virtual void SetIsList(bool isList)
    {
        IsList = isList;

        foreach (var child in Definitions)
        {
            child.SetIsList(isList);
        }
    }

    /// <summary>
    /// This method is used for double-linking of the definition expression tree.
    /// </summary>
    /// <param name="condition">Parent condition</param>
    /// <param name="definition">Definition that was added below the condition</param>
    /// <returns>The conditioned definition coming out of the inputs</returns>
    internal static Conditioned<T> Link<T>(IfCondition condition, T definition)
    {
        var conditionedDefinition = new Conditioned<T>(definition, condition);
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
    internal static Conditioned<T> Link<T>(IfCondition condition, Conditioned<T> conditionedDefinition)
    {
        conditionedDefinition.Condition = condition;
        condition.Parent?.Definitions.Add(conditionedDefinition);
        conditionedDefinition.Parent = condition.Parent;
        return conditionedDefinition;
    }

    /// <summary>
    /// This method is used for double-linking of the definition expression tree.
    /// </summary>
    /// <param name="condition">Parent condition</param>
    /// <param name="items">Items to add to a condition</param>
    /// <returns>The conditioned definition coming out of the inputs</returns>
    internal static Conditioned<T> Link<T>(IfCondition condition, IEnumerable<Conditioned<T>> items)
    {
        var conditionedDefinition = new Conditioned<T>(default!, condition);
        conditionedDefinition.Definitions.AddRange(items);
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
    internal static Conditioned<T> Link<T>(IfCondition condition, Template<T> template)
    {
        if (condition.Parent == null)
        {
            template.Condition = condition;
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
/// Represents an item that might or might not have a condition.
/// Example of regular definition:
/// <code lang="csharp">
/// Task("publish")
/// </code>
/// will output:
/// <code lang="yaml">
/// - task: publish
/// </code>
/// Example of conditioned definition:
/// <code lang="csharp">
/// If.Equal(variables["_RunAsInternal"], "true")
///     .Variable("NetCoreInternal-Pool", true)
/// .EndIf
/// </code>
/// will output:
/// <code lang="yaml">
/// - ${{ if eq(variables._RunAsInternal, True) }}:
///   - name: NetCoreInternal-Pool
///     value: true
/// </code>
/// </summary>
public record Conditioned<T> : Conditioned
{
    // Make sure we can for example assign a string into ConditionedDefinition<string>
    /// <summary>
    /// Implicitly converts a value into a <see cref="Conditioned{T}"/> instance with a definition.
    /// </summary>
    /// <param name="value">The definition.</param>
    public static implicit operator Conditioned<T>([NotNullIfNotNull(nameof(value))]T? value) =>
        value == null ? null! : new(definition: value);

    // Make sure we can assign ${{ parameters.name }} into conditioned
    /// <summary>
    /// Implicitly converts a <see cref="ParameterReference"/> into a <see cref="Conditioned{T}"/> instance with a parameter reference.
    /// </summary>
    /// <param name="parameterRef">The parameter reference.</param>
    public static implicit operator Conditioned<T>(ParameterReference parameterRef) => new ConditionedParameterReference<T>(parameterRef);

    /// <summary>
    /// The actual definition (value).
    /// </summary>
    internal T? Definition { get; }

    internal Conditioned(T definition, IfCondition condition) : base(condition)
    {
        Definition = definition;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Conditioned{T}"/> with the given definition.
    /// </summary>
    /// <param name="definition">The definition.</param>
    public Conditioned(T definition) : this()
    {
        Definition = definition;
    }

    /// <summary>
    /// Creates a new instance of <see cref="Conditioned{T}"/> with the given condition.
    /// </summary>
    /// <param name="condition">The condition.</param>
    protected Conditioned(IfCondition? condition) : base(condition)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="Conditioned{T}"/> with no condition.
    /// </summary>
    protected Conditioned() : base((IfCondition?)null)
    {
    }

    /// <summary>
    /// Starts a new <c>${{ if (...) }}</c> section.
    /// For example:
    /// <code lang="csharp">
    /// .If.And(Equal("e", "f"), NotEqual("g", "h"))
    ///     .Variable("feature", "on")
    ///     .Variable("feature2", "on")
    /// </code>
    /// will generate:
    /// <code lang="yaml">
    /// - ${{ if and(eq('e', 'f'), ne('g', 'h')) }}:
    ///   - name: feature
    ///     value: on
    ///   - name: feature2
    ///     value: on
    /// </code>
    /// </summary>
    public IfConditionBuilder<T> If => new(this);

    /// <summary>
    /// Starts a new <c>${{ elseif (...) }}</c> section.
    /// For example:
    /// <code lang="csharp">
    /// If.IsBranch("dev")
    ///     .Group("Development")
    /// ElseIf.IsBranch("prod")
    ///     .Group("Production")
    /// </code>
    /// will generate:
    /// <code lang="yaml">
    /// - ${{ if eq(variables['Build.SourceBranch'], 'refs/heads/dev') }}:
    ///   - group: Development
    /// - ${{ elseif eq(variables['Build.SourceBranch'], 'refs/heads/prod') }}:
    ///   - group: Production
    /// </code>
    /// </summary>
    public IfConditionBuilder ElseIf
    {
        get
        {
            // If we're top-level, we create a fake new top with empty definition to collect all the definitions
            if (Parent == null)
            {
                Parent = new Conditioned<T>();
                Parent.Definitions.Add(this);
            }

            if (Condition == null)
            {
                throw new InvalidOperationException("No condition to match ElseIf against");
            }

            return new IfConditionBuilder(Parent, true);
        }
    }

    /// <summary>
    /// Ends a <c>${{ if (...) }}</c> section.
    /// For example:
    /// <code lang="csharp">
    /// new Job("Job")
    /// {
    ///     Pool = If.Equal("A", "B")
    ///         .Pool(new HostedPool("pool-A")
    ///         {
    ///             Demands = { "SomeProperty -equals SomeValue" }
    ///         })
    ///     .EndIf
    ///     .If.Equal("C", "D")
    ///         .Pool(new HostedPool("pool-B")),
    /// }
    /// </code>
    /// will generate:
    /// <code lang="yaml">
    /// - job: Job
    ///   pool:
    ///     ${{ if eq('A', 'B') }}:
    ///       name: pool-A
    ///       demands:
    ///       - SomeProperty -equals SomeValue
    ///     ${{ if eq('C', 'D') }}:
    ///       name: pool-B
    /// </code>
    /// </summary>
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
                ?? throw new InvalidOperationException(
                    $"You have called {nameof(EndIf)} on a top-level statement, " +
                    $"{nameof(EndIf)} can only be used to return from a nested definition");
        }
    }

    /// <summary>
    /// Ends an <c>${{ each (...) }}</c> section.
    /// For example:
    /// <code lang="csharp">
    /// Each("foo", "bar")
    ///    .Job(new Job("job-${{ foo }}"))
    /// .EndEach
    /// .If.Equal("foo", "bar")
    ///     .Job(new Job("job2-${{ foo }}"))
    /// </code>
    /// will generate:
    /// <code lang="yaml">
    /// - ${{ each foo in bar }}:
    ///   - job: job-${{ foo }}
    /// - ${{ if eq('foo', 'bar') }}:
    ///   - job: job2-${{ foo }}
    /// </code>
    /// </summary>
    public Conditioned<T> EndEach
    {
        get
        {
            if (Condition?.EachExpression != null)
            {
                Condition.EachExpression = null;
                return this;
            }

            // If we're top-level, we create a fake new top with empty definition to collect all the definitions
            if (Parent == null)
            {
                Parent = new Conditioned<T>();
                Parent.Definitions.Add(this);
            }

            return Parent as Conditioned<T>
                ?? throw new InvalidOperationException(
                    $"You have called {nameof(EndEach)} on a top-level statement, " +
                    $"{nameof(EndEach)} must only be used after Each");
        }
    }

    /// <summary>
    /// Start an <c>${{ else () }}</c> section.
    /// For example:
    /// <code lang="csharp">
    /// If.Equal("a", "b")
    ///     .Variable("feature", "on")
    ///     .Variable("feature2", "on")
    /// .Else
    ///     .Variable("feature", "off")
    ///     .Variable("feature2", "off"),
    /// </code>
    /// will generate:
    /// <code lang="yaml">
    /// - ${{ if eq('a', 'b') }}:
    ///   - name: feature
    ///     value: on
    ///   - name: feature2
    ///     value: on
    /// - ${{ else }}:
    ///   - name: feature
    ///     value: off
    ///   - name: feature2
    ///     value: off
    /// </code>
    /// </summary>
    public IfCondition<T> Else
    {
        get
        {
            // If we're top-level, we create a fake new top with empty definition to collect all the definitions
            if (Parent == null)
            {
                Parent = new Conditioned<T>();
                Parent.Definitions.Add(this);
            }

            if (Condition == null)
            {
                throw new InvalidOperationException("No condition to match Else against");
            }

            return SharplinerConfiguration.Current.Serialization.UseElseExpression
                ? new ElseCondition<T>()
                {
                    Parent = Parent
                }
                : new IfNotCondition<T>(Condition)
                {
                    Parent = Parent
                };
        }
    }

    internal override void WriteInternal(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
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

    internal override void SetIsList(bool isList)
    {
        base.SetIsList(isList);

        if (Definition is Conditioned conditioned)
        {
            conditioned.SetIsList(isList);
        }
    }

    /// <summary>
    /// <para>
    /// This method's responsibility is to serialize a single item which might or might not have conditions underneath.
    /// </para>
    /// Example #1 (no condition)
    /// <code lang="yaml">
    ///   name: value1
    /// </code>
    /// Example #2 (conditions)
    /// <code lang="yaml">
    ///   name:
    ///     ${{ if eq(...) }}
    ///       name2: value1
    ///     ${{ if ne(...) }}
    ///       name2: value2
    /// </code>
    /// </summary>
    private void WriteValue(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        if (Condition != null)
        {
            emitter.Emit(new Scalar(Condition.TagStart + Condition.WithoutTags() + Condition.TagEnd));
        }
        else if (Definitions.Count > 0)
        {
            emitter.Emit(new MappingStart());
        }

        if (Condition?.EachExpression != null)
        {
            emitter.Emit(new Scalar(Condition.EachExpression.ToString()));
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

        if (Condition == null && Definitions.Count > 0)
        {
            emitter.Emit(new MappingEnd());
        }
    }

    /// <summary>
    /// This method's responsibility is to serialize a list of items which can share a common condition.
    /// </summary>
    private void WriteList(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        if (Condition != null)
        {
            emitter.Emit(new MappingStart());
            emitter.Emit(new Scalar(Condition.TagStart + IfCondition.WithoutTags(Condition) + Condition.TagEnd));
            emitter.Emit(new SequenceStart(AnchorName.Empty, TagName.Empty, true, SequenceStyle.Block));
        }

        if (Condition?.EachExpression != null)
        {
            emitter.Emit(new MappingStart());
            emitter.Emit(new Scalar(Condition.EachExpression.ToString()));
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

        if (Condition?.EachExpression != null)
        {
            emitter.Emit(new SequenceEnd());
            emitter.Emit(new MappingEnd());
        }

        if (Condition != null)
        {
            emitter.Emit(new SequenceEnd());
            emitter.Emit(new MappingEnd());
        }
    }

    internal virtual void SerializeSelf(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        if (Definition != null)
        {
            nestedObjectSerializer(Definition);
        }
    }

    /// <summary>
    /// Pulls all definitions from a definition tree, regardless of conditions (or their evaluation).
    /// Use cautiously.
    /// Example: from a simple ${{ if }}/ ${{ else }} statements, returns both elements from each branch.
    /// </summary>
    public IReadOnlyCollection<T> FlattenDefinitions()
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

    /// <inheritdoc/>
    public override string ToString() =>
        $"{(Condition == null ? "" : Condition + ": ")}{typeof(T).Name} " +
        $"with {(Definition == null ? Definitions.Count : Definitions.Count + 1)} items";
}
