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
    private Dictionary<string, IDictionary<string, object>> Mappers { get; } = [];

    public void AddMappings(string mapKey, IDictionary<string, object> mappings)
    {
        if (!Mappers.TryAdd(mapKey, mappings))
            Mappers[mapKey] = mappings;
    }

    public string Render(string template, object model)
    { 
        var templateInstance = new Template(template);
        var extractedModel = model.GetType().GetProperty("model")?.GetValue(model) ?? model;
        templateInstance.Add("model", extractedModel);
        foreach (var (key, value) in Mappers ?? new Dictionary<string, IDictionary<string, object>>())
            templateInstance.Group.DefineDictionary(key, value);
        return templateInstance.Render();
    }

    public string Render(Stream stream, object model)
    {
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        return Render(template, model);
    }
}
