using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core.SourceParsers;
public class FileBasedSourceParserFactory : BaseFactory<ISourceParser>
{
    private IDictionary<string, string> _parameters = new Dictionary<string, string>();
    protected override string ClassToken => "Source";
    private IDictionary<string, ISourceBuilder> Builders { get; } = ListSourceBuilders();

    public FileBasedSourceParserFactory(IDictionary<string, string> parameters)
        : base(parameters) { }

    protected override void Initialize(IDictionary<string, string> parameters)
    {
        _parameters = parameters;
        items.Clear();
        items.Add(".json", Builders[".json"].Build(parameters, ".json"));
        items.Add(".yaml", Builders[".yaml"].Build(parameters, ".yaml"));
        items.Add(".yml", Builders[".yml"].Build(parameters, "yml"));
        items.Add(".csv", Builders[".csv"].Build(parameters, "csv"));
        items.Add(".xml", Builders[".xml"].Build(parameters, "xml"));
        items.Add(".url", Builders[".url"].Build(parameters, "url"));
        items.Add(".md", Builders[".md"].Build(parameters, ".md"));
    }

    public override void AddOrReplace(IEnumerable<KeyValuePair<string, string>> keyValues)
        => keyValues.ToList().ForEach(kv => AddOrReplace(kv.Key, kv.Value));

    public virtual void AddOrReplace(string newExtension, string oldExtention)
    {
        var engine = Builders.TryGetValue(oldExtention.NormalizeExtension(), out var builder)
                        ? builder.Build(_parameters, newExtension)
                        : GetByTag(oldExtention);

        if (!items.TryAdd(newExtension.NormalizeExtension(), engine))
            items[newExtension.NormalizeExtension()] = engine;
    }

    private static IDictionary<string, ISourceBuilder> ListSourceBuilders()
    {
        var dict = new Dictionary<string, ISourceBuilder>();

        var asm = typeof(FileBasedSourceParserFactory).Assembly;
        var types = asm.GetTypes()
                .Where(t => t.Namespace == typeof(FileBasedSourceParserFactory).Namespace
                        && typeof(ISourceBuilder).IsAssignableFrom(t)
                        && !t.IsInterface
                        && !t.IsAbstract);

        foreach (var type in types)
        {
            var builder = (ISourceBuilder)Activator.CreateInstance(type)!;

            var extensions = type
                                .GetCustomAttribute<ExtensionAttribute>()
                                ?.Extensions.Select(e => e.NormalizeExtension())
                                ?? [];

            foreach (var extension in extensions)
                dict.Add(extension, builder);
        }

        return dict;
    }
}
