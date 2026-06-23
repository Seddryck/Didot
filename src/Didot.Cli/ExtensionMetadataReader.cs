using System.Reflection;
using System.Runtime.Loader;
using Didot.Core;

namespace Didot.Cli;

public class ExtensionMetadataReader
{
    public virtual bool TryRead(string assemblyPath, out ExtensionMetadata? metadata, out string? error)
    {
        metadata = null;
        error = null;

        try
        {
            var fullPath = Path.GetFullPath(assemblyPath);
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(fullPath);
            var attribute = assembly.GetCustomAttribute<DidotExtensionAttribute>();
            if (attribute is null)
            {
                error = $"Missing assembly attribute {nameof(DidotExtensionAttribute)}.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(attribute.Id))
            {
                error = "Extension metadata id is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(attribute.Name))
            {
                error = "Extension metadata name is required.";
                return false;
            }

            var assemblyName = assembly.GetName().Name ?? Path.GetFileNameWithoutExtension(fullPath);
            var version = string.IsNullOrWhiteSpace(attribute.Version)
                ? assembly.GetName().Version?.ToString() ?? "0.0.0.0"
                : attribute.Version.Trim();

            metadata = new ExtensionMetadata(
                attribute.Id.Trim(),
                attribute.Name.Trim(),
                version,
                fullPath,
                assemblyName
            );

            return true;
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return false;
        }
    }

    public virtual ExtensionMetadata Read(string assemblyPath)
    {
        if (!TryRead(assemblyPath, out var metadata, out var error) || metadata is null)
            throw new InvalidOperationException($"Unable to read extension metadata from '{assemblyPath}'. {error}");

        return metadata;
    }
}
