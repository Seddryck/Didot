using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;

namespace Didot.Core.TemplateEngines;
internal class HandlebarsRenderer : IRenderer
{
    private readonly string _template;
    private readonly Func<IHandlebars> _createContext;
    private HandlebarsTemplate<object, object>? _compiledTemplate;

    public HandlebarsRenderer(string template, Func<IHandlebars> createContext)
        => (_template, _createContext) = (template, createContext);

    public string Render(object model)
    {
        _compiledTemplate ??= _createContext().Compile(_template);
        return _compiledTemplate(model);
    }
}
