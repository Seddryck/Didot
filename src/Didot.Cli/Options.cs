using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Didot.Cli;
public class Options
{
    [Option('t', "Template", Required = true, HelpText = "Path to the template file.")]
    public required string Template { get; set; }

    [Option('s', "Source", Required = false, Separator = ';', HelpText = "Path to the source file.", SetName = "Input")]
    public IEnumerable<string>? Sources { get; set; }

    [Option('i', "StdIn", Required = false, HelpText = "Indicates that the input will come from stdin.", SetName = "Input")]
    public bool StdIn { get; set; }

    [Option('p', "Parser", Required = false, HelpText = "The parser to use when reading from StdIn.")]
    public string? Parser { get; set; }

    [Option('e', "Engine", Required = false, HelpText = "Force a specific engine.")]
    public string? Engine { get; set; }

    [Option('x', "EngineExtension", Required = false, Separator = ';', HelpText = "Associate an extension to a specific engine.")]
    public required IEnumerable<string> EngineExtensions { get; set; }

    [Option('X', "ParserExtension", Required = false, Separator = ';', HelpText = "Associate an extension to a specific engine.")]
    public required IEnumerable<string> ParserExtensions { get; set; }

    [Option('o', "Output", Required = false, HelpText = "Path to the generated file.")]
    public string? Output { get; set; }
}
