using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.TemplateEngines;
using HandlebarsDotNet;

namespace Didot.Core;
public class TemplateEngineBuilder : ITemplateEngineBuildable
{
    private Type? _templateEngineType;
    private ITemplateEngineOptionsBuilder? _optionsBuilder;

    public ITemplateEngineBuildable UseDotLiquid()
    {
        _templateEngineType = typeof(DotLiquidWrapper);
        return this;
    }

    public ITemplateEngineBuildable UseFluid()
    {
        _templateEngineType = typeof(FluidWrapper);
        return this;
    }

    public ITemplateEngineBuildable UseHandlebars()
    {
        _templateEngineType = typeof(HandlebarsWrapper);
        return this;
    }

    public ITemplateEngineBuildable UseMorestachio()
    {
        _templateEngineType = typeof(MorestachioWrapper);
        return this;
    }

    public ITemplateEngineBuildable UseScriban()
    {
        _templateEngineType = typeof(ScribanWrapper);
        return this;
    }

    public ITemplateEngineBuildable UseSmartFormat()
    {
        _templateEngineType = typeof(SmartFormatWrapper);
        return this;
    }

    public ITemplateEngineBuildable UseStringTemplate()
    {
        _templateEngineType = typeof(StringTemplateWrapper);
        return this;
    }

    public ITemplateEngineBuildable UseStringTemplate(Func<StringTemplateOptionsBuilder, StringTemplateOptionsBuilder> builder)
    {
        _templateEngineType = typeof(StringTemplateWrapper);
        _optionsBuilder = builder(new());
        return this;
    }

    ITemplateEngine ITemplateEngineBuildable.Build()
    {
        if (_templateEngineType is null)
            throw new InvalidOperationException("Template engine type is not set.");

        var parameters = new List<object>();
        if (_optionsBuilder != null)
            parameters.Add(_optionsBuilder.Build());

        if (parameters.Count == 0)
            return (ITemplateEngine)Activator.CreateInstance(_templateEngineType)!;
        else
        {
            var constructor = _templateEngineType.GetConstructor([.. parameters.Select(p => p.GetType())]);
            return constructor is null
                ? throw new InvalidOperationException($"No suitable constructor found for type {_templateEngineType.Name}.")
                : (ITemplateEngine)constructor.Invoke([.. parameters]);
        }
    }
}

public interface ITemplateEngineBuildable
{
    ITemplateEngine Build();
}
