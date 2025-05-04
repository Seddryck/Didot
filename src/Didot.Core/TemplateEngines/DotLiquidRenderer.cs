using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DotLiquid;

namespace Didot.Core.TemplateEngines;
internal class DotLiquidRenderer : IRenderer
{
    private readonly Template _template;
    private readonly Func<object, Hash> _createContext;

    public DotLiquidRenderer(Template template, Func<object, Hash> createContext)
        => (_template, _createContext) = (template, createContext);

    public string Render(object model)
    {
        var context = _createContext(model);
        return _template.Render(context);
    }
}
