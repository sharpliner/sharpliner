using System;
using System.Runtime.CompilerServices;

namespace Sharpliner;

internal static class Require
{
    internal static string NotNullAndNotEmpty(string input, [CallerArgumentExpression(nameof(input))]string paramName = "")
    {
        #if NET7_0_OR_GREATER
        ArgumentException.ThrowIfNullOrEmpty(input, paramName);
        #else
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("Value cannot be null or empty.", paramName);
        }
        #endif
        return input;
    }
}