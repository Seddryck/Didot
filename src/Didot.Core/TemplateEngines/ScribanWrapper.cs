using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scriban;

namespace Didot.Core.TemplateEngines;
public class ScribanWrapper : ITemplateEngine
{
    private Dictionary<string, IDictionary<string, object>> Mappers { get; } = [];

    public void AddMappings(string mapKey, IDictionary<string, object> mappings)
        => throw new NotImplementedException();

    public string Render(string template, object model)
    {
        var templateInstance = Template.Parse(template);
        return templateInstance.Render(model);
    }

    public string Render(Stream stream, object model)
    {
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        var templateInstance = Template.Parse(template);
        return templateInstance.Render(model);
    }
}
