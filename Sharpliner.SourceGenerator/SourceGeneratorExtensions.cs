using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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

    public static ClassDeclarationSyntax GetClassDeclarationSyntax(this INamedTypeSymbol @class)
    {
        return @class.DeclaringSyntaxReferences
            .Select(s => s.GetSyntax())
            .OfType<ClassDeclarationSyntax>()
            .First();
    }

    public static ConstructorInitializerSyntax WithArgumentsInSerializeMethodCall(
        this ConstructorInitializerSyntax constructorInitializerSyntax)
    {
        return constructorInitializerSyntax.WithArgumentList(
            constructorInitializerSyntax.ArgumentList.WithArguments(
                SyntaxFactory.SeparatedList(
                    constructorInitializerSyntax.ArgumentList.Arguments
                        .Select(x => x.WrapInSerializeMethodCall())
                )
            )
        );
    }

    public static ArgumentSyntax WrapInSerializeMethodCall(this ArgumentSyntax argumentSyntax)
    {
        return SyntaxFactory.Argument(SyntaxFactory.ParseExpression($"Serialize({argumentSyntax.ToString()})"));
    }

    public static string GetClassDefinition(this ClassDeclarationSyntax @class)
    {
        return $"{@class.Modifiers} {@class.Keyword} {@class.Identifier}{@class.TypeParameterList} {@class.BaseList}";
    }

    public static TSource RemoveTrivia<TSource>(this TSource source, SyntaxKind syntaxKind) where TSource : MemberDeclarationSyntax
    {
        var trivia = source.DescendantTrivia().Where(t => t.Kind() == syntaxKind);

        source.ReplaceTrivia(trivia, (_, _) => SyntaxFactory.Whitespace(""));

        return source;
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
