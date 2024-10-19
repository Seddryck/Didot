using System;
using CommandLine;
using Didot.Core;
using Didot.Core.SourceParsers;
using Didot.Core.TemplateEngines;

namespace Didot.Cli;
public class Program
{
    public static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(RunWithOptions)
            .WithNotParsed(HandleParseError);
    }

    static void RunWithOptions(Options opts)
    {
        var sourceExtension = new FileInfo(opts.Source).Extension;
        var parserFactory = new FileBasedSourceParserFactory();
        var parser = parserFactory.GetSourceParser(sourceExtension);

        var templateExtension = new FileInfo(opts.Template).Extension;
        var engineFactory = new FileBasedTemplateEngineFactory();
        var engine = engineFactory.GetTemplateEngine(templateExtension);

        var printer = new Printer(engine, parser);
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
