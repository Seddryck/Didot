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
public class StringTemplateWrapper : BaseTemplateEngine
{
    public StringTemplateOptions Options { get; }

    public StringTemplateWrapper()
        : this(StringTemplateOptions.Default)
    { }

    public StringTemplateWrapper(StringTemplateOptions options)
        : base()
        => Options = options;

    public StringTemplateWrapper(TemplateConfiguration configuration)
        : this(StringTemplateOptions.Default, configuration)
    { }

    public StringTemplateWrapper(StringTemplateOptions options, TemplateConfiguration configuration)
        : base(configuration)
        => Options = options;

    public override string Render(string template, object model)
    {
        var templateInstance = new Template(template, Options.Delimiters.Left, Options.Delimiters.Right);
        var extractedModel = model.GetType().GetProperty("model")?.GetValue(model) ?? model;
        templateInstance.Add("model", extractedModel);
        foreach (var (key, value) in Mappings)
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

        foreach (var namedTemplate in Functions)
        {
            var content = namedTemplate.Value.Invoke();
            if (TryParseTemplate(content, out var name, out var arguments, out var text))
            {
                templateInstance.Group.DefineTemplate(namedTemplate.Key, text, arguments);
                if (name != namedTemplate.Key)
                    templateInstance.Group.DefineTemplate(name, text, arguments);
            }
            else
                templateInstance.Group.DefineTemplate(namedTemplate.Key, content);
        }

        foreach (var include in Partials)
            templateInstance.Group.DefineTemplate(include.Key, include.Value.Invoke());

        return templateInstance.Render();
    }

    public override string Render(Stream stream, object model)
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
    private static bool TryParseTemplate(string value, out string? name, out string[]? arguments, out string? template)
    {
        var end = value.IndexOf("::=");
        if (end < 0)
        {
            (name, arguments, template) = (null, null, value);
            return false;
        }

        var tokens = value[..end].Split('(');
        (name, arguments, template) = (tokens[0].Trim()
            , tokens[1].Trim()[..^1].Split(',').Select(x => x.Trim()).ToArray()
            , value[(end + 3)..]);
        return true;
    }
}
