using System;
using CommandLine;
using Didot.Core;
using Didot.Core.DataSourceEngines;
using Didot.Core.TemplateEngines;

namespace Didot.Cli;
class Program
{
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(RunWithOptions)
            .WithNotParsed(HandleParseError);
    }

    static void RunWithOptions(Options opts)
    {
        var printer = new Printer(new ScribanWrapper(), new YamlSource());
        using var source = new StreamReader(opts.Source);
        using var template = new StreamReader(opts.Template);
        var output = printer.Render(template, source);
        File.WriteAllText(opts.Output, output);
    }

    static void HandleParseError(IEnumerable<Error> errs)
    {
        // Handle errors here (e.g., show help message)
        System.Console.WriteLine("Error parsing arguments.");
    }
}
