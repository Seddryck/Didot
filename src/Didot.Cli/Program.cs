using System;
using System.Diagnostics;
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
        if ((opts.Sources is null || !opts.Sources.Any()) && string.IsNullOrEmpty(opts.Parser))
        {
            Console.WriteLine("Error: Missing input source. You must provide either the --parser option when reading from StdIn or the --source option for a file. At least one of these options is required.");
            return;
        }

        if (opts.StdIn && string.IsNullOrEmpty(opts.Parser))
        {
            Console.WriteLine("Error: Missing input parser. When --StdIn is set, you must provide the --parser option.");
            return;
        }


        var split = (string input) =>
        {
            var parts = input.Split(':');
            if (parts.Length > 2)
                throw new ArgumentException(nameof(input));
            if (parts.Length == 1)
                return new KeyValuePair<string, string>(string.Empty, parts[0]);
            return new KeyValuePair<string, string>(parts[0].Trim(), parts[1].Trim());
        };

        var parserFactory = new FileBasedSourceParserFactory();
        parserFactory.AddOrReplace(opts.ParserExtensions
                        ?.Where(x => !string.IsNullOrEmpty(x))
                        ?.Select(x => split(x))
                        ?? []);

        var engineFactory = new FileBasedTemplateEngineFactory();
        engineFactory.AddOrReplace(opts.EngineExtensions
                        ?.Where(x => !string.IsNullOrEmpty(x))
                        ?.Select(x => split(x))
                        ?? []);

        var sourceExtension = string.IsNullOrEmpty(opts.Parser)
                                ? new FileInfo(opts.Sources!.First()).Extension
                                : $".{opts.Parser!.ToLowerInvariant()}";

        var sources = new Dictionary<string, ISource>();
        foreach (var source in opts.Sources?.Where(x => !string.IsNullOrEmpty(x)).DefaultIfEmpty(null) ?? [null])
        {
            KeyValuePair<string, string>? sourceTag = string.IsNullOrEmpty(source) ? null : split(source);
            var sourceStream = sourceTag is null
                                    ? copyInStream()
                                    : File.OpenRead(sourceTag.Value.Value);

            static Stream copyInStream()
            {
                string consoleInput = Console.In.ReadToEnd();
                byte[] inputBytes = Encoding.UTF8.GetBytes(consoleInput);
                return new MemoryStream(inputBytes);
            }

            var parser = GetSourceParser(parserFactory, sourceTag.HasValue ? sourceTag.Value.Value : null, opts);
            sources.Add(sourceTag.HasValue ? sourceTag.Value.Key : string.Empty
                , new Source(sourceStream, parser));
        }
        var engine = GetTemplateEngine(engineFactory, opts);

        var printer = new Printer(engine);
        var output = string.Empty;
        try
        {
            using var template = File.OpenRead(opts.Template);
            output = printer.Render(template, sources);
        }
        catch (Exception )
        { throw; }
        finally
        {
            foreach (var source in sources)
                source.Value.Content.Dispose();
        }

        if (string.IsNullOrEmpty(opts.Output))
            Console.Out.WriteLine(output);
        else
            File.WriteAllText(opts.Output, output);
    }


    private static ISourceParser GetSourceParser(FileBasedSourceParserFactory factory, string? sourceFile, Options opts)
    {
        if (opts.Parser is not null)
            return factory.GetByTag(opts.Parser);
        else
        {
            var sourceExtension = new FileInfo(sourceFile!).Extension;
            return factory.GetByExtension(sourceExtension);
        }
    }

    private static ITemplateEngine GetTemplateEngine(FileBasedTemplateEngineFactory factory, Options opts)
    {
        if (opts.Engine is not null)
            return factory.GetByTag(opts.Engine);
        else
        {
            var templateExtension = new FileInfo(opts.Template).Extension;
            return factory.GetByExtension(templateExtension);
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
