using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers;
using HandlebarsDotNet.Helpers.Enums;

namespace Didot.Core.TemplateEngines;
public class HandlebarsWrapper : ITemplateEngine
{
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
        return handlebarsContext;
    }
}
