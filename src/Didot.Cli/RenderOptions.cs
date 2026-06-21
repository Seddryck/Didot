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
    public Option<string> Template { get; } = new (
        "--template", "-t"
    )
    {
        Description = "Path to the template file.",
        Required = false,
        Arity = ArgumentArity.ExactlyOne
    };

    public Option<string> Engine { get; } = new (
        "--engine", "-e"
    )
    {
        Description = "The template engine to use or to enforce when rendering the template.",
        Arity = ArgumentArity.ZeroOrOne,
        CustomParser = result => result.Tokens.Any() ? result.Tokens[0].Value.Trim() : string.Empty
    };

    public Option<Dictionary<string, string>> EngineExtensions { get; } = new (
        "--engine-extension", "-x"
    )
    {
        Description = "Associate a file's extension to a specific template engine.",
        Arity = ArgumentArity.ZeroOrMore,
        AllowMultipleArgumentsPerToken = false,
        CustomParser = result => ParseKeyValuePairs(result, ':', ';', NormalizeExtension)
    };

    public Option<Dictionary<string, string>> Sources { get; } = new (
        "--source", "-s"
    )
    {
        Description = "The source file(s) to use when rendering the template. The key is an optional identifier for the source that can be used in the template to reference it. If only one source file is provided without a key, it will be associated with an empty string key. This option cannot be used together with --stdin.",
        Arity = ArgumentArity.ZeroOrMore,
        AllowMultipleArgumentsPerToken = true,
        CustomParser = result => ParseKeyValuePairs(result, ':', ';', null, true)
    };

    public Option<bool> StdIn { get; } = new (
        "--stdin", "-i"
        
    )
    {
        Description = "Indicates that the input will come from stdin.",
        Arity = ArgumentArity.ZeroOrOne
    };

    public Option<string> Parser { get; } = new (
        "--parser", "-r"
    )
    {
        Description = "The parser to use when reading from StdIn or to enforce when reading from files.",
        Arity = ArgumentArity.ZeroOrOne,
        CustomParser = result => result.Tokens.Any() ? result.Tokens[0].Value.Trim() : string.Empty
    };

    public Option<Dictionary<string, string>> ParserExtensions { get; } = new Option<Dictionary<string, string>>(
        "--parser-extension", "-X"
    )
    {
        Description = "Associate a file's extension to a specific parser.",
        Arity = ArgumentArity.ZeroOrMore,
        AllowMultipleArgumentsPerToken = true,
        CustomParser = result => ParseKeyValuePairs(result, ':', ';', NormalizeExtension)
    };

    public Option<Dictionary<string, string>> ParserParams { get; } = new (
        "--parser-parameter", "-P"
    )
    {
        Description = "Provide key-value parameters for parsers, such as configuration or dialect settings.",
        Arity = ArgumentArity.ZeroOrMore,
        AllowMultipleArgumentsPerToken = true,
        CustomParser = result => ParseKeyValuePairs(result, ':', ';')
    };

    public Option<string> Output { get; } = new (
        "--output", "-o"
    )
    {
        Description = "Path to the output file. If not provided, the rendered template will be written to stdout.",
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
                        result.AddError($"The key is missing for the key-value pair: {pair}. A key is required when multiple key-value pairs are provided.");
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
