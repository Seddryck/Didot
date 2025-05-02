using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Morestachio;
using Morestachio.Rendering;

namespace Didot.Core.TemplateEngines;
public class MorestachioWrapper : BaseTemplateEngine
{
    public MorestachioWrapper()
        : base()
    { }

    public MorestachioWrapper(TemplateConfiguration configuration)
        : base(configuration)
    { }
    public override void AddFunction(string name, Func<string> template)
        => throw new NotImplementedException();

    public override void AddPartial(string name, Func<string> template)
        => throw new NotImplementedException();

    public override string Render(string template, object model)
    {
        if (string.IsNullOrWhiteSpace(template))
            throw new ArgumentException("Template content cannot be null or whitespace.", nameof(template));

        var options = ParserOptionsBuilder
               .New()
               .WithTemplate(template)
               .WithDisableContentEscaping(!Configuration.HtmlEncode)
               .WithEncoding(Encoding.Default);

        static object? map(IDictionary<string, object> mappings, string key)
        {
            if (mappings.TryGetValue(key, out var result))
                return result;
            return null;
        }

        foreach (var mapping in Mappings)
            options.WithFormatter((string value) => map(mapping.Value, value), mapping.Key);

        foreach (var formatter in Formatters)
            options.WithFormatter(formatter.Value, formatter.Key);

        var document = options.BuildAndParse();
        var renderer = document.CreateRenderer();

        return renderer.RenderAndStringify(model);
    }

    public override string Render(Stream stream, object model)
    {
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        return Render(template, model);
    }
}
