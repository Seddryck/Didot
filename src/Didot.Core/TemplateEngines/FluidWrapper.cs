using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fluid;
using SmartFormat.Core.Extensions;

namespace Didot.Core.TemplateEngines;
public class FluidWrapper : ITemplateEngine
{
    private static readonly FluidParser Parser = new();

    public string Render(string source, object model)
    {
        var template = Parser.Parse(source);
        var context = new TemplateContext(model);
        return template.Render(context);
    }

    public string Render(Stream stream, object model)
    {
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        return Render(template, model);
    }
}

