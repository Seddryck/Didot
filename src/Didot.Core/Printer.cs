using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core;

namespace Didot.Core;

public class Printer
{
    protected ITemplateEngine TemplateEngine { get; }
    protected ISourceParser SourceParser { get; }

    public Printer(ITemplateEngine templateEngine, ISourceParser sourceParser)
        => (TemplateEngine, SourceParser) = (templateEngine, sourceParser);

    public string Render(string template, string content)
        => TemplateEngine.Render(template, new { model = SourceParser.Parse(content)});

    public string Render(Stream template, string content)
        => TemplateEngine.Render(template, new { model = SourceParser.Parse(content) });

    public string Render(string template, Stream content)
        => TemplateEngine.Render(template, new { model = SourceParser.Parse(content) });

    public string Render(Stream template, Stream content)
        => TemplateEngine.Render(template, new { model = SourceParser.Parse(content) });
}
