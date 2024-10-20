using System;
using System.Text;
using CommandLine;
using CommandLine.Text;
using Didot.Core;
using Didot.Core.SourceParsers;
using Didot.Core.TemplateEngines;
using HandlebarsDotNet;

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
        if (string.IsNullOrEmpty(opts.Source) && string.IsNullOrEmpty(opts.Parser))
        {
            Console.WriteLine("Error: Missing input source. You must provide either the --parser option when reading from StdIn or the --source option for a file. At least one of these options is required.");
            return;
        }

        var sourceExtension = string.IsNullOrEmpty(opts.Source)
                                ? $".{opts.Parser!.ToLowerInvariant()}"
                                : new FileInfo(opts.Source).Extension;

        using var source = string.IsNullOrEmpty(opts.Source)
                                ? copyInStream()
                                : File.OpenRead(opts.Source);

        static Stream copyInStream()
        {
            string consoleInput = Console.In.ReadToEnd();
            byte[] inputBytes = Encoding.UTF8.GetBytes(consoleInput);
            return new MemoryStream(inputBytes);
        }

        var parserFactory = new FileBasedSourceParserFactory();
        var parser = parserFactory.GetSourceParser(sourceExtension);

        var templateExtension = new FileInfo(opts.Template).Extension;
        var engineFactory = new FileBasedTemplateEngineFactory();
        var engine = engineFactory.GetTemplateEngine(templateExtension);

        var printer = new Printer(engine, parser);
        using var template = File.OpenRead(opts.Template);
        var output = printer.Render(template, source);

        if (string.IsNullOrEmpty(opts.Output))
            Console.Out.WriteLine(output);
        else
            File.WriteAllText(opts.Output, output);
    }

    static void HandleParseError(IEnumerable<Error> errs)
    {
        // Handle errors here (e.g., show help message)
        System.Console.WriteLine("Error parsing arguments.");
    }
}
