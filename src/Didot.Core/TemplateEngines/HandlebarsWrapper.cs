using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;

namespace Didot.Core.TemplateEngines;
public class HandlebarsWrapper : ITemplateEngine
{
    public string Render(string template, object model)
    {
        var templateInstance = Handlebars.Compile(template);
        return templateInstance(model);
    }

    public string Render(Stream stream, object model)
    {
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        return Render(template, model);
    }
}
