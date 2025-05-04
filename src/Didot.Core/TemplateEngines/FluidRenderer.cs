using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fluid;

namespace Didot.Core.TemplateEngines;
internal class FluidRenderer : IRenderer
{
    private readonly IFluidTemplate _template;
    private readonly Func<object, TemplateContext> _createContext;
    private readonly TextEncoder? _encoder;

    public FluidRenderer(IFluidTemplate template, Func<object, TemplateContext> createContext, TextEncoder? encoder = null)
        => (_template, _createContext, _encoder) = (template, createContext, encoder);

    public string Render(object model)
    {
        var context = _createContext(model);
        return _encoder is null
            ? _template.Render(context)
            : _template.Render(context, _encoder);
    }
}
