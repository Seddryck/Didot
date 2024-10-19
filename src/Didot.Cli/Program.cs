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
        var extension = new FileInfo(opts.Source).Extension;
        var sourceFactory = new FileBasedSourceEngineFactory();
        var parser = sourceFactory.GetSourceParser(extension);

        var printer = new Printer(new ScribanWrapper(), parser);
        using var source = File.OpenRead(opts.Source);
        using var template = File.OpenRead(opts.Template);
        var output = printer.Render(template, source);
        File.WriteAllText(opts.Output, output);
    }

    static void HandleParseError(IEnumerable<Error> errs)
    {
        // Handle errors here (e.g., show help message)
        System.Console.WriteLine("Error parsing arguments.");
    }
}
