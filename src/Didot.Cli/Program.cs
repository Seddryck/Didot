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

        if (opts.StdIn && string.IsNullOrEmpty(opts.Parser))
        {
            Console.WriteLine("Error: Missing input parser. When --StdIn is set, you must provide the --parser option.");
            return;
        }

        var sourceExtension = string.IsNullOrEmpty(opts.Parser)
                                ? new FileInfo(opts.Source!).Extension
                                : $".{opts.Parser!.ToLowerInvariant()}";

        using var source = string.IsNullOrEmpty(opts.Source)
                                ? copyInStream()
                                : File.OpenRead(opts.Source);

        static Stream copyInStream()
        {
            string consoleInput = Console.In.ReadToEnd();
            byte[] inputBytes = Encoding.UTF8.GetBytes(consoleInput);
            return new MemoryStream(inputBytes);
        }

        var parser = GetSourceParser(opts);
        var engine = GetTemplateEngine(opts);

        var printer = new Printer(engine, parser);
        using var template = File.OpenRead(opts.Template);
        var output = printer.Render(template, source);

        if (string.IsNullOrEmpty(opts.Output))
            Console.Out.WriteLine(output);
        else
            File.WriteAllText(opts.Output, output);
    }

    private static ISourceParser GetSourceParser(Options opts)
    {
        var parserFactory = new FileBasedSourceParserFactory();
        if (opts.Parser is not null)
            return parserFactory.GetByTag(opts.Parser);
        else
        {
            if (opts.ParserExtensions is not null && opts.ParserExtensions.Any())
            {
                foreach (var extensionAssociation in opts.ParserExtensions)
                {
                    if (!string.IsNullOrWhiteSpace(extensionAssociation))
                    {
                        var split = extensionAssociation.Split(':');
                        if (split.Length != 2)
                            throw new Exception(extensionAssociation);
                        (string extension, var engineTag) = (split[0], split[1]);
                        var instance = parserFactory.GetByTag(engineTag);
                        parserFactory.AddOrReplace(extension, instance);
                    }
                }
            }
            var sourceExtension = new FileInfo(opts.Source!).Extension;
            return parserFactory.GetByExtension(sourceExtension);
        }
    }

    private static ITemplateEngine GetTemplateEngine(Options opts)
    {
        var engineFactory = new FileBasedTemplateEngineFactory();
        if (opts.Engine is not null)
            return engineFactory.GetByTag(opts.Engine);
        else
        {
            if (opts.EngineExtensions is not null && opts.EngineExtensions.Any())
            {
                foreach (var extensionAssociation in opts.EngineExtensions)
                {
                    if (!string.IsNullOrWhiteSpace(extensionAssociation))
                    {
                        var split = extensionAssociation.Split(':');
                        if (split.Length != 2)
                            throw new Exception(extensionAssociation);
                        (string extension, var engineTag) = (split[0], split[1]);
                        var engineInstance = engineFactory.GetByTag(engineTag);
                        engineFactory.AddOrReplace(extension, engineInstance);
                    }
                }
            }
            var templateExtension = new FileInfo(opts.Template).Extension;
            return engineFactory.GetByExtension(templateExtension);
        }
    }

    static void HandleParseError(IEnumerable<Error> errs)
    {
        Console.WriteLine("Error parsing arguments.");
        foreach (var error in errs)
        {
            if (error is UnknownOptionError unknown)
                Console.WriteLine($"{unknown.Tag}: {unknown.Token}");
            else
                Console.WriteLine(error);
        }
    }
}
