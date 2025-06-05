﻿using System.Collections.Generic;
using Sharpliner.AzureDevOps.Expressions;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Sharpliner.AzureDevOps;

/// <summary>
/// Abstract parent for all libraries (re-usable sets of items).
/// </summary>
/// <typeparam name="T">Type of items in the library</typeparam>
public abstract class DefinitionLibrary<T> : AzureDevOpsDefinition
{
    internal abstract IEnumerable<AdoExpression<T>> Items { get; }

    internal DefinitionLibrary() : base()
    {
    }
}

/// <summary>
/// This class is used to reference a library in a pipeline.
/// </summary>
/// <typeparam name="T">Type of items in the library</typeparam>
public record LibraryReference<T> : AdoExpression<T>
{
    internal IEnumerable<AdoExpression<T>> Items { get; }

    /// <summary>
    /// Instantiates a new instance of <see cref="LibraryReference{T}"/> with the given <see cref="DefinitionLibrary{T}"/>.
    /// </summary>
    /// <param name="library">The library to reference.</param>
    public LibraryReference(DefinitionLibrary<T> library) : base()
    {
        Items = library.Items;
    }

    /// <summary>
    /// Instantiates a new instance of <see cref="LibraryReference{T}"/> with the given items.
    /// </summary>
    /// <param name="items">The items to reference.</param>
    public LibraryReference(IEnumerable<AdoExpression<T>> items) : base()
    {
        Items = items;
    }

    internal override void SerializeSelf(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
    {
        foreach (var item in Items)
        {
            nestedObjectSerializer(item);
        }
    }

    internal override void SetIsList(bool isList)
    {
        base.SetIsList(true);

        foreach (var item in Items)
        {
            item.SetIsList(true);
        }
    }
}
