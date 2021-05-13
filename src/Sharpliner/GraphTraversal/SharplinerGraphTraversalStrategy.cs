using YamlDotNet.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System;
using YamlDotNet.Core;
using YamlDotNet.Serialization.Utilities;
using System.Linq;
using System.Reflection;

namespace Sharpliner.GraphTraversal
{
    /// <summary>
    /// This code was taken from YamlDotNet directly.
    /// It is a workaround that skips empty collections and doesn't render them as [].
    /// The proper way to do this is in a PR here: https://github.com/aaubry/YamlDotNet/pull/608
    /// Before that is fixed, this is how we're going to do things (╯°□°）╯︵ ┻━┻
    /// </summary>
    public class SharplinerGraphTraversalStrategy : IObjectGraphTraversalStrategy
    {
        private readonly int _maxRecursion;
        private readonly ITypeInspector _typeDescriptor;
        private readonly ITypeResolver _typeResolver;
        private readonly INamingConvention _namingConvention;

        public SharplinerGraphTraversalStrategy(ITypeInspector typeDescriptor, ITypeResolver typeResolver, int maxRecursion, INamingConvention namingConvention)
        {
            if (maxRecursion <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRecursion), maxRecursion, "maxRecursion must be greater than 1");
            }

            _typeDescriptor = typeDescriptor ?? throw new ArgumentNullException(nameof(typeDescriptor));
            _typeResolver = typeResolver ?? throw new ArgumentNullException(nameof(typeResolver));

