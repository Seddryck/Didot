using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Runtime.CompilerServices;
using Didot.Core;

namespace Didot.Cli;
public class RenderOptions
{
    public Option<string> Template { get; } = new Option<string>(
        new[] { "-t", "--template" },
        description: "Path to the template file."
    )
    {
        IsRequired = true,
        Arity = ArgumentArity.ExactlyOne
    };

    public Option<string> Engine { get; } = new Option<string>(
        new[] { "-e", "--engine" },
        description: "The template engine to use or to enforce when rendering the template.",
        parseArgument: result => result.Tokens.Any() ? result.Tokens[0].Value.Trim() : string.Empty
    )
    {
        Arity = ArgumentArity.ZeroOrOne
    };

    public Option<Dictionary<string, string>> EngineExtensions { get; } = new Option<Dictionary<string, string>>(
        new[] { "-x", "--engine-extension" },
        description: "Associate a file's extension to a specific template engine.",
        parseArgument: result => ParseKeyValuePairs(result, ':', ';', NormalizeExtension)
    )
    {
        Arity = ArgumentArity.ZeroOrMore,
        AllowMultipleArgumentsPerToken = false
    };

    public Option<Dictionary<string, string>> Sources { get; } = new Option<Dictionary<string, string>>(
        new[] { "-s", "--source" },
        description: "Path to the source file.",
        parseArgument: result => ParseKeyValuePairs(result, ':', ';', null, true)
    )
    {
        Arity = ArgumentArity.ZeroOrMore,
        AllowMultipleArgumentsPerToken = true
    };

    public Option<bool> StdIn { get; } = new Option<bool>(
        new[] { "-i", "--stdin" },
        description: "Indicates that the input will come from stdin."
    )
    {
        Arity = ArgumentArity.ZeroOrOne
    };

    public Option<string> Parser { get; } = new Option<string>(
        new[] { "-r", "--parser" },
        description: "The parser to use when reading from StdIn or to enforce when reading from files.",
        parseArgument: result => result.Tokens.Any() ? result.Tokens[0].Value.Trim() : string.Empty
    )
    {
        Arity = ArgumentArity.ZeroOrOne
    };

    public Option<Dictionary<string, string>> ParserExtensions { get; } = new Option<Dictionary<string, string>>(
            new[] { "-X", "--parser-extension" },
            description: "Associate a file's extension to a specific parser.",
            parseArgument: result => ParseKeyValuePairs(result, ':', ';', NormalizeExtension)
    )
    {
        Arity = ArgumentArity.ZeroOrMore,
        AllowMultipleArgumentsPerToken = true
    };

    public Option<Dictionary<string, string>> ParserParams { get; } = new Option<Dictionary<string, string>>(
            new[] { "-P", "--parser-parameter" },
            description: "Provide key-value parameters for parsers, such as configuration or dialect settings.",
            parseArgument: result => ParseKeyValuePairs(result, ':', ';')
    )
    {
        Arity = ArgumentArity.ZeroOrMore,
        AllowMultipleArgumentsPerToken = true
    };

    public Option<string> Output { get; } = new Option<string>(
        new[] { "-o", "--output" },
        description: "Path to the generated file."
    )
    {
        Arity = ArgumentArity.ZeroOrOne
    };

    private static Dictionary<string, string> ParseKeyValuePairs(ArgumentResult result, char keyValueSeparator, char pairSeparator, Func<string, string>? normalizeKey = null, bool allowEmptyKey = false)
    {
        var dictionary = new Dictionary<string, string>();

        foreach (var token in result.Tokens)
        {
            var pairs = token.Value.Split(pairSeparator, StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split(keyValueSeparator, 2);
                if (keyValue.Length == 1 && allowEmptyKey)
                {
                    if (!dictionary.Any())
                        dictionary[string.Empty] = keyValue[0].Trim();
                    else
                    {
                        result.ErrorMessage = $"The key is missing for the key-value pair: {pair}. A key is required when multiple key-value pairs are provided.";
                        return dictionary;
                    }
                }
                else if (keyValue.Length == 2)
                {
                    var value = keyValue[1].Length > 0 && keyValue[1].Trim().Length == 0 ? keyValue[1] : keyValue[1].Trim();
                    dictionary[normalizeKey?.Invoke(keyValue[0]) ?? keyValue[0]] = value;
                }
                else
                {
                    result.ErrorMessage = $"Invalid key-value pair: {pair}";
                    return dictionary;
                }
            }
        }
        return dictionary;
    }

    private static string NormalizeExtension(string value)
        => value.NormalizeExtension();
}
