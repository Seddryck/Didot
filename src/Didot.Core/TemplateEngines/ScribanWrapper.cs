using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace Didot.Core.TemplateEngines;
public class ScribanWrapper : BaseTemplateEngine
{
    public ScribanWrapper()
        : base()
    { }

    public ScribanWrapper(TemplateConfiguration configuration)
        : base(configuration)
    { }

    //public override void AddMappings(string mapKey, IDictionary<string, object> mappings)
    //        => throw new NotImplementedException();

    public override string Render(string template, object model)
    {
        var scriptObject = new ScriptObject();

        var modelScriptObject = new ScriptObject();
        modelScriptObject.Import(model);
        scriptObject.Import(modelScriptObject);

        foreach (var (funcName, function) in Formatters)
            scriptObject.Import(funcName, (string value) => function(value));

        static object? map(IDictionary<string, object> mappings, string key)
        {
            if (mappings.TryGetValue(key, out var result))
                return result;
            return null;
        }

        foreach (var (funcName, dict) in Mappings)
            scriptObject.Import(funcName, (string value) => map(dict, value));

        var context = Configuration.HtmlEncode ? new HtmlEncodeTemplateContext() : new TemplateContext();
        context.PushGlobal(scriptObject);

        var templateInstance = Template.Parse(template);
        return templateInstance.Render(context);
    }

    public override string Render(Stream stream, object model)
    {
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        return Render(template, model);
    }

    private class HtmlEncodeTemplateContext : TemplateContext
    {
        public override TemplateContext Write(SourceSpan span, object textAsObject)
            => base.Write(span, textAsObject is string text ? WebUtility.HtmlEncode(text) : textAsObject);
    }
}
