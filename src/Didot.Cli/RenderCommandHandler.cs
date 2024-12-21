using System;
using System.CommandLine;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Didot.Core;
using Didot.Core.SourceParsers;
using Didot.Core.TemplateEngines;
using Microsoft.Extensions.Logging;

namespace Didot.Cli;
public class RenderCommandHandler
{
    protected ILogger<RenderCommand>? Logger { get; }

    public RenderCommandHandler(ILogger<RenderCommand>? logger = null)
        => Logger = logger;

    public virtual void Execute(
        string template
        , string engine
        , IDictionary<string, string> engineExtensions
        , IDictionary<string, string> sources
        , string parser
        , IDictionary<string, string> parserExtensions
        , IDictionary<string, string> parserParams
        , string output
    )
    {
        Logger?.LogInformation("Starting RenderCommand execution.");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            Logger?.LogInformation("Configured {ParserExtensionCount} parser associations to file extensions.", parserExtensions.Count());
            Logger?.LogInformation("Configured {ParserExtensionCount} parser parameters.", parserParams.Count());
            foreach (var param in parserParams)
                Logger?.LogInformation("Parser parameter: {ParserParamKey}={ParserParamValue}.", param.Key, param.Value);
            var parserFactory = GetSourceParserFactory(parserExtensions, parserParams);

            Logger?.LogInformation("Configured {EnginerExtensionCount} template engine associations to file extensions.", engineExtensions.Count());
            var engineFactory = GetTemplateEngineFactory(engineExtensions);

            Logger?.LogInformation("Configured {DataSourceFileCount} data source file(s) for processing.", sources.Count());
            var allSources = GetSources(sources, parserFactory, parser);

            var templateEngine = GetTemplateEngine(engineFactory, engine, template);
            Logger?.LogInformation("Using template engine: {TemplateEngineName}.", templateEngine.GetType().Name);

            var result = GetRenderedOutput(templateEngine, template, allSources);

            if (string.IsNullOrEmpty(output))
            {
                Logger?.LogInformation("Output destination is Console. Writing result to Console.Out.");
                Console.Out.WriteLine(result);
            }
            else
            {
                Logger?.LogInformation("Output destination is file. Writing result to {FilePath}.", output);
                File.WriteAllText(output, result);
            }

            Logger?.LogInformation("RenderCommand execution completed.");
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error during RenderCommand execution.");
            throw;
        }
        finally
        {
            stopwatch.Stop();
            Logger?.LogInformation("RenderCommand execution time: {ElapsedMilliseconds} ms.", stopwatch.ElapsedMilliseconds);
        }
    }

    protected virtual FileBasedSourceParserFactory GetSourceParserFactory(IDictionary<string, string> extensions, IDictionary<string, string> parameters)
    {
        var factory = new FileBasedSourceParserFactory(parameters);
        factory.AddOrReplace(extensions);
        
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
