using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.TemplateEngines;

namespace Didot.Core.TemplateEngines;
public class FileBasedTemplateEngineFactory
{
    protected Dictionary<string, ITemplateEngine> engines = new();

    public FileBasedTemplateEngineFactory()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        engines.Clear();
        engines.Add(".scriban", new ScribanWrapper());
        engines.Add(".liquid", new DotLiquidWrapper());
        engines.Add(".hbs", new HandlebarsWrapper());
        engines.Add(".smart", new SmartFormatWrapper());
        engines.Add(".st", new StringTemplateWrapper());
        engines.Add(".stg", new StringTemplateWrapper());
    }

    public virtual void AddOrReplaceEngine(string extension, ITemplateEngine engine)
    {
        extension = NormalizeExtension(extension);

        if (engines.ContainsKey(extension))
            engines[extension] = engine;
        else
            engines.Add(extension, engine);
    }

    protected virtual string NormalizeExtension(string extension)
    {
        extension = extension.Trim().ToLowerInvariant();
        if (!extension.StartsWith('.'))
            extension = $".{extension}";
        return extension;
    }

    public ITemplateEngine GetTemplateEngineByExtension(string extension)
    {
        extension = NormalizeExtension(extension);
        if (engines.TryGetValue(extension, out var engine))
            return engine;
        throw new NotSupportedException(nameof(extension));
    }

    public Dictionary<string, Type> ListEngineByTags()
    {
        var asm = typeof(ITemplateEngine).Assembly;
        var types = asm.GetTypes()
                .Where(t => t.Namespace == GetType().Namespace
                        && typeof(ITemplateEngine).IsAssignableFrom(t)
                        && !t.IsInterface
                        && !t.IsAbstract);

        var dict = new Dictionary<string, Type>();
        foreach (var type in types)
        {
            var tag = type.Name.Replace("Wrapper", string.Empty).Trim().ToLowerInvariant();
            dict.Add(tag, type);
        }
        return dict;
    }

    public ITemplateEngine GetTemplateEngineByTag(string tag)
    {
        tag = tag.Trim().ToLowerInvariant();
        if (!ListEngineByTags().TryGetValue(tag, out var engineType))
            throw new Exception(tag);
        return (ITemplateEngine)Activator.CreateInstance(engineType)!;
    }
}
