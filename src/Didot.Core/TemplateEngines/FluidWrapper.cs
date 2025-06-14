﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fluid;
using Fluid.Values;

namespace Didot.Core.TemplateEngines;
public class FluidWrapper : BaseTemplateEngine
{
    private static readonly FluidParser Parser = new();

    public FluidWrapper()
        : base()
    { }

    public FluidWrapper(TemplateConfiguration configuration)
        : base(configuration)
    { }

    public override void AddFunction(string name, Func<string> template)
        => throw new NotImplementedException();

    public override void AddPartial(string name, Func<string> template)
        => throw new NotImplementedException();

    public override string Render(string source, object model)
    {
        var template = Parser.Parse(source);
        var context = CreateContext(model);

        return Configuration.HtmlEncode
            ? template.Render(context, HtmlEncoder.Default)
            : template.Render(context);
    }

    public override string Render(Stream stream, object model)
    {
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        return Render(template, model);
    }

    public override IRenderer Prepare(string template)
    {
        return new FluidRenderer(template, (model) => CreateContext(model), Configuration.HtmlEncode ? HtmlEncoder.Default : null);
    }

    protected virtual TemplateContext CreateContext(object model)
    {
        if (Configuration.WrapAsModel)
        {
            var isAlreadyWrapped = model.GetType().GetProperty("model") != null;
            if (!isAlreadyWrapped)
                model = new { model };
        }

        var context = new TemplateContext(model);

        // Register Mappings as Fluid filters
        foreach (var (dictName, dictValues) in Mappings)
        {
            context.Options.Filters.AddFilter(dictName, (input, arguments, ctx) =>
            {
                if (input is StringValue keyStr && dictValues.TryGetValue(keyStr.ToStringValue(), out var value))
                {
                    return new StringValue(value?.ToString() ?? string.Empty);
                }
                return NilValue.Instance;
            });
        }

        foreach (var (funcName, function) in Formatters)
        {
            context.Options.Filters.AddFilter(funcName, (input, arguments, ctx) =>
            {
                return new StringValue(function(input.ToObjectValue()));
            });
        }
        return context;
    }
}

