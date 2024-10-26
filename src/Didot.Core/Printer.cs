using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core;
using SmartFormat.Core.Extensions;

namespace Didot.Core;

public class Printer
{
    protected ITemplateEngine TemplateEngine { get; }

    public Printer(ITemplateEngine templateEngine)
        => (TemplateEngine) = (templateEngine);

    public string Render(string template, object model)
        => TemplateEngine.Render(template, model);

    public string Render(Stream template, object model)
        => TemplateEngine.Render(template, model);

    public string Render(string template, string content, ISourceParser parser)
        => Render(template, new { model = parser.Parse(content)});

    public string Render(Stream template, string content, ISourceParser parser)
        => Render(template, new { model = parser.Parse(content) });

    public string Render(string template, Stream content, ISourceParser parser)
        => Render(template, new { model = parser.Parse(content) });

    public string Render(Stream template, Stream content, ISourceParser parser)
        => Render(template, new { model = parser.Parse(content) });

    public string Render(string template, IDictionary<string, ISource> sources)
        => Render(template, new { model = BuildModel(sources) });

    public string Render(Stream template, IDictionary<string, ISource> sources)
        => Render(template, new { model = BuildModel(sources) });

    private object BuildModel(IDictionary<string, ISource> sources)
    {
        if (sources.Count == 1 && string.IsNullOrEmpty(sources.First().Key))
            return sources.First().Value.Parser.Parse(sources.First().Value.Content);

        var model = new Dictionary<string, object>();
        foreach (var source in sources)
        {
            model.Add(source.Key, source.Value.Parser.Parse(source.Value.Content));
            source.Value.Content.Dispose();
        }
        return model;
    }
}
