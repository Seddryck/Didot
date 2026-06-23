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
    private InstallationExtensionSourceResolver? SourceResolver { get; }
    private ExtensionAssemblyLoader? AssemblyLoader { get; }

    public RenderCommandHandler(
        ILogger<RenderCommand>? logger = null,
        InstallationExtensionSourceResolver? sourceResolver = null,
        ExtensionAssemblyLoader? assemblyLoader = null)
    {
        Logger = logger;
        SourceResolver = sourceResolver;
        AssemblyLoader = assemblyLoader;
    }

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

            var hooks = LoadExtensionHooks();
            Logger?.LogInformation("Loaded {HookCount} extension hook(s).", hooks.Count);

            var result = GetRenderedOutput(templateEngine, template, allSources, hooks);

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
                var sourceStream = OpenReadOrThrowCli(source.Value);

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

    protected virtual string GetRenderedOutput(ITemplateEngine engine, string template, IDictionary<string, ISource> sources, IList<IPipelineExtensionHook> hooks)
    {
        try
        {
            using var templateStream = OpenReadOrThrowCli(template);

            var pipeline = new RenderPipeline();
            var context = new RenderPipelineContext()
            {
                TemplateEngine = engine,
                TemplateStream = templateStream,
                Inputs = sources.ToDictionary(x => x.Key, x => (IModelInput)new SourceModelInput(x.Value)),
                Hooks = hooks,
            };
            pipeline.Execute(context);
            return context.Output ?? string.Empty;
        }
        finally
        {
            foreach (var source in sources)
                source.Value.Content.Dispose();
        }
    }

    private static FileStream OpenReadOrThrowCli(string path)
    {
        try
        {
            return File.OpenRead(path);
        }
        catch (FileNotFoundException ex)
        {
            throw new RenderCliException(
                CliExitCode.NotFound,
                $"Input file '{path}' was not found.",
                path,
                ex);
        }
        catch (DirectoryNotFoundException ex)
        {
            throw new RenderCliException(
                CliExitCode.NotFound,
                $"Input file '{path}' was not found.",
                path,
                ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            throw new RenderCliException(
                CliExitCode.LoadFailure,
                $"Access denied while reading '{path}'.",
                path,
                ex);
        }
        catch (IOException ex)
        {
            throw new RenderCliException(
                CliExitCode.LoadFailure,
                $"I/O error while reading '{path}'. {ex.Message}",
                path,
                ex);
        }
    }

    protected virtual IList<IPipelineExtensionHook> LoadExtensionHooks()
    {
        if (SourceResolver is null || AssemblyLoader is null)
            return [];

        if (!SourceResolver.HasRegistryFile())
            return [];

        IReadOnlyList<RegisteredExtensionSource> registeredSources;
        try
        {
            registeredSources = SourceResolver.ResolveEnabled();
        }
        catch (ExtensionException ex)
        {
            throw ex.ToCliException();
        }

        var hooks = new List<IPipelineExtensionHook>();
        foreach (var source in registeredSources)
        {
            try
            {
                var loadedExtension = AssemblyLoader.Load(source.AssemblyPath, source.Id);
                hooks.Add(loadedExtension.Instance);
            }
            catch (ExtensionException ex)
            {
                throw ex.ToCliException();
            }
        }

        return hooks;
    }
}
