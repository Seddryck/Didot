using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.TemplateEngines;

namespace Didot.Core;
public class TemplateEngineFactory
{
    private Dictionary<string, ITemplateEngineConfigurabable> _engines = [];
    private Func<TemplateConfigurationBuilder, TemplateConfigurationBuilder>? _configure;

    public TemplateEngineFactory()
    { }

    private static TemplateEngineFactory BuildDefault()
    {
        var factory = new TemplateEngineFactory();
        factory.AddOrReplace(".liquid", new TemplateEngineBuilder().UseDotLiquid());
        factory.AddOrReplace(".hbs", new TemplateEngineBuilder().UseHandlebars());
        factory.AddOrReplace(".stg", new TemplateEngineBuilder().UseStringTemplate());
        factory.AddOrReplace(".st", new TemplateEngineBuilder().UseStringTemplate());
        factory.AddOrReplace(".morestachio", new TemplateEngineBuilder().UseMorestachio());
        factory.AddOrReplace(".mustache", new TemplateEngineBuilder().UseMorestachio());
        factory.AddOrReplace(".smart", new TemplateEngineBuilder().UseSmartFormat());
        factory.AddOrReplace(".scriban", new TemplateEngineBuilder().UseScriban());
        return factory;
    }
    private static TemplateEngineFactory _default = BuildDefault();
    public static TemplateEngineFactory Default { get; } = _default;

    public void Clear()
        => _engines.Clear();

    public void Remove(string extension)
    {
        if (string.IsNullOrEmpty(extension))
            throw new ArgumentNullException(nameof(extension));
        if (!extension.StartsWith('.'))
            extension = '.' + extension;
        _engines.Remove(extension);
    }

    public bool Exists(string extension)
    {
        if (string.IsNullOrEmpty(extension))
            throw new ArgumentNullException(nameof(extension));
        if (!extension.StartsWith('.'))
            extension = '.' + extension;
        return _engines.ContainsKey(extension);
    }

    public int Count => _engines.Count;

    public void AddOrReplace(string extension, ITemplateEngineConfigurabable engine)
    {
        if (string.IsNullOrEmpty(extension))
            throw new ArgumentNullException(nameof(extension));

        if (!extension.StartsWith('.'))
            extension = '.' + extension;

        if (!_engines.TryAdd(extension, engine))
            _engines[extension] = engine;
    }

    public void AddOrReplace(string extension, Func<TemplateEngineBuilder, ITemplateEngineConfigurabable> engine)
        => AddOrReplace(extension, engine(new ()));

    public string[] AllSupportedExtensions()
        => [.. _engines.Keys];

    public ITemplateEngine Create(string extension)
    {
        if (string.IsNullOrEmpty(extension))
            throw new ArgumentNullException(nameof(extension));

        if (!extension.StartsWith('.'))
            extension = '.' + extension;

        if (_engines.TryGetValue(extension, out var builder))
        {
            var configured = _configure is not null ? builder.WithConfiguration(_configure) : builder;
            return configured.Build();
        }
        throw new NotSupportedException($"Template engine for extension '{extension}' is not supported.");
    }

    public void Configure(Func<TemplateConfigurationBuilder, TemplateConfigurationBuilder> configure)
        => _configure = configure;
}
