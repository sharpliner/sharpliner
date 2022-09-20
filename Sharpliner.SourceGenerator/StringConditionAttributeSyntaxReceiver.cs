using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sharpliner.SourceGenerator;

internal class StringConditionAttributeSyntaxReceiver : ISyntaxContextReceiver
{
    public List<IMethodSymbol> IdentifiedMethods { get; } = new();
    public List<INamedTypeSymbol> IdentifiedClasses { get; } = new();

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        if (context.Node is MethodDeclarationSyntax methodDeclarationSyntax
            && methodDeclarationSyntax.AttributeLists.Any())
        {
            ProcessMethod(context, methodDeclarationSyntax);
        }

        if (context.Node is ClassDeclarationSyntax classDeclarationSyntax
            && classDeclarationSyntax.AttributeLists.Any())
        {
            ProcessClass(context, classDeclarationSyntax);
        }
    }

    private void ProcessClass(GeneratorSyntaxContext context, ClassDeclarationSyntax classDeclarationSyntax)
    {
        if (IsValid(context, classDeclarationSyntax, out INamedTypeSymbol? typeSymbol))
        {
            IdentifiedClasses.Add(typeSymbol);
        }
    }

    private void ProcessMethod(GeneratorSyntaxContext context, MethodDeclarationSyntax methodDeclarationSyntax)
    {
        if (IsValid(context, methodDeclarationSyntax, out IMethodSymbol? methodSymbol))
        {
            IdentifiedMethods.Add(methodSymbol);
        }
    }

    private static bool IsValid<TSymbol>(GeneratorSyntaxContext context,
        SyntaxNode methodDeclarationSyntax, out TSymbol? outSymbol)
    {
        outSymbol = default;
        var nodeSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax);

        var attributes = nodeSymbol.GetAttributes<StringConditionAttribute>();

        if (!attributes.Any() || nodeSymbol is not TSymbol tSymbol)
        {
            return false;
        }

        outSymbol = tSymbol;
        return true;
    }
}
