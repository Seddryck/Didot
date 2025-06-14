﻿using System;
using System.Collections.Generic;
using System.Dynamic;
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

    public override string Render(string template, object model)
    {
        var context = CreateContext(model);

        if (Functions.Count > 0)
        {
            var include = string.Empty;
            foreach (var name in Functions.Keys)
                include += $"{{{{ include '{name}' ~}}}}\r\n";

            template = include + template;
        }

        var parsed = Template.Parse(template);

        if (parsed.HasErrors)
            throw new InvalidOperationException($"Scriban template parse error: {string.Join(", ", parsed.Messages)}");

        return parsed.Render(context);
    }

    public override string Render(Stream stream, object model)
    {
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        return Render(template, model);
    }

    public override IRenderer Prepare(string template)
    {
        return new ScribanRenderer(template, CreateContext);
    }

    protected virtual TemplateContext CreateContext(object model)
    {
        var modelScriptObject = new ScriptObject();
        if (!Configuration.WrapAsModel && model is IDictionary<string, object?>)
        {
            foreach (var item in (IDictionary<string, object?>)model)
                modelScriptObject[item.Key] = item.Value;
        }
        else
        {
            var extractedModel = model.GetType().GetProperty("model") is null ? new { model } : model;
            modelScriptObject.Import(extractedModel);
        }
        var scriptObject = new ScriptObject();
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
        var merged = Functions.Concat(Partials).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        var templateLoader = new InlineIncludeTemplateLoader(merged);
        context.TemplateLoader = templateLoader;
        context.PushGlobal(scriptObject);
        return context;
    }

    private class HtmlEncodeTemplateContext : TemplateContext
    {
        public override TemplateContext Write(SourceSpan span, object textAsObject)
            => base.Write(span, textAsObject is string text ? WebUtility.HtmlEncode(text) : textAsObject);
    }

    public class InlineIncludeTemplateLoader : ITemplateLoader
    {
        private readonly IDictionary<string, Func<string>> _namedTemplates;

        public InlineIncludeTemplateLoader(IDictionary<string, Func<string>> namedTemplates)
            => _namedTemplates = namedTemplates;

        public ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath)
            => ValueTask.FromResult(Load(context, callerSpan, templatePath));

        public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
            => templateName;

        public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
        {
            if (!_namedTemplates.TryGetValue(templatePath, out var tplFactory))
                throw new FileNotFoundException($"Named template '{templatePath}' not registered.");

            return tplFactory();
        }
    }

    private static bool TryParseTemplate(string value, out string? name, out string[]? arguments, out string? template)
    {
        var start = value.IndexOf("func");
        var end = value.IndexOf(')');
        if (start < 0 || end < 0)
        {
            (name, arguments, template) = (null, null, value);
            return false;
        }

        var tokens = value[start..end].Split([' ', '('], StringSplitOptions.RemoveEmptyEntries);
        (name, arguments, template) = (tokens[1].Trim()
            , tokens[2].Trim()[..^1].Split(',').Select(x => x.Trim()).ToArray()
            , value[(end + 3)..]);
        return true;
    }
}
