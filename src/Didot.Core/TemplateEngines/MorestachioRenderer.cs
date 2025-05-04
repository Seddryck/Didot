using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;
using Morestachio;
using Morestachio.Rendering;

namespace Didot.Core.TemplateEngines;
internal class MorestachioRenderer : IRenderer
{
    private readonly string _template;
    private Morestachio.Rendering.IRenderer? _renderer;

    private readonly Func<string, IParserOptionsBuilder> _createContext;

    public MorestachioRenderer(string template, Func<string, IParserOptionsBuilder> createContext)
        => (_template, _createContext) = (template, createContext);

    public string Render(object model)
    {
        _renderer ??= _createContext(_template).BuildAndParse().CreateRenderer();
        return _renderer.RenderAndStringify(model);
    }
}
