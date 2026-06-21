namespace Didot.Cli;

public class RegisterExtensionCommandHandler
{
    private ExtensionReferenceResolver Resolver { get; }
    private ExtensionMetadataReader MetadataReader { get; }
    private InstallationExtensionRegistryRepository Repository { get; }

    public RegisterExtensionCommandHandler(
        ExtensionReferenceResolver resolver,
        ExtensionMetadataReader metadataReader,
        InstallationExtensionRegistryRepository repository)
    {
        Resolver = resolver;
        MetadataReader = metadataReader;
        Repository = repository;
    }

    public virtual int Execute(string reference, string? name)
    {
        try
        {
            var assemblyPath = Resolver.ResolveAssemblyPath(reference);
            var metadata = MetadataReader.Read(assemblyPath);

            var entry = new ExtensionRegistryEntry
            {
                Id = metadata.Id,
                Name = string.IsNullOrWhiteSpace(name) ? metadata.Name : name.Trim(),
                Assembly = assemblyPath,
                Enabled = true,
                Version = metadata.Version,
                RegisteredAt = DateTimeOffset.UtcNow,
            };

            Repository.Register(entry);

            Console.Out.WriteLine($"Registered extension '{entry.Name}' ({entry.Id}) version {entry.Version} from '{entry.Assembly}'.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }
}
