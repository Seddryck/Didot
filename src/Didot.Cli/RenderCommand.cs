using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Didot.Core;
using Didot.Core.SourceParsers;
using Didot.Core.TemplateEngines;
using HandlebarsDotNet;
using SmartFormat.Core.Output;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Didot.Cli;
public class RenderCommand : RootCommand
{
    public RenderCommand(RenderOptions options)
        : base("Didot Command Line Interface")
    {
        options.EngineExtensions.SetDefaultValue(new Dictionary<string, string>());
        options.ParserExtensions.SetDefaultValue(new Dictionary<string, string>());
        options.Sources.SetDefaultValue(new Dictionary<string, string>());

        AddOption(options.Template);
        AddOption(options.Engine);
        AddOption(options.EngineExtensions);
        AddOption(options.Sources);
        AddOption(options.StdIn);
        AddOption(options.Parser);
        AddOption(options.ParserExtensions);
        AddOption(options.Output);

        AddValidator(result =>
        {
            var stdInProvided = result.GetValueForOption(options.StdIn);
            var sourcesProvided = false;
            try
            {
                sourcesProvided = result.GetValueForOption(options.Sources) is not null
                                    && result.GetValueForOption(options.Sources)!.Any();
            }
            catch { }

            if (stdInProvided && sourcesProvided)
                result.ErrorMessage = "The --stdin option cannot be provided together with the --source option.";
        });

        AddValidator(result =>
        {
            var stdInProvided = result.GetValueForOption(options.StdIn);
            var parserProvided = !string.IsNullOrEmpty(result.GetValueForOption(options.Parser));

            if (stdInProvided && !parserProvided)
                result.ErrorMessage = "The --parser option is required when using --stdin to specify the input source.";
        });

        AddValidator(result =>
        {
            var stdInProvided = result.GetValueForOption(options.StdIn);
            try
            {
                var sourceProvided = result.GetValueForOption(options.Sources) is null || result.GetValueForOption(options.Sources)!.Any();
                if (!stdInProvided && !sourceProvided)
                    result.ErrorMessage = "The --stdin option is required when not using --source to specify the input file(s).";
            }
            catch { }
        });

        this.SetHandler(Execute
            , options.Template
            , options.Engine
            , options.EngineExtensions
            , options.Sources
            , options.Parser
            , options.ParserExtensions
            , options.Output);
    }

    public virtual void Execute(
        string template
        , string engine
        , IDictionary<string, string> engineExtensions
        , IDictionary<string, string> sources
        , string parser
        , IDictionary<string, string> parserExtensions
        , string output
    )
    {
        var parserFactory = GetSourceParserFactory(parserExtensions);
        var engineFactory = GetTemplateEngineFactory(engineExtensions);
        var allSources = GetSources(sources, parserFactory, parser);

        var templateEngine = GetTemplateEngine(engineFactory, engine, template);
        var result = GetRenderedOutput(templateEngine, template, allSources);

        if (string.IsNullOrEmpty(output))
            Console.Out.WriteLine(result);
        else
            File.WriteAllText(output, result);
    }

    protected virtual FileBasedSourceParserFactory GetSourceParserFactory(IDictionary<string, string> keyValues)
    {
        var factory = new FileBasedSourceParserFactory();
        factory.AddOrReplace(keyValues);
        return factory;
    }

    protected virtual FileBasedTemplateEngineFactory GetTemplateEngineFactory(IDictionary<string, string> keyValues)
    {
        var factory = new FileBasedTemplateEngineFactory();
        factory.AddOrReplace(keyValues);
        return factory;
    }

    protected virtual Dictionary<string, ISource> GetSources(IDictionary<string, string> sources, FileBasedSourceParserFactory factory, string parserTag)
    {
        var result = new Dictionary<string, ISource>();
        if (sources.Any())
        {
            foreach (var source in sources)
            {
                var sourceStream = File.OpenRead(source.Value);
                var parser = string.IsNullOrEmpty(parserTag)
                                ? factory.GetByExtension(new FileInfo(source.Value).Extension)
                                : factory.GetByTag(parserTag);
                result.Add(source.Key, new Source(sourceStream, parser));
            }
        }
        else
        {
            var consoleInput = Console.In.ReadToEnd();
            var inputBytes = Encoding.UTF8.GetBytes(consoleInput);
            result.Add(string.Empty, new Source(new MemoryStream(inputBytes), factory.GetByTag(parserTag)));
        }
        return result;
    }

    private static ITemplateEngine GetTemplateEngine(FileBasedTemplateEngineFactory factory, string engineTag, string template)
    {
        if (!string.IsNullOrEmpty(engineTag))
            return factory.GetByTag(engineTag);
        else
        {
            var templateExtension = new FileInfo(template).Extension;
            return factory.GetByExtension(templateExtension);
        }
    }

    protected virtual string GetRenderedOutput(ITemplateEngine engine, string template, IDictionary<string, ISource> sources)
    {
        try
        {
            var printer = new Printer(engine);
            using var templateStream = File.OpenRead(template);
            return printer.Render(templateStream, sources);
        }
        catch (Exception)
        { throw; }
        finally
        {
            foreach (var source in sources)
                source.Value.Content.Dispose();
        }
    }
}
