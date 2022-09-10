
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Sharpliner.SourceGenerator;

[Generator]
public class StringConditionGenerator : ISourceGenerator
{
    private static readonly string[] _stringConditionsParameters =
    {
        "string ", "VariableReference ", "ParameterReference "
    };

    public void Initialize(GeneratorInitializationContext context)
    {
#if DEBUG
        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
        }
#endif

        context.RegisterForSyntaxNotifications(() => new StringConditionAttributeSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is not StringConditionAttributeSyntaxReceiver syntaxReciever)
        {
            return;
        }

        foreach (var containingClassGroup in syntaxReciever.IdentifiedMethods.GroupBy(x => x.ContainingType))
        {
            var containingClass = containingClassGroup.Key;
            var namespaceSymbol = containingClass.ContainingNamespace;

            var source = GenerateClass(context, containingClass, namespaceSymbol, containingClassGroup.ToList());
            context.AddSource($"{containingClass.ContainingNamespace.ToDisplayString().Replace('.', '_')}_{containingClass.Name}_StringConditions.generated", SourceText.From(source, Encoding.UTF8));
        }
    }

    private string GenerateClass(GeneratorExecutionContext context, INamedTypeSymbol @class, INamespaceSymbol @namespace, List<IMethodSymbol> methods)
    {
        var classBuilder = new CodeGenerationTextWriter();

        classBuilder.WriteLine(context.GetUsingStatementsForTypes(
            typeof(string),
            typeof(StringConditionAttribute),
            typeof(CallerMemberNameAttribute)
            ));
        classBuilder.WriteLine("using Sharpliner.AzureDevOps.ConditionedExpressions;");
        classBuilder.WriteLine();
        classBuilder.WriteLine($"namespace {@namespace.ToDisplayString()}");
        classBuilder.WriteLine("{");
        classBuilder.WriteLine($"public partial class {@class.Name}{GetGenericType(@class)}");
        classBuilder.WriteLine("{");

        foreach (IMethodSymbol methodSymbol in methods)
        {
            var methodDeclarationSyntax = methodSymbol.DeclaringSyntaxReferences
                    .Select(s => s.GetSyntax())
                    .OfType<MethodDeclarationSyntax>()
                    .First();

            foreach (var parameter1 in _stringConditionsParameters)
            {
                foreach (var parameter2 in _stringConditionsParameters)
                {
                    if (parameter1 == "string " && parameter2 == "string ")
                    {
                        // These are already declared by the base code.
                        continue;
                    }

                    classBuilder.WriteLine(
                        $"public Condition {methodSymbol.Name}({parameter1}{GetParameterName(methodSymbol, 1)}, {parameter2} {GetParameterName(methodSymbol, 2)})");
                    WriteMethodBody(classBuilder, methodDeclarationSyntax);
                }
            }
        }

        classBuilder.WriteLine("}");
        classBuilder.WriteLine("}");

        return classBuilder.ToString();
    }

    private string GetGenericType(INamedTypeSymbol @class)
    {
        if (!@class.TypeArguments.Any())
        {
            return string.Empty;
        }

        var genericTypes = @class.TypeArguments.Select(x => x.Name);
        return $"<{string.Join(", ", genericTypes)}>";
    }

    private static void WriteMethodBody(TextWriter classBuilder, MethodDeclarationSyntax methodDeclarationSyntax)
    {
        if (!string.IsNullOrEmpty(methodDeclarationSyntax.ExpressionBody?.ToString()))
        {
            classBuilder.Write(methodDeclarationSyntax.ExpressionBody);
            classBuilder.WriteLine(';');
            classBuilder.WriteLine();
            return;
        }

        classBuilder.WriteLine("{");
        foreach (StatementSyntax statementSyntax in methodDeclarationSyntax.Body.Statements)
        {
            classBuilder.WriteLine(statementSyntax.ToFullString().Trim());
        }
        classBuilder.WriteLine("}");
        classBuilder.WriteLine();
    }

    private static string GetParameterName(IMethodSymbol methodSymbol, int index) =>
        methodSymbol.Parameters.Where(x => x.Type.SpecialType == SpecialType.System_String).ToList()[index-1].Name;
}
