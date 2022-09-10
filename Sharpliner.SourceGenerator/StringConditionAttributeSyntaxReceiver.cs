using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sharpliner.SourceGenerator;

internal class StringConditionAttributeSyntaxReceiver : ISyntaxContextReceiver
{
    public List<IMethodSymbol> IdentifiedMethods { get; } = new();

    public Dictionary<IPropertySymbol, List<IFieldSymbol>> IdentifiedPropertiesAndAssociatedFields { get; } = new();

    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        if (context.Node is MethodDeclarationSyntax methodDeclarationSyntax && methodDeclarationSyntax.AttributeLists.Any())
        {
            ProcessMethod(context, methodDeclarationSyntax);
        }
    }

    private void ProcessMethod(GeneratorSyntaxContext context, MethodDeclarationSyntax methodDeclarationSyntax)
    {
        var nodeSymbol = context.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax);

        var attributes = nodeSymbol
                .GetAttributes()
                .Where(a => a.AttributeClass.ToDisplayString(SymbolDisplayFormats.NamespaceAndType) == typeof(StringConditionAttribute).FullName)
                .ToList();

        if (!attributes.Any())
        {
            return;
        }

        if (nodeSymbol is IMethodSymbol methodSymbol)
        {
            IdentifiedMethods.Add(methodSymbol);
        }
    }
}
