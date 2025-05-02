using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core.TemplateEngines;
public abstract class BaseTemplateEngine : ITemplateEngine
{
    protected TemplateConfiguration Configuration { get; }

    public BaseTemplateEngine()
        : this(new TemplateConfiguration())
    { }

    public BaseTemplateEngine(TemplateConfiguration configuration)
        => Configuration = configuration;

    protected Dictionary<string, IDictionary<string, object>> Mappings { get; } = [];
    protected Dictionary<string, Func<object?, string>> Formatters { get; } = [];
    protected Dictionary<string, Func<string>> NamedTemplates { get; } = [];
    protected Dictionary<string, Func<string>> Includes { get; } = [];

    public virtual void AddMappings(string mapKey, IDictionary<string, object> mappings)
    {
        if (!Mappings.TryAdd(mapKey, mappings))
            Mappings[mapKey] = mappings;
    }

    public virtual void AddFormatter(string name, Func<object?, string> function)
    {
        if (!Formatters.TryAdd(name, function))
            Formatters[name] = function;
    }

    public virtual void AddFunction(string name, Func<string> template)
    {
        if (!NamedTemplates.TryAdd(name, template))
            NamedTemplates[name] = template;
    }

    public virtual void AddPartial(string name, Func<string> template)
    {
        if (!Includes.TryAdd(name, template))
            Includes[name] = template;
    }

    public abstract string Render(string template, object model);
    public abstract string Render(Stream stream, object model);
}
