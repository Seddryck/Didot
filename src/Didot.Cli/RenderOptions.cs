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
    public Option<string> Template { get; } = new Option<string>("--template", ["-t"])
    {
        Description = "Path to the template file.",
        Required = true,
        Arity = ArgumentArity.ExactlyOne
    };

    public Option<string> Engine { get; } = new Option<string>("--engine", ["-e"])
    {
        CustomParser = result => result.Tokens.Any() ? result.Tokens[0].Value.Trim() : string.Empty,
        Description = "The template engine to use or to enforce when rendering the template.",
        Arity = ArgumentArity.ZeroOrOne
    };

    public Option<Dictionary<string, string>> EngineExtensions { get; } = new Option<Dictionary<string, string>>(
        "--engine-extension",["-x"]
    )
    {
        CustomParser = result => ParseKeyValuePairs(result, ':', ';', NormalizeExtension),
        Description = "Associate a file's extension to a specific template engine.",
        Arity = ArgumentArity.ZeroOrMore,
        AllowMultipleArgumentsPerToken = false
    };

    public Option<Dictionary<string, string>> Sources { get; } = new Option<Dictionary<string, string>>(
        "--source", ["-s"]
    )
    {
        Description = "Path to the source file.",
        CustomParser = result => ParseKeyValuePairs(result, ':', ';', null, true),
        Arity = ArgumentArity.ZeroOrMore,
        AllowMultipleArgumentsPerToken = true
    };

    public Option<bool> StdIn { get; } = new Option<bool>(
        "--stdin", ["-i"]
    )
    {
        Description = "Indicates that the input will come from stdin.",
        Arity = ArgumentArity.ZeroOrOne
    };

    public Option<string> Parser { get; } = new Option<string>(
        "--parser", ["-r"]
    )
    {
        CustomParser = result => result.Tokens.Any() ? result.Tokens[0].Value.Trim() : string.Empty,
        Description = "The parser to use when reading from StdIn or to enforce when reading from files.",
        Arity = ArgumentArity.ZeroOrOne
    };

    public Option<Dictionary<string, string>> ParserExtensions { get; } = new Option<Dictionary<string, string>>(
            "--parser-extension", ["-X"]
    )
    {
        Arity = ArgumentArity.ZeroOrMore,
        AllowMultipleArgumentsPerToken = true,
        CustomParser = result => ParseKeyValuePairs(result, ':', ';', NormalizeExtension),
        Description = "Associate a file's extension to a specific parser.",
    };

    public Option<Dictionary<string, string>> ParserParams { get; } = new Option<Dictionary<string, string>>(
            "--parser-parameter", ["-P"]
    )
    {
        Arity = ArgumentArity.ZeroOrMore,
        AllowMultipleArgumentsPerToken = true,
        CustomParser = result => ParseKeyValuePairs(result, ':', ';'),
        Description = "Provide key-value parameters for parsers, such as configuration or dialect settings.",
    };

    public Option<string> Output { get; } = new Option<string>(
        "--output", ["-o"]
    )
    {
        Arity = ArgumentArity.ZeroOrOne,
        Description = "Path to the generated file."
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
                        result.AddError($"The key is missing for the key-value pair: {pair}. A key is required when multiple key-value pairs are provided.");
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
                    result.AddError($"Invalid key-value pair: {pair}");
                    return dictionary;
                }
            }
        }
        return dictionary;
    }

    private static string NormalizeExtension(string value)
        => value.NormalizeExtension();
}
