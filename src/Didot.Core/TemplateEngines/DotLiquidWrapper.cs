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
    private Dictionary<string, IDictionary<string, object>> Mappers { get; } = [];
    private Dictionary<string, Func<object?, string>> Formatters { get; } = [];

    public void AddFormatter(string name, Func<object?, string> function)
        => throw new NotImplementedException();

    public void AddMappings(string mapKey, IDictionary<string, object> mappings)
    {
        if (!Mappers.TryAdd(mapKey, mappings))
            Mappers[mapKey] = mappings;
    }

    public string Render(string source, object model)
    {
        var templateInstance = Template.Parse(source);
        var hash = Hash.FromAnonymousObject(model);

        foreach (var (dictName, dictValues) in Mappers)
            hash[dictName] = dictValues;

        return templateInstance.Render(hash);
    }

    public string Render(Stream stream, object model)
    {
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        return Render(template, model);
    }
}
