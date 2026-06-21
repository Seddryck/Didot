using System.Reflection;
using NUnit.Framework;

namespace Didot.Cli.Testing.Extensions;

public class ExtensionRegistrationServicesTests
{
    [Test]
    public void MetadataReader_WithoutVersionInAttribute_FallsBackToAssemblyVersion()
    {
        var reader = new Cli.ExtensionMetadataReader();
        var assemblyPath = Assembly.GetExecutingAssembly().Location;

        var metadata = reader.Read(assemblyPath);

        Assert.That(metadata.Id, Is.EqualTo("didot.cli.testing.extension"));
        Assert.That(metadata.Name, Is.EqualTo("Cli Testing Extension"));
        Assert.That(metadata.Version, Is.EqualTo(Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0.0"));
    }

    [Test]
    public void ReferenceResolver_ExplicitDllPath_ReturnsAbsolutePath()
    {
        var reader = new Cli.ExtensionMetadataReader();
        var resolver = new Cli.ExtensionReferenceResolver(reader, Path.GetTempPath());

        var sourceAssembly = Assembly.GetExecutingAssembly().Location;
        var tempDirectory = Path.Combine(Path.GetTempPath(), $"didot-ext-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDirectory);

        try
        {
            var destination = Path.Combine(tempDirectory, "Didot.Extension.dll");
            File.Copy(sourceAssembly, destination, true);

            var resolved = resolver.ResolveAssemblyPath(destination);

            Assert.That(resolved, Is.EqualTo(Path.GetFullPath(destination)));
        }
        finally
        {
            if (Directory.Exists(tempDirectory))
                Directory.Delete(tempDirectory, true);
        }
    }

    [Test]
    public void ReferenceResolver_NonPathWithMultipleMatches_Throws()
    {
        var sourceAssembly = Assembly.GetExecutingAssembly().Location;
        var rootDirectory = Path.Combine(Path.GetTempPath(), $"didot-ext-test-{Guid.NewGuid():N}");
        var extensionsDirectory = Path.Combine(rootDirectory, "extensions");
        Directory.CreateDirectory(extensionsDirectory);

        var originalCurrentDirectory = Directory.GetCurrentDirectory();
        try
        {
            File.Copy(sourceAssembly, Path.Combine(rootDirectory, "A.dll"), true);
            File.Copy(sourceAssembly, Path.Combine(extensionsDirectory, "B.dll"), true);

            Directory.SetCurrentDirectory(rootDirectory);
            var reader = new Cli.ExtensionMetadataReader();
            var resolver = new Cli.ExtensionReferenceResolver(reader, Path.Combine(rootDirectory, "install"));

            Assert.That(
                () => resolver.ResolveAssemblyPath("didot.cli.testing.extension"),
                Throws.TypeOf<InvalidOperationException>().With.Message.Contains("Multiple extensions match")
            );
        }
        finally
        {
            Directory.SetCurrentDirectory(originalCurrentDirectory);
            if (Directory.Exists(rootDirectory))
                Directory.Delete(rootDirectory, true);
        }
    }

    [Test]
    public void InstallationRegistryRepository_RegisterAndRead_Success()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"didot-ext-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);

        var registryPath = Path.Combine(directory, "didot.extensions.registry.json");
        var repository = new Cli.InstallationExtensionRegistryRepository(registryPath);

        try
        {
            repository.Register(new Cli.ExtensionRegistryEntry
            {
                Id = "didot.expressif",
                Name = "Expressif",
                Assembly = @"C:\\extensions\\Didot.Expressif.dll",
                Enabled = true,
                Version = "1.0.0",
                RegisteredAt = DateTimeOffset.UtcNow,
            });

            var entries = repository.ReadAll();
            Assert.That(entries, Has.Count.EqualTo(1));
            Assert.That(entries[0].Id, Is.EqualTo("didot.expressif"));
        }
        finally
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
        }
    }

    [Test]
    public void InstallationRegistryRepository_RegisterDuplicateId_Throws()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"didot-ext-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);

        var registryPath = Path.Combine(directory, "didot.extensions.registry.json");
        var repository = new Cli.InstallationExtensionRegistryRepository(registryPath);

        try
        {
            var first = new Cli.ExtensionRegistryEntry
            {
                Id = "didot.expressif",
                Name = "Expressif",
                Assembly = @"C:\extensions\Didot.Expressif.dll",
                Enabled = true,
                Version = "1.0.0",
                RegisteredAt = DateTimeOffset.UtcNow,
            };

            var duplicateId = new Cli.ExtensionRegistryEntry
            {
                Id = "didot.expressif",
                Name = "Expressif 2",
                Assembly = @"C:\extensions\Didot.Expressif2.dll",
                Enabled = true,
                Version = "1.0.1",
                RegisteredAt = DateTimeOffset.UtcNow,
            };

            repository.Register(first);

            Assert.That(
                () => repository.Register(duplicateId),
                Throws.TypeOf<InvalidOperationException>().With.Message.Contains("already registered")
            );
        }
        finally
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
        }
    }

    [Test]
    public void MetadataReader_WhenAttributeMissing_ReturnsError()
    {
        var reader = new Cli.ExtensionMetadataReader();
        var assemblyPath = typeof(Cli.Program).Assembly.Location;

        var success = reader.TryRead(assemblyPath, out var metadata, out var error);

        Assert.That(success, Is.False);
        Assert.That(metadata, Is.Null);
        Assert.That(error, Does.Contain("DidotExtensionAttribute"));
    }
}
