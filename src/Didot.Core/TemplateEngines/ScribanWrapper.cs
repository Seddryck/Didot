using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core.TemplateEngines;
public class ScribanWrapper : ITemplateEngine
{
    public string Render(string template, dynamic model)
    {
        var templateInstance = Scriban.Template.Parse(template);
        return templateInstance.Render(model);
    }

    public string Render(StreamReader templateReader, dynamic model)
    {
        var template = templateReader.ReadToEnd();
        var templateInstance = Scriban.Template.Parse(template);
        return templateInstance.Render(model);
    }
}
