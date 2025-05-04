using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartFormat;

namespace Didot.Core.TemplateEngines;
internal class SmartFormatRenderer : IRenderer
{
    private readonly string _template;
    private readonly Func<object, object> _createContext;

    public SmartFormatRenderer(string template, Func<object, object> createContext)
        => (_template, _createContext) = (template, createContext);

    public string Render(object model)
    {
        var context = _createContext(model);
        return Smart.Format(_template, context);
    }
}
