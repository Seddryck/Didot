using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core;
public static class StringExtensions
{
    public static string NormalizeExtension(this string value)
    {
        value = value.Trim().ToLowerInvariant();
        if (value.StartsWith('.'))
            return value;
        return $".{value}";
    }

    public static bool ToBoolean(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Input cannot be null or empty", nameof(value));

        value = value.Trim().ToLowerInvariant();

        return value == "1" || value == "true";
    }
}
