using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scriban;

namespace Didot.Core.TemplateEngines;
internal class ScribanRenderer : IRenderer
{
    private readonly Template _compiledTemplate;
    private readonly Func<object, TemplateContext> _createContext;

    public ScribanRenderer(Template compiledTemplate, Func<object, TemplateContext> createContext)
        => (_compiledTemplate, _createContext) = (compiledTemplate, createContext);

    public string Render(object model)
    {
        var context = _createContext(model);
        return _compiledTemplate.Render(context);
    }
}
