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

    internal static T NotNull<T>(T input, [CallerArgumentExpression(nameof(input))]string paramName = "") where T : class
    {
        #if NET7_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(input, paramName);
        #else
        if (input is null)
        {
            throw new ArgumentException("Value cannot be null.", paramName);
        }
        #endif
        return input;
    }
}