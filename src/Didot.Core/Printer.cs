using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core;

public class Printer
{
    protected ITemplateEngine TemplateEngine { get; }
    private IList<IPipelineExtensionHook> Hooks { get; } = [];

    public Printer(ITemplateEngine templateEngine)
        => (TemplateEngine) = (templateEngine);

    public Printer AddHook(IPipelineExtensionHook hook)
    {
        if (hook is null)
            throw new ArgumentNullException(nameof(hook));

        Hooks.Add(hook);
        return this;
    }

    public Printer AddHooks(IEnumerable<IPipelineExtensionHook> hooks)
    {
        if (hooks is null)
            throw new ArgumentNullException(nameof(hooks));

        foreach (var hook in hooks)
            AddHook(hook);

        return this;
    }

    public string Render(string template, object model)
        => TemplateEngine.Render(template, model);

    public string Render(Stream template, object model)
        => TemplateEngine.Render(template, model);

    public string Render(string template, string content, ISourceParser parser)
        => RenderPipeline(template, new Dictionary<string, IModelInput>()
        {
            [string.Empty] = new StringModelInput(content, parser),
        });

    public string Render(Stream template, string content, ISourceParser parser)
        => RenderPipeline(template, new Dictionary<string, IModelInput>()
        {
            [string.Empty] = new StringModelInput(content, parser),
        });

    public string Render(string template, Stream content, ISourceParser parser)
        => RenderPipeline(template, new Dictionary<string, IModelInput>()
        {
            [string.Empty] = new StreamModelInput(content, parser),
        });

    public string Render(Stream template, Stream content, ISourceParser parser)
        => RenderPipeline(template, new Dictionary<string, IModelInput>()
        {
            [string.Empty] = new StreamModelInput(content, parser),
        });

    public string Render(string template, IDictionary<string, ISource> sources)
        => RenderPipeline(template, ToModelInputs(sources));

    public string Render(Stream template, IDictionary<string, ISource> sources)
        => RenderPipeline(template, ToModelInputs(sources));

    protected virtual IPipeline<RenderPipelineContext> BuildRenderPipeline()
        => new RenderPipeline();

    private Dictionary<string, IModelInput> ToModelInputs(IDictionary<string, ISource> sources)
    {
        var inputs = new Dictionary<string, IModelInput>();
        foreach (var source in sources)
            inputs.Add(source.Key, new SourceModelInput(source.Value));
        return inputs;
    }

    private string RenderPipeline(string template, IDictionary<string, IModelInput> inputs)
    {
        var context = new RenderPipelineContext()
        {
            TemplateEngine = TemplateEngine,
            Template = template,
            Inputs = inputs,
            Hooks = Hooks,
        };
        BuildRenderPipeline().Execute(context);
        return context.Output ?? string.Empty;
    }

    private string RenderPipeline(Stream template, IDictionary<string, IModelInput> inputs)
    {
        var context = new RenderPipelineContext()
        {
            TemplateEngine = TemplateEngine,
            TemplateStream = template,
            Inputs = inputs,
            Hooks = Hooks,
        };
        BuildRenderPipeline().Execute(context);
        return context.Output ?? string.Empty;
    }
}
