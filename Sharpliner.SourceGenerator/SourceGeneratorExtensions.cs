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
}
