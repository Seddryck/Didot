using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.TemplateEngines;
using HandlebarsDotNet;

namespace Didot.Core;

/// <summary>
/// Builder class for creating template engine instances with a fluent API.
/// </summary>
public class TemplateEngineBuilder : ITemplateEngineConfigurabable
{
    private Type? _templateEngineType;
    private ITemplateEngineOptionsBuilder? _optionsBuilder;
    private TemplateConfigurationBuilder? _configBuilder;

    public ITemplateEngineConfigurabable UseDotLiquid()
    {
        _templateEngineType = typeof(DotLiquidWrapper);
        return this;
    }

    public ITemplateEngineConfigurabable UseFluid()
    {
        _templateEngineType = typeof(FluidWrapper);
        return this;
    }

    public ITemplateEngineConfigurabable UseHandlebars()
    {
        _templateEngineType = typeof(HandlebarsWrapper);
        return this;
    }

    public ITemplateEngineConfigurabable UseMorestachio()
    {
        _templateEngineType = typeof(MorestachioWrapper);
        return this;
    }

    public ITemplateEngineConfigurabable UseScriban()
    {
        _templateEngineType = typeof(ScribanWrapper);
        return this;
    }

    public ITemplateEngineConfigurabable UseSmartFormat()
    {
        _templateEngineType = typeof(SmartFormatWrapper);
        return this;
    }

    public ITemplateEngineConfigurabable UseStringTemplate()
    {
        _templateEngineType = typeof(StringTemplateWrapper);
        return this;
    }

    public ITemplateEngineConfigurabable UseStringTemplate(Func<StringTemplateOptionsBuilder, StringTemplateOptionsBuilder> builder)
    {
        _templateEngineType = typeof(StringTemplateWrapper);
        _optionsBuilder = builder(new());
        return this;
    }

    ITemplateEngineBuildable ITemplateEngineConfigurabable.WithConfiguration(Func<TemplateConfigurationBuilder, TemplateConfigurationBuilder> builder)
    {
        _configBuilder = builder(new());
        return this;
    }

    ITemplateEngine ITemplateEngineBuildable.Build()
    {
        if (_templateEngineType is null)
            throw new InvalidOperationException("Template engine type is not set. Call one of the Use* methods first (e.g., UseHandlebars(), UseStringTemplate(), etc.).");

        var parameters = new List<object>();
        if (_optionsBuilder is not null)
            parameters.Add(_optionsBuilder.Build());

        if (_configBuilder is not null)
            parameters.Add(_configBuilder.Build());

        if (parameters.Count == 0)
            try
            {
                return (ITemplateEngine)Activator.CreateInstance(_templateEngineType)!;
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Failed to create instance of {_templateEngineType.Name}.", ex);
            }
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

public interface ITemplateEngineConfigurabable : ITemplateEngineBuildable
{
    ITemplateEngineBuildable WithConfiguration(Func<TemplateConfigurationBuilder, TemplateConfigurationBuilder> builder);
}
