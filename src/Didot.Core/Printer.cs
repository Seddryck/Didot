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
    {
        var source = SourceParser.Parse(content);
        var model = new { model = source };
        return TemplateEngine.Render(template, model);
    }

    public string Render(StreamReader template, string content)
    {
        var source = SourceParser.Parse(content);
        var model = new { model = source };
        return TemplateEngine.Render(template, model);
    }

    public string Render(string template, StreamReader content)
    {
        var source = SourceParser.Parse(content);
        var model = new { model = source };
        return TemplateEngine.Render(template, model);
    }

    public string Render(StreamReader template, StreamReader content)
    {
        var source = SourceParser.Parse(content);
        var model = new { model = source };
        return TemplateEngine.Render(template, model);
    }
}
