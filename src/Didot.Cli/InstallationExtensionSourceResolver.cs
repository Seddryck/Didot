using Didot.Core;

namespace Didot.Cli;

public class InstallationExtensionSourceResolver
{
    private InstallationExtensionRegistryRepository Repository { get; }
    private string InstallationDirectory { get; }

    public InstallationExtensionSourceResolver(InstallationExtensionRegistryRepository repository)
        : this(repository, AppContext.BaseDirectory)
    { }

    public InstallationExtensionSourceResolver(InstallationExtensionRegistryRepository repository, string installationDirectory)
    {
        Repository = repository;
        InstallationDirectory = Path.GetFullPath(installationDirectory);
    }

    public virtual bool HasRegistryFile()
        => File.Exists(Repository.RegistryPath);

    public virtual IReadOnlyList<RegisteredExtensionSource> ResolveEnabled()
    {
        if (!HasRegistryFile())
            throw new RegistrationFileNotFoundException(sourcePath: Repository.RegistryPath);

        try
        {
            var entries = Repository.ReadAll();

            return entries
                .Where(x => x.Enabled)
                .Select(x => new RegisteredExtensionSource(
                    x.Id,
                    x.Name,
                    NormalizePath(x.Assembly)
                ))
                .ToList();
        }
        catch (Exception ex)
        {
            throw new RegistrationFileInvalidException(Repository.RegistryPath, ex);
        }
    }

    private string NormalizePath(string assemblyPath)
    {
        var trimmed = assemblyPath.Trim();
        if (Path.IsPathRooted(trimmed))
            return Path.GetFullPath(trimmed);

        return Path.GetFullPath(Path.Combine(InstallationDirectory, trimmed));
    }
}
