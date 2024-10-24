using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core.SourceParsers;
public abstract class BaseFactory<T>
{
    protected Dictionary<string, T> items = new();

    protected abstract string ClassToken {get;}

    public BaseFactory()
    {
        Initialize();
    }

    protected abstract void Initialize();

    public virtual void AddOrReplace(string extension, T item)
    {
        extension = NormalizeExtension(extension);

        if (items.ContainsKey(extension))
            items[extension] = item;
        else
            items.Add(extension, item);
    }

    protected virtual string NormalizeExtension(string extension)
    {
        extension = extension.Trim().ToLowerInvariant();
        if (!extension.StartsWith('.'))
            extension = $".{extension}";
        return extension;
    }

    public T GetByExtension(string extension)
    {
        extension = NormalizeExtension(extension);
        if (items.TryGetValue(extension, out var engine))
            return engine;
        throw new NotSupportedException(nameof(extension));
    }

    public Dictionary<string, Type> ListByTags()
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

    public T GetByTag(string tag)
    {
        tag = tag.Trim().ToLowerInvariant();
        if (!ListByTags().TryGetValue(tag, out var engineType))
            throw new Exception(tag);
        return (T)Activator.CreateInstance(engineType)!;
    }
}
