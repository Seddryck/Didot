using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.SourceParsers;

namespace Didot.Core;
public abstract class BaseFactory<T>
{
    protected Dictionary<string, T> items = new();

    protected abstract string ClassToken {get;}

    protected BaseFactory()
        => Initialize(new Dictionary<string, string>());

    protected BaseFactory(IDictionary<string, string> parameters)
        => Initialize(parameters);

    protected abstract void Initialize(IDictionary<string, string> parameters);

    public virtual void AddOrReplace(string extension, T item)
    {
        extension = extension.NormalizeExtension();

        if (!items.TryAdd(extension, item))
            items[extension] = item;
    }

    public virtual void AddOrReplace(IEnumerable<KeyValuePair<string, T>> keyValues)
        => keyValues.ToList().ForEach(kv => AddOrReplace(kv.Key, kv.Value));

    public virtual void AddOrReplace(IEnumerable<KeyValuePair<string, string>> keyValues)
        => keyValues.ToList().ForEach(kv => AddOrReplace(kv.Key, GetByTag(kv.Value)));

    public T GetByExtension(string extension)
    {
        extension = extension.NormalizeExtension(); ;
        if (items.TryGetValue(extension, out var engine))
            return engine;
        throw new NotSupportedException(nameof(extension));
    }

    protected Dictionary<string, Type> ListByTags()
    {
        var asm = typeof(T).Assembly;
        var types = asm.GetTypes()
                .Where(t => t.Namespace == GetType().Namespace
                        && typeof(T).IsAssignableFrom(t)
                        && !t.IsInterface
                        && !t.IsAbstract);

        var dict = new Dictionary<string, Type>();
        foreach (var type in types)
        {
            var tag = type.Name.Replace(ClassToken, string.Empty).Trim().ToLowerInvariant();
            dict.Add(tag, type);
        }
        return dict;
    }

    public virtual T GetByTag(string tag)
    {
        tag = tag.Trim().ToLowerInvariant();
        if (!ListByTags().TryGetValue(tag, out var engineType))
            throw new Exception(tag);

        var engine = items.Values.FirstOrDefault(x => x is not null && x.GetType() == engineType)
            ?? (T)Activator.CreateInstance(engineType)!;
        return engine;
    }
}
