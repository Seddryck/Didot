using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Morestachio;
using Morestachio.Rendering;

namespace Didot.Core.TemplateEngines;
public class MorestachioWrapper : ITemplateEngine
{
    private Dictionary<string, IDictionary<string, object>> Mappers { get; } = [];

    public void AddMappings(string mapKey, IDictionary<string, object> mappings)
        => throw new NotImplementedException();

    public string Render(string template, object model)
    {
        if (string.IsNullOrWhiteSpace(template))
            throw new ArgumentException("Template content cannot be null or whitespace.", nameof(template));

        var document = ParserOptionsBuilder
               .New()
               .WithTemplate(template)
               .WithEncoding(Encoding.Default)
               .BuildAndParse();
        var renderer = document.CreateRenderer();

        return renderer.RenderAndStringify(model);
    }

    public string Render(Stream stream, object model)
    {
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        return Render(template, model);
    }
}
