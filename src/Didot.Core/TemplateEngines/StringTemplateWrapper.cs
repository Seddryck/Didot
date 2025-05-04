using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Antlr4.StringTemplate;
using Antlr4.StringTemplate.Compiler;
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
        var context = CreateContext();
        var compiledTemplate = new Template(context, template).impl;
        var instance = context.CreateStringTemplate(compiledTemplate);
        instance = SetModel(instance, model);
        return instance.Render();
    }

    public override string Render(Stream stream, object model)
    {
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        return Render(template, model);
    }

    public override IRenderer Prepare(string template)
    {
        return new StringTemplateRenderer(template, CreateContext, SetModel);
    }

    protected virtual TemplateGroup CreateContext()
    {
        var group = new TemplateGroup(Options.Delimiters.Left, Options.Delimiters.Right);

        foreach (var (key, value) in Mappings)
            group.DefineDictionary(key, value);

        if (Formatters.Count > 0)
        {
            var renderer = (object? value, string name) =>
            {
                if (string.IsNullOrEmpty(name) || !Formatters.TryGetValue(name, out var function))
                    return value?.ToString() ?? string.Empty;
                return function(value);
            };
            group.RegisterRenderer(typeof(object), new RendererWrapper(renderer));
        }

        foreach (var namedTemplate in Functions)
        {
            var content = namedTemplate.Value.Invoke();
            if (TryParseTemplate(content, out var name, out var arguments, out var text))
            {
                group.DefineTemplate(namedTemplate.Key, text, arguments);
                if (name != namedTemplate.Key)
                    group.DefineTemplate(name, text, arguments);
            }
            else
                group.DefineTemplate(namedTemplate.Key, content);
        }

        foreach (var include in Partials)
            group.DefineTemplate(include.Key, include.Value.Invoke());

        return group;
    }

    protected virtual Template SetModel(Template template, object model)
    {
        if (!Configuration.WrapAsModel && model is IDictionary<string, object?> dict)
        {
            foreach (var (key, value) in dict)
                template.Add(key, value);
        }
        else
        {
            var extractedModel = model.GetType().GetProperty("model")?.GetValue(model) ?? model;
            template.Add("model", extractedModel);
        }
        return template;
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
