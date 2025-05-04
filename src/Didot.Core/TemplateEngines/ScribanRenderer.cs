using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scriban;

namespace Didot.Core.TemplateEngines;
internal class ScribanRenderer : IRenderer
{
    private readonly string _template;
    private Template? _compiledTemplate;
    private readonly Func<object, TemplateContext> _createContext;

    public ScribanRenderer(string template, Func<object, TemplateContext> createContext)
        => (_template, _createContext) = (template, createContext);

    public string Render(object model)
    {
        _compiledTemplate ??= Template.Parse(_template);
        if (_compiledTemplate.HasErrors)
            throw new InvalidOperationException($"Scriban template parse error: {string.Join(", ", _compiledTemplate.Messages)}");

        var context = _createContext(model);
        return _compiledTemplate.Render(context);
    }
}
