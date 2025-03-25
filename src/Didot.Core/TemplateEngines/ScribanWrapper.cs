using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scriban;
using Scriban.Runtime;

namespace Didot.Core.TemplateEngines;
public class ScribanWrapper : ITemplateEngine
{
    private Dictionary<string, IDictionary<string, object>> Mappers { get; } = [];
    private Dictionary<string, Func<object?, string>> Formatters { get; } = [];

    public void AddFormatter(string name, Func<object?, string> function)
    {
        if (!Formatters.TryAdd(name, function))
            Formatters[name] = function;
    }

    public void AddMappings(string mapKey, IDictionary<string, object> mappings)
        => throw new NotImplementedException();

    public string Render(string template, object model)
    {
        var scriptObject = new ScriptObject();

        var modelScriptObject = new ScriptObject();
        modelScriptObject.Import(model); 
        scriptObject.Import(modelScriptObject);
        
        foreach (var (funcName, function) in Formatters)
        {
            scriptObject.Import(funcName, (string value) => function(value));
        }

        var context = new TemplateContext();
        context.PushGlobal(scriptObject);

        var templateInstance = Template.Parse(template);
        return templateInstance.Render(context);
    }

    public string Render(Stream stream, object model)
    {
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        return Render(template, model);
    }
}
