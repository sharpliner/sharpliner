using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sharpliner.SourceGenerator;

[AttributeUsage(AttributeTargets.Class)]
public sealed class SharplinerTemplateParametersAttribute : Attribute
{
}

[Generator]
public class SharplinerTemplateParametersSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var templateParametersDeclarations = context.SyntaxProvider.ForAttributeWithMetadataName(
            typeof(SharplinerTemplateParametersAttribute).FullName!,
            predicate: static (node, ctx) => node is ClassDeclarationSyntax classDeclaration && IsTemplateParametersClass(classDeclaration),
            transform: static (syntaxContext, ctx) => (ClassDeclarationSyntax)syntaxContext.TargetNode);

        context.RegisterSourceOutput(templateParametersDeclarations, static (ctx, classDeclaration) =>
        {
            var generatedMembers = GenerateTemplateParameters(classDeclaration);
            var className = classDeclaration.Identifier.Text;

            ctx.AddSource($"{className}.g.cs", generatedMembers);
        });
    }

    private static bool IsTemplateParametersClass(ClassDeclarationSyntax classDeclaration)
    {
        if (classDeclaration.BaseList?.Types.Count is not 1)
        {
            return false;
        }

        var baseType = classDeclaration.BaseList.Types[0].Type;
        return baseType is GenericNameSyntax genericName && genericName.Identifier.Text is "TemplateParametersProviderBase"
            && genericName.TypeArgumentList.Arguments.Count is 1
            && genericName.TypeArgumentList.Arguments[0] is IdentifierNameSyntax identifierName && identifierName.Identifier.Text.Equals(classDeclaration.Identifier.Text);
    }

    private static string GenerateTemplateParameters(ClassDeclarationSyntax classDeclaration)
    {
        var builder = new StringBuilder();
        var writer = new IndentedTextWriter(new StringWriter(builder));

        writer.WriteLine("using System;");
        writer.WriteLine();
        var namespaceName = classDeclaration.Parent?.Ancestors().OfType<NamespaceDeclarationSyntax>().Single().Name.ToString();
        writer.WriteLine($"namespace {namespaceName};");

        writer.WriteLine($"partial class {classDeclaration.Identifier.Text}");
        writer.WriteLine("{");
        writer.Indent++;

        var parameters = new List<string>();
        foreach (var property in classDeclaration.Members.OfType<PropertyDeclarationSyntax>())
        {
            var propertyName = property.Identifier.Text;
            var defaultValue = property.Initializer?.Value.ToString() ?? "default";

            var rawParameterType = property.Type.ToString();
            var parameterType = rawParameterType switch
            {
                "string" => "StringParameter",
                "int?" => "NumberParameter",
                "bool?" => "BooleanParameter",
                "Step" => "StepParameter",
                "ConditionedList<Step>" => "StepListParameter",
                "JobBase" => "JobParameter",
                "ConditionedList<JobBase>" => "JobListParameter",
                "DeploymentJob" => "DeploymentParameter",
                "ConditionedList<DeploymentJob>" => "DeploymentListParameter",
                "Stage" => "StageParameter",
                "ConditionedList<Stage>" => "StageListParameter",
                _ => $"ObjectParameter<{rawParameterType}>"
            };

            var parameterName = propertyName + "Parameter";
            writer.WriteLine($"public static {parameterType} {parameterName} {{ get; }} = new(\"{propertyName}\", defaultValue: {defaultValue});");

            parameters.Add(parameterName);
        }

        writer.WriteLine($"public override List<Parameter> ToParameters() => [ {string.Join(", ", parameters)} ];");
        writer.Indent--;

        return builder.ToString();
    }
}
