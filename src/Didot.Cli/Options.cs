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

    [Option('s', "Source", Required = true, HelpText = "Path to the source file.")]
    public required string Source { get; set; }

    [Option('o', "Output", Required = false, HelpText = "Path to the generated file.")]
    public required string Output { get; set; }
}
