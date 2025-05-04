using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.StringTemplate;
using Antlr4.StringTemplate.Compiler;

namespace Didot.Core.TemplateEngines;
internal class StringTemplateRenderer : IRenderer
{
    private readonly string _template;
    private CompiledTemplate? _compiledTemplate;

    private readonly Func<TemplateGroup> _createContext;
    private TemplateGroup? _context;

    private readonly Func<Template, object, Template> _setModel;

    public StringTemplateRenderer(string template, Func<TemplateGroup> createContext, Func<Template, object, Template> setModel)
        => (_template, _createContext, _setModel) = (template, createContext, setModel);

    public string Render(object model)
    {
        _context ??= _createContext();
        _compiledTemplate ??= new Template(_context, _template).impl;

        var instance = _context.CreateStringTemplate(_compiledTemplate);
        instance = _setModel(instance, model);
        return instance.Render();
    }
}
