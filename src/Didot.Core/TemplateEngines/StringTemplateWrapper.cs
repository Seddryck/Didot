using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.StringTemplate;

namespace Didot.Core.TemplateEngines;
public class StringTemplateWrapper : ITemplateEngine
{
    public string Render(string template, object model)
    {
        var templateInstance = new Template(template);
        var dict = model.GetType().GetProperty("model")!.GetValue(model);
        templateInstance.Add("model", dict);
        return templateInstance.Render();
    }

    public string Render(Stream stream, object model)
    {
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        return Render(template, model);
    }
}
