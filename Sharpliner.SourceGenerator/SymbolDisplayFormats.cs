using Microsoft.CodeAnalysis;

namespace Sharpliner.SourceGenerator;

public static class SymbolDisplayFormats
{
    public static readonly SymbolDisplayFormat NamespaceAndType =
        new(
            SymbolDisplayGlobalNamespaceStyle.Omitted,
            SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            SymbolDisplayGenericsOptions.IncludeTypeParameters
        );
}
