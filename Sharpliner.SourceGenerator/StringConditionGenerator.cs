using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Sharpliner.SourceGenerator;

[Generator]
public class StringConditionGenerator : ISourceGenerator
{
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

        var index = 0;
        GenerateClassesForMethods(context, syntaxReciever, ref index);
        GenerateClasses(context, syntaxReciever, ref index);
    }

    private void GenerateClassesForMethods(GeneratorExecutionContext context,
        StringConditionAttributeSyntaxReceiver syntaxReciever, ref int index)
    {
        foreach (var containingClassGroup in syntaxReciever.IdentifiedMethods.GroupBy(x => x.ContainingType))
        {
            index++;
            var containingClass = containingClassGroup.Key;
            var namespaceSymbol = containingClass.ContainingNamespace;

            var source = GenerateClass(context, containingClass, namespaceSymbol, containingClassGroup.ToList());
            context.AddSource(
                $"{containingClass.ContainingNamespace.ToDisplayString().Replace('.', '_')}_{containingClass.Name}_StringConditions{index}.generated",
                SourceText.From(source, Encoding.UTF8));
        }
    }

    private void GenerateClasses(GeneratorExecutionContext context,
        StringConditionAttributeSyntaxReceiver syntaxReciever, ref int index)
    {
        foreach (var @class in syntaxReciever.IdentifiedClasses)
        {
            index++;
            var namespaceSymbol = @class.ContainingNamespace;

            var source = GenerateClass(context, namespaceSymbol, @class);
            context.AddSource(
                $"{@class.ContainingNamespace.ToDisplayString().Replace('.', '_')}_{@class.Name}_StringConditions{index}.generated",
                SourceText.From(source, Encoding.UTF8));
        }
    }

    private string GenerateClass(GeneratorExecutionContext context, INamespaceSymbol @namespace,
        INamedTypeSymbol @class)
    {
        var classBuilder = new CodeGenerationTextWriter();

        classBuilder.WriteLine(context.GetUsingStatementsForTypes(
            typeof(string),
            typeof(StringConditionAttribute),
            typeof(CallerMemberNameAttribute),
            typeof(IEnumerable<>),
            typeof(Enumerable)
        ));
        classBuilder.WriteLine("using Sharpliner.AzureDevOps.ConditionedExpressions;");
        classBuilder.WriteLine();
        classBuilder.WriteLine($"namespace {@namespace.ToDisplayString()}");
        classBuilder.WriteLine("{");

        var isInlineConditionClass = @class.Name.StartsWith("Inline");

        var possibleTypes = isInlineConditionClass
            ? PossibleType.InlinePossibleTypes
            : PossibleType.IfPossibleTypes;

        var possibleTypesForParameter1 = possibleTypes.ToArray();
        var possibleTypesForParameter2 = possibleTypes.ToArray();

        // TODO If(has attribute ArrayType param1) typesParam1.Add(Array);
        // TODO If(has attribute ArrayType param2) typesParam2.Add(Array);

        var constructor = @class.Constructors.First();
        var countOfStringParameters =
            constructor.Parameters.Count(p => p.Type.SpecialType == SpecialType.System_String);

        foreach (var possibleType in possibleTypesForParameter1)
        {
            WriteEnumerableObjectConstructors(@class, classBuilder, possibleType);

            if (countOfStringParameters == 1)
            {
                if (possibleType == PossibleType.String)
                {
                    // Already defined manually.
                    continue;
                }

                WriteConstructor(@class, possibleType, null, classBuilder, constructor);
                continue;
            }

            foreach (var possibleType2 in possibleTypesForParameter2)
            {
                if (possibleType == PossibleType.String && possibleType2 == PossibleType.String)
                {
                    // Already defined manually.
                    continue;
                }

                WriteConstructor(@class, possibleType, possibleType2, classBuilder, constructor);
            }
        }

        classBuilder.WriteLine("}");

        if (@class.Name.StartsWith("IfBranchCondition"))
        {
            Console.WriteLine("BREAK");
        }

        return classBuilder.ToString();
    }

    private static void WriteEnumerableObjectConstructors(INamedTypeSymbol @class,
        CodeGenerationTextWriter classBuilder, PossibleType nonArrayParameterType)
    {
        foreach (var constructor in @class.Constructors
                     .Where(c => c.Parameters
                         .Any(p => p.Type is IArrayTypeSymbol)))
        {
            classBuilder.WriteLine(@class.GetClassDeclarationSyntax().GetClassDefinition());
            classBuilder.WriteLine("{");

            var constructorDeclarationSyntax = constructor.GetConstructorDeclarationSyntax();

            classBuilder.Write($"{constructorDeclarationSyntax.Modifiers} {constructorDeclarationSyntax.Identifier}(");

            var newParameters = constructor.Parameters
                .Select(constructorParameter => constructorParameter.Type is IArrayTypeSymbol
                    ? $"{PossibleType.EnumerableObject.ClassName} {constructorParameter.Name}"
                    : $"{nonArrayParameterType.ClassName} {constructorParameter.Name}");

            classBuilder.Write(string.Join(", ", newParameters));

            classBuilder.WriteLine(
                $") {constructorDeclarationSyntax.Initializer?.WithArgumentsInSerializeMethodCall(constructor)}");

            classBuilder.WriteLine("{");
            classBuilder.WriteLine("}");

            classBuilder.WriteLine("}");
        }
    }

    private void WriteConstructor(INamedTypeSymbol @class, PossibleType possibleType, PossibleType possibleType2,
        CodeGenerationTextWriter classBuilder, IMethodSymbol constructor, bool removeParams = false)
    {
        // var newClassName = @class.Name.Replace(
        //     PossibleType.String.ShortName,
        //     $"{possibleType.ShortName}{possibleType2.ShortName}"
        // );

        classBuilder.WriteLine(@class.GetClassDeclarationSyntax().GetClassDefinition());
        classBuilder.WriteLine("{");

        WriteConstructor(classBuilder, constructor, possibleType, possibleType2, removeParams);

        classBuilder.WriteLine("}");
    }

    private void WriteConstructor(CodeGenerationTextWriter classBuilder, IMethodSymbol constructor, PossibleType possibleType, PossibleType? possibleType2, bool removeParams)
    {
        var constructorDeclarationSyntax = constructor.GetConstructorDeclarationSyntax();

        var parameter1Name = GetParameterName(constructor, 1);
        var parameter2Name = GetParameterName(constructor, 2);

        classBuilder.Write($"{constructorDeclarationSyntax.Modifiers} {constructorDeclarationSyntax.Identifier}(");

        var newParameters = constructor.Parameters
            .Select(constructorParameter =>
            {
                var isArray = constructorParameter.Type is IArrayTypeSymbol;
                var arraySyntax = isArray ? string.Empty : "[]";

                if (constructorParameter.Name == parameter1Name)
                {
                    return $"{possibleType.ClassName}{arraySyntax} {constructorParameter.Name}";
                }

                if (constructorParameter.Name == parameter2Name && possibleType2 != null)
                {
                    return $"{possibleType2?.ClassName}{arraySyntax} {constructorParameter.Name}";
                }

                return $"{constructorParameter.Type} {constructorParameter.Name}";
            });

        classBuilder.Write(string.Join(", ", newParameters));

        classBuilder.WriteLine(
            $") {constructorDeclarationSyntax.Initializer?.WithArgumentsInSerializeMethodCall(constructor)}");

        classBuilder.WriteLine("{");
        classBuilder.WriteLine("}");
    }

    private string GenerateClass(GeneratorExecutionContext context, INamedTypeSymbol @class, INamespaceSymbol @namespace, List<IMethodSymbol> methods)
    {
        var classBuilder = new CodeGenerationTextWriter();

        classBuilder.WriteLine(context.GetUsingStatementsForTypes(
            typeof(string),
            typeof(StringConditionAttribute),
            typeof(CallerMemberNameAttribute),
            typeof(IEnumerable<>),
            typeof(Enumerable)
            ));
        classBuilder.WriteLine("using Sharpliner.AzureDevOps.ConditionedExpressions;");
        classBuilder.WriteLine();
        classBuilder.WriteLine($"namespace {@namespace.ToDisplayString()}");
        classBuilder.WriteLine("{");
        classBuilder.WriteLine(@class.GetClassDeclarationSyntax().GetClassDefinition());
        classBuilder.WriteLine("{");

        foreach (IMethodSymbol methodSymbol in methods)
        {
            var methodDeclarationSyntax = methodSymbol.DeclaringSyntaxReferences
                    .Select(s => s.GetSyntax())
                    .OfType<MethodDeclarationSyntax>()
                    .First();

            var methodParameters = methodSymbol.Parameters;
            var countOfStringParameters = methodParameters.Count(p => p.Type.SpecialType == SpecialType.System_String);

            var isInlineConditionClass = methodSymbol.ReturnType.Name.StartsWith("Inline");

            var isArrayType = methodParameters.Any(p => p.Type.ToDisplayString(SymbolDisplayFormats.NamespaceAndType) == typeof(string[]).FullName);

            var possibleTypes = isInlineConditionClass
                ? PossibleType.InlinePossibleTypes
                : PossibleType.IfPossibleTypes;

            var possibleTypesForParameter1 = possibleTypes.ToArray();
            var possibleTypesForParameter2 = possibleTypes.ToArray();

            foreach (var parameter1 in possibleTypesForParameter1)
            {
                foreach (var parameter2 in possibleTypesForParameter2)
                {
                    if (isArrayType)
                    {
                        if (parameter1 == PossibleType.String && parameter2 == PossibleType.String)
                        {
                            // These are already declared by the base code.
                            continue;
                        }

                        WriteArrayMethod(classBuilder, methodSymbol, methodDeclarationSyntax, parameter1.ClassName,
                            parameter2.ClassName + "[]");
                        continue;
                    }

                    if (parameter1 == PossibleType.String && parameter2 == PossibleType.String)
                    {
                        // These are already declared by the base code.
                        continue;
                    }

                    if (countOfStringParameters == 2)
                    {
                        WriteTwoStrings(classBuilder, methodSymbol, methodDeclarationSyntax, parameter1.ClassName, parameter2.ClassName);
                        continue;
                    }
                }

                if (isArrayType)
                {
                    WriteArrayMethod(classBuilder, methodSymbol, methodDeclarationSyntax, parameter1.ClassName,
                        PossibleType.EnumerableObject.ClassName);
                    continue;
                }

                if (countOfStringParameters == 1)
                {
                    if (parameter1 == PossibleType.String)
                    {
                        // These are already declared by the base code.
                        continue;
                    }

                    WriteSingleString(classBuilder, methodSymbol, methodDeclarationSyntax, parameter1.ClassName);
                    continue;
                }
            }
        }

        classBuilder.WriteLine("}");
        classBuilder.WriteLine("}");

        return classBuilder.ToString();
    }

    private void WriteArrayMethod(CodeGenerationTextWriter classBuilder, IMethodSymbol methodSymbol, MethodDeclarationSyntax methodDeclarationSyntax, string parameter1, string parameter2)
    {
        WriteTwoStrings(classBuilder, methodSymbol, methodDeclarationSyntax, parameter1, parameter2);
    }

    private void WriteSingleString(CodeGenerationTextWriter classBuilder, IMethodSymbol methodSymbol, MethodDeclarationSyntax methodDeclarationSyntax, string parameter1)
    {
        classBuilder.WriteLine($"public Condition {methodSymbol.Name}{GetGenericType(methodSymbol)}({parameter1} {GetParameterName(methodSymbol, 1)})");
        WriteMethodBody(classBuilder, methodDeclarationSyntax, null);
    }

    private void WriteTwoStrings(CodeGenerationTextWriter classBuilder, IMethodSymbol methodSymbol,
        MethodDeclarationSyntax methodDeclarationSyntax, string parameter1,
        string parameter2)
    {
        var parameter1Name = GetParameterName(methodSymbol, 1);
        var parameter2Name = GetParameterName(methodSymbol, 2);
        classBuilder.WriteLine(
            $"public {methodSymbol.ReturnType.Name} {methodSymbol.Name}{GetGenericType(methodSymbol)}({parameter1} {parameter1Name}, {parameter2} {parameter2Name})");
        WriteMethodBody(classBuilder, methodDeclarationSyntax, parameter2.EndsWith("[]") ? parameter2Name : null);
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

    private string GetGenericType(IMethodSymbol @class)
    {
        if (!@class.TypeArguments.Any())
        {
            return string.Empty;
        }

        var genericTypes = @class.TypeArguments.Select(x => x.Name);
        return $"<{string.Join(", ", genericTypes)}>";
    }

    private static void WriteMethodBody(TextWriter classBuilder, BaseMethodDeclarationSyntax methodDeclarationSyntax,
        string? arrayParameterName)
    {
        // TODO - Convert array types to string[]
        var expressionBody = methodDeclarationSyntax.ExpressionBody?.ToString();
        if (!string.IsNullOrEmpty(expressionBody))
        {
            if (arrayParameterName != null)
            {
                expressionBody = expressionBody.Replace($" {arrayParameterName}", $" {arrayParameterName}.Cast<object>()");
            }

            classBuilder.Write(expressionBody);
            classBuilder.WriteLine(';');
            classBuilder.WriteLine();
            return;
        }

        classBuilder.WriteLine("{");
        foreach (StatementSyntax statementSyntax in methodDeclarationSyntax.Body.Statements)
        {
            var statementString = statementSyntax.ToFullString();

            if (arrayParameterName != null)
            {
                statementString = statementString.Replace($" {arrayParameterName}", $" {arrayParameterName}.Cast<object>()");
            }

            classBuilder.WriteLine(statementString.Trim());
        }
        classBuilder.WriteLine("}");
        classBuilder.WriteLine();
    }

    private static string? GetParameterName(IMethodSymbol methodSymbol, int index) =>
        methodSymbol.Parameters.Where(x =>
            x.Type.ToDisplayString(SymbolDisplayFormats.NamespaceAndType) == typeof(string).FullName
            || x.Type.ToDisplayString(SymbolDisplayFormats.NamespaceAndType) == typeof(string[]).FullName
        ).ToList().ElementAtOrDefault(index - 1)?.Name;
}
