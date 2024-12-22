using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

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

    public static int[] ToArrayInt(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Input cannot be null or empty.", nameof(value));

        if (int.TryParse(value, out int single))
            return [single];

        while(value.Contains(' '))
            value = value.Replace(" ","");

        if (value[0]!='[' || value[^1]!=']')
            throw new ArgumentException("Input must start by '[' and end by ']'.", nameof(value));
        value = value.Substring(1, value.Length - 2);

        var values = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var intValues = new int[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            if (int.TryParse(values[i], out int integer))
                intValues[i] = integer;
            else
                throw new ArgumentException($"Input contains '{values[i]}' which can't be parsed as an integer.", nameof(value));
        }
        return intValues;
    }
}
