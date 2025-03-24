using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fluid;
using Fluid.Values;
using SmartFormat.Core.Extensions;

namespace Didot.Core.TemplateEngines;
public class FluidWrapper : ITemplateEngine
{
    private static readonly FluidParser Parser = new();

    private Dictionary<string, IDictionary<string, object>> Mappers { get; } = [];
    private Dictionary<string, Func<object?, string>> Formatters { get; } = [];

    public void AddFormatter(string name, Func<object?, string> function)
    {
        if (!Formatters.TryAdd(name, function))
            Formatters[name] = function;
    }

    public void AddMappings(string mapKey, IDictionary<string, object> mappings)
    {
        if (!Mappers.TryAdd(mapKey, mappings))
            Mappers[mapKey] = mappings;
    }


    public string Render(string source, object model)
    {
        var template = Parser.Parse(source);
        var context = new TemplateContext(model);

        // Register Mappers as Fluid filters
        foreach (var (dictName, dictValues) in Mappers)
        {
            context.Options.Filters.AddFilter(dictName, (input, arguments, ctx) =>
            {
                if (input is StringValue keyStr && dictValues.TryGetValue(keyStr.ToStringValue(), out var value))
                {
                    return new StringValue(value?.ToString() ?? string.Empty);
                }
                return NilValue.Instance;
            });
        }

        foreach (var (funcName, function) in Formatters)
        {
            context.Options.Filters.AddFilter(funcName, (input, arguments, ctx) =>
            {
                return new StringValue(function(input.ToObjectValue()));
            });
        }

        return template.Render(context);
    }

    public string Render(Stream stream, object model)
    {
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        return Render(template, model);
    }
}

