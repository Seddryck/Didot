﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http.Metrics;
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

    protected virtual void Enrich(ref IHandlebars context, ref object model)
    {
        foreach (var include in Partials)
            context.RegisterTemplate(include.Key, include.Value.Invoke());

        if (Configuration.WrapAsModel)
        {
            var isAlreadyWrapped = model.GetType().GetProperty("model") != null;

            if (!isAlreadyWrapped)
                model = new { model };
        }
    }

    public override string Render(string template, object model)
    {
        var handlebarsContext = CreateContext();
        Enrich(ref handlebarsContext, ref model);
        
        var templateInstance = handlebarsContext.Compile(template);
        return templateInstance(model);
    }

    public override string Render(Stream stream, object model)
    {
        var handlebarsContext = CreateContext();
        Enrich(ref handlebarsContext, ref model);

        using var reader = new StreamReader(stream);
        var templateInstance = handlebarsContext.Compile(reader);

        using var writer = new StringWriter(); // StringWriter as TextWriter for output
        templateInstance(writer, model);
        return writer.ToString();
    }

    public override IRenderer Prepare(string template)
    {
        return new HandlebarsRenderer(template, CreateContext);
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
