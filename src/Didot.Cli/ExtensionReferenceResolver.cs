using System.Reflection;

namespace Didot.Cli;

public class ExtensionReferenceResolver
{
    private ExtensionMetadataReader MetadataReader { get; }
    private string InstallationDirectory { get; }

    public ExtensionReferenceResolver(ExtensionMetadataReader metadataReader)
        : this(metadataReader, AppContext.BaseDirectory)
    { }

    public ExtensionReferenceResolver(ExtensionMetadataReader metadataReader, string installationDirectory)
    {
        MetadataReader = metadataReader;
        InstallationDirectory = Path.GetFullPath(installationDirectory);
    }

    public virtual string ResolveAssemblyPath(string reference)
    {
        if (string.IsNullOrWhiteSpace(reference))
            throw new InvalidOperationException("The extension reference cannot be empty.");

        return IsExplicitPath(reference)
            ? ResolveFromExplicitPath(reference)
            : ResolveFromLookup(reference.Trim());
    }

    private static bool IsExplicitPath(string reference)
        => Path.IsPathRooted(reference)
            || reference.Contains(Path.DirectorySeparatorChar)
            || reference.Contains(Path.AltDirectorySeparatorChar)
            || reference.StartsWith('.');

    private static string ResolveFromDirectory(string directory)
    {
        var assemblies = Directory.GetFiles(directory, "*.dll", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFullPath)
            .ToList();

        if (assemblies.Count == 0)
            throw new InvalidOperationException($"No extension assembly (*.dll) found in '{directory}'.");

        if (assemblies.Count > 1)
            throw new InvalidOperationException(
                $"Multiple extension assemblies were found in '{directory}'.{Environment.NewLine}"
                + string.Join(Environment.NewLine, assemblies.Select(x => $"- {x}"))
            );

        return assemblies[0];
    }

    private string ResolveFromExplicitPath(string reference)
    {
        var fullPath = Path.GetFullPath(reference.Trim());
        if (File.Exists(fullPath))
        {
            if (!fullPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only .dll files can be registered as extensions.");
            return fullPath;
        }

        if (Directory.Exists(fullPath))
            return ResolveFromDirectory(fullPath);

        throw new InvalidOperationException($"No file or directory found for reference '{reference}'.");
    }

    private string ResolveFromLookup(string reference)
    {
        var candidates = FindCandidates(reference).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

        if (candidates.Count == 0)
            throw new InvalidOperationException($"No extension matches '{reference}'.");

        if (candidates.Count > 1)
        {
            throw new InvalidOperationException(
                $"Multiple extensions match '{reference}'.{Environment.NewLine}{Environment.NewLine}"
                + "Found:" + Environment.NewLine
                + string.Join(Environment.NewLine, candidates.Select(x => $"- {x}"))
                + Environment.NewLine + Environment.NewLine
                + "Register one explicitly:" + Environment.NewLine
                + $"didot extensions register {candidates[0]}"
            );
        }

        return candidates[0];
    }

    private IEnumerable<string> FindCandidates(string reference)
    {
        var normalizedReference = reference.Trim();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var assemblyPath in ProbeAssemblyPaths())
        {
            var fullPath = Path.GetFullPath(assemblyPath);
            if (!seen.Add(fullPath))
                continue;

            var assemblyName = GetAssemblyName(fullPath);
            if (assemblyName is null)
                continue;

            if (assemblyName.Equals(normalizedReference, StringComparison.OrdinalIgnoreCase))
            {
                yield return fullPath;
                continue;
            }

            if (!MetadataReader.TryRead(fullPath, out var metadata, out _ ) || metadata is null)
                continue;

            if (metadata.Id.Equals(normalizedReference, StringComparison.OrdinalIgnoreCase)
                || metadata.Name.Equals(normalizedReference, StringComparison.OrdinalIgnoreCase))
            {
                yield return fullPath;
            }
        }
    }

    private static string? GetAssemblyName(string assemblyPath)
    {
        try
        {
            return AssemblyName.GetAssemblyName(assemblyPath).Name;
        }
        catch
        {
            return null;
        }
    }

    private IEnumerable<string> ProbeAssemblyPaths()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var currentExtensions = Path.Combine(currentDirectory, "extensions");
        var installationExtensions = Path.Combine(InstallationDirectory, "extensions");
        var userExtensions = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".didot",
            "extensions");

        foreach (var path in EnumerateDlls(currentDirectory, SearchOption.TopDirectoryOnly))
            yield return path;

        foreach (var path in EnumerateDlls(currentExtensions, SearchOption.AllDirectories))
            yield return path;

        foreach (var path in EnumerateDlls(installationExtensions, SearchOption.AllDirectories))
            yield return path;

        foreach (var path in EnumerateDlls(userExtensions, SearchOption.AllDirectories))
            yield return path;
    }

    private static IEnumerable<string> EnumerateDlls(string directory, SearchOption searchOption)
    {
        if (!Directory.Exists(directory))
            return [];

        var options = new EnumerationOptions
        {
            RecurseSubdirectories = searchOption == SearchOption.AllDirectories,
            IgnoreInaccessible = true,
        };

        return Directory.EnumerateFiles(directory, "*.dll", options);
    }
}