            _maxRecursion = maxRecursion;
            _namingConvention = namingConvention ?? throw new ArgumentNullException(nameof(namingConvention));
        }

        void IObjectGraphTraversalStrategy.Traverse<TContext>(IObjectDescriptor graph, IObjectGraphVisitor<TContext> visitor, TContext context)
        {
            Traverse("<root>", graph, visitor, context, new Stack<ObjectPathSegment>(_maxRecursion));
        }

        protected struct ObjectPathSegment
        {
            public readonly object Name;
            public readonly IObjectDescriptor Value;

            public ObjectPathSegment(object name, IObjectDescriptor value)
            {
                Name = name;
                Value = value;
            }
        }

        protected virtual void Traverse<TContext>(object name, IObjectDescriptor value, IObjectGraphVisitor<TContext> visitor, TContext context, Stack<ObjectPathSegment> path)
        {
            if (path.Count >= _maxRecursion)
            {
                var message = new StringBuilder();
                message.AppendLine("Too much recursion when traversing the object graph.");
                message.AppendLine("The path to reach this recursion was:");

                var lines = new Stack<KeyValuePair<string, string>>(path.Count);
                var maxNameLength = 0;
                foreach (var segment in path)
                {
                    var segmentName = TypeConverter.ChangeType<string>(segment.Name);
                    maxNameLength = Math.Max(maxNameLength, segmentName.Length);
                    lines.Push(new KeyValuePair<string, string>(segmentName, segment.Value.Type.FullName!));
                }

                foreach (var line in lines)
                {
                    message
                        .Append(" -> ")
                        .Append(line.Key.PadRight(maxNameLength))
                        .Append("  [")
                        .Append(line.Value)
                        .AppendLine("]");
                }

                throw new MaximumRecursionLevelReachedException(message.ToString());
            }

            if (!visitor.Enter(value, context))
            {
                return;
            }

            path.Push(new ObjectPathSegment(name, value));
            try
            {
                var typeCode = value.Type.GetTypeCode();
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                    case TypeCode.String:
                    case TypeCode.Char:
                    case TypeCode.DateTime:
                        visitor.VisitScalar(value, context);
                        break;

                    case TypeCode.Empty:
                        throw new NotSupportedException($"TypeCode.{typeCode} is not supported.");

                    default:
                        if (value.IsDbNull())
                        {
                            visitor.VisitScalar(new ObjectDescriptor(null, typeof(object), typeof(object)), context);
                        }

                        if (value.Value == null || value.Type == typeof(TimeSpan))
                        {
                            visitor.VisitScalar(value, context);
                            break;
                        }

                        var underlyingType = Nullable.GetUnderlyingType(value.Type);
                        if (underlyingType != null)
                        {
                            // This is a nullable type, recursively handle it with its underlying type.
                            // Note that if it contains null, the condition above already took care of it
                            Traverse("Value", new ObjectDescriptor(value.Value, underlyingType, value.Type, value.ScalarStyle), visitor, context, path);
                        }
                        else
                        {
                            TraverseObject(value, visitor, context, path);
                        }
                        break;
                }
            }
            finally
            {
                path.Pop();
            }
        }

        protected virtual void TraverseObject<TContext>(IObjectDescriptor value, IObjectGraphVisitor<TContext> visitor, TContext context, Stack<ObjectPathSegment> path)
        {
            if (typeof(IDictionary).IsAssignableFrom(value.Type))
            {
                TraverseDictionary(value, visitor, typeof(object), typeof(object), context, path);
                return;
            }

            var genericDictionaryType = ReflectionExtensions.GetImplementedGenericInterface(value.Type, typeof(IDictionary<,>));
            if (genericDictionaryType != null)
            {
                var genericArguments = genericDictionaryType.GetGenericArguments();
                var adaptedDictionary = Activator.CreateInstance(typeof(GenericDictionaryToNonGenericAdapter<,>).MakeGenericType(genericArguments), value.Value)!;
                TraverseDictionary(new ObjectDescriptor(adaptedDictionary, value.Type, value.StaticType, value.ScalarStyle), visitor, genericArguments[0], genericArguments[1], context, path);
                return;
            }

            if (typeof(IEnumerable).IsAssignableFrom(value.Type))
            {
                TraverseList(value, visitor, context, path);
                return;
            }

            TraverseProperties(value, visitor, context, path);
        }

        protected virtual void TraverseDictionary<TContext>(IObjectDescriptor dictionary, IObjectGraphVisitor<TContext> visitor, Type keyType, Type valueType, TContext context, Stack<ObjectPathSegment> path)
        {
            visitor.VisitMappingStart(dictionary, keyType, valueType, context);

            var isDynamic = dictionary.Type.FullName!.Equals("System.Dynamic.ExpandoObject");
            foreach (DictionaryEntry? entry in (IDictionary)dictionary.NonNullValue())
            {
                var entryValue = entry!.Value;
                var keyValue = isDynamic ? _namingConvention.Apply(entryValue.Key.ToString()!) : entryValue.Key;
                var key = GetObjectDescriptor(keyValue, keyType);
                var value = GetObjectDescriptor(entryValue.Value, valueType);

                if (visitor.EnterMapping(key, value, context))
                {
                    Traverse(keyValue, key, visitor, context, path);
                    Traverse(keyValue, value, visitor, context, path);
                }
            }

            visitor.VisitMappingEnd(dictionary, context);
        }

        private void TraverseList<TContext>(IObjectDescriptor value, IObjectGraphVisitor<TContext> visitor, TContext context, Stack<ObjectPathSegment> path)
        {
            var enumerableType = ReflectionExtensions.GetImplementedGenericInterface(value.Type, typeof(IEnumerable<>));
            var itemType = enumerableType != null ? enumerableType.GetGenericArguments()[0] : typeof(object);

            visitor.VisitSequenceStart(value, itemType, context);

            var index = 0;

            foreach (var item in (IEnumerable)value.NonNullValue())
            {
                Traverse(index, GetObjectDescriptor(item, itemType), visitor, context, path);
                ++index;
            }

            visitor.VisitSequenceEnd(value, context);
        }

        protected virtual void TraverseProperties<TContext>(IObjectDescriptor value, IObjectGraphVisitor<TContext> visitor, TContext context, Stack<ObjectPathSegment> path)
        {
            visitor.VisitMappingStart(value, typeof(string), typeof(object), context);

            var source = value.NonNullValue();
            foreach (var propertyDescriptor in _typeDescriptor.GetProperties(value.Type, source))
            {
                var propertyValue = propertyDescriptor.Read(source);

                if (visitor.EnterMapping(propertyDescriptor, propertyValue, context))
                {
                    if (propertyValue.Value is IEnumerable collection && !collection.GetEnumerator().MoveNext())
                    {
                        // Skip empty arrays/collections
                        continue;
                    }

                    Traverse(propertyDescriptor.Name, new ObjectDescriptor(propertyDescriptor.Name, typeof(string), typeof(string)), visitor, context, path);
                    Traverse(propertyDescriptor.Name, propertyValue, visitor, context, path);
                }
            }

            visitor.VisitMappingEnd(value, context);
        }

        private IObjectDescriptor GetObjectDescriptor(object? value, Type staticType)
        {
            return new ObjectDescriptor(value, _typeResolver.Resolve(staticType, value), staticType);
        }

        private sealed class GenericDictionaryToNonGenericAdapter<TKey, TValue> : IDictionary
            where TKey : notnull
        {
            private readonly IDictionary<TKey, TValue> genericDictionary;

            public GenericDictionaryToNonGenericAdapter(IDictionary<TKey, TValue> genericDictionary)
            {
                this.genericDictionary = genericDictionary ?? throw new ArgumentNullException(nameof(genericDictionary));
            }

            public void Add(object key, object? value)
            {
                throw new NotSupportedException();
            }

            public void Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(object key)
            {
                throw new NotSupportedException();
            }

            public IDictionaryEnumerator GetEnumerator()
            {
                return new DictionaryEnumerator(genericDictionary.GetEnumerator());
            }

            public bool IsFixedSize
            {
                get { throw new NotSupportedException(); }
            }

            public bool IsReadOnly
            {
                get { throw new NotSupportedException(); }
            }

            public ICollection Keys
            {
                get { throw new NotSupportedException(); }
            }

            public void Remove(object key)
            {
                throw new NotSupportedException();
            }

            public ICollection Values
            {
                get { throw new NotSupportedException(); }
            }

            public object? this[object key]
            {
                get
                {
                    throw new NotSupportedException();
                }
                set
                {
                    genericDictionary[(TKey)key] = (TValue)value!;
                }
            }

            public void CopyTo(Array array, int index)
            {
                throw new NotSupportedException();
            }

            public int Count
            {
                get { throw new NotSupportedException(); }
            }

            public bool IsSynchronized
            {
                get { throw new NotSupportedException(); }
            }

            public object SyncRoot
            {
                get { throw new NotSupportedException(); }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private class DictionaryEnumerator : IDictionaryEnumerator
            {
                private readonly IEnumerator<KeyValuePair<TKey, TValue>> enumerator;

                public DictionaryEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> enumerator)
                {
                    this.enumerator = enumerator;
                }

                public DictionaryEntry Entry
                {
                    get
                    {
                        return new DictionaryEntry(Key, Value);
                    }
                }

                public object Key
                {
                    get { return enumerator.Current.Key!; }
                }

                public object? Value
                {
                    get { return enumerator.Current.Value; }
                }

                public object Current
                {
                    get { return Entry; }
                }

                public bool MoveNext()
                {
                    return enumerator.MoveNext();
                }

                public void Reset()
                {
                    enumerator.Reset();
                }
            }
        }
    }

    internal static class ReflectionExtensions
    {
        public static Type? GetImplementedGenericInterface(Type type, Type genericInterfaceType)
        {
            foreach (var interfacetype in GetImplementedInterfaces(type))
            {
                if (interfacetype.IsGenericType() && interfacetype.GetGenericTypeDefinition() == genericInterfaceType)
                {
                    return interfacetype;
                }
            }
            return null;
        }

        public static IEnumerable<Type> GetImplementedInterfaces(Type type)
        {
            if (type.IsInterface())
            {
                yield return type;
            }

            foreach (var implementedInterface in type.GetInterfaces())
            {
                yield return implementedInterface;
            }
        }

        public static Type? BaseType(this Type type)
        {
            return type.BaseType;
        }

        public static bool IsValueType(this Type type)
        {
            return type.IsValueType;
        }

        public static bool IsGenericType(this Type type)
        {
            return type.IsGenericType;
        }

        public static bool IsGenericTypeDefinition(this Type type)
        {
            return type.IsGenericTypeDefinition;
        }

        public static bool IsInterface(this Type type)
        {
            return type.IsInterface;
        }

        public static bool IsEnum(this Type type)
        {
            return type.IsEnum;
        }

        public static bool IsDbNull(this object value)
        {
            return value is DBNull;
        }

        /// <summary>
        /// Determines whether the specified type has a default constructor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///     <c>true</c> if the type has a default constructor; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasDefaultConstructor(this Type type)
        {
            return type.IsValueType || type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null) != null;
        }

        public static TypeCode GetTypeCode(this Type type)
        {
            return Type.GetTypeCode(type);
        }

        public static PropertyInfo? GetPublicProperty(this Type type, string name)
        {
            return type.GetProperty(name);
        }

        public static FieldInfo? GetPublicStaticField(this Type type, string name)
        {
            return type.GetField(name, BindingFlags.Static | BindingFlags.Public);
        }

        public static IEnumerable<PropertyInfo> GetProperties(this Type type, bool includeNonPublic)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public;

            if (includeNonPublic)
            {
                bindingFlags |= BindingFlags.NonPublic;
            }

            return type.IsInterface
                ? (new Type[] { type })
                    .Concat(type.GetInterfaces())
                    .SelectMany(i => i.GetProperties(bindingFlags))
                : type.GetProperties(bindingFlags);
        }

        public static IEnumerable<PropertyInfo> GetPublicProperties(this Type type) => type.GetProperties(false);

        public static IEnumerable<FieldInfo> GetPublicFields(this Type type)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public);
        }

        public static IEnumerable<MethodInfo> GetPublicStaticMethods(this Type type)
        {
            return type.GetMethods(BindingFlags.Static | BindingFlags.Public);
        }

        public static MethodInfo GetPrivateStaticMethod(this Type type, string name)
        {
            return type.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)
                ?? throw new MissingMethodException($"Expected to find a method named '{name}' in '{type.FullName}'.");
        }

        public static MethodInfo? GetPublicStaticMethod(this Type type, string name, params Type[] parameterTypes)
        {
            return type.GetMethod(name, BindingFlags.Public | BindingFlags.Static, null, parameterTypes, null);
        }

        public static MethodInfo? GetPublicInstanceMethod(this Type type, string name)
        {
            return type.GetMethod(name, BindingFlags.Public | BindingFlags.Instance);
        }

        private static readonly FieldInfo? RemoteStackTraceField = typeof(Exception)
                .GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);

        public static Exception Unwrap(this TargetInvocationException ex)
        {
            var result = ex.InnerException;
            if (result == null)
            {
                return ex;
            }

            if (RemoteStackTraceField != null)
            {
                RemoteStackTraceField.SetValue(result, result.StackTrace + "\r\n");
            }
            return result;
        }

        public static bool IsInstanceOf(this Type type, object o)
        {
            return type.IsInstanceOfType(o);
        }

        public static Attribute[] GetAllCustomAttributes<TAttribute>(this PropertyInfo property)
        {
            // Don't use IMemberInfo.GetCustomAttributes, it ignores the inherit parameter
            return Attribute.GetCustomAttributes(property, typeof(TAttribute));
        }
    }
}
