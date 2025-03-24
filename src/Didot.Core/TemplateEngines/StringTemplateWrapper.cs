using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Antlr4.StringTemplate;
using Morestachio.Rendering;

namespace Didot.Core.TemplateEngines;
public class StringTemplateWrapper : ITemplateEngine
{
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

    public string Render(string template, object model)
    { 
        var templateInstance = new Template(template);
        var extractedModel = model.GetType().GetProperty("model")?.GetValue(model) ?? model;
        templateInstance.Add("model", extractedModel);
        foreach (var (key, value) in Mappers)
            templateInstance.Group.DefineDictionary(key, value);

        if (Formatters.Count>0)
        {
            var renderer = (object? value, string name) =>
            {
                if (string.IsNullOrEmpty(name) || !Formatters.TryGetValue(name, out var function))
                    return value?.ToString() ?? string.Empty;
                return function(value);
            };
            templateInstance.Group.RegisterRenderer(typeof(object), new RendererWrapper(renderer));
        }
        
        return templateInstance.Render();
    }

    public string Render(Stream stream, object model)
    {
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        return Render(template, model);
    }

    internal class RendererWrapper : StringRenderer
    {
        protected Func<object?, string, string> Renderer { get; }

        public RendererWrapper(Func<object?, string, string> renderer)
            => Renderer = renderer;

        public override string ToString(object? obj, string formatString, CultureInfo culture)
            => Renderer(obj, formatString);
    }
}
