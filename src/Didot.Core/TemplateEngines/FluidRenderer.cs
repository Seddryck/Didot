using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fluid;
using Fluid.Values;

namespace Didot.Core.TemplateEngines;
internal class FluidRenderer : IRenderer
{
    private readonly string _template;
    private IFluidTemplate? _compiledTemplate;
    private readonly Func<object, TemplateContext> _createContext;
    private readonly TextEncoder? _encoder;

    private readonly object lockObj = new();

    public FluidRenderer(string template, Func<object, TemplateContext> createContext, TextEncoder? encoder = null)
        => (_template, _createContext, _encoder) = (template, createContext, encoder);

    public string Render(object model)
    {
        if (_compiledTemplate is null)
        {
            lock (lockObj)
            {
                if (_compiledTemplate is null)
                {
                    var parser = new FluidParser();
                    if (!parser.TryParse(_template, out var template, out var errors))
                        throw new InvalidOperationException($"Failed to parse Fluid template: {string.Join(", ", errors)}");
                    _compiledTemplate = template;
                }
            }
        }

        var context = _createContext(model);
        return _encoder is null
            ? _compiledTemplate.Render(context)
            : _compiledTemplate.Render(context, _encoder);
    }
}
