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

    [Option('s', "Source", Required = false, HelpText = "Path to the source file.", SetName = "Input")]
    public string? Source { get; set; }

    [Option('i', "StdIn", Required = false, HelpText = "Indicates that the input will come from stdin.", SetName = "Input")]
    public bool StdIn { get; set; }

    [Option('p', "Parser", Required = false, HelpText = "The parser to use when reading from StdIn.")]
    public string? Parser { get; set; }

    [Option('o', "Output", Required = false, HelpText = "Path to the generated file.")]
    public string? Output { get; set; }
}
