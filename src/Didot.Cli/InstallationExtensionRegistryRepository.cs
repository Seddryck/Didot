using System.Text.Json;

namespace Didot.Cli;

public class InstallationExtensionRegistryRepository
{
    public const string RegistryFileName = "didot.extensions.registry.json";

    public string RegistryPath { get; }

    public InstallationExtensionRegistryRepository()
        : this(Path.Combine(AppContext.BaseDirectory, RegistryFileName))
    { }

    public InstallationExtensionRegistryRepository(string registryPath)
        => RegistryPath = Path.GetFullPath(registryPath);

    public virtual IReadOnlyList<ExtensionRegistryEntry> ReadAll()
    {
        if (!File.Exists(RegistryPath))
            return [];

        var json = File.ReadAllText(RegistryPath);
        if (string.IsNullOrWhiteSpace(json))
            return [];

        return JsonSerializer.Deserialize<List<ExtensionRegistryEntry>>(json) ?? [];
    }

    public virtual void Register(ExtensionRegistryEntry entry)
    {
        var entries = ReadAll().ToList();

        if (entries.Any(x => x.Id.Equals(entry.Id, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"An extension with id '{entry.Id}' is already registered.");

        if (entries.Any(x => Path.GetFullPath(x.Assembly).Equals(Path.GetFullPath(entry.Assembly), StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"The assembly '{entry.Assembly}' is already registered.");

        entries.Add(entry);
        Save(entries);
    }

    private void Save(IReadOnlyList<ExtensionRegistryEntry> entries)
    {
        var directory = Path.GetDirectoryName(RegistryPath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(entries, new JsonSerializerOptions
        {
            WriteIndented = true,
        });

        var tempPath = $"{RegistryPath}.tmp";
        File.WriteAllText(tempPath, json);
        File.Move(tempPath, RegistryPath, true);
    }
}
