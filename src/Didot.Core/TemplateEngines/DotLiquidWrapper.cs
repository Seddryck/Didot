using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotLiquid;

namespace Didot.Core.TemplateEngines;
public class DotLiquidWrapper : ITemplateEngine
{
    public string Render(string template, dynamic model)
    {
        var templateInstance = Template.Parse(template);
        var hash = Hash.FromAnonymousObject(model);
        return templateInstance.Render(hash);
    }

    public string Render(Stream stream, dynamic model)
    {
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        var templateInstance = Template.Parse(template);
        var hash = Hash.FromAnonymousObject(model);
        return templateInstance.Render(hash);
    }
}
