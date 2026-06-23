using System.Reflection;
using System.Runtime.Loader;
using Didot.Core;

namespace Didot.Cli;

public class ExtensionAssemblyLoader
{
    public virtual LoadedExtension Load(string extensionSource, string? extensionId = null)
    {
        if (string.IsNullOrWhiteSpace(extensionSource))
            throw new ExtensionSourceNotFoundException(sourcePath: extensionSource);

        var fullPath = Path.GetFullPath(extensionSource.Trim());
        if (!File.Exists(fullPath))
            throw new ExtensionSourceNotFoundException(sourcePath: fullPath);

        var assembly = LoadAssembly(fullPath);
        var extensionType = FindExtensionType(assembly, fullPath, extensionId);
        var instance = Instantiate(extensionType, fullPath);

        return new LoadedExtension(fullPath, extensionType.FullName ?? extensionType.Name, instance);
    }

    protected virtual Assembly LoadAssembly(string assemblyPath)
    {
        try
        {
            return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
        }
        catch (FileNotFoundException ex)
        {
            throw new AssemblyLoadFailedException(assemblyPath, ex);
        }
        catch (FileLoadException ex)
        {
            throw new AssemblyLoadFailedException(assemblyPath, ex);
        }
        catch (BadImageFormatException ex)
        {
            throw new AssemblyLoadFailedException(assemblyPath, ex);
        }
        catch (Exception ex)
        {
            throw new AssemblyLoadFailedException(assemblyPath, ex);
        }
    }

    protected virtual Type FindExtensionType(Assembly assembly, string assemblyPath, string? extensionId)
    {
        var candidates = GetLoadableTypes(assembly)
            .Where(x => x is { IsClass: true, IsAbstract: false })
            .Select(x => new
            {
                Type = x,
                Attribute = x.GetCustomAttribute<DidotExtensionAttribute>(),
            })
            .Where(x => x.Attribute is not null)
            .Where(x => typeof(IPipelineExtensionHook).IsAssignableFrom(x.Type))
            .ToList();

        if (!string.IsNullOrWhiteSpace(extensionId))
            candidates = candidates
                .Where(x => x.Attribute!.Id.Equals(extensionId, StringComparison.OrdinalIgnoreCase))
                .ToList();

        if (candidates.Count == 0)
            throw new ExtensionTypeNotFoundException(sourcePath: assemblyPath);

        if (candidates.Count > 1)
            throw new ExtensionTypeAmbiguousException(sourcePath: assemblyPath);

        return candidates[0].Type;
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(x => x is not null)!;
        }
    }

    protected virtual IPipelineExtensionHook Instantiate(Type extensionType, string assemblyPath)
    {
        try
        {
            if (extensionType.GetConstructor(Type.EmptyTypes) is null)
                throw new ExtensionInstantiationFailedException(sourcePath: assemblyPath);

            var instance = Activator.CreateInstance(extensionType) as IPipelineExtensionHook;
            if (instance is null)
                throw new ExtensionInstantiationFailedException(sourcePath: assemblyPath);

            return instance;
        }
        catch (ExtensionException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ExtensionInstantiationFailedException(assemblyPath, ex);
        }
    }
}
