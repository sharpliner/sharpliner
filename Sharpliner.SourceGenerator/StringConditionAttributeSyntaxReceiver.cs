using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sharpliner.SourceGenerator;

internal class StringConditionAttributeSyntaxReceiver : ISyntaxContextReceiver
{
    public List<IMethodSymbol> IdentifiedMethods { get; } = new();
    public List<ITypeSymbol> IdentifiedClasses { get; } = new();

    public Dictionary<IPropertySymbol, List<IFieldSymbol>> IdentifiedPropertiesAndAssociatedFields { get; } = new();

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
        if (!HasSourceGeneratorAttribute(context, classDeclarationSyntax, out ISymbol nodeSymbol))
        {
            return;
        }

        if (nodeSymbol is ITypeSymbol typeSymbol)
        {
            IdentifiedClasses.Add(typeSymbol);
        }
    }

    private void ProcessMethod(GeneratorSyntaxContext context, MethodDeclarationSyntax methodDeclarationSyntax)
    {
        if (!HasSourceGeneratorAttribute(context, methodDeclarationSyntax, out ISymbol nodeSymbol))
        {
            return;
        }

        if (nodeSymbol is IMethodSymbol methodSymbol)
        {
            IdentifiedMethods.Add(methodSymbol);
        }
    }

    private static bool HasSourceGeneratorAttribute(GeneratorSyntaxContext context,
        SyntaxNode methodDeclarationSyntax, out ISymbol nodeSymbol)
    {
        nodeSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax);

        var attributes = nodeSymbol
            .GetAttributes()
            .Where(a => a.AttributeClass.ToDisplayString(SymbolDisplayFormats.NamespaceAndType) ==
                        typeof(StringConditionAttribute).FullName)
            .ToList();

        if (!attributes.Any())
        {
            return false;
        }

        return true;
    }
}
