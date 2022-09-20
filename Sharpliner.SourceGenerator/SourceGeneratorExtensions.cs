using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sharpliner.SourceGenerator;

public static class SourceGeneratorExtensions
{
    public static ConstructorDeclarationSyntax GetConstructorDeclarationSyntax(this IMethodSymbol constructor)
    {
        return constructor.DeclaringSyntaxReferences
            .Select(s => s.GetSyntax())
            .OfType<ConstructorDeclarationSyntax>()
            .First();
    }

    public static AttributeData GetAttribute<TAttribute>(this ISymbol nodeSymbol) where TAttribute : Attribute
    {
        return nodeSymbol.GetAttributes<TAttribute>().First();
    }

    public static IEnumerable<AttributeData> GetAttributes<TAttribute>(this ISymbol nodeSymbol) where TAttribute : Attribute
    {
        return nodeSymbol
            .GetAttributes()
            .Where(a => a.AttributeClass.ToDisplayString(SymbolDisplayFormats.NamespaceAndType) == typeof(TAttribute).FullName)
            .ToList();
    }
}
