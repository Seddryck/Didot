using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;

namespace Didot.Core.TemplateEngines;
public class HandlebarsWrapper : ITemplateEngine
{
    private Dictionary<string, IDictionary<string, object>> Mappers { get; } = [];

    public void AddMappings(string mapKey, IDictionary<string, object> mappings)
    {
        if (!Mappers.TryAdd(mapKey, mappings))
            Mappers[mapKey] = mappings;
    }

    public string Render(string template, object model)
    {
        var handlebarsContext = CreateContext();
        var templateInstance = handlebarsContext.Compile(template);
        return templateInstance(model);
    }

    public string Render(Stream stream, object model)
    {
        var handlebarsContext = CreateContext();

        using var reader = new StreamReader(stream);
        var templateInstance = handlebarsContext.Compile(reader);

        using var writer = new StringWriter(); // StringWriter as TextWriter for output
        templateInstance(writer, model);
        return writer.ToString();
    }

    private IHandlebars CreateContext()
    {
        var handlebarsContext = Handlebars.Create();
        HandlebarsHelpers.Register(handlebarsContext);

        foreach (var (dictName, dictValues) in Mappers)
        {
            handlebarsContext.RegisterHelper(dictName, (output, context, args) =>
            {
                if (args.Length == 1 && args[0] is string key && dictValues.TryGetValue(key, out var value))
                {
                    output.Write(value.ToString());
                }
            });
        }

        return handlebarsContext;
    }
}
