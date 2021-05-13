using YamlDotNet.Serialization;
using System.Collections.Generic;
using System.Collections;
using YamlDotNet.Serialization.ObjectGraphTraversalStrategies;

namespace Sharpliner.GraphTraversal
{
    /// <summary>
    /// This code was taken from YamlDotNet directly.
    /// It is a workaround that skips empty collections and doesn't render them as [].
    /// The proper way to do this is in a PR here: https://github.com/aaubry/YamlDotNet/pull/608
    /// Before that is fixed, this is how we're going to do things (╯°□°）╯︵ ┻━┻
    /// </summary>
    public class SharplinerGraphTraversalStrategy : FullObjectGraphTraversalStrategy
    {
        private readonly ITypeInspector _typeDescriptor;

        public SharplinerGraphTraversalStrategy(ITypeInspector typeDescriptor, ITypeResolver typeResolver, int maxRecursion, INamingConvention namingConvention)
            : base(typeDescriptor, typeResolver, maxRecursion, namingConvention)
        {
            _typeDescriptor = typeDescriptor;
        }

        protected override void TraverseProperties<TContext>(IObjectDescriptor value, IObjectGraphVisitor<TContext> visitor, TContext context, Stack<ObjectPathSegment> path)
        {
            visitor.VisitMappingStart(value, typeof(string), typeof(object), context);

            var source = value.NonNullValue();
            foreach (var propertyDescriptor in _typeDescriptor.GetProperties(value.Type, source))
            {
                var propertyValue = propertyDescriptor.Read(source);

                if (visitor.EnterMapping(propertyDescriptor, propertyValue, context))
                {
                    // This is the actual change
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
    }
}
