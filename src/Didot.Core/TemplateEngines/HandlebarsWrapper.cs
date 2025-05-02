using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;

namespace Didot.Core.TemplateEngines;
public class HandlebarsWrapper : BaseTemplateEngine
{
    public HandlebarsWrapper()
        : base()
    { }

    public HandlebarsWrapper(TemplateConfiguration configuration)
        : base(configuration)
    { }
    public override void AddFunction(string name, Func<string> template)
        => throw new NotImplementedException();

    public override string Render(string template, object model)
    {
        var handlebarsContext = CreateContext();
        var templateInstance = handlebarsContext.Compile(template);
        return templateInstance(model);
    }

    public override string Render(Stream stream, object model)
    {
        var handlebarsContext = CreateContext();

        using var reader = new StreamReader(stream);
        var templateInstance = handlebarsContext.Compile(reader);

        foreach (var include in Partials)
            handlebarsContext.RegisterTemplate(include.Key, include.Value.Invoke());

        using var writer = new StringWriter(); // StringWriter as TextWriter for output
        templateInstance(writer, model);
        return writer.ToString();
    }

    private IHandlebars CreateContext()
    {
        var config = new HandlebarsConfiguration { NoEscape = !Configuration.HtmlEncode };
        var handlebarsContext = Handlebars.Create(config);
        HandlebarsHelpers.Register(handlebarsContext);

        foreach (var (dictName, dictValues) in Mappings)
        {
            handlebarsContext.RegisterHelper(dictName, (output, context, args) =>
            {
                if (args.Length == 1 && args[0] is string key && dictValues.TryGetValue(key, out var value))
                {
                    output.Write(value.ToString());
                }
            });
        }

        foreach (var (funcName, function) in Formatters)
        {
            handlebarsContext.RegisterHelper(funcName, (output, context, args) =>
            {
                if (args.Length == 1)
                {
                    output.Write(function(args[0]));
                }
            });
        }

        return handlebarsContext;
    }
}
